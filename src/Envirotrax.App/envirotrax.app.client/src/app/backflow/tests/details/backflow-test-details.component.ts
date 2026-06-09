import { Component, OnInit, TemplateRef, ViewChild } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";
import { NgForm } from "@angular/forms";
import { BackflowTest } from "../../../shared/models/backflow/backflow-test";
import { BackflowTestService } from "../../../shared/services/backflow/backflow-test.service";
import { BackflowReasonForTest, BackflowTestResult } from "../../../shared/models/backflow/backflow-test-enums";
import { State } from "../../../shared/models/lookup/state";
import { LookupService } from "../../../shared/services/lookup/lookup.service";
import { InputOption } from "../../../shared/components/input/input.component";
import { AuthService } from "../../../shared/services/auth/auth.service";
import { PermissionAction, PermissionType } from "../../../shared/models/permission-type";
import { ToastService } from "../../../shared/services/toast.service";
import { HelperService } from "../../../shared/services/helpers/helper.service";
import { TableViewModel } from "../../../shared/models/table-view-model";
import { CellTemplateData, TableColumn } from "../../../shared/components/data-components/table/table.component";
import { ColumnType } from "../../../shared/components/data-components/sorting-filtering/query-view-model";
import { ComparisonOperator } from "../../../shared/models/query";

interface ImageSlot {
    type: string;
    pathKey: keyof BackflowTest;
    label: string;
    alt: string;
}

interface ValveCheck {
    label: string;
    field: keyof BackflowTest;
}

interface ValveColumn {
    index: number;
    group: 'main' | 'bypass';
    header: string;
    initialVerb: string;
    initialValue?: keyof BackflowTest;
    initialChecks: ValveCheck[];
    repairField?: keyof BackflowTest;
    repairDetailsField?: keyof BackflowTest;
    finalVerb?: string;
    finalValue?: keyof BackflowTest;
    finalChecks: ValveCheck[];
}

@Component({
    selector: 'app-backflow-test-details',
    standalone: false,
    templateUrl: './backflow-test-details.component.html',
})
export class BackflowTestDetailsComponent implements OnInit {
    public id: number = 0;
    public test: BackflowTest | null = null;
    public isLoading: boolean = false;
    public savingSection: string | null = null;
    public canModify: boolean = false;
    public canViewTesters: boolean = false;
    public states: InputOption<State>[] = [];
    public validationErrors: string[] = [];

    public gaugeDescription: string = '';
    public visibleValveColumns: ValveColumn[] = [];
    public showAssemblySubheader: boolean = false;
    public mainColumnCount: number = 0;
    public bypassColumnCount: number = 0;
    public showMeterTest: boolean = false;

    public readonly BackflowTestResult = BackflowTestResult;
    public readonly BackflowReasonForTest = BackflowReasonForTest;

    @ViewChild('historyStatusTemplate', { static: true })
    public historyStatusTemplate!: TemplateRef<CellTemplateData<BackflowTest>>;

    @ViewChild('historyDatesTemplate', { static: true })
    public historyDatesTemplate!: TemplateRef<CellTemplateData<BackflowTest>>;

    @ViewChild('historyAssemblyTemplate', { static: true })
    public historyAssemblyTemplate!: TemplateRef<CellTemplateData<BackflowTest>>;

    @ViewChild('historyBpatTemplate', { static: true })
    public historyBpatTemplate!: TemplateRef<CellTemplateData<BackflowTest>>;

    public historyTable: TableViewModel<BackflowTest> = {
        columns: [],
        query: {
            sort: { testDate: 'Desc' },
            filter: []
        }
    };

    public propertyTypeOptions: InputOption[] = [
        { id: 0, text: 'Residential' },
        { id: 1, text: 'Commercial' }
    ];

    public readonly assemblyImageSlots: ImageSlot[] = [
        { type: 'assembly', pathKey: 'assemblyImagePath', label: 'Upload an image of the entire assembly', alt: 'Assembly' },
        { type: 'serial-number', pathKey: 'serialNumberImagePath', label: 'Upload an image of the Serial Number of the assembly', alt: 'Serial number' }
    ];

