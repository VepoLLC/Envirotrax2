import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { NgForm } from '@angular/forms';
import { CsiInspectionService } from '../../../../shared/services/csi/csi-inspection.service';
import { ProfesisonalService } from '../../../../shared/services/professionals/professional.service';
import { ProfesionalUserService } from '../../../../shared/services/professionals/professional-user.service';
import { ProfessionalUserLicenseService } from '../../../../shared/services/professionals/professional-user-license.service';
import { SiteService } from '../../../../shared/services/sites/site.service';
import { CsiInspection } from '../../../../shared/models/csi/csi-inspection';
import { Professional } from '../../../../shared/models/professionals/professional';
import { ProfessionalUser } from '../../../../shared/models/professionals/professional-user';
import { ProfessionalUserLicense, ExpirationType, ProfessionalType } from '../../../../shared/models/professionals/licenses/professional-user-license';
import { ProfessionalWaterSupplier } from '../../../../shared/models/professionals/professional-water-supplier';
import { InsuranceCheck, InsuranceCheckResult } from '../../../../shared/models/professionals/insurance-check';
import { Site } from '../../../../shared/models/sites/site';
import { CsiInspectionReason, csiInspectionReasonLabels } from '../../../../shared/enums/csi-inspection-reason.enum';
import { MAX_PAGE_SIZE } from '../../../../shared/models/page-info';
import { ProfessionalSupplierService } from '../../../../shared/services/professionals/professional-supplier.service';
import { CheckoutService } from '../../../../shared/services/professionals/checkout.service';
import { ToastService, InputOption, ModalHelperService } from '@envirotrax/common-ui';
import { ModalSize } from '@developer-partners/ngx-modal-dialog';
import { CsiInspectionAssembly, CsiInspectionAssemblyRequest } from '../../../../shared/models/csi/csi-inspection-assembly';
import { BackflowTestResult, BYPASS_DEVICE_TYPES } from '../../../../shared/models/backflow/backflow-test-enums';
import { AddCsiInspectionAssemblyComponent } from './add-csi-inspection-assembly.component';

// A row of the Assemblies tab. `request` is what gets saved (the checkbox binds to it); everything else is display.
interface AssemblyRowVm {
    request: CsiInspectionAssemblyRequest;
    isCurrent: boolean;
    isPassing: boolean;
    inService: boolean;
    testDate?: string;
    expirationDate?: string;
    hasBypass: boolean;
    serialNumber?: string;
    serialNumber2?: string;
    assemblyDescription?: string;
    assemblyDescription2?: string;
    hazardDescription: string;
    locationDescription?: string;
}

@Component({
    standalone: false,
    templateUrl: './csi-submission-create.component.html',
    styleUrl: './csi-submission-create.component.scss'
})
export class CsiSubmissionCreateComponent implements OnInit {
    public isLoading = false;
    public submitSuccess = false;
    public activeTab: 'main' | 'assemblies' | 'additional' | 'images' = 'main';
    public submitted = false;
    public validationErrors: string[] = [];

    public site?: Site;
    public professional?: Professional;
    public editingId: number | null = null;

    private csiUsers: ProfessionalUser[] = [];
    private waterSuppliers: ProfessionalWaterSupplier[] = [];

    public selectedCsiUserId!: number;
    public selectedWaterSupplierId?: number;
    public currentLicense?: ProfessionalUserLicense;
    public isLoadingLicense = false;
    public insuranceCheck?: InsuranceCheck;

    public csiAccountOptions: InputOption[] = [];
    public waterSupplierOptions: InputOption[] = [];

    public legalAcknowledgment = false;
    public pendingImages: { file: File; description: string; previewUrl: string }[] = [];
    public assemblies: AssemblyRowVm[] = [];

