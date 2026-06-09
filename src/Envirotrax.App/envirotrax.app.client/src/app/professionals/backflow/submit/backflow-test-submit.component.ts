import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { NgForm } from '@angular/forms';
import { BackflowTest } from '../../../shared/models/backflow/backflow-test';
import { BackflowTestImages } from '../../../shared/models/backflow/backflow-test-images';
import { BackflowTestService } from '../../../shared/services/backflow/backflow-test.service';
import { BackflowTestOptionsService } from '../../../shared/services/backflow/backflow-test-options.service';
import { BackflowGaugeService } from '../../../shared/services/backflow/backflow-gauge.service';
import { ProfesisonalService } from '../../../shared/services/professionals/professional.service';
import { ProfesionalUserService } from '../../../shared/services/professionals/professional-user.service';
import { ProfessionalSupplierService } from '../../../shared/services/professionals/professional-supplier.service';
import { Professional } from '../../../shared/models/professionals/professional';
import { ExpirationType, ProfessionalUser } from '../../../shared/models/professionals/professional-user';
import { ProfessionalWaterSupplier } from '../../../shared/models/professionals/professional-water-supplier';
import { BackflowGauge, GaugeExpirationType } from '../../../shared/models/backflow/backflow-gauge';
import { BackflowTestResult, BackflowReasonForTest, BackflowDeviceType } from '../../../shared/models/backflow/backflow-test-enums';
import { InputOption } from '../../../shared/components/input/input.component';
import { MAX_PAGE_SIZE } from '../../../shared/models/page-info';

@Component({
    standalone: false,
    templateUrl: './backflow-test-submit.component.html'
})
export class BackflowTestSubmitComponent implements OnInit {
    public isLoading = false;
    public submitSuccess = false;
    public submitted = false;
    public validationErrors: string[] = [];

    public professional?: Professional;
    public selectedBpat?: ProfessionalUser;
    public selectedWaterSupplier?: ProfessionalWaterSupplier;
    public selectedGauge?: BackflowGauge;
    public previousTest?: BackflowTest;

    private _bpats: ProfessionalUser[] = [];
    private _waterSuppliers: ProfessionalWaterSupplier[] = [];
    private _gauges: BackflowGauge[] = [];

    public bpatOptions: InputOption[] = [];
    public waterSupplierOptions: InputOption[] = [];
    public gaugeOptions: InputOption[] = [];

    public selectedBpatId?: number;
    public selectedWaterSupplierId?: number;
    public selectedGaugeId?: number;

    public readonly BackflowTestResult = BackflowTestResult;
    public readonly BackflowReasonForTest = BackflowReasonForTest;
    public readonly BackflowDeviceType = BackflowDeviceType;
    public readonly ExpirationType = ExpirationType;
    public readonly GaugeExpirationType = GaugeExpirationType;

    public readonly deviceTypeOptions: InputOption[];
    public readonly hazardTypeOptions: InputOption[];
    public readonly reasonOptions: InputOption[];

    public images: BackflowTestImages = {};
    public assemblyImagePreview: string | null = null;
    public serialNumberImagePreview: string | null = null;
    public bypassAssemblyImagePreview: string | null = null;
    public bypassSerialNumberImagePreview: string | null = null;
    public airGapImagePreview: string | null = null;

    public onAssemblyFileInputChange(event: Event): void {
        const file = (event.target as HTMLInputElement).files?.[0] ?? null;
        this.onAssemblyImageChange(file);
        (event.target as HTMLInputElement).value = '';
    }

    public onAssemblyImageChange(file: File | null): void {
        this.images.assemblyImage = file;
        this.assemblyImagePreview = file ? URL.createObjectURL(file) : null;
    }

    public onSerialNumberFileInputChange(event: Event): void {
        const file = (event.target as HTMLInputElement).files?.[0] ?? null;
        this.onSerialNumberImageChange(file);
        (event.target as HTMLInputElement).value = '';
    }

    public onSerialNumberImageChange(file: File | null): void {
        this.images.serialNumberImage = file;
        this.serialNumberImagePreview = file ? URL.createObjectURL(file) : null;
    }