    public readonly bypassImageSlots: ImageSlot[] = [
        { type: 'bypass-assembly', pathKey: 'bypassAssemblyImagePath', label: 'Upload an image of the entire Bypass assembly', alt: 'Bypass assembly' },
        { type: 'bypass-serial-number', pathKey: 'bypassSerialNumberImagePath', label: 'Upload an image of the Serial Number of the Bypass assembly', alt: 'Bypass serial number' }
    ];

    public readonly airGapImageSlots: ImageSlot[] = [
        { type: 'air-gap', pathKey: 'airGapImagePath', label: 'Upload an image of the air gap', alt: 'Air gap' }
    ];

    public imageUrls: Record<string, string | null> = {};
    public stagedImageFiles: Record<string, File | null> = {};
    public stagedImagePreviews: Record<string, string | null> = {};
    public savingImageTypes: string[] = [];

    private readonly _bypassDeviceTypes = ['DCD', 'DCD2', 'RPPD', 'RPPD2'];
    private readonly _meterTestDeviceTypes = ['DCD', 'DCD2', 'RPPD', 'RPPD2'];

    public hazardTypeOptions: InputOption[] = [
        { id: '', text: '' },
        { id: 'Agricultural/Feed Lot', text: 'Agricultural/Feed Lot' },
        { id: 'Domestic/Premises Isolation', text: 'Domestic/Premises Isolation' },
        { id: 'Fire System', text: 'Fire System' },
        { id: 'Gas Station/Car Wash', text: 'Gas Station/Car Wash' },
        { id: 'Irrigation - Non Chemical', text: 'Irrigation - Non Chemical' },
        { id: 'Irrigation - Chemical Feed', text: 'Irrigation - Chemical Feed' },
        { id: 'Laundry/Cleaners', text: 'Laundry/Cleaners' },
        { id: 'Medical/Dental/Laboratory/Mortuary', text: 'Medical/Dental/Laboratory/Mortuary' },
        { id: 'Nails/Salon/Grooming', text: 'Nails/Salon/Grooming' },
        { id: 'Pool/Recreation/Athletics', text: 'Pool/Recreation/Athletics' },
        { id: 'Restaurant/Vending/Grocery', text: 'Restaurant/Vending/Grocery' },
        { id: 'Fire Hydrant/Temporary Construction', text: 'Fire Hydrant/Temporary Construction' },
        { id: 'Fountains/Garden Ponds/Water Features', text: 'Fountains/Garden Ponds/Water Features' },
        { id: 'Water Softener', text: 'Water Softener' },
        { id: 'Other', text: 'Other' }
    ];

