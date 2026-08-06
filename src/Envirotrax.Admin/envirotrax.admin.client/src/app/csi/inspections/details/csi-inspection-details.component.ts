import { CommonModule } from '@angular/common';
import { Component, OnDestroy, OnInit, TemplateRef, ViewChild } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CellTemplateData, ColumnType, InputOption, TableColumn } from '@envirotrax/common-ui';
import { SharedComponentsModule } from '../../../shared/components/shared.components.module';
import {
    CsiInspection,
    CsiInspectionAssembly,
    CsiInspectionDetails,
    CsiInspectionImage,
    CsiInspectionReason,
    csiInspectionReasonLabels
} from '../../../shared/models/csi/csi-inspection';
import { RecordLog, recordLogTypeLabels } from '../../../shared/models/logs/record-log';
import { State } from '../../../shared/models/lookup/state';
import { PropertyType } from '../../../shared/models/sites/site';
import { CsiInspectionService } from '../../../shared/services/csi/csi-inspection.service';
import { LookupService } from '../../../shared/services/lookup/lookup.service';
import { WindowReference } from '../../../window/window-config';

type CsiInspectionTab = 'results' | 'assemblies' | 'additional' | 'images' | 'logs';

interface ComplianceItem {
    number: number;
    text: string;
    isCompliant: boolean;
}

interface RecordLogRow extends RecordLog {
    logTypeLabel: string;
}

const SaveMessageDurationMs = 5000;

@Component({
    templateUrl: './csi-inspection-details.component.html',
    imports: [CommonModule, FormsModule, SharedComponentsModule],
})
export class CsiInspectionDetailsComponent implements OnInit, OnDestroy {
    @ViewChild('assemblyStatusCell', { static: true })
    public assemblyStatusCell?: TemplateRef<CellTemplateData<CsiInspectionAssembly>>;

    @ViewChild('assemblyDatesCell', { static: true })
    public assemblyDatesCell?: TemplateRef<CellTemplateData<CsiInspectionAssembly>>;

    @ViewChild('assemblyInfoCell', { static: true })
    public assemblyInfoCell?: TemplateRef<CellTemplateData<CsiInspectionAssembly>>;

    @ViewChild('assemblyIdentifiedCell', { static: true })
    public assemblyIdentifiedCell?: TemplateRef<CellTemplateData<CsiInspectionAssembly>>;

    @ViewChild('logDescriptionCell', { static: true })
    public logDescriptionCell?: TemplateRef<CellTemplateData<RecordLogRow>>;

    public id: number = 0;
    public idPrefix: string = 'csi';

    public isLoading: boolean = false;
    public isSaving: boolean = false;
    public saveSuccessMessage: string = '';

    private _saveMessageTimeoutId?: ReturnType<typeof setTimeout>;
    public isLoadingAssemblies: boolean = false;
    public isLoadingImages: boolean = false;
    public isLoadingLogs: boolean = false;

    public inspection: CsiInspectionDetails = {};

    public selectedTab: CsiInspectionTab = 'results';

    public waterSupplierHeader: string = 'Water Supplier';
    public inspectorHeader: string = 'CSI Inspector';
    public waterSupplierCityStateZip: string = '';
    public inspectorCityStateZip: string = '';

    public assembliesTabTitle: string = 'Assemblies at Location';
    public recordLogTabTitle: string = 'Record Log';

    public assemblies: CsiInspectionAssembly[] = [];
    public images: CsiInspectionImage[] = [];
    public logs: RecordLogRow[] = [];

    public assemblyColumns: TableColumn<CsiInspectionAssembly>[] = [];
    public logColumns: TableColumn<RecordLogRow>[] = [];

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

    public readonly reasonOptions: InputOption[] = [
        {
            id: String(CsiInspectionReason.NewConstruction),
            text: csiInspectionReasonLabels[CsiInspectionReason.NewConstruction]
        },
        {
            id: String(CsiInspectionReason.ExistingServiceContaminantHazardsSuspected),
            text: csiInspectionReasonLabels[CsiInspectionReason.ExistingServiceContaminantHazardsSuspected]
        },
        {
            id: String(CsiInspectionReason.MajorRenovationOrExpansion),
            text: csiInspectionReasonLabels[CsiInspectionReason.MajorRenovationOrExpansion]
        }
    ];

    private readonly _complianceTexts: string[] = [
        'No direct or indirect connection between the public drinking water supply and a potential source of contamination exists. Potential sources of contamination are isolated from the public water system by an air gap or an appropriate backflow prevention assembly in accordance with Commission regulations.',
        'No cross-connection between the public drinking water supply and a private water system exists. Where an actual air gap is not maintained between the public water supply and a private water supply, an approved reduced pressure principle backflow prevention assembly is properly installed.',
        'No connection exists which would allow the return of water used for condensing, cooling or industrial processes back to the public water supply.',
        'No pipe or pipe fitting which contains more than 8.0% lead exists in private water distribution facilities installed on or after July 1, 1988 and prior to January 4, 2014.',
        'Plumbing installed on or after January 4, 2014 bears the expected labeling indicating ≤0.25% lead content. If not properly labeled, please provide written comment.',
        'No solder or flux which contains more than 0.2% lead exists in private water distribution facilities installed on or after July 1, 1988.'
    ];