    public model: CsiInspection = {
        site: {},
        waterSupplier: {},
        inspectorUser: {},
        materialServiceLineLead: false,
        materialServiceLineCopper: false,
        materialServiceLinePVC: false,
        materialServiceLineOther: false,
        materialSolderLead: false,
        materialSolderLeadFree: false,
        materialSolderSolventWeld: false,
        materialSolderOther: false,
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

    public get verificationPassed(): boolean {
        return this.hasValidLicense && !!this.insuranceCheck?.isSatisfied;
    }

    public get showInsuranceRow(): boolean {
        return !!this.insuranceCheck && this.insuranceCheck.result !== InsuranceCheckResult.NotRequired;
    }

    constructor(
        private readonly _activatedRoute: ActivatedRoute,
        private readonly _router: Router,
        private readonly _professionalService: ProfesisonalService,
        private readonly _userService: ProfesionalUserService,
        private readonly _licenseService: ProfessionalUserLicenseService,
        private readonly _siteService: SiteService,
        private readonly _inspectionService: CsiInspectionService,
        private readonly _professionalSupplierService: ProfessionalSupplierService,
        private readonly _toastService: ToastService,
        private readonly _checkoutService: CheckoutService,
        private readonly _modalHelper: ModalHelperService
    ) { }

    public ngOnInit(): void {
        this._activatedRoute.paramMap.subscribe(async params => {
            const editIdParam = params.get('editId');

            if (editIdParam) {
                this.editingId = Number(editIdParam);
                await this.loadForEdit(this.editingId);
                return;
            }

            const idParam = params.get('siteId');
            this._siteId = this._siteId = idParam ? Number(idParam) : 0;

            if (this._siteId > 0) {
                await this.loadData();
            }
        });
    }

    public async onCsiAccountChange(value: number): Promise<void> {
        this.selectedCsiUserId = value;
        this.selectedCsiUser = this.csiUsers.find(u => u.id === value);
        this.model.inspectorUser = { id: value };

        this.isLoading = true;

        try {
            await this.loadLicense(value);
        } finally {
            this.isLoading = false;
        }
    }

    public async onWaterSupplierChange(value: number): Promise<void> {
        this.selectedWaterSupplierId = value;
        this.selectedWaterSupplier = this.waterSuppliers.find(s => s.waterSupplier?.id === value);
        this.model.waterSupplier = { id: value };

        this.isLoading = true;

        try {
            await this.loadInsuranceCheck();
        } finally {
            this.isLoading = false;
        }
    }

    public onCommentsChange(value: string | undefined): void {
        this.model.comments = value;
        this.remarksLength = value?.length ?? 0;
    }

    public onComplianceChange(): void {
        if (this.submitted) {
            this.complianceIsInvalid = this.model.compliance1 == null || this.model.compliance2 == null || this.model.compliance3 == null ||
                this.model.compliance4 == null || this.model.compliance5 == null || this.model.compliance6 == null;
        }
    }

    public onMaterialChange(): void {
        if (this.submitted) {
            this.serviceLineIsInvalid = !this.model.materialServiceLineLead &&
                !this.model.materialServiceLineCopper &&
                !this.model.materialServiceLinePVC &&
                !this.model.materialServiceLineOther;
            this.solderIsInvalid = !this.model.materialSolderLead &&
                !this.model.materialSolderLeadFree &&
                !this.model.materialSolderSolventWeld &&
                !this.model.materialSolderOther;
        }
    }

    public onImageFileChange(event: Event): void {
        const input = event.target as HTMLInputElement;
        if (!input.files?.length) return;
        const file = input.files[0];
        input.value = '';
        if (this.pendingImages.length >= 24) return;
        this.pendingImages.push({ file, description: '', previewUrl: URL.createObjectURL(file) });
    }

    public removePendingImage(index: number): void {
        URL.revokeObjectURL(this.pendingImages[index].previewUrl);
        this.pendingImages.splice(index, 1);
    }

    public addAssembly(): void {
        this._modalHelper.show<CsiInspectionAssemblyRequest>(AddCsiInspectionAssemblyComponent, {
            title: 'Add Backflow Device',
            size: ModalSize.large
        }).result().subscribe(request => {
            this.assemblies = [...this.assemblies, this.buildNewAssemblyRow(request)];
        });
    }

    public deleteAssembly(row: AssemblyRowVm): void {
        this._modalHelper.confirm({
            title: 'Confirm Assembly Deletion',
            messages: ['Are you sure you want to delete the record for the following assembly?', row.assemblyDescription ?? '']
        }).result().subscribe(() => {
            this.assemblies = this.assemblies.filter(assembly => assembly !== row);
        });
    }

    // V1 toggle: marks every row, or clears them all when every row is already marked.
    public markAllAssembliesVisuallyIdentified(): void {
        const visuallyIdentified = !this.assemblies.every(row => row.request.visuallyIdentified);

        for (const row of this.assemblies) {
            row.request.visuallyIdentified = visuallyIdentified;
        }
    }

    public async submit(submitForm: NgForm): Promise<void> {
        this.resetValidation();
        this.collectValidationErrors();

        if (!submitForm.valid || this.validationErrors.length > 0) {
            return;
        }

        this.isLoading = true;
        try {
            const payload = { ...this.model, site: { id: this._siteId } };
            const result = this.editingId
                ? await this._inspectionService.updateForProfessional(this.editingId, payload)
                : await this._inspectionService.submit(payload);

            try {
                await this._inspectionService.saveAssemblies(result.id!, this.assemblies.map(row => row.request));
            } catch {
                this._toastService.failedToSave('Assemblies at This Location');
            }

            let imagesFailed = false;
            for (const img of this.pendingImages) {
                try {
                    await this._inspectionService.addImage(result.id!, img.description || null, img.file);
                } catch {
                    imagesFailed = true;
                }
            }
            if (imagesFailed) {
                this._toastService.failedToSave('One or more images');
            }
            this.submitSuccess = true;
            this._checkoutService.refresh();
        } finally {
            this.isLoading = false;
        }
    }

    public returnToAccountOverview(): void {
        this._router.navigate(['/']);
    }

    public submitAnother(): void {
        this._router.navigate(['..'], { relativeTo: this._activatedRoute });
    }

    public goToCheckout(): void {
        this._router.navigate(['/professionals/checkout'], { queryParams: { tab: 'csi' } });
    }

    private initializeSiteId(): boolean {
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

            const [professional, usersPage, site, siteAssemblies] = await Promise.all([
                this._professionalService.getLoggedInProfessional(),
                this._userService.getAll({ pageSize: MAX_PAGE_SIZE }, { sort: {}, filter: [{ columnName: 'isCsiInspector', comparisonOperator: 'Eq', value: 'true' }] }),
                this._siteService.getForProfessional(this._siteId),
                this._inspectionService.getSiteAssemblies(this._siteId)
            ]);

            this.professional = professional;
            this.csiUsers = usersPage.data ?? [];
            this.site = site;
            this.assemblies = siteAssemblies.map(assembly => this.buildAssemblyRow(assembly));

            const waterSuppliersPage = await this._professionalSupplierService.getAllMy({ hasCsiInspection: true });
            this.waterSuppliers = waterSuppliersPage.data ?? [];

            this.buildDropdownOptions();
            await this.setDefaultCsiUser();
            await this.setDefaultWaterSupplier(site);
        } finally {
            this.isLoading = false;
        }
    }