    private readonly _valveColumns: ValveColumn[] = [
        {
            index: 1, group: 'main', header: 'Check Valve #1',
            initialVerb: 'Held at', initialValue: 'initCV1HeldPSID',
            initialChecks: [{ label: 'Closed Tight', field: 'initCV1ClosedTight' }, { label: 'Leaked', field: 'initCV1Leaked' }],
            repairField: 'repairCV1', repairDetailsField: 'repairCV1Details',
            finalVerb: 'Held at', finalValue: 'finalCV1HeldPSID',
            finalChecks: [{ label: 'Closed Tight', field: 'finalCV1ClosedTight' }]
        },
        {
            index: 2, group: 'main', header: 'Check Valve #2 ***',
            initialVerb: 'Held at', initialValue: 'initCV2HeldPSID',
            initialChecks: [{ label: 'Closed Tight', field: 'initCV2ClosedTight' }, { label: 'Leaked', field: 'initCV2Leaked' }],
            repairField: 'repairCV2', repairDetailsField: 'repairCV2Details',
            finalVerb: 'Held at', finalValue: 'finalCV2HeldPSID',
            finalChecks: [{ label: 'Closed Tight', field: 'finalCV2ClosedTight' }]
        },
        {
            index: 3, group: 'main', header: 'Relief Valve',
            initialVerb: 'Opened at', initialValue: 'initRVOpenedPSID',
            initialChecks: [{ label: 'Did not open', field: 'initRVDidNotOpen' }],
            repairField: 'repairRV', repairDetailsField: 'repairRVDetails',
            finalVerb: 'Opened at', finalValue: 'finalRVOpenedPSID',
            finalChecks: []
        },
        {
            index: 4, group: 'bypass', header: 'Check Valve #1',
            initialVerb: 'Held at', initialValue: 'initCV1HeldPSID2',
            initialChecks: [{ label: 'Closed Tight', field: 'initCV1ClosedTight2' }, { label: 'Leaked', field: 'initCV1Leaked2' }],
            repairField: 'repairCV12', repairDetailsField: 'repairCV1Details2',
            finalVerb: 'Held at', finalValue: 'finalCV1HeldPSID2',
            finalChecks: [{ label: 'Closed Tight', field: 'finalCV1ClosedTight2' }]
        },
        {
            index: 5, group: 'bypass', header: 'Check Valve #2 ***',
            initialVerb: 'Held at', initialValue: 'initCV2HeldPSID2',
            initialChecks: [{ label: 'Closed Tight', field: 'initCV2ClosedTight2' }, { label: 'Leaked', field: 'initCV2Leaked2' }],
            repairField: 'repairCV22', repairDetailsField: 'repairCV2Details2',
            finalVerb: 'Held at', finalValue: undefined,
            finalChecks: [{ label: 'Closed Tight', field: 'finalCV2ClosedTight2' }]
        },
        {
            index: 6, group: 'bypass', header: 'Relief Valve',
            initialVerb: 'Opened at', initialValue: 'initRVOpenedPSID2',
            initialChecks: [{ label: 'Did not open', field: 'initRVDidNotOpen2' }],
            repairField: 'repairRV2', repairDetailsField: 'repairRVDetails2',
            finalVerb: 'Opened at', finalValue: 'finalRVOpenedPSID2',
            finalChecks: []
        },
        {
            index: 7, group: 'bypass', header: 'Bypass Check',
            initialVerb: 'Held at', initialValue: 'initBCHeldPSID',
            initialChecks: [{ label: 'Closed Tight', field: 'initBCClosedTight' }, { label: 'Leaked', field: 'initBCLeaked' }],
            repairField: 'repairBC', repairDetailsField: 'repairBCDetails',
            finalVerb: 'Held at', finalValue: 'finalBCHeldPSID',
            finalChecks: [{ label: 'Closed Tight', field: 'finalBCClosedTight' }]
        },
        {
            index: 8, group: 'main', header: 'Air Inlet',
            initialVerb: 'Opened at', initialValue: 'initPvbAirInletOpenedPSID',
            initialChecks: [{ label: 'Did not open', field: 'initPvbAirInletDidNotOpen' }, { label: 'Did it fully open', field: 'initPvbAirInletFullyOpened' }],
            finalVerb: 'Opened at', finalValue: 'finalPvbAirInletOpenedPSID',
            finalChecks: [{ label: 'Did it fully open', field: 'finalPvbAirInletFullyOpened' }]
        },
        {
            index: 9, group: 'main', header: 'Check Valve',
            initialVerb: 'Held at', initialValue: 'initPvbCVHeldPSID',
            initialChecks: [{ label: 'Leaked', field: 'initPvbCVLeaked' }],
            finalVerb: 'Held at', finalValue: 'finalPvbCVHeldPSID',
            finalChecks: []
        }
    ];

    private readonly _deviceColumnMap: Record<string, number[]> = {
        DC: [1, 2],
        DCD: [1, 2, 4, 5],
        DCD2: [1, 2, 7],
        RP: [1, 2, 3],
        RPPD: [1, 2, 3, 4, 5, 6],
        RPPD2: [1, 2, 3, 7],
        PVB: [8, 9],
        SVB: [8, 9]
    };

