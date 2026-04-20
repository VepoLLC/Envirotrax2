import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { NgForm } from '@angular/forms';
import { CsiSubmissionService } from '../../../shared/services/csi/csi-submission.service';
import {
    CsiAccountOption,
    CsiInspectorData,
    CsiLicenseData,
    CsiSubmissionCreateViewModel,
    CsiSubmissionSaveRequest,
    CsiWaterSupplierOption
} from '../../../shared/models/csi/csi-submission-create-view-model';
import { CsiInspectionReason, csiInspectionReasonLabels } from '../../../shared/enums/csi-inspection-reason.enum';
import { InputOption } from '../../../shared/components/input/input.component';

@Component({
    standalone: false,
    templateUrl: './csi-submission-create.component.html'
})
export class CsiSubmissionCreateComponent implements OnInit {
    public isLoading = true;
    public isSaving = false;
    public isSubmitted = false;
    public viewModel?: CsiSubmissionCreateViewModel;
    public activeTab: 'main' | 'assemblies' | 'additional' | 'images' = 'main';
    public errorMessage = '';
    public submitted = false;
    public validationErrors: string[] = [];

    public selectedCsiAccountUserId!: number;
    public selectedWaterSupplierId?: number;
    public legalAcknowledgment = false;

    // null = not yet selected, true = Compliance, false = Non-compliance
    public complianceValues: (boolean | null)[] = [null, null, null, null, null, null];

    public form: Omit<CsiSubmissionSaveRequest,
        'siteId' | 'waterSupplierId' | 'selectedCsiAccountUserId' |
        'compliance1' | 'compliance2' | 'compliance3' |
        'compliance4' | 'compliance5' | 'compliance6' | 'reasonForInspection'> & {
        reasonForInspection: CsiInspectionReason | null
    } = {
        inspectionDate: undefined,
        reasonForInspection: null,
        materialServiceLineLead: false,
        materialServiceLineCopper: false,
        materialServiceLinePVC: false,
        materialServiceLineOther: false,
        materialServiceLineOtherDescription: undefined,
        materialSolderLead: false,
        materialSolderLeadFree: false,
        materialSolderSolventWeld: false,
        materialSolderOther: false,
        materialSolderOtherDescription: undefined,
        comments: undefined,
    };

    public readonly reasonOptions: InputOption[] = [
        { id: CsiInspectionReason.NewConstruction, text: csiInspectionReasonLabels[CsiInspectionReason.NewConstruction] },
        { id: CsiInspectionReason.ExistingServiceContaminantHazardsSuspected, text: csiInspectionReasonLabels[CsiInspectionReason.ExistingServiceContaminantHazardsSuspected] },
        { id: CsiInspectionReason.MajorRenovationOrExpansion, text: csiInspectionReasonLabels[CsiInspectionReason.MajorRenovationOrExpansion] }
    ];

    public readonly complianceStatements: string[] = [
        'No direct or indirect connection between the public drinking water supply and a potential source of contamination exists. Potential sources of contamination are isolated from the public water system by an air gap or an appropriate backflow prevention assembly in accordance with Commission regulations.',
        'No cross-connection between the public drinking water supply and a private water system exists. Where an actual air gap is not maintained between the public water supply and a private water supply, an approved reduced pressure principle backflow prevention assembly is properly installed.',
        'No connection exists which would allow the return of water used for condensing, cooling or industrial processes back to the public water supply.',
        'No pipe or pipe fitting which contains more than 8.0% lead exists in private water distribution facilities installed on or after July 1, 1988 and prior to January 4, 2014.',
        'Plumbing installed on or after January 4, 2014 bears the expected labeling indicating \u22640.25% lead content. If not properly labeled, please provide written comment.',
        'No solder or flux which contains more than 0.2% lead exists in private water distribution facilities installed on or after July 1, 1988.'
    ];

    private _siteId!: number;

    constructor(
        private readonly _activatedRoute: ActivatedRoute,
        private readonly _router: Router,
        private readonly _submissionService: CsiSubmissionService
    ) {}

    public async ngOnInit(): Promise<void> {
        await this.initialize();
    }

    private async initialize(): Promise<void>{
        const idParam = this._activatedRoute.snapshot.paramMap.get('siteId');
        this._siteId = idParam ? Number(idParam) : 0;

        if(this._siteId <= 0){
            this.isLoading = false;
            return;
        }

        await this.loadViewModel();
    }

    public get selectedCsiAccount(): CsiAccountOption | undefined {
        return this.viewModel?.availableCsiAccounts.find(a => a.userId === this.selectedCsiAccountUserId);
    }

    public get currentLicense(): CsiLicenseData | undefined {
        return this.selectedCsiAccount?.csiLicense;
    }

    public get hasValidLicense(): boolean {
        return this.currentLicense?.isValid === true;
    }

    public get currentInspector(): CsiInspectorData | undefined {
    if (!this.viewModel?.inspector) {
        return undefined;
    }

    const inspector = this.viewModel.inspector;
    const selectedInspector = this.selectedCsiAccount;

    return {
        ...inspector,
        contactName: selectedInspector?.contactName,
        jobTitle: selectedInspector?.jobTitle,
        emailAddress: selectedInspector?.emailAddress,
    };
}