    public onBypassAssemblyFileInputChange(event: Event): void {
        const file = (event.target as HTMLInputElement).files?.[0] ?? null;
        this.onBypassAssemblyImageChange(file);
        (event.target as HTMLInputElement).value = '';
    }

    public onBypassAssemblyImageChange(file: File | null): void {
        this.images.bypassAssemblyImage = file;
        this.bypassAssemblyImagePreview = file ? URL.createObjectURL(file) : null;
    }

    public onBypassSerialNumberFileInputChange(event: Event): void {
        const file = (event.target as HTMLInputElement).files?.[0] ?? null;
        this.onBypassSerialNumberImageChange(file);
        (event.target as HTMLInputElement).value = '';
    }

    public onBypassSerialNumberImageChange(file: File | null): void {
        this.images.bypassSerialNumberImage = file;
        this.bypassSerialNumberImagePreview = file ? URL.createObjectURL(file) : null;
    }

    public onAirGapFileInputChange(event: Event): void {
        const file = (event.target as HTMLInputElement).files?.[0] ?? null;
        this.onAirGapImageChange(file);
        (event.target as HTMLInputElement).value = '';
    }

    public onAirGapImageChange(file: File | null): void {
        this.images.airGapImage = file;
        this.airGapImagePreview = file ? URL.createObjectURL(file) : null;
    }

    public model: BackflowTest = {
        id: 0,
        testResult: BackflowTestResult.Pass,
        properlyInstalled: true,
        nonPotable: false,
        ossf: false
    };

    // Repair checkboxes (serialized to text strings in the model)
    public repairCV1 = { cleaned: false, disc: false, spring: false, guide: false, pinRetainer: false, hingePin: false, seat: false, diaphragm: false };
    public repairCV2 = { cleaned: false, disc: false, spring: false, guide: false, pinRetainer: false, hingePin: false, seat: false, diaphragm: false };
    public repairCV12 = { cleaned: false, disc: false, spring: false, guide: false, pinRetainer: false, hingePin: false, seat: false, diaphragm: false };
    public repairCV22 = { cleaned: false, disc: false, spring: false, guide: false, pinRetainer: false, hingePin: false, seat: false, diaphragm: false };
    public repairRV = { cleaned: false, discUpper: false, discLower: false, spring: false, diaphragmUpper: false, diaphragmLower: false, diaphragmSmall: false, seatUpper: false, seatLower: false, spacerLower: false };
    public repairRV2 = { cleaned: false, discUpper: false, discLower: false, spring: false, diaphragmUpper: false, diaphragmLower: false, diaphragmSmall: false, seatUpper: false, seatLower: false, spacerLower: false };
    public repairBC = { cleaned: false, disc: false, spring: false, guide: false, pinRetainer: false, hingePin: false, seat: false, diaphragm: false };

    public get verificationComplete(): boolean {
        if (!this.selectedBpatId || !this.selectedWaterSupplierId || (!this.isAirGap && !this.selectedGaugeId)) {
            return false;
        }
        if (this.selectedBpat?.bpatLicenseExpirationType === ExpirationType.Expired
            || this.professional?.insuranceExpirationType === ExpirationType.Expired
            || this.selectedGauge?.expirationType === GaugeExpirationType.Expired
        ) {
            return false;
        }
        
        return true;
    }
    public get isAirGap(): boolean { return this.model.deviceType === BackflowDeviceType.AG; }
    public get deviceTypeLabel(): string {
        return this.deviceTypeOptions.find(o => o.id === this.model.deviceType)?.text ?? '';
    }
    public get isDC(): boolean { return [BackflowDeviceType.DC, BackflowDeviceType.DCD, BackflowDeviceType.DCD2].includes(this.model.deviceType as BackflowDeviceType); }
    public get isRP(): boolean { return [BackflowDeviceType.RP, BackflowDeviceType.RPPD, BackflowDeviceType.RPPD2].includes(this.model.deviceType as BackflowDeviceType); }
    public get isPVB(): boolean { return [BackflowDeviceType.PVB, BackflowDeviceType.SVB].includes(this.model.deviceType as BackflowDeviceType); }
    public get hasBypassCV(): boolean { return [BackflowDeviceType.DCD, BackflowDeviceType.RPPD].includes(this.model.deviceType as BackflowDeviceType); }
    public get hasBypassBC(): boolean { return [BackflowDeviceType.DCD2, BackflowDeviceType.RPPD2].includes(this.model.deviceType as BackflowDeviceType); }
    public get initialTestFailed(): boolean {
        if (!this.model.initialTestDate) return false;
        if (this.isDC) return !this.model.initCV1ClosedTight || !!this.model.initCV1Leaked || !this.model.initCV2ClosedTight || !!this.model.initCV2Leaked;
        if (this.isRP) return !this.model.initCV1ClosedTight || !!this.model.initCV1Leaked || !!this.model.initRVDidNotOpen;
        if (this.isPVB) return !!this.model.initPvbAirInletDidNotOpen || !this.model.initPvbAirInletFullyOpened || !!this.model.initPvbCVLeaked;
        return false;
    }
    public get isOtherHazardType(): boolean { return this.model.hazardType === 'Other'; }
    public get remarksLength(): number { return this.model.comments?.length ?? 0; }