    private readonly _propertyTypeLabels: Record<number, string> = {
        0: 'Residential',
        1: 'Commercial'
    };

    private readonly _reasonForTestLabels: Record<number, string> = {
        [BackflowReasonForTest.AnnualTest]: 'Annual Test',
        [BackflowReasonForTest.NewInstallation]: 'New Installation',
        [BackflowReasonForTest.ExistingInstallation]: 'Existing Installation',
        [BackflowReasonForTest.Replacement]: 'Replacement',
        [BackflowReasonForTest.Repair]: 'Repair',
        [BackflowReasonForTest.AnnualTestAfterRepairs]: 'Annual Test After Repairs'
    };

    private readonly _testResultLabels: Record<number, string> = {
        [BackflowTestResult.Pass]: 'Passed',
        [BackflowTestResult.Fail]: 'Failed',
        [BackflowTestResult.PassAfterRepairs]: 'Passed After Repairs'
    };

    private readonly _deviceTypeLabels: Record<string, string> = {
        DC: 'DC - Double Check Valve',
        DCD: 'DCD - Double Check Detector',
        DCD2: 'DCD2 - Double Check Detector Type II',
        RP: 'RP - Reduced Pressure Principle',
        RPPD: 'RPPD - Reduced Pressure Principle Detector',
        RPPD2: 'RPPD2 - Reduced Pressure Principle Detector Type II',
        PVB: 'PVB - Pressure Vacuum Breaker',
        SVB: 'SVB - Spill-Resistant Pressure Vacuum Breaker',
        AG: 'AG - Air Gap'
    };

    constructor(
        private readonly _activatedRoute: ActivatedRoute,
        private readonly _router: Router,
        private readonly _testService: BackflowTestService,
        private readonly _lookupService: LookupService,
        private readonly _authService: AuthService,
        private readonly _toastService: ToastService,
        private readonly _helper: HelperService
    ) { }

    public async ngOnInit(): Promise<void> {
        await this.initialize();
    }

    private async initialize(): Promise<void> {
        const [states, canModify, canViewTesters] = await Promise.all([
            this._lookupService.getAllStatesAsOptions(true),
            this._authService.hasAnyPermisison(PermissionAction.CanModify, PermissionType.BackflowTests),
            this._authService.hasAnyPermisison(PermissionAction.CanView, PermissionType.BackflowTesters)
        ]);

        this.states = states;
        this.canModify = canModify;
        this.canViewTesters = canViewTesters;
        this.historyTable.columns = this.getHistoryColumns();

        this._activatedRoute.paramMap.subscribe(async params => {
            const id = params.get('id');

            if (id) {
                this.id = +id;
                await this.loadTest();
            }
        });
    }

    private async loadTest(): Promise<void> {
        try {
            this.isLoading = true;
            this.test = await this._testService.get(this.id);
            this.updateDisplayState();
            await this.loadImageUrls();
            await this.loadHistory();
        } finally {
            this.isLoading = false;
        }
    }

    public viewHistoryTest(test: BackflowTest): void {
        if (test?.id == null) {
            return;
        }
        const url = this._router.serializeUrl(
            this._router.createUrlTree(['/backflow', 'tests', test.id, 'view'])
        );
        window.open(url, '_blank');
    }

    public async loadHistoryPage(): Promise<void> {
        await this.loadHistory();
    }

    private async loadHistory(): Promise<void> {
        const siteId = this.test?.site?.id;
        if (siteId == null) {
            return;
        }

        try {
            this.historyTable.isLoading = true;
            this.historyTable.query.filter = [{
                columnName: 'site.id',
                value: siteId.toString(),
                comparisonOperator: 'Eq' as ComparisonOperator
            }];
            this.historyTable.items = await this._testService.getAll(
                this.historyTable.items?.pageInfo || {},
                this.historyTable.query
            );
        } finally {
            this.historyTable.isLoading = false;
        }
    }

