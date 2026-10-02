import { CommonModule } from '@angular/common';
import { Component, OnInit, ViewChild } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { InputOption, RecordLog, ToastService, ToastType } from '@envirotrax/common-ui';
import { SharedComponentsModule } from '../../../shared/components/shared.components.module';
import {
    FogInspection,
    FogInspectionUpdateRequest
} from '../../../shared/models/fog/fog-inspection';
import { State } from '../../../shared/models/lookup/state';
import { FogInspectionService } from '../../../shared/services/fog/fog-inspection.service';
import { LookupService } from '../../../shared/services/lookup/lookup.service';
import { SiteEditComponent } from '../../../sites/edit/site-edit.component';
import { WindowReference } from '../../../window/window-config';
import { WindowService } from '../../../shared/services/window.service';
import { FogInspectionImagesComponent } from './images/fog-inspection-images.component';
import { FogInspectionMailingComponent } from './mailing/fog-inspection-mailing.component';
import { FogInspectionPropertyComponent } from './property/fog-inspection-property.component';
import { FogInspectionResultsComponent, isInvalidChamberValue } from './results/fog-inspection-results.component';
import { FogInspectionTrapComponent } from './trap/fog-inspection-trap.component';

type FogInspectionTab = 'results' | 'images' | 'logs';

/**
 * Water Supplier, FOG Inspector, Trap and Inspection Results are still read-only; Property and Mailing are
 * edited through child sections that share this component's NgForm and inspection object. One Save at the
 * bottom submits the whole page, matching BackflowTestDetailsComponent.
 */
@Component({
    templateUrl: './fog-inspection-details.component.html',
    imports: [
        CommonModule,
        FormsModule,
        SharedComponentsModule,
        FogInspectionPropertyComponent,
        FogInspectionMailingComponent,
        FogInspectionTrapComponent,
        FogInspectionResultsComponent,
        FogInspectionImagesComponent
    ],
})
export class FogInspectionDetailsComponent implements OnInit {
    @ViewChild('detailsForm') public detailsForm?: NgForm;

    public id: number = 0;

    /** Namespaces DOM ids: the window container can hold several details windows at once. */
    public idPrefix: string = 'fog';

    public isLoading: boolean = false;
    public isLoadingRecordLogs: boolean = false;
    public isSaving: boolean = false;

    public validationErrors: string[] = [];

    public inspection: FogInspection = {};
    public recordLogs: RecordLog[] = [];
    public stateOptions: InputOption<State>[] = [];

    public selectedTab: FogInspectionTab = 'results';


    // Display values, computed once after load - never called from the template.
    public waterSupplierHeader: string = 'Water Supplier';
    public inspectorHeader: string = 'FOG Inspector';
    public waterSupplierCityStateZip: string = '';
    public inspectorCityStateZip: string = '';
    public recordLogTabTitle: string = 'Record Log';

    constructor(
        private readonly _windowReference: WindowReference<{ id?: number }>,
        private readonly _inspectionService: FogInspectionService,
        private readonly _lookupService: LookupService,
        private readonly _windowService: WindowService,
        private readonly _toastService: ToastService
    ) {

    }

    public async ngOnInit(): Promise<void> {
        this.id = this._windowReference.config.model?.id ?? 0;
        this.idPrefix = `fog-${this.id}`;

        await Promise.all([
            this.loadInspection(),
            this.loadStates(),
            this.loadRecordLogs()
        ]);
    }

    public async save(): Promise<void> {
        if (!this.collectValidationErrors()) {
            return;
        }

        try {
            this.isSaving = true;

            await this._inspectionService.update(
                this.id,
                this.inspection.waterSupplier?.id ?? 0,
                this.buildUpdateRequest());

            const refreshFailed = await this.refreshQuietly();

            this.reportSaveResult(refreshFailed);
        } finally {
            this.isSaving = false;
        }
    }

    private reportSaveResult(refreshFailed: boolean): void {
        if (refreshFailed) {
            this._toastService.show({
                text: 'The inspection was saved, but the latest data could not be reloaded. Please reload or reopen the window.',
                type: ToastType.Warning
            });
        } else {
            this._toastService.successfullySaved();
        }
    }

    private async refreshAfterSave(): Promise<void> {
        if (!this.id) {
            return;
        }

        this.inspection = await this._inspectionService.get(this.id);
        this.setDisplayValues(this.inspection);

        this.detailsForm?.form.markAsPristine();

        await this.loadRecordLogs();
    }

    private async refreshQuietly(): Promise<boolean> {
        try {
            await this.refreshAfterSave();

            return false;
        } catch {
            return true;
        }
    }

    public openSite(): void {
        const siteId = this.inspection.site?.id;

        if (siteId == null) {
            return;
        }

        this._windowService.addWindow(SiteEditComponent, {
            title: this.inspection.site?.accountNumber ?? 'Site',
            model: {
                siteId: siteId,
                waterSupplierId: this.inspection.waterSupplier?.id
            }
        });
    }

