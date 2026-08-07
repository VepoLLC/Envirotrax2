import { CommonModule } from '@angular/common';
import { Component, OnInit, ViewChild } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { InputOption, ToastService, ToastType } from '@envirotrax/common-ui';
import { FacilityType, GreaseTrapType, PropertyType } from '../../shared/models/sites/site';
import { SiteDetail, SiteEditWindowModel } from '../../shared/models/sites/site-detail';
import { SiteGisUpdate, SiteUpdate } from '../../shared/models/sites/site-update';
import { SiteService } from '../../shared/services/sites/site.service';
import { LookupService } from '../../shared/services/lookup/lookup.service';
import { SharedComponentsModule } from '../../shared/components/shared.components.module';
import { WindowReference } from '../../window/window-config';
import { SiteEditSectionsModule } from './sections/site-edit-sections.module';

/**
 * Edit Site page/window coordinator.
 *
 * Responsibilities: read the identifiers from the window reference, load the site detail, own the
 * loading/error state, hold the editable site model, coordinate shared lookup data (states), own the
 * single page-level NgForm (whole-page validity + dirty), and — in later slices — build the
 * SiteUpdateDto, make the normal-Site and separate GIS API calls, and coordinate Save/Close.
 *
 * Persistence logic lives here, never in the child section components. Child sections only render
 * their assigned part of `editableSite` and register their controls into this component's form.
 */
@Component({
    templateUrl: './site-edit.component.html',
    standalone: true,
    imports: [CommonModule, FormsModule, SharedComponentsModule, SiteEditSectionsModule],
})
export class SiteEditComponent implements OnInit {
    // The single page-level form. Child section controls register into it via ControlContainer, so
    // this component can read whole-page validity (form.valid) and dirty state (form.dirty) for the
    // future Save flow.
    @ViewChild('form') public form?: NgForm;

    public isLoading: boolean = false;
    public isSaving: boolean = false;

    public siteId?: number;
    public waterSupplierId?: number;

    // Pristine snapshot of the loaded site — never mutated. Used later to detect whether normal-Site
    // or GIS values changed, to compare the renewal-trigger fields (PropertyType /
    // HasOnSiteSewageFacility / HasAuxWaterSupply), to restore on cancel/reload, and to re-baseline
    // after a successful save.
    public originalSite?: SiteDetail;

    // Working copy bound by every child section. Editing mutates only this object — never originalSite.
    public editableSite?: SiteDetail;

    public stateOptions: InputOption[] = [];

    constructor(
        private readonly _windowReference: WindowReference<SiteEditWindowModel>,
        private readonly _siteService: SiteService,
        private readonly _lookupService: LookupService,
        private readonly _toastService: ToastService
    ) {

    }

    public async ngOnInit(): Promise<void> {
        const model = this._windowReference.config.model;

        this.siteId = model?.siteId;
        this.waterSupplierId = model?.waterSupplierId;

        await this.loadStates();
        await this.loadSite();
    }

    private async loadStates(): Promise<void> {
        const states = await this._lookupService.getAllStates();

        this.stateOptions = states.map(state => ({ id: state.id!, text: state.code ?? '' }));
    }

    public async loadSite(): Promise<void> {
        if (this.siteId == null) {
            return;
        }

        try {
            this.isLoading = true;

            const site = await this._siteService.getById(this.siteId);

            // Two independent deep clones: the pristine snapshot and the editable working copy.
            this.originalSite = structuredClone(site);
            this.editableSite = structuredClone(site);
        } finally {
            this.isLoading = false;
        }
    }