    // Checkout "Edit": full field load of an own, still-unpaid inspection — unlike loadData, which builds a
    // blank form pre-scoped only to a site id.
    private async loadForEdit(id: number): Promise<void> {
        try {
            this.isLoading = true;

            const inspection = await this._inspectionService.getProfessionalInspection(id);
            this._siteId = inspection.site?.id ?? 0;

            const [professional, usersPage, site, savedAssemblies] = await Promise.all([
                this._professionalService.getLoggedInProfessional(),
                this._userService.getAll({ pageSize: MAX_PAGE_SIZE }, { sort: {}, filter: [{ columnName: 'isCsiInspector', comparisonOperator: 'Eq', value: 'true' }] }),
                this._siteService.getForProfessional(this._siteId),
                this._inspectionService.getProfessionalAssemblies(id)
            ]);

            this.professional = professional;
            this.csiUsers = usersPage.data ?? [];
            this.site = site;
            this.assemblies = savedAssemblies.map(assembly => this.buildAssemblyRow(assembly));

            const waterSuppliersPage = await this._professionalSupplierService.getAllMy({ hasCsiInspection: true });
            this.waterSuppliers = waterSuppliersPage.data ?? [];

            this.buildDropdownOptions();

            this.model = { ...inspection };
            this.remarksLength = this.model.comments?.length ?? 0;

            this.selectedWaterSupplierId = inspection.waterSupplier?.id;
            this.selectedWaterSupplier = this.waterSuppliers.find(s => s.waterSupplier?.id === inspection.waterSupplier?.id);
            await this.loadInsuranceCheck();

            this.selectedCsiUserId = inspection.inspectorUser?.id ?? 0;
            this.selectedCsiUser = this.csiUsers.find(u => u.id === this.selectedCsiUserId);
            if (this.selectedCsiUserId) {
                await this.loadLicense(this.selectedCsiUserId);
            }
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
            this.model.inspectorUser = { id: defaultUser.id };
            await this.loadLicense(defaultUser.id);
        }
    }

