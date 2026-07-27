import { Component, OnInit } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";
import { NgForm } from "@angular/forms";
import { ModalSize } from "@developer-partners/ngx-modal-dialog";
import { FogSignaturePadModalComponent, FogSignatureModel } from "../../inspections/create/fog-signature-pad-modal.component";
import { SiteService } from "../../../../shared/services/sites/site.service";
import { Site } from "../../../../shared/models/sites/site";
import { ProfesisonalService } from "../../../../shared/services/professionals/professional.service";
import { ProfesionalUserService } from "../../../../shared/services/professionals/professional-user.service";
import { ProfessionalSupplierService } from "../../../../shared/services/professionals/professional-supplier.service";
import { ProfessionalFogVehicleService } from "../../../../shared/services/fog/professional-fog-vehicle.service";
import { ProfessionalFogDisposalSiteService } from "../../../../shared/services/fog/professional-fog-disposal-site.service";
import { ProfessionalUserLicenseService } from "../../../../shared/services/professionals/professional-user-license.service";
import { FogTripTicketService } from "../../../../shared/services/fog/fog-trip-ticket.service";
import { Professional } from "../../../../shared/models/professionals/professional";
import { ProfessionalUser, ExpirationType } from "../../../../shared/models/professionals/professional-user";
import { AvailableWaterSupplier } from "../../../../shared/models/professionals/professional-water-supplier";
import { ProfessionalType, ExpirationType as LicenseExpirationType, ProfessionalUserLicense } from "../../../../shared/models/professionals/licenses/professional-user-license";
import { FogVehicle } from "../../../../shared/models/fog/fog-vehicle";
import { FogDisposalSite } from "../../../../shared/models/fog/fog-disposal-site";
import { FogVehicleCapacityType } from "../../../../shared/models/fog/fog-vehicle-enums";
import { FogTripTicket } from "../../../../shared/models/fog/fog-trip-ticket";
import { FogTripTicketImages } from "../../../../shared/models/fog/fog-trip-ticket-images";
import { WaterSupplier } from "../../../../shared/models/water-suppliers/water-supplier";
import { LookupService } from "../../../../shared/services/lookup/lookup.service";
import { MAX_PAGE_SIZE } from "../../../../shared/models/page-info";
import { InputOption, ModalHelperService } from "@envirotrax/common-ui";

interface VerificationCheck {
    label: string;
    message: string;
    valid: boolean;
}

@Component({
    standalone: false,
    templateUrl: './professional-fog-trip-ticket-submission-create.component.html'
})
export class ProfessionalFogTripTicketSubmissionCreateComponent implements OnInit {
    public isLoading = false;
    public submitted = false;
    public submitSuccess = false;
    public validationErrors: string[] = [];

    public site?: Site;
    public professional?: Professional;

    public transporterOptions: InputOption<ProfessionalUser>[] = [];
    public waterSupplierOptions: InputOption[] = [];
    public disposalSiteOptions: InputOption<FogDisposalSite>[] = [];
    public vehicleOptions: InputOption<FogVehicle>[] = [];

    public selectedTransporterUserId?: number;
    public selectedWaterSupplierId?: number;
    public selectedDisposalSiteId?: number;
    public selectedVehicleId?: number;

    public selectedTransporter?: ProfessionalUser;
    public selectedVehicle?: FogVehicle;
    public selectedDisposalSite?: FogDisposalSite;
    public selectedWaterSupplier?: WaterSupplier;
    public selectedWaterSupplierStateName?: string;

    public checks: VerificationCheck[] = [];
    public verificationPassed = false;

    public readonly FogVehicleCapacityType = FogVehicleCapacityType;

    public readonly interceptorTypeOptions: InputOption[] = [
        { id: '', text: 'Select Tank/Trap Type' },
        { id: 'Grease Trap', text: 'Grease Trap' },
        { id: 'Grit Trap', text: 'Grit Trap' },
        { id: 'Septic Tank', text: 'Septic Tank' },
        { id: 'Chemical Toilet', text: 'Chemical Toilet' },
        { id: 'Lint Trap', text: 'Lint Trap' },
        { id: 'Other', text: 'Other' }
    ];

    public readonly capacityTypeOptions: InputOption[] = [
        { id: FogVehicleCapacityType.Gallons, text: 'Gallons' },
        { id: FogVehicleCapacityType.CubicYards, text: 'Cubic Yards' }
    ];

    public remarksLength = 0;

    public model: Partial<FogTripTicket> = {
        interceptorCapacityType: FogVehicleCapacityType.Gallons,
        interceptorWasteRemovedType: FogVehicleCapacityType.Gallons
    };

