import { Component, OnDestroy, OnInit, TemplateRef, ViewChild } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";
import { Subscription } from "rxjs";
import { CellTemplateData, ColumnType, InputOption, TableColumn } from "@envirotrax/common-ui";
import { BackflowTest } from "../../../../shared/models/backflow/backflow-test";
import { BackflowOutOfServiceRequest } from "../../../../shared/models/backflow/backflow-out-of-service-request";
import { OutOfServiceType } from "../../../../shared/models/backflow/out-of-service-type.enum";
import { BackflowTestService } from "../../../../shared/services/backflow/backflow-test.service";
import { BackflowOutOfServiceRequestService } from "../../../../shared/services/backflow/backflow-out-of-service-request.service";
import { ToastService } from "../../../../shared/services/toast.service";
import { HelperService } from "../../../../shared/services/helpers/helper.service";

@Component({
    standalone: false,
    templateUrl: './backflow-out-of-service-request.component.html'
})
export class BackflowOutOfServiceRequestComponent implements OnInit, OnDestroy {
    public readonly outOfServiceType = OutOfServiceType;

    public id: number = 0;
    public test: BackflowTest | null = null;
    public isLoading: boolean = false;
    public validationErrors: string[] = [];

    public selectedType: string = '';
    public request: BackflowOutOfServiceRequest = { testId: 0 };

    public assemblyDescription: string = '';
    public showConfirmModal: boolean = false;

    public candidates: BackflowTest[] = [];
    public candidatesLoaded: boolean = false;
    public selectedCandidate: BackflowTest | null = null;
    public candidateColumns: TableColumn<BackflowTest>[] = [];

    public readonly reasonOptions: InputOption[] = [
        { id: '', text: '' },
        { id: OutOfServiceType.Replaced.toString(), text: 'The assembly was replaced by another assembly' },
        { id: OutOfServiceType.Removed.toString(), text: 'The assembly was removed' }
    ];

    @ViewChild('testDateCell', { static: true })
    private testDateCellTemplate!: TemplateRef<CellTemplateData<BackflowTest>>;

    @ViewChild('descriptionCell', { static: true })
    private descriptionCellTemplate!: TemplateRef<CellTemplateData<BackflowTest>>;

    @ViewChild('addressCell', { static: true })
    private addressCellTemplate!: TemplateRef<CellTemplateData<BackflowTest>>;

    @ViewChild('selectCell', { static: true })
    private selectCellTemplate!: TemplateRef<CellTemplateData<BackflowTest>>;

    private _routeSub?: Subscription;

    constructor(
        private readonly _activatedRoute: ActivatedRoute,
        private readonly _router: Router,
        private readonly _testService: BackflowTestService,
        private readonly _requestService: BackflowOutOfServiceRequestService,
        private readonly _toastService: ToastService,
        private readonly _helper: HelperService
    ) { }

    public ngOnInit(): void {
        this.candidateColumns = this.getCandidateColumns();

        this._routeSub = this._activatedRoute.paramMap.subscribe(async params => {
            const idParam = params.get('id');

            if (idParam) {
                this.id = +idParam;
                this.request = { testId: this.id };
                await this.loadTest();
            }
        });
    }

    public ngOnDestroy(): void {
        this._routeSub?.unsubscribe();
    }

    public get isReplaced(): boolean {
        return this.request.type === OutOfServiceType.Replaced;
    }

    public get isRemoved(): boolean {
        return this.request.type === OutOfServiceType.Removed;
    }

    public get canContinue(): boolean {
        if (this.isReplaced) {
            return this.request.replacementAssemblyTestId != null;
        }

        if (this.isRemoved) {
            return !!this.request.description && this.request.description.trim().length > 0;
        }

        return false;
    }

