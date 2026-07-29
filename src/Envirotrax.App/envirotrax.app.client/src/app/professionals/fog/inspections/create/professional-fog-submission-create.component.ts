import { Component, OnInit } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";
import { NgForm } from "@angular/forms";
import { ModalSize } from "@developer-partners/ngx-modal-dialog";
import { FogSignaturePadModalComponent, FogSignatureModel } from "./fog-signature-pad-modal.component";
import { ProfessionalFogInspectionService } from "../../../../shared/services/fog/professional-fog-inspection.service";
import { ProfesisonalService } from "../../../../shared/services/professionals/professional.service";
import { ProfesionalUserService } from "../../../../shared/services/professionals/professional-user.service";
import { SiteService } from "../../../../shared/services/sites/site.service";
import { ProfessionalSupplierService } from "../../../../shared/services/professionals/professional-supplier.service";
import { CheckoutService } from "../../../../shared/services/professionals/checkout.service";
import { Professional } from "../../../../shared/models/professionals/professional";
import { ProfessionalUser } from "../../../../shared/models/professionals/professional-user";
import { ProfessionalWaterSupplier } from "../../../../shared/models/professionals/professional-water-supplier";
import { Site } from "../../../../shared/models/sites/site";
import { FogInspection } from "../../../../shared/models/fog/fog-inspection";
import { FogInspectionImages } from "../../../../shared/models/fog/fog-inspection-images";
import { MAX_PAGE_SIZE } from "../../../../shared/models/page-info";
import { FogInspectionOptionsService } from "../../../../shared/services/fog/fog-inspection-options.service";
import { InterceptorType } from "../../../../shared/enums/interceptor-type.enum";
import { FogInspectionResult } from "../../../../shared/models/fog/fog-inspection-enums";
import { InputOption, ModalHelperService } from "@envirotrax/common-ui";

@Component({
    standalone: false,
    templateUrl: './professional-fog-submission-create.component.html',
    styleUrl: './professional-fog-submission-create.component.scss'
})
export class ProfessionalFogSubmissionCreateComponent implements OnInit {
    public isLoading = false;
    public submitSuccess = false;
    public submitted = false;
    public validationErrors: string[] = [];
    public activeTab: 'main' | 'tripTickets' = 'main';

    public site?: Site;
    public professional?: Professional;

    private fogUsers: ProfessionalUser[] = [];
    private waterSuppliers: ProfessionalWaterSupplier[] = [];

    public selectedFogUserId!: number;
    public selectedWaterSupplierId?: number;
    public selectedFogUser?: ProfessionalUser;
    public selectedWaterSupplier?: ProfessionalWaterSupplier;

    public fogAccountOptions: InputOption[] = [];
    public waterSupplierOptions: InputOption[] = [];

    public readonly InterceptorType = InterceptorType;
    public readonly FogInspectionResult = FogInspectionResult;

    public readonly interceptorTypeOptions: InputOption[];
    public readonly capacityTypeOptions: InputOption[];
    public readonly reasonOptions: InputOption[];
    public readonly facilityTypeOptions: InputOption[];
    public readonly sampledFromOptions: InputOption[];

    public remarksLength = 0;

    public images: FogInspectionImages = {};
    public exteriorImagePreview: string | null = null;
    public interiorImagePreview: string | null = null;

    public signatureImagePreview: string | null = null;

    // Display-only intermediate percentages (not persisted)
    public inletGreaseLayerPercent = 0;
    public inletSedimentLayerPercent = 0;
    public outletGreaseLayerPercent = 0;
    public outletSedimentLayerPercent = 0;

    public model: FogInspection = {
        id: 0,
        inletChamberWettingHeight: '0',
        inletChamberGreaseBlanket: '0',
        inletChamberSediments: '0',
        outletChamberWettingHeight: '0',
        outletChamberGreaseBlanket: '0',
        outletChamberSediments: '0',
        inletTotalCapacityPercent: 0,
        outletTotalCapacityPercent: 0,
        totalCapacityPercent: 0
    };

    private _siteId = 0;

    constructor(
        private readonly _activatedRoute: ActivatedRoute,
        private readonly _router: Router,
        private readonly _professionalService: ProfesisonalService,
        private readonly _userService: ProfesionalUserService,
        private readonly _siteService: SiteService,
        private readonly _professionalSupplierService: ProfessionalSupplierService,
        private readonly _inspectionService: ProfessionalFogInspectionService,
        private readonly _fogOptions: FogInspectionOptionsService,
        private readonly _modalHelper: ModalHelperService,
        private readonly _checkoutService: CheckoutService
    ) {
        this.interceptorTypeOptions = this._fogOptions.interceptorTypeOptions;
        this.capacityTypeOptions = this._fogOptions.capacityTypeOptions;
        this.reasonOptions = this._fogOptions.reasonOptions;
        this.facilityTypeOptions = this._fogOptions.facilityTypeOptions;
        this.sampledFromOptions = this._fogOptions.sampledFromOptions;
    }