    constructor(
        private readonly _activatedRoute: ActivatedRoute,
        private readonly _router: Router,
        private readonly _backflowTestService: BackflowTestService,
        private readonly _gaugeService: BackflowGaugeService,
        private readonly _professionalService: ProfesisonalService,
        private readonly _userService: ProfesionalUserService,
        private readonly _supplierService: ProfessionalSupplierService,
        private readonly _options: BackflowTestOptionsService
    ) {
        this.deviceTypeOptions = this._options.deviceTypeOptions;
        this.hazardTypeOptions = this._options.hazardTypeOptions;
        this.reasonOptions = this._options.reasonOptions;
    }

    public ngOnInit(): void {
        this._activatedRoute.paramMap.subscribe(async params => {
            const testId = params.get('testId');
            await this.loadData(testId && testId !== 'new' ? Number(testId) : null);
        });
    }

    public onBpatChange(value: number): void {
        this.selectedBpatId = value;
        this.selectedBpat = this._bpats.find(u => u.id === value);
        this.model.bpatLicenseNumber = this.selectedBpat?.bpatLicenseNumber;
        this.model.bpatLicenseExpiration = this.selectedBpat?.bpatLicenseExpirationDate;
    }

    public onWaterSupplierChange(value: number): void {
        this.selectedWaterSupplierId = value;
        this.selectedWaterSupplier = this._waterSuppliers.find(s => s.waterSupplier?.id === value);
    }

    public onGaugeChange(value: number): void {
        this.selectedGaugeId = value;
        this.selectedGauge = this._gauges.find(g => g.id === value);
    }

    public async submit(form: NgForm): Promise<void> {
        this.submitted = true;
        this.validationErrors = [];
        this.collectValidationErrors();

        if (!form.valid || this.validationErrors.length > 0) return;

        this.serializeRepairs();

        const submission: BackflowTest = {
            ...this.model,
            waterSupplier: this.selectedWaterSupplierId ? { id: this.selectedWaterSupplierId } : undefined,
            bpat: this.selectedBpatId ? { id: this.selectedBpatId } : undefined,
            site: this.previousTest?.site ? { id: this.previousTest.site.id } : undefined,
            gaugeManufacturer: this.selectedGauge?.manufacturer,
            gaugeModel: this.selectedGauge?.model,
            gaugeSerialNumber: this.selectedGauge?.serialNumber,
            gaugeLastCalibrationDate: this.selectedGauge?.lastCalibrationDate as string | undefined
        };

        this.isLoading = true;
        try {
            await this._backflowTestService.submit(submission, this.images);
            this.submitSuccess = true;
        } finally {
            this.isLoading = false;
        }
    }

    public submitAnother(): void {
        this._router.navigate(['..'], { relativeTo: this._activatedRoute });
    }

    public returnToAccountOverview(): void {
        this._router.navigate(['/']);
    }

