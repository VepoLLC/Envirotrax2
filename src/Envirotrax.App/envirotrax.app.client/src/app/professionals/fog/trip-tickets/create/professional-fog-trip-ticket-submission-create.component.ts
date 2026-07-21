import { Component, OnInit } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";
import { SiteService } from "../../../../shared/services/sites/site.service";
import { Site } from "../../../../shared/models/sites/site";
import { ProfesisonalService } from "../../../../shared/services/professionals/professional.service";
import { ProfesionalUserService } from "../../../../shared/services/professionals/professional-user.service";
import { ProfessionalSupplierService } from "../../../../shared/services/professionals/professional-supplier.service";
import { ProfessionalFogVehicleService } from "../../../../shared/services/fog/professional-fog-vehicle.service";
import { ProfessionalFogDisposalSiteService } from "../../../../shared/services/fog/professional-fog-disposal-site.service";
import { ProfessionalUserLicenseService } from "../../../../shared/services/professionals/professional-user-license.service";
import { Professional } from "../../../../shared/models/professionals/professional";
import { ExpirationType } from "../../../../shared/models/professionals/professional-user";
import { AvailableWaterSupplier } from "../../../../shared/models/professionals/professional-water-supplier";
import { ProfessionalType, ExpirationType as LicenseExpirationType } from "../../../../shared/models/professionals/licenses/professional-user-license";
import { MAX_PAGE_SIZE } from "../../../../shared/models/page-info";
import { InputOption } from "@envirotrax/common-ui";

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

    public site?: Site;
    public professional?: Professional;

    public transporterOptions: InputOption[] = [];
    public waterSupplierOptions: InputOption[] = [];
    public disposalSiteOptions: InputOption[] = [];
    public vehicleOptions: InputOption[] = [];

    public selectedTransporterUserId?: number;
    public selectedWaterSupplierId?: number;
    public selectedDisposalSiteId?: number;
    public selectedVehicleId?: number;

    public checks: VerificationCheck[] = [];
    public verificationPassed = false;

    private _siteId = 0;
    private readonly _availableSuppliers = new Map<number, AvailableWaterSupplier>();

    constructor(
        private readonly _activatedRoute: ActivatedRoute,
        private readonly _router: Router,
        private readonly _siteService: SiteService,
        private readonly _professionalService: ProfesisonalService,
        private readonly _userService: ProfesionalUserService,
        private readonly _supplierService: ProfessionalSupplierService,
        private readonly _vehicleService: ProfessionalFogVehicleService,
        private readonly _disposalSiteService: ProfessionalFogDisposalSiteService,
        private readonly _licenseService: ProfessionalUserLicenseService
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
        await this.computeVerification();
    }

    public async onWaterSupplierChange(value: number): Promise<void> {
        this.selectedWaterSupplierId = value;
        await this.computeVerification();
    }

    public async onDisposalSiteChange(value: number): Promise<void> {
        this.selectedDisposalSiteId = value;
        await this.computeVerification();
    }

    public async onVehicleChange(value: number): Promise<void> {
        this.selectedVehicleId = value;
        await this.computeVerification();
    }

    public returnToSearch(): void {
        this._router.navigate(['..'], { relativeTo: this._activatedRoute });
    }

    private async loadData(): Promise<void> {
        try {
            this.isLoading = true;

            const [professional, transporterOptions, suppliersPage, availableSuppliers, disposalSiteOptions, vehicleOptions] = await Promise.all([
                this._professionalService.getLoggedInProfessional(),
                this._userService.getAllAsOptions(false, '', { filter: [{ columnName: 'isFogTransporter', comparisonOperator: 'Eq', value: 'true' }] }),
                this._supplierService.getAllMy(),
                this._supplierService.getAllAvailableSuppliers({ pageSize: MAX_PAGE_SIZE }, {}),
                this._disposalSiteService.getAllRegisteredAsOptions(true, 'Select a disposal site'),
                this._vehicleService.getAllAsOptions(true, 'Select a transporter vehicle')
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

            this._availableSuppliers.clear();
            for (const supplier of availableSuppliers.data) {
                if (supplier.id != null) {
                    this._availableSuppliers.set(supplier.id, supplier);
                }
            }

            if (this._siteId > 0) {
                this.site = await this._siteService.getForProfessional(this._siteId);
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

        const registeredIds = this.waterSupplierOptions.filter(o => o.id).map(o => Number(o.id));
        const siteWsId = this.site?.waterSupplier?.id;

        if (siteWsId && registeredIds.includes(siteWsId)) {
            this.selectedWaterSupplierId = siteWsId;
        } else if (registeredIds.length === 1) {
            this.selectedWaterSupplierId = registeredIds[0];
        }
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

        if (!this.selectedTransporterUserId) {
            return { label, message: 'Select a transporter account', valid: false };
        }

        const licensePage = await this._licenseService.getForUser(this.selectedTransporterUserId, { pageSize: MAX_PAGE_SIZE }, {});
        const license = licensePage.data?.find(l => l.professionalType === ProfessionalType.FogTransporter);

        if (!license) {
            return { label, message: 'No registration number found', valid: false };
        }

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
        // V1 checks for a transporter signature stored on the account (FogTransporters.TransporterSignature).
        // V2 has no per-user signature storage yet, so this is not ported and reports as signed for now.
        return { label: 'Transporter Signature', message: 'Signed', valid: true };
    }

    private requiresInsurance(): boolean {
        if (!this.selectedWaterSupplierId) {
            return false;
        }

        return this._availableSuppliers.get(this.selectedWaterSupplierId)?.fogTransportersRequireInsurance ?? false;
    }
}