    public images: FogTripTicketImages = {};
    public generatorSignaturePreview: string | null = null;
    public receiverSignaturePreview: string | null = null;
    public transporterSignatureUrl: string | null = null;

    private _siteId = 0;
    private _transporterLicense?: ProfessionalUserLicense;
    private readonly _availableSuppliers = new Map<number, AvailableWaterSupplier>();
    private readonly _myWaterSuppliers = new Map<number, WaterSupplier>();
    private readonly _stateNamesById = new Map<number, string>();

    constructor(
        private readonly _activatedRoute: ActivatedRoute,
        private readonly _router: Router,
        private readonly _siteService: SiteService,
        private readonly _professionalService: ProfesisonalService,
        private readonly _userService: ProfesionalUserService,
        private readonly _supplierService: ProfessionalSupplierService,
        private readonly _vehicleService: ProfessionalFogVehicleService,
        private readonly _disposalSiteService: ProfessionalFogDisposalSiteService,
        private readonly _licenseService: ProfessionalUserLicenseService,
        private readonly _tripTicketService: FogTripTicketService,
        private readonly _lookupService: LookupService,
        private readonly _modalHelper: ModalHelperService
    ) { }

    public ngOnInit(): void {
        this._activatedRoute.paramMap.subscribe(async params => {
            const idParam = params.get('siteId');
            this._siteId = idParam ? Number(idParam) : 0;

            await this.loadData();
        });
    }

    public async onTransporterChange(value: number): Promise<void> {
        this.selectedTransporterUserId = value;
        this.selectedTransporter = this.transporterOptions.find(o => o.id === value)?.data;

        await this.loadTransporterSignatureUrl();
        await this.computeVerification();
    }

    public async onWaterSupplierChange(value: number): Promise<void> {
        this.selectedWaterSupplierId = value;
        this.applySelectedWaterSupplier(value);

        await this.computeVerification();
    }

    public async onDisposalSiteChange(value: number): Promise<void> {
        this.selectedDisposalSiteId = value;
        this.selectedDisposalSite = this.disposalSiteOptions.find(o => o.id === value)?.data;

        await this.computeVerification();
    }

    public async onVehicleChange(value: number): Promise<void> {
        this.selectedVehicleId = value;
        this.selectedVehicle = this.vehicleOptions.find(o => o.id === value)?.data;

        await this.computeVerification();
    }

    public onCommentsChange(value: string | undefined): void {
        this.model.comments = value;
        this.remarksLength = value?.length ?? 0;
    }

    public onRemovedAmountChange(value: string | number | undefined): void {
        this.model.interceptorWasteRemovedAmount = this.toNonNegative(value);
    }

    public onCapacityChange(value: string | number | undefined): void {
        this.model.interceptorCapacity = this.toNonNegative(value);
    }

    public openGeneratorSignature(): void {
        this.openSignaturePad(this.generatorSignaturePreview, (preview, file) => {
            this.generatorSignaturePreview = preview;
            this.images.generatorSignature = file;
        }, 'generator-signature.png');
    }

    public openReceiverSignature(): void {
        this.openSignaturePad(this.receiverSignaturePreview, (preview, file) => {
            this.receiverSignaturePreview = preview;
            this.images.receiverSignature = file;
        }, 'receiver-signature.png');
    }

    public async submit(submitForm: NgForm): Promise<void> {
        this.submitted = true;
        this.validationErrors = [];
        this.collectValidationErrors();

        if (!submitForm.valid || this.validationErrors.length > 0) {
            return;
        }

        try {
            this.isLoading = true;

            const ticket: FogTripTicket = {
                ...this.model,
                id: 0,
                site: { id: this._siteId },
                waterSupplier: { id: this.selectedWaterSupplierId },
                transporter: { id: this.selectedTransporterUserId },
                vehicleId: this.selectedVehicleId,
                receiverDisposalSiteId: this.selectedDisposalSiteId,
                transporterLicenseNumber: this._transporterLicense?.licenseNumber,
                transporterLicenseExpiration: this._transporterLicense?.expirationDate
            };

            await this._tripTicketService.submit(ticket, this.images);

            this.submitSuccess = true;
        } finally {
            this.isLoading = false;
        }
    }

    public returnToSearch(): void {
        this._router.navigate(['..'], { relativeTo: this._activatedRoute });
    }

    public submitAnother(): void {
        this._router.navigate(['..'], { relativeTo: this._activatedRoute });
    }