    /**
     * Re-baselines both snapshots from the persisted server state after a successful save, WITHOUT toggling
     * isLoading. Unlike loadSite(), this keeps the form — and the embedded vp-map — mounted: reloading via
     * isLoading would destroy and re-create the map, making the Google Maps loader re-run setOptions() and
     * throw an IntersectionObserver error during the re-init. Here the map instance persists and simply
     * receives updated latitude/longitude inputs. The programmatic model refresh does not dirty the form,
     * but we mark it pristine explicitly so the post-save baseline is clean.
     */
    private async refreshAfterSave(): Promise<void> {
        if (this.siteId == null) {
            return;
        }

        const site = await this._siteService.getById(this.siteId);

        this.originalSite = structuredClone(site);
        this.editableSite = structuredClone(site);

        this.form?.form.markAsPristine();
    }

    /**
     * Saves the Edit Site changes.
     *
     * Flow: validate the whole page form → send the normal (non-GIS) PUT only when an approved normal field
     * changed → send the GIS PUT only when a GIS value changed (nothing changed → send neither, just report a
     * standard success) → re-baseline both snapshots from the server without tearing the view down. GIS is
     * handled separately because the backend keeps it write-isolated from the normal Site update; a GIS
     * failure after a successful normal save is reported as a partial save rather than silently swallowed.
     * Only the normal Site PUT failing is treated as a Save failure — a failed GIS update or a failed
     * post-save refresh each report their own message without claiming the Site wasn't saved.
     */
    public async save(): Promise<void> {
        if (this.siteId == null || !this.editableSite || !this.originalSite) {
            return;
        }

        if (!this.validateForSave()) {
            return;
        }

        // The normal PUT is sent only when an approved normal field changed; GIS only when a GIS value
        // changed. Both are captured before any reload replaces editableSite.
        const normalChanged = this.hasNormalSiteChanged();
        const gisChanged = this.hasGisChanged();

        // Nothing changed — send neither PUT (a no-op save). Report the standard success feedback.
        if (!normalChanged && !gisChanged) {
            this._toastService.successfullySaved();

            return;
        }

        try {
            this.isSaving = true;

            // The normal (non-GIS) save is the only thing whose failure counts as a Save failure — it stays
            // in this try so it reaches the catch below. GIS and refresh each swallow their own errors.
            if (normalChanged) {
                await this._siteService.update(this.siteId, this.buildSiteUpdate(this.editableSite!));
            }

            const gisFailed = await this.saveGisIfChanged(gisChanged);
            const refreshFailed = await this.refreshQuietly();

            this.reportSaveResult(normalChanged, gisFailed, refreshFailed);
        } catch {
            this._toastService.failedToSave('Site');
        } finally {
            this.isSaving = false;
        }
    }

    // Runs the Trip Ticket Interval range check, then the whole-page validity check. On failure, marks the
    // form touched, shows the standard error toast, and returns false to block the save.
    private validateForSave(): boolean {
        this.applyTripTicketIntervalValidation();

        // Child section ngModels register into this form via ControlContainer.
        if (this.form?.invalid) {
            this.form.form.markAllAsTouched();

            this._toastService.show({
                text: 'Please correct the highlighted fields before saving.',
                type: ToastType.Error
            });

            return false;
        }

        return true;
    }

    // Saves GIS through its separate endpoint when a GIS value changed. Swallows its own error and returns
    // whether it failed, so a GIS failure is never reported as a Save failure.
    private async saveGisIfChanged(gisChanged: boolean): Promise<boolean> {
        if (!gisChanged) {
            return false;
        }

        try {
            await this._siteService.updateGis(this.siteId!, this.buildGisUpdate());

            return false;
        } catch {
            return true;
        }
    }

    // Re-baselines both snapshots from the persisted state, keeping the view mounted. Persistence has
    // already succeeded by now, so its error is swallowed and surfaced separately (never a Save failure).
    private async refreshQuietly(): Promise<boolean> {
        try {
            await this.refreshAfterSave();

            return false;
        } catch {
            return true;
        }
    }