    private assembliesLoaded: boolean = false;
    private imagesLoaded: boolean = false;
    private logsLoaded: boolean = false;

    constructor(
        private readonly _windowReference: WindowReference<CsiInspection>,
        private readonly _inspectionService: CsiInspectionService,
        private readonly _lookupService: LookupService
    ) {

    }

    public async ngOnInit(): Promise<void> {
        this.id = this._windowReference.config.model?.id ?? 0;
        this.idPrefix = `csi-${this.id}`;

        this.assemblyColumns = this.getAssemblyColumns();
        this.logColumns = this.getLogColumns();

        await Promise.all([
            this.loadStates(),
            this.loadInspection(),
            this.loadCounts()
        ]);
    }

    public ngOnDestroy(): void {
        this.dismissSaveMessage();
    }

    public async onTabChange(tab: CsiInspectionTab): Promise<void> {
        this.selectedTab = tab;

        if (tab === 'assemblies' && !this.assembliesLoaded) {
            await this.loadAssemblies();
        }

        if (tab === 'images' && !this.imagesLoaded) {
            await this.loadImages();
        }

        if (tab === 'logs' && !this.logsLoaded) {
            await this.loadLogs();
        }
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

        this.logsLoaded = false;

        await this.loadCounts();

        if (this.selectedTab === 'logs') {
            await this.loadLogs();
        }

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

    public async onImageSelected(event: Event): Promise<void> {
        const input = event.target as HTMLInputElement;
        const file = input.files?.item(0);

        if (!file) {
            return;
        }

        try {
            this.isLoadingImages = true;
            const image = await this._inspectionService.addImage(this.id, file, '');
            this.images = [...this.images, image];
        } finally {
            this.isLoadingImages = false;
            input.value = '';
        }
    }

    public async deleteImage(image: CsiInspectionImage): Promise<void> {
        if (image.id == null) {
            return;
        }

        try {
            this.isLoadingImages = true;
            await this._inspectionService.deleteImage(this.id, image.id);
            this.images = this.images.filter(existing => existing.id !== image.id);
        } finally {
            this.isLoadingImages = false;
        }
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

    private async loadAssemblies(): Promise<void> {
        try {
            this.isLoadingAssemblies = true;
            this.assemblies = await this._inspectionService.getAssemblies(this.id);
        } finally {
            this.assembliesLoaded = true;
            this.isLoadingAssemblies = false;
        }
    }

    private async loadImages(): Promise<void> {
        try {
            this.isLoadingImages = true;
            this.images = await this._inspectionService.getImages(this.id);
        } finally {
            this.imagesLoaded = true;
            this.isLoadingImages = false;
        }
    }

    private async loadLogs(): Promise<void> {
        try {
            this.isLoadingLogs = true;

            const logs = await this._inspectionService.getLogs(this.id);

            this.logs = logs.map(log => ({
                ...log,
                logTypeLabel: log.logType == null ? '' : recordLogTypeLabels[log.logType]
            }));
        } finally {
            this.logsLoaded = true;
            this.isLoadingLogs = false;
        }
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

    private getAssemblyColumns(): TableColumn<CsiInspectionAssembly>[] {
        return [
            { field: 'id', caption: 'ID', type: ColumnType.number },
            { field: 'isCurrent', caption: 'Status', type: ColumnType.other, cellTemplate: this.assemblyStatusCell, queryColumnExcluded: true },
            { field: 'testDate', caption: 'Dates', type: ColumnType.other, cellTemplate: this.assemblyDatesCell, queryColumnExcluded: true },
            { field: 'serialNumber', caption: 'Serial #', type: ColumnType.text },
            { field: 'assemblyDescription', caption: 'Assembly Information', type: ColumnType.other, cellTemplate: this.assemblyInfoCell, queryColumnExcluded: true },
            { field: 'visuallyIdentified', caption: '', type: ColumnType.other, cellTemplate: this.assemblyIdentifiedCell, queryColumnExcluded: true }
        ];
    }

    private getLogColumns(): TableColumn<RecordLogRow>[] {
        return [
            { field: 'logDate', caption: 'Log Date', type: ColumnType.date },
            { field: 'user.email', caption: 'User ID', type: ColumnType.text },
            { field: 'logTypeLabel', caption: 'Type', type: ColumnType.text },
            { field: 'description', caption: 'Description', type: ColumnType.other, cellTemplate: this.logDescriptionCell, queryColumnExcluded: true }
        ];
    }
}