    private getHistoryColumns(): TableColumn<BackflowTest>[] {
        return [
            {
                field: '',
                caption: 'Status',
                type: ColumnType.other,
                queryColumnExcluded: true,
                cellTemplate: this.historyStatusTemplate
            },
            {
                field: '',
                caption: 'Dates',
                type: ColumnType.other,
                queryColumnExcluded: true,
                cellTemplate: this.historyDatesTemplate
            },
            {
                field: 'serialNumber',
                caption: 'Serial #',
                type: ColumnType.text
            },
            {
                field: '',
                caption: 'Assembly Description',
                type: ColumnType.other,
                queryColumnExcluded: true,
                cellTemplate: this.historyAssemblyTemplate
            },
            {
                field: '',
                caption: 'BPAT Information',
                type: ColumnType.other,
                queryColumnExcluded: true,
                cellTemplate: this.historyBpatTemplate
            }
        ];
    }

    public get showBypassImages(): boolean {
        return !!this.test?.deviceType && this._bypassDeviceTypes.includes(this.test.deviceType);
    }

    public get showAirGapImages(): boolean {
        return this.test?.deviceType === 'AG';
    }

    private get visibleImageSlots(): ImageSlot[] {
        if (this.showAirGapImages) {
            return [...this.airGapImageSlots];
        }

        const slots = [...this.assemblyImageSlots];
        if (this.showBypassImages) {
            slots.push(...this.bypassImageSlots);
        }
        return slots;
    }

    private async loadImageUrls(): Promise<void> {
        if (this.test == null) {
            return;
        }

        await Promise.all(this.visibleImageSlots.map(async slot => {
            const path = this.test![slot.pathKey];
            if (path) {
                try {
                    this.imageUrls[slot.type] = await this._testService.getImageUrl(this.id, slot.type);
                } catch {
                    this.imageUrls[slot.type] = null;
                }
            } else {
                this.imageUrls[slot.type] = null;
            }
        }));
    }

    public onImageFileChange(file: File | null, type: string): void {
        this.clearStagedPreview(type);

        if (!file) {
            this.stagedImageFiles[type] = null;
            return;
        }

        this.stagedImageFiles[type] = file;
        this.stagedImagePreviews[type] = URL.createObjectURL(file);
    }

    public onImageInputChange(event: Event, type: string): void {
        const input = event.target as HTMLInputElement;
        const file = input.files?.[0] ?? null;
        input.value = '';

        if (file) {
            this.onImageFileChange(file, type);
        }
    }

    public removeStagedImage(type: string): void {
        this.clearStagedPreview(type);
        this.stagedImageFiles[type] = null;
    }

    public hasStagedImages(slots: ImageSlot[]): boolean {
        return slots.some(slot => this.stagedImageFiles[slot.type] != null);
    }

    public isSavingImages(slots: ImageSlot[]): boolean {
        return slots.some(slot => this.savingImageTypes.includes(slot.type));
    }

    public async saveImages(slots: ImageSlot[]): Promise<void> {
        if (this.test == null || this.savingImageTypes.length > 0) {
            return;
        }

        const staged = slots.filter(slot => this.stagedImageFiles[slot.type] != null);
        if (staged.length === 0) {
            return;
        }

        try {
            this.savingImageTypes = staged.map(slot => slot.type);

            for (const slot of staged) {
                const file = this.stagedImageFiles[slot.type]!;
                await this._testService.uploadImage(this.id, slot.type, file);
                this.imageUrls[slot.type] = await this._testService.getImageUrl(this.id, slot.type);
                this.removeStagedImage(slot.type);
            }

            this._toastService.successfullySaved('Images');
        } catch (e) {
            this._toastService.failedToSave('Images');
            throw e;
        } finally {
            this.savingImageTypes = [];
        }
    }

    private clearStagedPreview(type: string): void {
        const preview = this.stagedImagePreviews[type];
        if (preview) {
            URL.revokeObjectURL(preview);
        }
        this.stagedImagePreviews[type] = null;
    }