    public get selectedWaterSupplier(): CsiWaterSupplierOption | undefined {
        return this.viewModel?.availableWaterSuppliers.find(s => s.id === this.selectedWaterSupplierId);
    }

    public waterSupplierOptions: InputOption[] = [];
    public csiAccountOptions: InputOption[] = [];

    public get licenseStatusText(): string {
        if (!this.currentLicense){
           return 'No license found'; 
        }
        return this.currentLicense.isValid ? 'License valid' : 'License expired';
    }

    public get licenseStatusClass(): string {
        if (!this.currentLicense){
            return 'text-danger';
        }
        return this.currentLicense.isValid ? 'text-success' : 'text-danger';
    }

    public get remarksLength(): number {
        return this.form.comments?.length ?? 0;
    }

    public get complianceIsInvalid(): boolean {
        return this.submitted && this.complianceValues.some(v => v === null);
    }

    public get serviceLineIsInvalid(): boolean {
        return this.submitted &&
            !this.form.materialServiceLineLead &&
            !this.form.materialServiceLineCopper &&
            !this.form.materialServiceLinePVC &&
            !this.form.materialServiceLineOther;
    }

    public get solderIsInvalid(): boolean {
        return this.submitted &&
            !this.form.materialSolderLead &&
            !this.form.materialSolderLeadFree &&
            !this.form.materialSolderSolventWeld &&
            !this.form.materialSolderOther;
    }

    public onCsiAccountChange(value: number): void {
        this.selectedCsiAccountUserId = value;
    }

    public async submit(submitForm: NgForm): Promise<void> {
        this.resetValidation();
        this.collectValidationErrors();

        if (!submitForm.valid || this.validationErrors.length > 0) {
            return;
        }

        if (!this.selectedWaterSupplierId) {
            this.errorMessage = 'Please select a water supplier.';
            return;
        }

        this.isSaving = true;
        try {
            await this._submissionService.submit(this.buildRequest());
            this.isSubmitted = true;
        } finally {
            this.isSaving = false;
        }
    }

    private resetValidation(): void {
        this.submitted = true;
        this.errorMessage = '';
        this.validationErrors = [];
    }

    private collectValidationErrors(): void {
        if (this.form.inspectionDate && new Date(this.form.inspectionDate) > new Date()) {
            this.validationErrors.push('Inspection Date cannot be in the future.');
        }
        if (this.complianceValues.some(v => v === null)) {
            this.validationErrors.push('Please answer all 6 compliance items before submitting.');
        }
        if (this.serviceLineIsInvalid) {
            this.validationErrors.push('Please select at least one Service Line material.');
        }
        if (this.solderIsInvalid) {
            this.validationErrors.push('Please select at least one Solder material.');
        }
        if (!this.legalAcknowledgment) {
            this.validationErrors.push('You must acknowledge the legal statement before submitting.');
        }
    }

    private buildRequest(): CsiSubmissionSaveRequest {
        return {
            siteId: this._siteId,
            waterSupplierId: this.selectedWaterSupplierId!,
            selectedCsiAccountUserId: this.selectedCsiAccountUserId,
            ...this.form,
            reasonForInspection: this.form.reasonForInspection!,
            compliance1: this.complianceValues[0] === true,
            compliance2: this.complianceValues[1] === true,
            compliance3: this.complianceValues[2] === true,
            compliance4: this.complianceValues[3] === true,
            compliance5: this.complianceValues[4] === true,
            compliance6: this.complianceValues[5] === true,
        };
    }

    public returnToAccountOverview(): void {
        this._router.navigate(['/']);
    }

    public submitAnother(): void {
        this._router.navigate(['..'], { relativeTo: this._activatedRoute });
    }

    private async loadViewModel(): Promise<void> {
        try {
            this.isLoading = true;
            this.viewModel = await this._submissionService.getCreateViewModel(this._siteId);
            this.csiAccountOptions = (this.viewModel.availableCsiAccounts ?? []).map(a => ({
                id: a.userId,
                text: a.contactName ?? `User ${a.userId}`
            }));
            this.waterSupplierOptions = (this.viewModel.availableWaterSuppliers ?? []).map(s => ({
                id: s.id,
                text: s.name
            }));

            const accounts = this.viewModel.availableCsiAccounts;
            const defaultId = this.viewModel.defaultCsiAccountUserId;
            const defaultInList = accounts.find(a => a.userId === defaultId);
            this.selectedCsiAccountUserId = defaultInList?.userId ?? accounts[0]?.userId;

            const siteWaterSupplierId = this.viewModel.defaultWaterSupplierId;
            const matchesSite = siteWaterSupplierId
                ? this.viewModel.availableWaterSuppliers.find(s => s.id === siteWaterSupplierId)
                : undefined;

            if (matchesSite) {
                this.selectedWaterSupplierId = matchesSite.id;
            } else if (this.viewModel.availableWaterSuppliers.length === 1) {
                this.selectedWaterSupplierId = this.viewModel.availableWaterSuppliers[0].id;
            }
        }finally {
            this.isLoading = false;
        }
    }
}
