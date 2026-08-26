import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { HelperService, InputOption } from '@envirotrax/common-ui';
import { SharedComponentsModule } from '../../../shared/components/shared.components.module';
import { CsiInspectorAccount, CsiInspectorAccountDetails } from '../../../shared/models/csi/csi-inspector-account';
import { State } from '../../../shared/models/lookup/state';
import { Professional, ProfessionalUser } from '../../../shared/models/professionals/professional';
import { ProfessionalWaterSupplier } from '../../../shared/models/professionals/professional-water-supplier';
import { CsiInspectorService } from '../../../shared/services/csi/csi-inspector.service';
import { LookupService } from '../../../shared/services/lookup/lookup.service';
import { WindowReference } from '../../../window/window-config';

@Component({
    templateUrl: './csi-inspector-details.component.html',
    imports: [
        CommonModule,
        FormsModule,
        SharedComponentsModule
    ],
})
export class CsiInspectorDetailsComponent implements OnInit {
    public professionalId: number = 0;
    public userId?: number;

    // Several of these windows can be open at once, so element ids must be unique per window
    // or a label's `for` would toggle the checkbox in a different window.
    public idPrefix: string = 'csi-inspector';

    public professional: Professional = {};
    public user: ProfessionalUser = {};
    public registrations: ProfessionalWaterSupplier[] = [];

    public states: InputOption<State>[] = [];

    public isLoading: boolean = false;
    public isSaving: boolean = false;
    public isSaved: boolean = false;
    public saveFailed: boolean = false;
    public validationErrors: string[] = [];

    public registrationsHeader: string = 'Water Supplier Registrations';

    constructor(
        private readonly _windowReference: WindowReference<CsiInspectorAccount>,
        private readonly _csiInspectorService: CsiInspectorService,
        private readonly _lookupService: LookupService,
        private readonly _helper: HelperService
    ) {

    }

    public async ngOnInit(): Promise<void> {
        const account = this._windowReference.config.model;

        this.professionalId = account?.professionalId ?? 0;
        this.userId = account?.id;
        this.idPrefix = `csi-inspector-${this.professionalId}-${this.userId ?? 0}`;

        await Promise.all([
            this.loadStates(),
            this.loadDetails()
        ]);
    }

    public async save(detailsForm: NgForm): Promise<void> {
        this.isSaved = false;
        this.saveFailed = false;
        this.validationErrors = [];

        if (!detailsForm.valid) {
            return;
        }

        try {
            this.isSaving = true;

            const saved = await this._csiInspectorService.updateDetails(this.professionalId, {
                professional: this.professional,
                user: this.user.id ? this.user : undefined,
                registrations: this.registrations.map(registration => this.normalizeFees(registration))
            });

            this.applyDetails(saved);

            this.isSaved = true;
        } catch (error) {
            const validationErrors: string[] = [];

            this.saveFailed = !this._helper.parseValidationErrors(error, validationErrors);
            this.validationErrors = validationErrors;
        } finally {
            this.isSaving = false;
        }
    }

    /**
     * vp-input hands back the raw <input> string, so a cleared fee cell arrives as '' and an edited
     * one as '25.5'. An empty string is not a valid JSON number, so it would fail model binding on
     * the decimal? column with a raw deserializer message. Coerce both back to number | null here.
     */
    private normalizeFees(registration: ProfessionalWaterSupplier): ProfessionalWaterSupplier {
        return {
            ...registration,
            csiCommercialInspectionFee: this.toFee(registration.csiCommercialInspectionFee),
            csiResidentialInspectionFee: this.toFee(registration.csiResidentialInspectionFee)
        };
    }

    private toFee(value: number | string | null | undefined): number | null {
        if (value == null || value === '') {
            return null;
        }

        const fee = Number(value);

        return isNaN(fee) ? null : fee;
    }

    private async loadStates(): Promise<void> {
        const states = await this._lookupService.getAllStates();

        this.states = [
            { id: '', text: '' },
            ...states.map(state => ({ id: state.id, text: state.name ?? '', data: state }))
        ];
    }

    private async loadDetails(): Promise<void> {
        if (!this.professionalId) {
            return;
        }

        try {
            this.isLoading = true;
            this.applyDetails(await this._csiInspectorService.getDetails(this.professionalId, this.userId));
        } finally {
            this.isLoading = false;
        }
    }

    private applyDetails(details: CsiInspectorAccountDetails): void {
        this.professional = details.professional ?? {};
        this.user = details.user ?? {};
        this.registrations = details.registrations ?? [];

        this.registrationsHeader = `Water Supplier Registrations (${this.registrations.length})`;
    }
}