    public ngOnInit(): void {
        this._activatedRoute.paramMap.subscribe(async params => {
            const idParam = params.get('siteId');
            this._siteId = idParam ? Number(idParam) : 0;

            if (this._siteId > 0) {
                await this.loadData();
            }
        });
    }

    public onFogAccountChange(value: number): void {
        this.selectedFogUserId = value;
        this.selectedFogUser = this.fogUsers.find(u => u.id === value);
        // FOG inspectors are currently unlicensed, so no license check is done here (matches V1, where the
        // license validation was commented out). If licensing is required later, mirror the CSI submission
        // flow (loadLicense / hasValidLicense / license status display, gating the form on a valid license)
        // using ProfessionalType.FogInspector — ProfessionalUserLicenseService.getForUser already supports it.
        // Reference: professionals/csi/inspections/create/csi-submission-create.component.ts
    }

    public onWaterSupplierChange(value: number): void {
        this.selectedWaterSupplierId = value;
        this.selectedWaterSupplier = this.waterSuppliers.find(s => s.waterSupplier?.id === value);
    }

    public onCommentsChange(value: string | undefined): void {
        this.model.comments = value;
        this.remarksLength = value?.length ?? 0;
    }

    public onExteriorImageChange(file: File | null): void {
        this.images.exteriorImage = file;
        this.exteriorImagePreview = file ? URL.createObjectURL(file) : null;
    }

    public onInteriorImageChange(file: File | null): void {
        this.images.interiorImage = file;
        this.interiorImagePreview = file ? URL.createObjectURL(file) : null;
    }

    public onExteriorFileInputChange(event: Event): void {
        const file = (event.target as HTMLInputElement).files?.[0] ?? null;
        this.onExteriorImageChange(file);
        (event.target as HTMLInputElement).value = '';
    }

    public onInteriorFileInputChange(event: Event): void {
        const file = (event.target as HTMLInputElement).files?.[0] ?? null;
        this.onInteriorImageChange(file);
        (event.target as HTMLInputElement).value = '';
    }

    // Opens the signature pad in a modal. Saving keeps the drawn signature in memory
    // (preview + File); it is persisted on form submit. Cancelling leaves it unchanged.
    public openSignaturePad(): void {
        this._modalHelper.show<FogSignatureModel, string>(
            FogSignaturePadModalComponent,
            {
                title: 'Signature',
                size: ModalSize.extraLarge,
                model: { existingSignature: this.signatureImagePreview }
            }
        ).result().subscribe((dataUrl: string) => {
            if (dataUrl) {
                this.signatureImagePreview = dataUrl;
                this.images.signatureImage = this.dataUrlToFile(dataUrl, 'signature.png');
            } else {
                this.signatureImagePreview = null;
                this.images.signatureImage = null;
            }
        });
    }

    private dataUrlToFile(dataUrl: string, fileName: string): File {
        const [header, base64] = dataUrl.split(',');
        const mimeType = header.match(/:(.*?);/)?.[1] ?? 'image/png';
        const binary = atob(base64);

        const bytes = new Uint8Array(binary.length);
        for (let i = 0; i < binary.length; i++) {
            bytes[i] = binary.charCodeAt(i);
        }

        return new File([bytes], fileName, { type: mimeType });
    }