    public propertyStateChanged(stateId: number): void {
        if (this.test == null) {
            return;
        }
        this.test.propertyState = stateId ? { id: stateId } : null;
    }

    public mailingStateChanged(stateId: number): void {
        if (this.test == null) {
            return;
        }
        this.test.mailingState = stateId ? { id: stateId } : null;
    }

    public copyFromPropertyAddress(): void {
        if (this.test == null) {
            return;
        }

        this.test.mailingStreetNumber = this.test.propertyStreetNumber;
        this.test.mailingStreetName = this.test.propertyStreetName;
        this.test.mailingNumber = this.test.propertyNumber;
        this.test.mailingCity = this.test.propertyCity;
        this.test.mailingState = this.test.propertyState ? { ...this.test.propertyState } : null;
        this.test.mailingZip = this.test.propertyZip;
    }

    public viewPropertyRecord(): void {
        if (this.test?.site?.id == null) {
            return;
        }
        this._router.navigate(['/sites', this.test.site.id, 'edit']);
    }

    public async save(form: NgForm, entityName: string): Promise<void> {
        if (this.test == null || !this.canModify || !form.valid) {
            return;
        }

        try {
            this.savingSection = entityName;
            this.validationErrors = [];
            await this._testService.update(this.test);
            this.test = await this._testService.get(this.id);
            this.updateDisplayState();
            this._toastService.successfullySaved(entityName);
        } catch (e) {
            if (!this._helper.parseValidationErrors(e, this.validationErrors)) {
                throw e;
            }
            this._toastService.failedToSave(entityName);
        } finally {
            this.savingSection = null;
        }
    }

    public getPropertyTypeLabel(): string {
        if (this.test?.propertyType == null) {
            return '';
        }
        return this._propertyTypeLabels[this.test.propertyType] ?? '';
    }

    public getReasonForTestLabel(): string {
        if (this.test?.reasonForTest == null) {
            return '';
        }
        return this._reasonForTestLabels[this.test.reasonForTest] ?? '';
    }

    public getTestResultLabel(): string {
        if (this.test?.testResult == null) {
            return '';
        }
        return this._testResultLabels[this.test.testResult] ?? '';
    }

    public getDeviceTypeLabel(): string {
        if (!this.test?.deviceType) {
            return '';
        }
        return this._deviceTypeLabels[this.test.deviceType] ?? this.test.deviceType;
    }

    private updateDisplayState(): void {
        const test = this.test;

        if (test == null) {
            this.gaugeDescription = '';
            this.visibleValveColumns = [];
            this.showAssemblySubheader = false;
            this.mainColumnCount = 0;
            this.bypassColumnCount = 0;
            this.showMeterTest = false;
            return;
        }

        const gaugeBase = [test.gaugeManufacturer, test.gaugeModel].filter(Boolean).join(' ');
        const gaugeSuffix = test.gaugeNonPotable ? ' (non-potable)' : ' (potable)';
        this.gaugeDescription = gaugeBase ? gaugeBase + gaugeSuffix : '';

        const indices = (test.deviceType && this._deviceColumnMap[test.deviceType]) || [];
        this.visibleValveColumns = this._valveColumns.filter(c => indices.includes(c.index));

        this.showAssemblySubheader = !!test.deviceType && this._bypassDeviceTypes.includes(test.deviceType);
        this.showMeterTest = !!test.deviceType && this._meterTestDeviceTypes.includes(test.deviceType);

        this.mainColumnCount = this.visibleValveColumns.filter(c => c.group === 'main').length;
        this.bypassColumnCount = this.visibleValveColumns.filter(c => c.group === 'bypass').length;
    }

    public field(name: keyof BackflowTest | undefined): unknown {
        if (!name || this.test == null) {
            return null;
        }
        return this.test[name];
    }

    public checked(name: keyof BackflowTest | undefined): boolean {
        return !!this.field(name);
    }
}