    private collectValidationErrors(): boolean {
        this.validationErrors = [];

        const chamberValues = [
            this.inspection.inletChamberWettingHeight,
            this.inspection.inletChamberGreaseBlanket,
            this.inspection.inletChamberSediments,
            this.inspection.outletChamberWettingHeight,
            this.inspection.outletChamberGreaseBlanket,
            this.inspection.outletChamberSediments
        ];

        if (chamberValues.some(value => isInvalidChamberValue(value))) {
            this.validationErrors.push('Chamber readings (wetted height, grease blanket, sediments) must be numbers of 0 or greater, or left blank.');
        }

        return this.validationErrors.length === 0;
    }

    private buildUpdateRequest(): FogInspectionUpdateRequest {
        const inspection = this.inspection;

        return {
            propertyType: inspection.propertyType,
            propertyBusinessName: inspection.propertyBusinessName,
            propertyStreetNumber: inspection.propertyStreetNumber,
            propertyStreetName: inspection.propertyStreetName,
            propertyNumber: inspection.propertyNumber,
            propertyCity: inspection.propertyCity,
            propertyState: inspection.propertyState,
            propertyZip: inspection.propertyZip,

            mailingCompanyName: inspection.mailingCompanyName,
            mailingContactName: inspection.mailingContactName,
            mailingStreetNumber: inspection.mailingStreetNumber,
            mailingStreetName: inspection.mailingStreetName,
            mailingNumber: inspection.mailingNumber,
            mailingCity: inspection.mailingCity,
            mailingState: inspection.mailingState,
            mailingZip: inspection.mailingZip,

            interceptorType: inspection.interceptorType,
            interceptorOtherDescription: inspection.interceptorOtherDescription,
            interceptorCapacity: inspection.interceptorCapacity,
            interceptorCapacityType: inspection.interceptorCapacityType,
            interceptorLocationDescription: inspection.interceptorLocationDescription,

            inspectionDate: inspection.inspectionDate,
            reasonForInspection: inspection.reasonForInspection,
            facilityType: inspection.facilityType,
            maintained: inspection.maintained,
            accessible: inspection.accessible,
            pastOverflow: inspection.pastOverflow,
            samplingPointAccessible: inspection.samplingPointAccessible,
            samplingPointClean: inspection.samplingPointClean,
            sampledFrom: inspection.sampledFrom,
            inletTeeIntact: inspection.inletTeeIntact,
            outletTeeIntact: inspection.outletTeeIntact,
            inletChamberWettingHeight: inspection.inletChamberWettingHeight,
            inletChamberGreaseBlanket: inspection.inletChamberGreaseBlanket,
            inletChamberSediments: inspection.inletChamberSediments,
            outletChamberWettingHeight: inspection.outletChamberWettingHeight,
            outletChamberGreaseBlanket: inspection.outletChamberGreaseBlanket,
            outletChamberSediments: inspection.outletChamberSediments,
            inletTotalCapacityPercent: inspection.inletTotalCapacityPercent,
            outletTotalCapacityPercent: inspection.outletTotalCapacityPercent,
            totalCapacityPercent: inspection.totalCapacityPercent,
            inspectionResult: inspection.inspectionResult,
            comments: inspection.comments
        };
    }

    private async loadStates(): Promise<void> {
        this.stateOptions = await this._lookupService.getStatesAsOptions();
    }

    private async loadInspection(): Promise<void> {
        if (!this.id) {
            return;
        }

        try {
            this.isLoading = true;
            this.inspection = await this._inspectionService.get(this.id);
            this.setDisplayValues(this.inspection);
        } finally {
            this.isLoading = false;
        }
    }

    private async loadRecordLogs(): Promise<void> {
        if (!this.id) {
            return;
        }

        try {
            this.isLoadingRecordLogs = true;
            this.recordLogs = await this._inspectionService.getLogs(this.id);
            this.recordLogTabTitle = `Record Log (${this.recordLogs.length})`;
        } finally {
            this.isLoadingRecordLogs = false;
        }
    }

    private setDisplayValues(inspection: FogInspection): void {
        this.waterSupplierHeader = inspection.waterSupplier?.name
            ? `Water Supplier - ${inspection.waterSupplier.name}`
            : 'Water Supplier';

        this.inspectorHeader = ['FOG Inspector', inspection.inspectorCompanyName, inspection.inspectorContactName]
            .filter(part => part)
            .join(' - ');

        this.waterSupplierCityStateZip = this.buildCityStateZip(inspection.waterSupplier?.city, inspection.waterSupplier?.state?.code, inspection.waterSupplier?.zipCode);
        this.inspectorCityStateZip = this.buildCityStateZip(inspection.inspectorCity, inspection.inspectorState, inspection.inspectorZip);
    }

    private buildCityStateZip(city?: string, stateCode?: string, zip?: string): string {
        const cityPart = city ? `${city},` : '';

        return [cityPart, stateCode, zip].filter(part => part).join(' ').trim();
    }
}
