import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { NgForm } from '@angular/forms';
import { BackflowTest } from '../../../shared/models/backflow/backflow-test';
import { BackflowTestService } from '../../../shared/services/backflow/backflow-test.service';
import { BackflowGaugeService } from '../../../shared/services/backflow/backflow-gauge.service';
import { ProfesisonalService } from '../../../shared/services/professionals/professional.service';
import { ProfesionalUserService } from '../../../shared/services/professionals/professional-user.service';
import { ProfessionalSupplierService } from '../../../shared/services/professionals/professional-supplier.service';
import { Professional } from '../../../shared/models/professionals/professional';
import { ExpirationType, ProfessionalUser } from '../../../shared/models/professionals/professional-user';
import { ProfessionalWaterSupplier } from '../../../shared/models/professionals/professional-water-supplier';
import { BackflowGauge } from '../../../shared/models/backflow/backflow-gauge';
import { BackflowTestResult, BackflowReasonForTest } from '../../../shared/models/backflow/backflow-test-enums';
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
    public readonly ExpirationType = ExpirationType;

    public readonly deviceTypeOptions: InputOption[] = [
        { id: 'DC', text: 'DC - Double Check Valve' },
        { id: 'DCD', text: 'DCD - Double Check Detector' },
        { id: 'DCD2', text: 'DCD2 - Double Check Detector Type II' },
        { id: 'RP', text: 'RP - Reduced Pressure Principle' },
        { id: 'RPPD', text: 'RPPD - Reduced Pressure Principle Detector' },
        { id: 'RPPD2', text: 'RPPD2 - Reduced Pressure Principle Detector Type II' },
        { id: 'PVB', text: 'PVB - Pressure Vacuum Breaker' },
        { id: 'SVB', text: 'SVB - Spill-Resistant Pressure Vacuum Breaker' },
        { id: 'AG', text: 'AG - Air Gap' }
    ];

    public readonly hazardTypeOptions: InputOption[] = [
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

    public readonly reasonOptions: InputOption[] = [
        { id: BackflowReasonForTest.AnnualTest, text: 'Annual Test' },
        { id: BackflowReasonForTest.NewInstallation, text: 'New Installation' },
        { id: BackflowReasonForTest.ExistingInstallation, text: 'Existing Installation' },
        { id: BackflowReasonForTest.Replacement, text: 'Replacement' },
        { id: BackflowReasonForTest.Repair, text: 'Repair' },
        { id: BackflowReasonForTest.AnnualTestAfterRepairs, text: 'Annual Test After Repairs' }
    ];

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
        return !!this.selectedBpatId && !!this.selectedWaterSupplierId;
    }
    public get isAirGap(): boolean { return this.model.deviceType === 'AG'; }
    public get deviceTypeLabel(): string {
        return this.deviceTypeOptions.find(o => o.id === this.model.deviceType)?.text ?? '';
    }
    public get isDC(): boolean { return ['DC', 'DCD', 'DCD2'].includes(this.model.deviceType ?? ''); }
    public get isRP(): boolean { return ['RP', 'RPPD', 'RPPD2'].includes(this.model.deviceType ?? ''); }
    public get isPVB(): boolean { return ['PVB', 'SVB'].includes(this.model.deviceType ?? ''); }
    public get hasBypassCV(): boolean { return ['DCD', 'RPPD'].includes(this.model.deviceType ?? ''); }
    public get hasBypassBC(): boolean { return ['DCD2', 'RPPD2'].includes(this.model.deviceType ?? ''); }
    public get initialTestFailed(): boolean {
        if (!this.model.initialTestDate) return false;
        if (this.isDC) return !this.model.initCV1ClosedTight || !!this.model.initCV1Leaked || !this.model.initCV2ClosedTight || !!this.model.initCV2Leaked;
        if (this.isRP) return !this.model.initCV1ClosedTight || !!this.model.initCV1Leaked || !!this.model.initRVDidNotOpen;
        if (this.isPVB) return !!this.model.initPvbAirInletDidNotOpen || !this.model.initPvbAirInletFullyOpened || !!this.model.initPvbCVLeaked;
        return false;
    }
    public get remarksLength(): number { return this.model.comments?.length ?? 0; }

    constructor(
        private readonly _activatedRoute: ActivatedRoute,
        private readonly _router: Router,
        private readonly _backflowTestService: BackflowTestService,
        private readonly _gaugeService: BackflowGaugeService,
        private readonly _professionalService: ProfesisonalService,
        private readonly _userService: ProfesionalUserService,
        private readonly _supplierService: ProfessionalSupplierService
    ) { }

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
            await this._backflowTestService.submit(submission);
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
                this._userService.getAll({ pageSize: MAX_PAGE_SIZE }, {}),
                this._gaugeService.getAll({ pageSize: MAX_PAGE_SIZE }, {})
            ]);

            this.professional = professional;
            this._bpats = (usersPage.data ?? []).filter(u => u.isBackflowTester);
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
        this.gaugeOptions = this._gauges.map(g => ({ id: g.id, text: `${g.manufacturer} ${g.model} ${g.serialNumber}` }));
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
        if (this._gauges.length === 1) {
            this.selectedGaugeId = this._gauges[0].id;
            this.selectedGauge = this._gauges[0];
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