    public async onTypeChange(value: string): Promise<void> {
        this.selectedType = value;

        if (value === '') {
            this.request.type = undefined;
        } else {
            this.request.type = Number(value) as OutOfServiceType;
        }

        // Reset conditional inputs when the reason changes.
        this.request.replacementAssemblyTestId = null;
        this.request.description = undefined;
        this.selectedCandidate = null;

        if (this.isReplaced && !this.candidatesLoaded) {
            await this.loadCandidates();
        }
    }

    public selectCandidate(candidate: BackflowTest): void {
        this.selectedCandidate = candidate;
        this.request.replacementAssemblyTestId = candidate.id;
    }

    public openConfirmModal(): void {
        if (this.canContinue) {
            this.showConfirmModal = true;
        }
    }

    public closeConfirmModal(): void {
        this.showConfirmModal = false;
    }

    public async completeSubmission(): Promise<void> {
        try {
            this.isLoading = true;
            this.validationErrors = [];
            await this._requestService.submit(this.request);
            this.showConfirmModal = false;
            this._toastService.successfullySaved('Out of Service Request');
            await this._router.navigate(['/professionals/backflow/tests', this.id, 'view']);
        } catch (error) {
            this.showConfirmModal = false;

            if (!this._helper.parseValidationErrors(error, this.validationErrors)) {
                this._toastService.failedToSave('Out of Service Request');
                throw error;
            }

            this._toastService.failedToSave('Out of Service Request');
        } finally {
            this.isLoading = false;
        }
    }

    public async cancel(): Promise<void> {
        await this._router.navigate(['/professionals/backflow/tests', this.id, 'view']);
    }

    public buildCandidateDescription(candidate: BackflowTest): string {
        return this.buildDeviceDescription(candidate);
    }

    public buildCandidateAddress(candidate: BackflowTest): string {
        const line1 = [candidate.propertyStreetNumber, candidate.propertyStreetName]
            .filter(part => part)
            .join(' ');
        const line2 = [candidate.propertyCity, candidate.propertyState?.code, candidate.propertyZip]
            .filter(part => part)
            .join(' ');

        return [line1, line2].filter(part => part).join(', ');
    }

    private async loadTest(): Promise<void> {
        try {
            this.isLoading = true;
            this.test = await this._testService.getForProfessional(this.id);
            this.assemblyDescription = this.test ? this.buildDeviceDescription(this.test) : '';
        } finally {
            this.isLoading = false;
        }
    }

    private async loadCandidates(): Promise<void> {
        try {
            this.isLoading = true;
            this.candidates = await this._requestService.getReplacementCandidates(this.id);
            this.candidatesLoaded = true;
        } finally {
            this.isLoading = false;
        }
    }

    // Matches V1's BackflowTest.DeviceDescription: "Manufacturer Model Size - DeviceType".
    private buildDeviceDescription(test: BackflowTest): string {
        const identity = [test.manufacturer, test.model, test.size]
            .filter(part => part)
            .join(' ');

        return test.deviceType ? `${identity} - ${test.deviceType}` : identity;
    }

    private getCandidateColumns(): TableColumn<BackflowTest>[] {
        return [
            {
                field: 'testDate',
                caption: 'Test Date',
                type: ColumnType.other,
                queryColumnExcluded: true,
                cellTemplate: this.testDateCellTemplate
            },
            {
                field: 'serialNumber',
                caption: 'Serial Number',
                type: ColumnType.text
            },
            {
                field: '',
                caption: 'Device Description',
                type: ColumnType.other,
                queryColumnExcluded: true,
                cellTemplate: this.descriptionCellTemplate
            },
            {
                field: '',
                caption: 'Property Address',
                type: ColumnType.other,
                queryColumnExcluded: true,
                cellTemplate: this.addressCellTemplate
            },
            {
                field: 'Select',
                caption: '',
                type: ColumnType.other,
                queryColumnExcluded: true,
                headerCssClass: 'text-center',
                cellTemplate: this.selectCellTemplate
            }
        ];
    }
}