    private async loadData(fromTestId: number | null): Promise<void> {
        this.isLoading = true;
        try {
            const [professional, usersPage, gaugesPage] = await Promise.all([
                this._professionalService.getLoggedInProfessional(),
                this._userService.getAll({ pageSize: MAX_PAGE_SIZE }, { sort: {}, filter: [{ columnName: 'isBackflowTester', comparisonOperator: 'Eq', value: 'true' }] }),
                this._gaugeService.getAll({ pageSize: MAX_PAGE_SIZE }, {})
            ]);

            this.professional = professional;
            this._bpats = usersPage.data ?? [];
            this._gauges = gaugesPage.data ?? [];

            const suppliersPage = await this._supplierService.getAllMy(false, true);
            this._waterSuppliers = suppliersPage.data ?? [];

            this.buildOptions();

            if (fromTestId) {
                this.previousTest = await this._backflowTestService.getForProfessional(fromTestId);
                this.populateFromPreviousTest(this.previousTest);
            }

            await this.setDefaults();
        } finally {
            this.isLoading = false;
        }
    }

    private buildOptions(): void {
        this.bpatOptions = this._bpats.map(u => ({ id: u.id, text: u.contactName ?? `User ${u.id}` }));
        this.waterSupplierOptions = this._waterSuppliers.map(ws => ({ id: ws.waterSupplier?.id, text: ws.waterSupplier?.name ?? '' }));
        this.gaugeOptions = this._gauges
            .filter(g => g.expirationType !== GaugeExpirationType.Expired)
            .map(g => ({ id: g.id, text: `${g.manufacturer} ${g.model} ${g.serialNumber}` }));
    }

    private async setDefaults(): Promise<void> {
        const myUser = await this._userService.getMyData();
        const defaultBpat = this._bpats.find(u => u.id === myUser.id) ?? this._bpats[0];
        if (defaultBpat?.id != null) {
            this.selectedBpatId = defaultBpat.id;
            this.selectedBpat = defaultBpat;
            this.model.bpatLicenseNumber = defaultBpat.bpatLicenseNumber;
            this.model.bpatLicenseExpiration = defaultBpat.bpatLicenseExpirationDate;
        }
        if (this._waterSuppliers.length === 1) {
            this.selectedWaterSupplierId = this._waterSuppliers[0].waterSupplier?.id;
            this.selectedWaterSupplier = this._waterSuppliers[0];
        }
        const validGauges = this._gauges.filter(g => g.expirationType !== GaugeExpirationType.Expired);
        if (validGauges.length === 1) {
            this.selectedGaugeId = validGauges[0].id;
            this.selectedGauge = validGauges[0];
        }
    }

    private populateFromPreviousTest(test: BackflowTest): void {
        this.model.accountNumber = test.accountNumber;
        this.model.propertyBusinessName = test.propertyBusinessName;
        this.model.propertyType = test.propertyType;
        this.model.propertyStreetNumber = test.propertyStreetNumber;
        this.model.propertyStreetName = test.propertyStreetName;
        this.model.propertyNumber = test.propertyNumber;
        this.model.propertyCity = test.propertyCity;
        this.model.propertyState = test.propertyState;
        this.model.propertyZip = test.propertyZip;
        this.model.mailingCompanyName = test.mailingCompanyName;
        this.model.mailingContactName = test.mailingContactName;
        this.model.mailingStreetNumber = test.mailingStreetNumber;
        this.model.mailingStreetName = test.mailingStreetName;
        this.model.mailingNumber = test.mailingNumber;
        this.model.mailingCity = test.mailingCity;
        this.model.mailingState = test.mailingState;
        this.model.mailingZip = test.mailingZip;
        this.model.mailingPhoneNumber = test.mailingPhoneNumber;
        this.model.mailingEmailAddress = test.mailingEmailAddress;
        this.model.deviceType = test.deviceType;
        this.model.manufacturer = test.manufacturer;
        this.model.model = test.model;
        this.model.size = test.size;
        this.model.serialNumber = test.serialNumber;
        this.model.locationDescription = test.locationDescription;
        this.model.hazardType = test.hazardType;
        this.model.hazardTypeOtherDescription = test.hazardTypeOtherDescription;
        this.model.bpatLicenseNumber = test.bpatLicenseNumber;
        this.model.bpatLicenseExpiration = test.bpatLicenseExpiration;
        this.model.replacementAssembly = test.replacementAssembly;
        this.model.manufacturer2 = test.manufacturer2;
        this.model.model2 = test.model2;
        this.model.size2 = test.size2;
        this.model.serialNumber2 = test.serialNumber2;
    }

