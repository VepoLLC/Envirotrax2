import { formatDate } from "@angular/common";
import { Component, OnDestroy, OnInit, TemplateRef, ViewChild } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";
import { Subscription } from "rxjs";
import { CellTemplateData, ColumnType, InputOption, ModalHelperService, TableColumn } from "@envirotrax/common-ui";
import { BackflowTest } from "../../../../shared/models/backflow/backflow-test";
import { BackflowOutOfServiceRequest } from "../../../../shared/models/backflow/backflow-out-of-service-request";
import { OutOfServiceType } from "../../../../shared/models/backflow/out-of-service-type.enum";
import { BackflowTestService } from "../../../../shared/services/backflow/backflow-test.service";
import { BackflowOutOfServiceRequestService } from "../../../../shared/services/backflow/backflow-out-of-service-request.service";
import { ToastService } from "../../../../shared/services/toast.service";
import { HelperService } from "../../../../shared/services/helpers/helper.service";

interface ReplacementCandidateVm {
    id?: number;
    testDate?: string;
    serialNumber?: string;
    deviceDescription: string;
    propertyAddress: string;
}

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

    public candidates: ReplacementCandidateVm[] = [];
    public candidatesLoaded: boolean = false;
    public selectedCandidate: ReplacementCandidateVm | null = null;
    public candidateColumns: TableColumn<ReplacementCandidateVm>[] = [];

    public readonly reasonOptions: InputOption[] = [
        { id: '', text: '' },
        { id: OutOfServiceType.Replaced.toString(), text: 'The assembly was replaced by another assembly' },
        { id: OutOfServiceType.Removed.toString(), text: 'The assembly was removed' }
    ];

    @ViewChild('selectCell', { static: true })
    private selectCellTemplate!: TemplateRef<CellTemplateData<ReplacementCandidateVm>>;

    private _routeSub?: Subscription;

    constructor(
        private readonly _activatedRoute: ActivatedRoute,
        private readonly _router: Router,
        private readonly _testService: BackflowTestService,
        private readonly _requestService: BackflowOutOfServiceRequestService,
        private readonly _modalHelper: ModalHelperService,
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

    public selectCandidate(candidate: ReplacementCandidateVm): void {
        this.selectedCandidate = candidate;
        this.request.replacementAssemblyTestId = candidate.id ?? null;
    }

    public submit(): void {
        if (!this.canContinue) {
            return;
        }

        this._modalHelper.confirm({
            title: 'Confirm Out of Service Request',
            messages: this.buildConfirmationMessages()
        }).result().subscribe(() => this.completeSubmission());
    }

    public async cancel(): Promise<void> {
        await this._router.navigate(['/professionals/backflow/tests', this.id, 'view']);
    }

    private buildConfirmationMessages(): string[] {
        if (this.isReplaced && this.selectedCandidate) {
            const testDate = this.selectedCandidate.testDate
                ? formatDate(this.selectedCandidate.testDate, 'MM/dd/yyyy', 'en-US')
                : '';

            return [
                'This assembly will be marked out of service because it was replaced by the following assembly:',
                `Test Date: ${testDate}`,
                `Serial Number: ${this.selectedCandidate.serialNumber ?? ''}`,
                `Device Description: ${this.selectedCandidate.deviceDescription}`,
                `Property Address: ${this.selectedCandidate.propertyAddress}`
            ];
        }

        return [
            'This assembly will be marked out of service because it was removed.',
            `Reason for removal: ${this.request.description ?? ''}`
        ];
    }

    private async completeSubmission(): Promise<void> {
        try {
            this.isLoading = true;
            this.validationErrors = [];

            await this._requestService.submit(this.request);

            this._toastService.successfullySaved('Out of Service Request');
            await this._router.navigate(['/professionals/backflow/tests', this.id, 'view']);
        } catch (error) {
            if (!this._helper.parseValidationErrors(error, this.validationErrors)) {
                this._toastService.failedToSave('Out of Service Request');
                throw error;
            }

            this._toastService.failedToSave('Out of Service Request');
        } finally {
            this.isLoading = false;
        }
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
            const results = await this._requestService.getReplacementCandidates(this.id);
            this.candidates = results.map(test => this.toCandidateVm(test));
            this.candidatesLoaded = true;
        } finally {
            this.isLoading = false;
        }
    }

    private toCandidateVm(test: BackflowTest): ReplacementCandidateVm {
        return {
            id: test.id,
            testDate: test.testDate,
            serialNumber: test.serialNumber,
            deviceDescription: this.buildDeviceDescription(test),
            propertyAddress: this.buildPropertyAddress(test)
        };
    }

    // Matches V1's BackflowTest.DeviceDescription: "Manufacturer Model Size - DeviceType".
    private buildDeviceDescription(test: BackflowTest): string {
        const identity = [test.manufacturer, test.model, test.size]
            .filter(part => part)
            .join(' ');

        return test.deviceType ? `${identity} - ${test.deviceType}` : identity;
    }

    private buildPropertyAddress(test: BackflowTest): string {
        const line1 = [test.propertyStreetNumber, test.propertyStreetName]
            .filter(part => part)
            .join(' ');
        const line2 = [test.propertyCity, test.propertyState?.code, test.propertyZip]
            .filter(part => part)
            .join(' ');

        return [line1, line2].filter(part => part).join(', ');
    }

    private getCandidateColumns(): TableColumn<ReplacementCandidateVm>[] {
        return [
            {
                field: 'testDate',
                caption: 'Test Date',
                type: ColumnType.date
            },
            {
                field: 'serialNumber',
                caption: 'Serial Number',
                type: ColumnType.text
            },
            {
                field: 'deviceDescription',
                caption: 'Device Description',
                type: ColumnType.text
            },
            {
                field: 'propertyAddress',
                caption: 'Property Address',
                type: ColumnType.text
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