    // Shows the outcome toast. A GIS failure takes priority (and keeps the "details saved" fact when normal
    // fields also changed); a refresh-only failure is a warning, not a Save failure; otherwise success.
    private reportSaveResult(normalChanged: boolean, gisFailed: boolean, refreshFailed: boolean): void {
        if (gisFailed) {
            this._toastService.show({
                text: normalChanged
                    ? 'The site details were saved, but the GIS update failed — please try again.'
                    : 'The GIS update failed — please try again.',
                type: ToastType.Error
            });
        } else if (refreshFailed) {
            this._toastService.show({
                text: 'The site was saved, but the latest data could not be reloaded. Please reload or reopen the window.',
                type: ToastType.Warning
            });
        } else {
            this._toastService.successfullySaved();
        }
    }

    /**
     * Closes the Edit Site window, leaving the Property Search results untouched. Mirrors the title-bar
     * close button (same window-removal path). No unsaved-changes prompt yet — that guard is a separate,
     * deferred item that should cover both this button and the title-bar close.
     */
    public close(): void {
        this._windowReference.close();
    }

    // Compares normalized GIS values (matching what buildGisUpdate sends). Normalization matters because
    // vp-input type="number" yields a string once edited, so a raw !== would flag "changed" when a
    // coordinate is edited back to its original numeric value (number vs numeric string) and fire a
    // needless GIS PUT.
    private hasGisChanged(): boolean {
        const original = this.originalSite!;
        const editable = this.editableSite!;

        return this.toNumberOrNull(original.gisLatitude) !== this.toNumberOrNull(editable.gisLatitude)
            || this.toNumberOrNull(original.gisLongitude) !== this.toNumberOrNull(editable.gisLongitude)
            || (original.gisStatus ?? 0) !== (editable.gisStatus ?? 0);
    }

    // True when the approved SiteUpdate payload differs from the one derived from the pristine snapshot —
    // i.e. an editable normal (non-GIS) field actually changed. Comparing the built payloads (not the raw
    // read model) means only fields that would actually be sent count, and each side is normalized the same
    // way, so number-vs-string edits don't produce false positives. When equal, the normal PUT would be a
    // no-op, so it is skipped.
    private hasNormalSiteChanged(): boolean {
        return JSON.stringify(this.buildSiteUpdate(this.editableSite!))
            !== JSON.stringify(this.buildSiteUpdate(this.originalSite!));
    }

    private buildGisUpdate(): SiteGisUpdate {
        const editable = this.editableSite!;

        return {
            latitude: this.toNumberOrNull(editable.gisLatitude),
            longitude: this.toNumberOrNull(editable.gisLongitude),
            status: editable.gisStatus ?? 0
        };
    }

    /**
     * Flags an out-of-range Trip Ticket Interval on its form control so the standard invalid-form handling
     * blocks Save (it is never silently coerced). The field is optional: blank/null/undefined is valid and
     * saved as 0 ("no interval", matching Vepo Manager). When provided, it must be a whole, non-negative
     * number of days — a negative or decimal value sets an `interval` error; a valid/blank value clears it.
     */
    private applyTripTicketIntervalValidation(): void {
        const control = this.form?.controls['tripTicketInterval'];

        if (!control) {
            return;
        }

        // Typed unknown: vp-input (type="number") hands back a string once the field is edited.
        const value: unknown = this.editableSite?.tripTicketInterval;
        const isBlank = value === null || value === undefined || value === '';
        const isNonNegativeWholeNumber = /^\d+$/.test(String(value).trim());

        if (!isBlank && !isNonNegativeWholeNumber) {
            control.setErrors({ ...(control.errors ?? {}), interval: true });
        } else if (control.hasError('interval')) {
            const errors = { ...control.errors };
            delete errors['interval'];
            control.setErrors(Object.keys(errors).length ? errors : null);
        }
    }

    /**
     * Coerces a vp-input number value (which arrives as a string once edited) to a real number. Blank /
     * null / undefined becomes null so a coordinate can be cleared. type="number" guarantees numeric
     * content, so no extra text validation is needed here.
     */
    private toNumberOrNull(value: unknown): number | null {
        if (value === null || value === undefined || value === '') {
            return null;
        }

        const parsed = Number(value);

        return Number.isNaN(parsed) ? null : parsed;
    }

