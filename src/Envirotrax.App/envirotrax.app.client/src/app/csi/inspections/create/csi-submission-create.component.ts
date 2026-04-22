import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { NgForm } from '@angular/forms';
import { CsiInspectionService } from '../../../shared/services/csi/csi-inspection.service';
import { ProfesisonalService } from '../../../shared/services/professionals/professional.service';
import { ProfesionalUserService } from '../../../shared/services/professionals/professional-user.service';
import { ProfessionalUserLicenseService } from '../../../shared/services/professionals/professional-user-license.service';
import { SiteService } from '../../../shared/services/sites/site.service';
import { CsiInspection } from '../../../shared/models/csi/csi-inspection';
import { Professional } from '../../../shared/models/professionals/professional';
import { ProfessionalUser } from '../../../shared/models/professionals/professional-user';
import { ProfessionalUserLicense, ExpirationType, ProfessionalType } from '../../../shared/models/professionals/licenses/professional-user-license';
import { ProfessionalWaterSupplier } from '../../../shared/models/professionals/professional-water-supplier';
import { Site } from '../../../shared/models/sites/site';
import { CsiInspectionReason, csiInspectionReasonLabels } from '../../../shared/enums/csi-inspection-reason.enum';
import { InputOption } from '../../../shared/components/input/input.component';
import { MAX_PAGE_SIZE } from '../../../shared/models/page-info';
import { ProfessionalSupplierService } from '../../../shared/services/professionals/professional-supplier.service';

@Component({
    standalone: false,
    templateUrl: './csi-submission-create.component.html',
    styleUrl: './csi-submission-create.component.scss'
})
export class CsiSubmissionCreateComponent implements OnInit {
    public isLoading = true;
    public isSaving = false;
    public submitSuccess = false;
    public activeTab: 'main' | 'assemblies' | 'additional' | 'images' = 'main';
    public submitted = false;
    public validationErrors: string[] = [];

    public site?: Site;
    public professional?: Professional;

    private csiUsers: ProfessionalUser[] = [];
    private waterSuppliers: ProfessionalWaterSupplier[] = [];


    public selectedCsiUserId!: number;
    public selectedWaterSupplierId?: number;
    public currentLicense?: ProfessionalUserLicense;
    public isLoadingLicense = false;

    public csiAccountOptions: InputOption[] = [];
    public waterSupplierOptions: InputOption[] = [];

    public legalAcknowledgment = false;
    public compliance1: boolean | null = null;
    public compliance2: boolean | null = null;
    public compliance3: boolean | null = null;
    public compliance4: boolean | null = null;
    public compliance5: boolean | null = null;
    public compliance6: boolean | null = null;

