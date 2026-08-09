import { CommonModule } from '@angular/common';
import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { InputOption } from '@envirotrax/common-ui';
import { SharedComponentsModule } from '../../../shared/components/shared.components.module';
import {
    CsiInspection,
    CsiInspectionDetails,
    CsiInspectionReason
} from '../../../shared/models/csi/csi-inspection';
import { State } from '../../../shared/models/lookup/state';
import { PropertyType } from '../../../shared/models/sites/site';
import { CsiInspectionService } from '../../../shared/services/csi/csi-inspection.service';
import { LookupService } from '../../../shared/services/lookup/lookup.service';
import { WindowReference } from '../../../window/window-config';
import { CsiInspectionAdditionalInformationComponent } from './additional-information/csi-inspection-additional-information.component';
import { CsiInspectionAssembliesComponent } from './assemblies/csi-inspection-assemblies.component';
import { CsiInspectionImagesComponent } from './images/csi-inspection-images.component';
import { CsiInspectionRecordLogComponent } from './record-log/csi-inspection-record-log.component';
import { ComplianceItem, CsiInspectionResultsComponent } from './results/csi-inspection-results.component';

type CsiInspectionTab = 'results' | 'assemblies' | 'additional' | 'images' | 'logs';

const SaveMessageDurationMs = 5000;

@Component({
    templateUrl: './csi-inspection-details.component.html',
    imports: [
        CommonModule,
        FormsModule,
        SharedComponentsModule,
        CsiInspectionResultsComponent,
        CsiInspectionAssembliesComponent,
        CsiInspectionAdditionalInformationComponent,
        CsiInspectionImagesComponent,
        CsiInspectionRecordLogComponent
    ],
})
export class CsiInspectionDetailsComponent implements OnInit, OnDestroy {
    @ViewChild(CsiInspectionRecordLogComponent)
    public recordLog?: CsiInspectionRecordLogComponent;

    public id: number = 0;
    public idPrefix: string = 'csi';

    public isLoading: boolean = false;
    public isSaving: boolean = false;
    public saveSuccessMessage: string = '';

    private _saveMessageTimeoutId?: ReturnType<typeof setTimeout>;

    public inspection: CsiInspectionDetails = {};

    public selectedTab: CsiInspectionTab = 'results';

    public waterSupplierHeader: string = 'Water Supplier';
    public inspectorHeader: string = 'CSI Inspector';
    public waterSupplierCityStateZip: string = '';
    public inspectorCityStateZip: string = '';

    public assembliesTabTitle: string = 'Assemblies at Location';
    public recordLogTabTitle: string = 'Record Log';

    public complianceItems: ComplianceItem[] = [];

    public propertyTypeId: string = '';
    public propertyStateId: string = '';
    public mailingStateId: string = '';
    public reasonForInspectionId: string = '';
    public inspectionDate: string = '';

    public stateOptions: InputOption<State>[] = [];

    public readonly propertyTypeOptions: InputOption[] = [
        { id: String(PropertyType.Residential), text: 'Residential' },
        { id: String(PropertyType.Commercial), text: 'Commercial' }
    ];

    private readonly _complianceTexts: string[] = [
        'No direct or indirect connection between the public drinking water supply and a potential source of contamination exists. Potential sources of contamination are isolated from the public water system by an air gap or an appropriate backflow prevention assembly in accordance with Commission regulations.',
        'No cross-connection between the public drinking water supply and a private water system exists. Where an actual air gap is not maintained between the public water supply and a private water supply, an approved reduced pressure principle backflow prevention assembly is properly installed.',
        'No connection exists which would allow the return of water used for condensing, cooling or industrial processes back to the public water supply.',
        'No pipe or pipe fitting which contains more than 8.0% lead exists in private water distribution facilities installed on or after July 1, 1988 and prior to January 4, 2014.',
        'Plumbing installed on or after January 4, 2014 bears the expected labeling indicating ≤0.25% lead content. If not properly labeled, please provide written comment.',
        'No solder or flux which contains more than 0.2% lead exists in private water distribution facilities installed on or after July 1, 1988.'
    ];

    constructor(
        private readonly _windowReference: WindowReference<CsiInspection>,
        private readonly _inspectionService: CsiInspectionService,
        private readonly _lookupService: LookupService
    ) {

    }

    public async ngOnInit(): Promise<void> {
        this.id = this._windowReference.config.model?.id ?? 0;
        this.idPrefix = `csi-${this.id}`;

        await Promise.all([
            this.loadStates(),
            this.loadInspection(),
            this.loadCounts()
        ]);
    }

    public ngOnDestroy(): void {
        this.dismissSaveMessage();
    }

    public onTabChange(tab: CsiInspectionTab): void {
        this.selectedTab = tab;
    }

    public async save(): Promise<void> {
        this.applyEditorsToInspection();
        this.dismissSaveMessage();

        try {
            this.isSaving = true;
            this.inspection = await this._inspectionService.update(this.id, this.inspection);
        } finally {
            this.isSaving = false;
        }

        this.applyInspectionToEditors();

        await this.loadCounts();
        await this.reloadRecordLog();

        this.showSaveMessage();
    }