    private serializeRepairs(): void {
        this.model.repairCV1 = this.serializeCV(this.repairCV1);
        this.model.repairCV2 = this.serializeCV(this.repairCV2);
        this.model.repairCV12 = this.serializeCV(this.repairCV12);
        this.model.repairCV22 = this.serializeCV(this.repairCV22);
        this.model.repairRV = this.serializeRV(this.repairRV);
        this.model.repairRV2 = this.serializeRV(this.repairRV2);
        this.model.repairBC = this.serializeCV(this.repairBC);
    }

    private serializeCV(cv: typeof this.repairCV1): string {
        const parts: string[] = [];
        if (cv.cleaned) parts.push('Cleaned');
        if (cv.disc) parts.push('Replaced Disc');
        if (cv.spring) parts.push('Replaced Spring');
        if (cv.guide) parts.push('Replaced Guide');
        if (cv.pinRetainer) parts.push('Replaced Pin Retainer');
        if (cv.hingePin) parts.push('Replaced Hinge Pin');
        if (cv.seat) parts.push('Replaced Seat');
        if (cv.diaphragm) parts.push('Replaced Diaphragm');
        return parts.join(', ');
    }

    private serializeRV(rv: typeof this.repairRV): string {
        const parts: string[] = [];
        if (rv.cleaned) parts.push('Cleaned');
        if (rv.discUpper) parts.push('Replaced Disc Upper');
        if (rv.discLower) parts.push('Replaced Disc Lower');
        if (rv.spring) parts.push('Replaced Spring');
        if (rv.diaphragmUpper) parts.push('Replaced Diaphragm Upper');
        if (rv.diaphragmLower) parts.push('Replaced Diaphragm Lower');
        if (rv.diaphragmSmall) parts.push('Replaced Diaphragm Small');
        if (rv.seatUpper) parts.push('Replaced Seat Upper');
        if (rv.seatLower) parts.push('Replaced Seat Lower');
        if (rv.spacerLower) parts.push('Replaced Spacer Lower');
        return parts.join(', ');
    }

    private collectValidationErrors(): void {
        if (!this.selectedBpatId) {
            this.validationErrors.push('Please select a BPAT account.');
        }
        if (!this.selectedWaterSupplierId) {
            this.validationErrors.push('Please select a water supplier.');
        }
        if (!this.model.hazardType) {
            this.validationErrors.push('Please select a hazard type.');
        }
        if (!this.isAirGap && !this.selectedGaugeId) {
            this.validationErrors.push('Please select a test gauge.');
        }
        if (!this.model.deviceType) {
            this.validationErrors.push('Please select a device type.');
        }

        if (!this.isAirGap) {
            if (!this.model.initialTestDate) {
                this.validationErrors.push('Please enter the initial test date and time.');
            } else if (new Date(this.model.initialTestDate) > new Date()) {
                this.validationErrors.push('Initial test date cannot be in the future.');
            }
            if (this.initialTestFailed) {
                if (!this.model.finalTestDate) {
                    this.validationErrors.push('Please enter the final test date and time after repairs.');
                } else if (new Date(this.model.finalTestDate) > new Date()) {
                    this.validationErrors.push('Final test date cannot be in the future.');
                } else if (this.model.initialTestDate && new Date(this.model.initialTestDate) > new Date(this.model.finalTestDate)) {
                    this.validationErrors.push('Initial test date cannot be after the final test date.');
                }
            }
        } else {
            if (!this.model.initialTestDate) {
                this.validationErrors.push('Please enter the air gap test date and time.');
            } else if (new Date(this.model.initialTestDate) > new Date()) {
                this.validationErrors.push('Air gap test date cannot be in the future.');
            }
        }
    }
}