    // Builds the update payload from the editable model. Only the approved editable fields are included;
    // GIS, ids, water supplier, audit, assignments and NeedsRenewalCheck are intentionally excluded (the
    // server owns/derives those). State is flattened to its id.
    //
    // SiteUpdate's fields are required, but they are read from the fully-optional SiteDetail read model, so
    // each is coalesced to the value the server would otherwise default it to (null / '' / false / 0 /
    // enum default). A loaded site always populates these and save() runs only when editableSite is set and
    // the form is valid, so the fallbacks never actually trigger — they exist only to satisfy the required
    // types without unsafe casts, and produce an identical payload.
    private buildSiteUpdate(site: SiteDetail): SiteUpdate {
        return {
            // Property Information
            propertyType: site.propertyType ?? PropertyType.Residential,
            businessName: site.businessName ?? null,
            streetNumber: site.streetNumber ?? null,
            streetName: site.streetName ?? null,
            propertyNumber: site.propertyNumber ?? null,
            city: site.city ?? null,
            state: site.state?.id != null ? { id: site.state.id } : null,
            zipCode: site.zipCode ?? null,

            // Mailing Information
            mailingCompanyName: site.mailingCompanyName ?? null,
            mailingContactName: site.mailingContactName ?? null,
            mailingStreetNumber: site.mailingStreetNumber ?? null,
            mailingStreetName: site.mailingStreetName ?? null,
            mailingNumber: site.mailingNumber ?? null,
            mailingCity: site.mailingCity ?? null,
            mailingState: site.mailingState?.id != null ? { id: site.mailingState.id } : null,
            mailingZipCode: site.mailingZipCode ?? null,
            mailingPhoneNumber: site.mailingPhoneNumber ?? null,
            mailingEmailAddress: site.mailingEmailAddress ?? null,

            // Property Settings
            accountNumber: site.accountNumber ?? '',
            active: site.active ?? false,
            invalidMailingAddress: site.invalidMailingAddress ?? false,
            outOfArea: site.outOfArea ?? false,
            isFeeExempt: site.isFeeExempt ?? false,
            bypassPropertyNumberValidation: site.bypassPropertyNumberValidation ?? false,
            backflowScheduleMonth: site.backflowScheduleMonth ?? 0,
            needsCsiInspection: site.needsCsiInspection ?? false,
            csiRenewalDate: site.csiRenewalDate ?? null,
            needsFogInspection: site.needsFogInspection ?? false,
            fogInspectionExpirationDate: site.fogInspectionExpirationDate ?? null,
            needsFogPermit: site.needsFogPermit ?? false,
            fogPermitExpirationDate: site.fogPermitExpirationDate ?? null,
            lastTripTicketDate: site.lastTripTicketDate ?? null,
            // Blank/null/undefined → 0 ("no interval"); Save is blocked earlier for negative/decimal values.
            tripTicketInterval: this.toNumberOrNull(site.tripTicketInterval) ?? 0,
            facilityType: site.facilityType ?? FacilityType.Other,
            greaseTrapType: site.greaseTrapType ?? GreaseTrapType.TrapNotRequired,
            hasOnSiteSewageFacility: site.hasOnSiteSewageFacility ?? false,
            hasAuxWaterSupply: site.hasAuxWaterSupply ?? false,
            hasFireSystem: site.hasFireSystem ?? false,
            fireSeparateWater: site.fireSeparateWater ?? false,
            hasGritTrap: site.hasGritTrap ?? false,
            hasIrrigation: site.hasIrrigation ?? false,
            irrigationSeparateWater: site.irrigationSeparateWater ?? false,
            hasDomesticPremisesIsolation: site.hasDomesticPremisesIsolation ?? false
        };
    }
}