    public returnToAccountOverview(): void {
        this._router.navigate(['/']);
    }

    private openSignaturePad(existing: string | null, apply: (preview: string | null, file: File | null) => void, fileName: string): void {
        this._modalHelper.show<FogSignatureModel, string>(
            FogSignaturePadModalComponent,
            {
                title: 'Signature',
                size: ModalSize.extraLarge,
                model: { existingSignature: existing }
            }
        ).result().subscribe((dataUrl: string) => {
            if (dataUrl) {
                apply(dataUrl, this.dataUrlToFile(dataUrl, fileName));
            } else {
                apply(null, null);
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

    private toNonNegative(value: string | number | undefined): number | undefined {
        if (value == null || value === '') {
            return undefined;
        }

        const num = Number(value);

        if (isNaN(num)) {
            return undefined;
        }

        return num < 0 ? 0 : num;
    }

    private collectValidationErrors(): void {
        if (!this.model.interceptorType) {
            this.validationErrors.push('Please select what the waste was removed from.');
        }

        if (this.model.interceptorType === 'Other' && !this.model.interceptorOtherDescription) {
            this.validationErrors.push('Please describe the "Other" trap or tank type.');
        }

        if (this.model.interceptorWasteRemovedAmount == null || Number(this.model.interceptorWasteRemovedAmount) <= 0) {
            this.validationErrors.push('Please enter the amount of waste removed.');
        }

        if (!this.model.interceptorWasteRemovedDate) {
            this.validationErrors.push('Please enter the date the waste was removed.');
        }

        if (!this.model.receiverWasteDeliveredDate) {
            this.validationErrors.push('Please enter the date the waste was delivered.');
        }

        if (!this.images.generatorSignature) {
            this.validationErrors.push('A generator signature is required.');
        }

        if (!this.images.receiverSignature) {
            this.validationErrors.push('A receiver signature is required.');
        }
    }

    private async loadData(): Promise<void> {
        try {
            this.isLoading = true;

            const [professional, transporterOptions, suppliersPage, availableSuppliers, disposalSiteOptions, vehicleOptions, states] = await Promise.all([
                this._professionalService.getLoggedInProfessional(),
                this._userService.getAllAsOptions(false, '', { filter: [{ columnName: 'isFogTransporter', comparisonOperator: 'Eq', value: 'true' }] }),
                this._supplierService.getAllMy(),
                this._supplierService.getAllAvailableSuppliers({ pageSize: MAX_PAGE_SIZE }, {}),
                this._disposalSiteService.getAllRegisteredAsOptions(true, 'Select a disposal site'),
                this._vehicleService.getAllAsOptions(true, 'Select a transporter vehicle'),
                this._lookupService.getAllStates()
            ]);

            this.professional = professional;
            this.transporterOptions = transporterOptions;
            this.disposalSiteOptions = disposalSiteOptions;
            this.vehicleOptions = vehicleOptions;

            this.waterSupplierOptions = [
                { id: '', text: 'Select a water supplier' },
                ...suppliersPage.data
                    .filter(s => s.waterSupplier?.id)
                    .map(s => ({ id: s.waterSupplier!.id, text: s.waterSupplier!.name ?? '' }))
            ];

            this._myWaterSuppliers.clear();
            for (const supplier of suppliersPage.data) {
                if (supplier.waterSupplier?.id != null) {
                    this._myWaterSuppliers.set(supplier.waterSupplier.id, supplier.waterSupplier);
                }
            }

            this._availableSuppliers.clear();
            for (const supplier of availableSuppliers.data) {
                if (supplier.id != null) {
                    this._availableSuppliers.set(supplier.id, supplier);
                }
            }

            this._stateNamesById.clear();
            for (const state of states) {
                if (state.id != null && state.name) {
                    this._stateNamesById.set(state.id, state.name);
                }
            }

            if (this._siteId > 0) {
                this.site = await this._siteService.getForProfessional(this._siteId);
                this.model.fogGeneratorPhoneNumber = this.site.fogGeneratorPhoneNumber;
                this.model.fogGeneratorEmailAddress = this.site.fogGeneratorEmailAddress;
            }

            await this.setDefaults();
            await this.computeVerification();
        } finally {
            this.isLoading = false;
        }
    }

    private async setDefaults(): Promise<void> {
        const myUser = await this._userService.getMyData();
        const defaultTransporter = this.transporterOptions.find(o => o.id === myUser.id) ?? this.transporterOptions[0];
        this.selectedTransporterUserId = defaultTransporter?.id;
        this.selectedTransporter = defaultTransporter?.data;

        await this.loadTransporterSignatureUrl();

        const registeredIds = this.waterSupplierOptions.filter(o => o.id).map(o => Number(o.id));
        const siteWsId = this.site?.waterSupplier?.id;

        if (siteWsId && registeredIds.includes(siteWsId)) {
            this.selectedWaterSupplierId = siteWsId;
        } else if (registeredIds.length === 1) {
            this.selectedWaterSupplierId = registeredIds[0];
        }

        this.applySelectedWaterSupplier(this.selectedWaterSupplierId);
    }

    private applySelectedWaterSupplier(waterSupplierId?: number): void {
        this.selectedWaterSupplier = waterSupplierId != null
            ? this._myWaterSuppliers.get(waterSupplierId)
            : undefined;

        const stateId = this.selectedWaterSupplier?.state?.id;
        this.selectedWaterSupplierStateName = stateId != null
            ? this._stateNamesById.get(stateId)
            : undefined;
    }

    private async computeVerification(): Promise<void> {
        const checks: VerificationCheck[] = [];

        checks.push(await this.buildRegistrationCheck());
        checks.push(this.buildInsuranceCheck());
        checks.push(this.buildDisposalSiteCheck());
        checks.push(this.buildVehicleCheck());
        checks.push(this.buildSignatureCheck());

        this.checks = checks;
        this.verificationPassed = checks.every(c => c.valid);
    }

    private async buildRegistrationCheck(): Promise<VerificationCheck> {
        const label = 'TCEQ - Registration Number';

        this._transporterLicense = undefined;

        if (!this.selectedTransporterUserId) {
            return { label, message: 'Select a transporter account', valid: false };
        }

        const licensePage = await this._licenseService.getForUser(this.selectedTransporterUserId, { pageSize: MAX_PAGE_SIZE }, {});
        const license = licensePage.data?.find(l => l.professionalType === ProfessionalType.FogTransporter);

        if (!license) {
            return { label, message: 'No registration number found', valid: false };
        }

        this._transporterLicense = license;

        if (license.expirationType === LicenseExpirationType.Expired) {
            return { label, message: 'Registration number expired', valid: false };
        }

        return { label, message: 'Registration number valid', valid: true };
    }

    private buildInsuranceCheck(): VerificationCheck {
        const label = 'Insurance Policy';

        if (!this.requiresInsurance()) {
            return { label, message: 'Insurance not required', valid: true };
        }

        const insuranceType = this.professional?.insuranceExpirationType;

        if (insuranceType == null) {
            return { label, message: 'No insurance policy found', valid: false };
        }

        if (insuranceType === ExpirationType.Expired) {
            return { label, message: 'Insurance policy expired', valid: false };
        }

        return { label, message: 'Insurance policy valid', valid: true };
    }

    private buildDisposalSiteCheck(): VerificationCheck {
        const label = 'Disposal Site';

        const selected = this.disposalSiteOptions.find(o => o.id === this.selectedDisposalSiteId);

        if (!this.selectedDisposalSiteId || !selected) {
            return { label, message: 'Select a disposal site', valid: false };
        }

        return { label, message: selected.text ?? 'Registered', valid: true };
    }

    private buildVehicleCheck(): VerificationCheck {
        const label = 'Transporter Vehicle';

        const selected = this.vehicleOptions.find(o => o.id === this.selectedVehicleId);

        if (!this.selectedVehicleId || !selected) {
            return { label, message: 'Select a transporter vehicle', valid: false };
        }

        // V1 also validates a per-vehicle-per-water-supplier permit and inspection expiry
        // (FogVehiclePermits). V2 has no equivalent data yet, so that sub-check is not ported.
        return { label, message: selected.text ?? 'Valid', valid: true };
    }

    private buildSignatureCheck(): VerificationCheck {
        const label = 'Transporter Signature';

        if (!this.selectedTransporter?.signaturePath) {
            return { label, message: 'No signature on record', valid: false };
        }

        return { label, message: 'Signed', valid: true };
    }

    private async loadTransporterSignatureUrl(): Promise<void> {
        this.transporterSignatureUrl = this.selectedTransporter?.signaturePath && this.selectedTransporterUserId
            ? await this._userService.getSignatureUrl(this.selectedTransporterUserId)
            : null;
    }

    private requiresInsurance(): boolean {
        if (!this.selectedWaterSupplierId) {
            return false;
        }

        return this._availableSuppliers.get(this.selectedWaterSupplierId)?.fogTransportersRequireInsurance ?? false;
    }
}