    public async submit(submitForm: NgForm): Promise<void> {
        this.submitted = true;
        this.validationErrors = [];
        this.collectValidationErrors();

        if (!submitForm.valid || this.validationErrors.length > 0) {
            return;
        }

        this.isLoading = true;
        try {
            await this._inspectionService.submit({
                ...this.model,
                site: { id: this._siteId },
                waterSupplier: { id: this.selectedWaterSupplierId },
                inspector: { id: this.selectedFogUserId }
            }, this.images);
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

    private collectValidationErrors(): void {
        if (this.model.inspectionDate && new Date(this.model.inspectionDate) > new Date()) {
            this.validationErrors.push('Inspection Date cannot be in the future.');
        }
        if (this.model.maintained == null) {
            this.validationErrors.push('Please answer "Is it properly maintained?".');
        }
        if (this.model.accessible == null) {
            this.validationErrors.push('Please answer "Is it accessible?".');
        }
        if (this.model.pastOverflow == null) {
            this.validationErrors.push('Please answer "Is there evidence of past overflow?".');
        }
        if (this.model.samplingPointAccessible == null) {
            this.validationErrors.push('Please answer whether the sampling point is accessible.');
        }
        if (this.model.samplingPointClean == null) {
            this.validationErrors.push('Please answer whether the sampling point needs to be cleaned.');
        }
        if (this.model.inspectionResult == null) {
            this.validationErrors.push('Please select an Inspection Result (Passed or Failed).');
        }
        if (this.model.interceptorCapacity != null && String(this.model.interceptorCapacity).trim() !== '') {
            const capacity = Number(this.model.interceptorCapacity);
            if (isNaN(capacity) || capacity < 0 || !Number.isInteger(capacity)) {
                this.validationErrors.push('Waste Trap or Tank Capacity must be a whole number of 0 or greater.');
            }
        }
        const chamberValues = [
            this.model.inletChamberWettingHeight,
            this.model.inletChamberGreaseBlanket,
            this.model.inletChamberSediments,
            this.model.outletChamberWettingHeight,
            this.model.outletChamberGreaseBlanket,
            this.model.outletChamberSediments
        ];
        if (chamberValues.some(v => this.isInvalidChamberValue(v))) {
            this.validationErrors.push('Chamber readings (wetted height, grease blanket, sediments) must be numbers of 0 or greater.');
        }
    }

    // A chamber reading is required and must be a non-negative number (decimals allowed).
    public isInvalidChamberValue(value: string | undefined): boolean {
        if (value == null || value.trim() === '') {
            return true;
        }
        const parsed = Number(value);
        return isNaN(parsed) || parsed < 0;
    }

    public recalcCapacity(): void {
        const inlet = this.chamberPercents(
            this.model.inletChamberWettingHeight,
            this.model.inletChamberGreaseBlanket,
            this.model.inletChamberSediments);
        this.inletGreaseLayerPercent = Math.round(inlet.grease);
        this.inletSedimentLayerPercent = Math.round(inlet.sediment);
        this.model.inletTotalCapacityPercent = Math.round(inlet.total);

        const outlet = this.chamberPercents(
            this.model.outletChamberWettingHeight,
            this.model.outletChamberGreaseBlanket,
            this.model.outletChamberSediments);
        this.outletGreaseLayerPercent = Math.round(outlet.grease);
        this.outletSedimentLayerPercent = Math.round(outlet.sediment);
        this.model.outletTotalCapacityPercent = Math.round(outlet.total);

        let total = 0;
        if (inlet.total > 0 && outlet.total > 0) {
            total = (inlet.total + outlet.total) / 2;
        } else if (inlet.total <= 0 && outlet.total > 0) {
            total = outlet.total;
        } else if (inlet.total > 0 && outlet.total <= 0) {
            total = inlet.total;
        }
        this.model.totalCapacityPercent = Math.round(total);
    }

    private chamberPercents(wetting?: string, grease?: string, sediment?: string): { grease: number; sediment: number; total: number } {
        const w = Number(wetting);
        const g = Number(grease);
        const s = Number(sediment);

        let greasePct = (g / w) * 100;
        let sedimentPct = (s / w) * 100;
        let total = greasePct + sedimentPct;

        if (isNaN(total) || !isFinite(total)) {
            greasePct = 0;
            sedimentPct = 0;
            total = 0;
        }

        return { grease: greasePct, sediment: sedimentPct, total };
    }

    private async loadData(): Promise<void> {
        try {
            this.isLoading = true;

            const [professional, usersPage, site] = await Promise.all([
                this._professionalService.getLoggedInProfessional(),
                this._userService.getAll({ pageSize: MAX_PAGE_SIZE }, { sort: {}, filter: [{ columnName: 'isFogInspector', comparisonOperator: 'Eq', value: 'true' }] }),
                this._siteService.getForProfessional(this._siteId)
            ]);

            this.professional = professional;
            this.fogUsers = usersPage.data ?? [];
            this.site = site;

            const waterSuppliersPage = await this._professionalSupplierService.getAllMy(false, false, true);
            this.waterSuppliers = waterSuppliersPage.data ?? [];

            this.buildDropdownOptions();
            await this.setDefaultFogUser();
            this.setDefaultWaterSupplier(site);
        } finally {
            this.isLoading = false;
        }
    }

    private buildDropdownOptions(): void {
        this.fogAccountOptions = this.fogUsers.map(u => ({
            id: u.id,
            text: u.contactName ?? `User ${u.id}`
        }));

        this.waterSupplierOptions = this.waterSuppliers.map(ws => ({
            id: ws.waterSupplier?.id,
            text: ws.waterSupplier?.name ?? ''
        }));
    }

    private async setDefaultFogUser(): Promise<void> {
        const myUser = await this._userService.getMyData();
        const defaultUser = this.fogUsers.find(u => u.id === myUser.id) ?? this.fogUsers[0];
        if (defaultUser?.id != null) {
            this.selectedFogUserId = defaultUser.id;
            this.selectedFogUser = defaultUser;
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
}