    private async setDefaultWaterSupplier(site: Site): Promise<void> {
        if (this.waterSuppliers.length === 1) {
            this.selectedWaterSupplierId = this.waterSuppliers[0].waterSupplier?.id;
        }

        const siteWsId = site.waterSupplier?.id;
        if (siteWsId && this.waterSuppliers.some(ws => ws.waterSupplier?.id === siteWsId)) {
            this.selectedWaterSupplierId = siteWsId;
        }

        this.selectedWaterSupplier = this.waterSuppliers.find(s => s.waterSupplier?.id === this.selectedWaterSupplierId);
        this.model.waterSupplier = { id: this.selectedWaterSupplierId };

        await this.loadInsuranceCheck();
    }

    private async loadInsuranceCheck(): Promise<void> {
        const waterSupplierId = this.selectedWaterSupplierId;

        this.insuranceCheck = undefined;

        if (!waterSupplierId) {
            return;
        }

        const check = await this._inspectionService.getInsuranceCheck(waterSupplierId);

        if (this.selectedWaterSupplierId === waterSupplierId) {
            this.insuranceCheck = check;
        }
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

    // A saved row (edit) or a current test at the site (new submission, id 0 — not saved yet).
    private buildAssemblyRow(assembly: CsiInspectionAssembly): AssemblyRowVm {
        return {
            request: {
                id: assembly.id || undefined,
                testId: assembly.testId,
                visuallyIdentified: assembly.visuallyIdentified ?? false
            },
            isCurrent: assembly.isCurrent ?? false,
            isPassing: assembly.testResult === BackflowTestResult.Pass,
            inService: !assembly.outOfService,
            testDate: assembly.testDate,
            expirationDate: assembly.expirationDate,
            hasBypass: BYPASS_DEVICE_TYPES.includes(assembly.deviceType ?? ''),
            serialNumber: assembly.serialNumber,
            serialNumber2: assembly.serialNumber2,
            assemblyDescription: assembly.assemblyDescription,
            assemblyDescription2: assembly.assemblyDescription2,
            hazardDescription: this.buildHazardDescription(assembly.hazardType, assembly.hazardTypeOtherDescription),
            locationDescription: assembly.locationDescription
        };
    }

    // An assembly added on the form becomes a current, in-service test once saved; its dates are the inspection date.
    private buildNewAssemblyRow(request: CsiInspectionAssemblyRequest): AssemblyRowVm {
        return {
            request,
            isCurrent: true,
            isPassing: true,
            inService: true,
            hasBypass: BYPASS_DEVICE_TYPES.includes(request.deviceType ?? ''),
            serialNumber: request.serialNumber,
            serialNumber2: request.serialNumber2,
            assemblyDescription: this.buildAssemblyDescription(request.manufacturer, request.model, request.size, request.deviceType),
            assemblyDescription2: this.buildAssemblyDescription(request.manufacturer2, request.model2, request.size2, request.deviceType),
            hazardDescription: this.buildHazardDescription(request.hazardType, request.hazardTypeOtherDescription),
            locationDescription: request.locationDescription
        };
    }

    // Same format the server stores: "{manufacturer} {model} {size} - {device type}".
    private buildAssemblyDescription(manufacturer?: string, model?: string, size?: string, deviceType?: string): string {
        const device = [manufacturer, model, size].filter(part => part?.trim()).join(' ');

        return device ? `${device} - ${deviceType ?? ''}` : deviceType ?? '';
    }

    private buildHazardDescription(hazardType?: string, otherDescription?: string): string {
        if (!hazardType) {
            return 'Unknown';
        }

        return hazardType === 'Other' ? `Other - ${otherDescription ?? ''}` : hazardType;
    }

    private resetValidation(): void {
        this.submitted = true;
        this.validationErrors = [];

        this.complianceIsInvalid = this.model.compliance1 == null || this.model.compliance2 == null || this.model.compliance3 == null ||
            this.model.compliance4 == null || this.model.compliance5 == null || this.model.compliance6 == null;

        this.serviceLineIsInvalid = !this.model.materialServiceLineLead &&
            !this.model.materialServiceLineCopper &&
            !this.model.materialServiceLinePVC &&
            !this.model.materialServiceLineOther;

        this.solderIsInvalid = !this.model.materialSolderLead &&
            !this.model.materialSolderLeadFree &&
            !this.model.materialSolderSolventWeld &&
            !this.model.materialSolderOther;
    }

    private collectValidationErrors(): void {
        if (this.model.inspectionDate && new Date(this.model.inspectionDate) > new Date()) {
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
}