    public dismissSaveMessage(): void {
        this.saveSuccessMessage = '';

        if (this._saveMessageTimeoutId != null) {
            clearTimeout(this._saveMessageTimeoutId);
            this._saveMessageTimeoutId = undefined;
        }
    }

    private showSaveMessage(): void {
        this.saveSuccessMessage = 'Inspection saved successfully.';

        this._saveMessageTimeoutId = setTimeout(() => this.dismissSaveMessage(), SaveMessageDurationMs);
    }

    private async reloadRecordLog(): Promise<void> {
        if (this.recordLog == null) {
            return;
        }

        await this.recordLog.reload();
    }

    private async loadStates(): Promise<void> {
        this.stateOptions = await this._lookupService.getStatesAsOptions();
    }

    private async loadInspection(): Promise<void> {
        try {
            this.isLoading = true;
            this.inspection = await this._inspectionService.get(this.id);
        } finally {
            this.isLoading = false;
        }

        this.applyInspectionToEditors();
        this.setHeaders();
    }

    private async loadCounts(): Promise<void> {
        const counts = await this._inspectionService.getCounts(this.id);

        this.assembliesTabTitle = `Assemblies at Location (${counts.assemblyCount ?? 0})`;
        this.recordLogTabTitle = `Record Log (${counts.recordLogCount ?? 0})`;
    }

    private applyInspectionToEditors(): void {
        this.propertyTypeId = String(this.inspection.propertyType ?? PropertyType.Residential);
        this.propertyStateId = this.inspection.propertyState?.id == null ? '' : String(this.inspection.propertyState.id);
        this.mailingStateId = this.inspection.mailingState?.id == null ? '' : String(this.inspection.mailingState.id);
        this.reasonForInspectionId = String(this.inspection.reasonForInspection ?? CsiInspectionReason.NewConstruction);
        this.inspectionDate = this.inspection.inspectionDate ?? '';

        this.complianceItems = [
            { number: 1, text: this._complianceTexts[0], isCompliant: this.inspection.compliance1 === true },
            { number: 2, text: this._complianceTexts[1], isCompliant: this.inspection.compliance2 === true },
            { number: 3, text: this._complianceTexts[2], isCompliant: this.inspection.compliance3 === true },
            { number: 4, text: this._complianceTexts[3], isCompliant: this.inspection.compliance4 === true },
            { number: 5, text: this._complianceTexts[4], isCompliant: this.inspection.compliance5 === true },
            { number: 6, text: this._complianceTexts[5], isCompliant: this.inspection.compliance6 === true }
        ];
    }

    private applyEditorsToInspection(): void {
        this.inspection.propertyType = Number(this.propertyTypeId) as PropertyType;
        this.inspection.propertyState = this.findState(this.propertyStateId);
        this.inspection.mailingState = this.findState(this.mailingStateId);
        this.inspection.reasonForInspection = Number(this.reasonForInspectionId) as CsiInspectionReason;
        this.inspection.inspectionDate = this.inspectionDate || undefined;

        this.inspection.compliance1 = this.complianceItems[0].isCompliant;
        this.inspection.compliance2 = this.complianceItems[1].isCompliant;
        this.inspection.compliance3 = this.complianceItems[2].isCompliant;
        this.inspection.compliance4 = this.complianceItems[3].isCompliant;
        this.inspection.compliance5 = this.complianceItems[4].isCompliant;
        this.inspection.compliance6 = this.complianceItems[5].isCompliant;
    }

    private findState(stateId: string): State | undefined {
        if (!stateId) {
            return undefined;
        }

        const option = this.stateOptions.find(existing => existing.id === stateId);

        return option?.data;
    }

    private setHeaders(): void {
        const supplier = this.inspection.waterSupplier;

        this.waterSupplierHeader = supplier?.name
            ? `Water Supplier - ${supplier.name}`
            : 'Water Supplier';

        this.waterSupplierCityStateZip = this.buildCityStateZip(supplier?.city, supplier?.state?.code, supplier?.zipCode);

        let inspectorHeader = 'CSI Inspector';

        if (this.inspection.inspectorCompanyName) {
            inspectorHeader = `${inspectorHeader} - ${this.inspection.inspectorCompanyName}`;
        }

        if (this.inspection.inspectorContactName) {
            inspectorHeader = `${inspectorHeader} - ${this.inspection.inspectorContactName}`;
        }

        this.inspectorHeader = inspectorHeader;

        this.inspectorCityStateZip = this.buildCityStateZip(
            this.inspection.inspectorCity,
            this.inspection.inspectorState,
            this.inspection.inspectorZip
        );
    }

    private buildCityStateZip(city?: string, state?: string, zip?: string): string {
        let result = city ?? '';

        if (state) {
            result = result ? `${result}, ${state}` : state;
        }

        if (zip) {
            result = result ? `${result}  ${zip}` : zip;
        }

        return result;
    }
}