    public form: {
        inspectionDate: string | undefined;
        reasonForInspection: CsiInspectionReason | null;
        materialServiceLineLead: boolean;
        materialServiceLineCopper: boolean;
        materialServiceLinePVC: boolean;
        materialServiceLineOther: boolean;
        materialServiceLineOtherDescription: string | undefined;
        materialSolderLead: boolean;
        materialSolderLeadFree: boolean;
        materialSolderSolventWeld: boolean;
        materialSolderOther: boolean;
        materialSolderOtherDescription: string | undefined;
        comments: string | undefined;
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

    public selectedCsiUser: ProfessionalUser | undefined = undefined;
    public selectedWaterSupplier: ProfessionalWaterSupplier | undefined = undefined;
    public hasValidLicense = false;
    public licenseStatusText = 'No license found';
    public licenseStatusClass = 'text-danger';
    public remarksLength = 0;
    public complianceIsInvalid = false;
    public serviceLineIsInvalid = false;
    public solderIsInvalid = false;

    private _siteId!: number;

    constructor(
        private readonly _activatedRoute: ActivatedRoute,
        private readonly _router: Router,
        private readonly _professionalService: ProfesisonalService,
        private readonly _userService: ProfesionalUserService,
        private readonly _licenseService: ProfessionalUserLicenseService,
        private readonly _siteService: SiteService,
        private readonly _inspectionService: CsiInspectionService,
        private readonly _professionalSupplierService: ProfessionalSupplierService
    ) {}

    public async ngOnInit(): Promise<void> {
        if (!this.initializeSiteId()) {
            return;
        }

        await this.loadData();
    }

    public async onCsiAccountChange(value: number): Promise<void> {
        this.selectedCsiUserId = value;
        this.selectedCsiUser = this.csiUsers.find(u => u.id === value);
        await this.loadLicense(value);
    }

    public onWaterSupplierChange(value: number): void {
        this.selectedWaterSupplierId = value;
        this.selectedWaterSupplier = this.waterSuppliers.find(s => s.waterSupplier?.id === value);
    }

    public onCommentsChange(value: string | undefined): void {
        this.form.comments = value;
        this.remarksLength = value?.length ?? 0;
    }

    public onComplianceChange(): void {
        if (this.submitted) {
            this.complianceIsInvalid = this.compliance1 === null || this.compliance2 === null || this.compliance3 === null ||
                this.compliance4 === null || this.compliance5 === null || this.compliance6 === null;
        }
    }

    public onMaterialChange(): void {
        if (this.submitted) {
            this.serviceLineIsInvalid = !this.form.materialServiceLineLead &&
                !this.form.materialServiceLineCopper &&
                !this.form.materialServiceLinePVC &&
                !this.form.materialServiceLineOther;
            this.solderIsInvalid = !this.form.materialSolderLead &&
                !this.form.materialSolderLeadFree &&
                !this.form.materialSolderSolventWeld &&
                !this.form.materialSolderOther;
        }
    }

    public async submit(submitForm: NgForm): Promise<void> {
        this.resetValidation();
        this.collectValidationErrors();

        if (!submitForm.valid || this.validationErrors.length > 0) {
            return;
        }

        this.isSaving = true;
        try {
            await this._inspectionService.submit(this.buildRequest());
            this.submitSuccess = true;
        } finally {
            this.isSaving = false;
        }
    }

    public returnToAccountOverview(): void {
        this._router.navigate(['/']);
    }

    public submitAnother(): void {
        this._router.navigate(['..'], { relativeTo: this._activatedRoute });
    }

    private initializeSiteId(): boolean{
         const idParam = this._activatedRoute.snapshot.paramMap.get('siteId');
        this._siteId = idParam ? Number(idParam) : 0;

        if (this._siteId <= 0) {
            this.isLoading = false;
            return false;
        }
        return true;
    }

    private async loadData(): Promise<void> {
        try {
            this.isLoading = true;

            const [professional, usersPage, site] = await Promise.all([
                this._professionalService.getLoggedInProfessional(),
                this._userService.getAll({ pageSize: MAX_PAGE_SIZE }, {}),
                this._siteService.getForProfessional(this._siteId)
            ]);

            this.professional = professional;
            this.csiUsers = (usersPage.data ?? []).filter(u => u.isCsiInspector);
            this.site = site;

            const waterSuppliersPage = await this._professionalSupplierService.getAllMy(true);
            this.waterSuppliers = waterSuppliersPage.data ?? [];

            this.buildDropdownOptions();
            await this.setDefaultCsiUser();
            this.setDefaultWaterSupplier(site);
        } finally {
            this.isLoading = false;
        }
    }

    private buildDropdownOptions(): void {
        this.csiAccountOptions = this.csiUsers.map(u => ({
            id: u.id,
            text: u.contactName ?? `User ${u.id}`
        }));

        this.waterSupplierOptions = this.waterSuppliers.map(ws => ({
            id: ws.waterSupplier?.id,
            text: ws.waterSupplier?.name ?? ''
        }));
    }

    private async setDefaultCsiUser(): Promise<void> {
        const myUser = await this._userService.getMyData();
        const defaultUser = this.csiUsers.find(u => u.id === myUser.id) ?? this.csiUsers[0];
        if (defaultUser?.id != null) {
            this.selectedCsiUserId = defaultUser.id;
            this.selectedCsiUser = defaultUser;
            await this.loadLicense(defaultUser.id);
        }
    }

    private setDefaultWaterSupplier(site: Site): void {
        if (this.waterSuppliers.length === 1) {
            this.selectedWaterSupplierId = this.waterSuppliers[0].waterSupplier?.id;
        }

        const siteWsId = site.waterSupplier?.id;
        if (siteWsId && this.waterSuppliers.some(ws => ws.waterSupplier?.id === siteWsId)) {
            this.selectedWaterSupplierId = siteWsId;
        }

        this.selectedWaterSupplier = this.waterSuppliers.find(s => s.waterSupplier?.id === this.selectedWaterSupplierId);
    }

    private async loadLicense(userId: number): Promise<void> {
        this.isLoadingLicense = true;
        try {
            const page = await this._licenseService.getForUser(userId, { pageSize: MAX_PAGE_SIZE }, {});
            this.currentLicense = (page.data ?? []).find(l => l.professionalType === ProfessionalType.CsiInspector);
            this.hasValidLicense = this.currentLicense != null && this.currentLicense.expirationType !== ExpirationType.Expired;
            if (!this.currentLicense) {
                this.licenseStatusText = 'No license found';
                this.licenseStatusClass = 'text-danger';
            } else if (this.currentLicense.expirationType === ExpirationType.Expired) {
                this.licenseStatusText = 'License expired';
                this.licenseStatusClass = 'text-danger';
            } else {
                this.licenseStatusText = 'License valid';
                this.licenseStatusClass = 'text-success';
            }
        } finally {
            this.isLoadingLicense = false;
        }
    }

    private resetValidation(): void {
        this.submitted = true;

        this.validationErrors = [];
        this.complianceIsInvalid = this.compliance1 === null || this.compliance2 === null || this.compliance3 === null ||
            this.compliance4 === null || this.compliance5 === null || this.compliance6 === null;
        this.serviceLineIsInvalid = !this.form.materialServiceLineLead &&
            !this.form.materialServiceLineCopper &&
            !this.form.materialServiceLinePVC &&
            !this.form.materialServiceLineOther;
        this.solderIsInvalid = !this.form.materialSolderLead &&
            !this.form.materialSolderLeadFree &&
            !this.form.materialSolderSolventWeld &&
            !this.form.materialSolderOther;
    }

    private collectValidationErrors(): void {
        if (this.form.inspectionDate && new Date(this.form.inspectionDate) > new Date()) {
            this.validationErrors.push('Inspection Date cannot be in the future.');
        }
        if (this.complianceIsInvalid) {
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

    private buildRequest(): CsiInspection {
        return {
            site: { id: this._siteId },
            waterSupplierId: this.selectedWaterSupplierId,
            inspectorUserId: this.selectedCsiUserId,
            inspectionDate: this.form.inspectionDate,
            reasonForInspection: this.form.reasonForInspection ?? undefined,
            compliance1: this.compliance1 === true,
            compliance2: this.compliance2 === true,
            compliance3: this.compliance3 === true,
            compliance4: this.compliance4 === true,
            compliance5: this.compliance5 === true,
            compliance6: this.compliance6 === true,
            materialServiceLineLead: this.form.materialServiceLineLead,
            materialServiceLineCopper: this.form.materialServiceLineCopper,
            materialServiceLinePVC: this.form.materialServiceLinePVC,
            materialServiceLineOther: this.form.materialServiceLineOther,
            materialServiceLineOtherDescription: this.form.materialServiceLineOtherDescription,
            materialSolderLead: this.form.materialSolderLead,
            materialSolderLeadFree: this.form.materialSolderLeadFree,
            materialSolderSolventWeld: this.form.materialSolderSolventWeld,
            materialSolderOther: this.form.materialSolderOther,
            materialSolderOtherDescription: this.form.materialSolderOtherDescription,
            comments: this.form.comments,
        };
    }
}
