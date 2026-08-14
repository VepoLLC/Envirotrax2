import { Component, OnInit, ViewChild, TemplateRef } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../shared/services/auth/auth.service';
import { CsiInspectionService } from '../../shared/services/csi/csi-inspection.service';
import { CsiInspection } from '../../shared/models/csi/csi-inspection';
import { ProfessionalFogInspectionService } from '../../shared/services/fog/professional-fog-inspection.service';
import { FogInspection } from '../../shared/models/fog/fog-inspection';
import { FogInspectionResult } from '../../shared/models/fog/fog-inspection-enums';
import { FeatureType } from '../../shared/models/feature-type';
import { ROLE_DEFINITIONS } from '../../shared/models/role-definitions';
import { ProfesionalUserService } from '../../shared/services/professionals/professional-user.service';
import { ProfessionalUserLicenseService } from '../../shared/services/professionals/professional-user-license.service';
import { ProfessionalInsuranceService } from '../../shared/services/professionals/professional-insurance.service';
import { BackflowGaugeService } from '../../shared/services/backflow/backflow-gauge.service';
import { BackflowTestService, BackflowExpiryRangeKey } from '../../shared/services/backflow/backflow-test.service';
import { BackflowTest } from '../../shared/models/backflow/backflow-test';
import { BackflowTestResult } from '../../shared/models/backflow/backflow-test-enums';
import { ProfessionalDashboardService } from '../../shared/services/professionals/professional-dashboard.service';
import { ProfessionalUser } from '../../shared/models/professionals/professional-user';
import { ProfessionalUserLicense, ExpirationType } from '../../shared/models/professionals/licenses/professional-user-license';
import { ProfessionalInsurance } from '../../shared/models/professionals/professional-insurance';
import { BackflowGauge, GaugeExpirationType } from '../../shared/models/backflow/backflow-gauge';
import { ProfessionalFogVehicleService } from '../../shared/services/fog/professional-fog-vehicle.service';
import { ProfessionalFogDisposalSiteService } from '../../shared/services/fog/professional-fog-disposal-site.service';
import { FogVehicle } from '../../shared/models/fog/fog-vehicle';
import { FogDisposalSite } from '../../shared/models/fog/fog-disposal-site';
import { FOG_VEHICLE_CAPACITY_TYPE_LABELS } from '../../shared/models/fog/fog-vehicle-enums';
import { PHYSICAL_TYPE_LABELS } from '../../shared/models/fog/fog-disposal-site-enums';
import { ProfessionalDashboardStats } from '../../shared/models/professionals/professional-dashboard-stats';
import { TableViewModel } from '../../shared/models/table-view-model';
import { CellTemplateData, ColumnType, FreeTextSearchSettings, TableColumn, ModalHelperService } from '@envirotrax/common-ui';
import { ModalSize } from '@developer-partners/ngx-modal-dialog';
import { FogSignaturePadModalComponent, FogSignatureModel } from '../fog/inspections/create/fog-signature-pad-modal.component';
import { AppContainerHelperService } from '../../shared/services/helpers/app-contaner-helper.service';

const VIEW_MODE_KEY = 'dashboardViewMode';

@Component({
    standalone: false,
    templateUrl: './dashboard.component.html'
})
export class DashboardComponent implements OnInit {
    // CSI inspection table templates
    @ViewChild('statusTemplate', { static: true })
    public statusTemplate!: TemplateRef<CellTemplateData<CsiInspection>>;

    @ViewChild('propertyTemplate', { static: true })
    public propertyTemplate!: TemplateRef<CellTemplateData<CsiInspection>>;

    @ViewChild('mailingTemplate', { static: true })
    public mailingTemplate!: TemplateRef<CellTemplateData<CsiInspection>>;


    @ViewChild('fogStatusTemplate', { static: true })
    public fogStatusTemplate!: TemplateRef<CellTemplateData<FogInspection>>;

    @ViewChild('fogPropertyTemplate', { static: true })
    public fogPropertyTemplate!: TemplateRef<CellTemplateData<FogInspection>>;

    @ViewChild('fogMailingTemplate', { static: true })
    public fogMailingTemplate!: TemplateRef<CellTemplateData<FogInspection>>;

    // License & insurance cell templates
    @ViewChild('licenseExpirationTemplate', { static: true })
    public licenseExpirationTemplate!: TemplateRef<CellTemplateData<ProfessionalUserLicense>>;

    @ViewChild('insuranceExpirationTemplate', { static: true })
    public insuranceExpirationTemplate!: TemplateRef<CellTemplateData<ProfessionalInsurance>>;

    // Gauge cell templates
    @ViewChild('gaugeCellTemplate', { static: true })
    public gaugeCellTemplate!: TemplateRef<CellTemplateData<BackflowGauge>>;

    @ViewChild('gaugeTypeTemplate', { static: true })
    public gaugeTypeTemplate!: TemplateRef<CellTemplateData<BackflowGauge>>;

    @ViewChild('gaugeTestDateTemplate', { static: true })
    public gaugeTestDateTemplate!: TemplateRef<CellTemplateData<BackflowGauge>>;

    @ViewChild('backflowStatusTemplate', { static: true })
    public backflowStatusTemplate!: TemplateRef<CellTemplateData<BackflowTest>>;

    @ViewChild('backflowDatesTemplate', { static: true })
    public backflowDatesTemplate!: TemplateRef<CellTemplateData<BackflowTest>>;

    @ViewChild('backflowSerialTemplate', { static: true })
    public backflowSerialTemplate!: TemplateRef<CellTemplateData<BackflowTest>>;

    @ViewChild('backflowAssemblyTemplate', { static: true })
    public backflowAssemblyTemplate!: TemplateRef<CellTemplateData<BackflowTest>>;

    @ViewChild('backflowPropertyTemplate', { static: true })
    public backflowPropertyTemplate!: TemplateRef<CellTemplateData<BackflowTest>>;

    @ViewChild('backflowMailingTemplate', { static: true })
    public backflowMailingTemplate!: TemplateRef<CellTemplateData<BackflowTest>>;

    public hasCsi = false;
    public hasFog = false;
    public hasBackflow = false;
    public hasFogTransportation = false;
    public isAdmin = false;
    public signatureUrl: string | null = null;
    public readonly FogInspectionResult = FogInspectionResult;
    public readonly BackflowTestResult = BackflowTestResult;
    public isLoading = true;
    public isStatsLoading = false;

    public viewMode: 'quick' | 'full' = 'full';

    public dashboardStats: ProfessionalDashboardStats = {};

    public subAccountsTable: TableViewModel<ProfessionalUser> = {
        query: { sort: {}, filter: [] },
        columns: [],
        freeTextSearch: {
            searchQuery: [
                { field: 'contactName', operator: 'Ct', multiWordSearch: true },
                { field: 'emailAddress', operator: 'Ct' }
            ]
        } as FreeTextSearchSettings
    };

    public licensesTable: TableViewModel<ProfessionalUserLicense> = {
        query: { sort: {}, filter: [] },
        columns: [],
        freeTextSearch: {
            searchQuery: [
                { field: 'licenseType.name' },
                { field: 'licenseNumber' }
            ]
        } as FreeTextSearchSettings
    };

    public insurancesTable: TableViewModel<ProfessionalInsurance> = {
        query: { sort: {}, filter: [] },
        columns: [],
        freeTextSearch: {
            searchQuery: [
                { field: 'insuranceNumber' }
            ]
        } as FreeTextSearchSettings
    };

    public gaugesTable: TableViewModel<BackflowGauge> = {
        query: { sort: {}, filter: [] },
        columns: [],
        freeTextSearch: {
            searchQuery: [
                { field: 'manufacturer' },
                { field: 'model' },
                { field: 'serialNumber' }
            ]
        } as FreeTextSearchSettings
    };

    // Sorted by id so paging is deterministic (Skip/Take with no ORDER BY can repeat or drop rows).
    // V1's Account Overview orders these the same way — main.aspx.vb:444 "ORDER BY ID ASC".
    public vehiclesTable: TableViewModel<FogVehicleRow> = {
        query: { sort: { id: 'Asc' }, filter: [] },
        columns: []
    };

    public disposalSitesTable: TableViewModel<FogDisposalSiteRow> = {
        query: { sort: { county: 'Asc' }, filter: [] },
        columns: []
    };

    public readonly ExpirationType = ExpirationType;
    public readonly GaugeExpirationType = GaugeExpirationType;

    public licenseInsuranceTab: 'licenses' | 'insurances' = 'licenses';

    public setLicenseInsuranceTab(tab: 'licenses' | 'insurances'): void {
        this.licenseInsuranceTab = tab;
    }

    public get licenseAndInsuranceCount(): number {
        return (this.dashboardStats.licenseCount ?? 0) + (this.dashboardStats.insuranceCount ?? 0);
    }

    public recentInspections: TableViewModel<CsiInspection> = {
        query: {},
        columns: []
    };

    public recentFogInspections: TableViewModel<FogInspection> = {
        query: {},
        columns: []
    };

    public recentBackflowTests: TableViewModel<BackflowTest> = {
        query: {},
        columns: []
    };

    public backflowExpiryButtons: { key: BackflowExpiryRangeKey; label: string; count: number; cssClass: string }[] = [];

    constructor(
        private readonly _authService: AuthService,
        private readonly _inspectionService: CsiInspectionService,
        private readonly _fogInspectionService: ProfessionalFogInspectionService,
        private readonly _userService: ProfesionalUserService,
        private readonly _licenseService: ProfessionalUserLicenseService,
        private readonly _insuranceService: ProfessionalInsuranceService,
        private readonly _gaugeService: BackflowGaugeService,
        private readonly _backflowTestService: BackflowTestService,
        private readonly _dashboardService: ProfessionalDashboardService,
        private readonly _vehicleService: ProfessionalFogVehicleService,
        private readonly _disposalSiteService: ProfessionalFogDisposalSiteService,
        private readonly _router: Router,
        private readonly _containerHelper: AppContainerHelperService,
        private readonly _modalHelper: ModalHelperService
    ) { }

    public async ngOnInit(): Promise<void> {
        this._containerHelper.setContainerVisibility(false);
        this.viewMode = (localStorage.getItem(VIEW_MODE_KEY) as 'quick' | 'full') ?? 'full';

        this.setupColumns();

        try {
            const [hasCsi, hasFog, hasBackflow, hasFogTransportation, isCsiInspector, isFogInspector, isBackflowTester, isFogTransporter, isAdmin] = await Promise.all([
                this._authService.hasAnyFeatures(FeatureType.CsiInspection),
                this._authService.hasAnyFeatures(FeatureType.FogInspection),
                this._authService.hasAnyFeatures(FeatureType.BackflowTesting),
                this._authService.hasAnyFeatures(FeatureType.FogTransportation),
                this._authService.hasAnyRoles(ROLE_DEFINITIONS.PROFESSIONALS.CSI_INSPECTOR),
                this._authService.hasAnyRoles(ROLE_DEFINITIONS.PROFESSIONALS.FOG_INSPECTOR),
                this._authService.hasAnyRoles(ROLE_DEFINITIONS.PROFESSIONALS.BACKFLOW_TESTER),
                this._authService.hasAnyRoles(ROLE_DEFINITIONS.PROFESSIONALS.FOG_TRANSPORTER),
                this._authService.hasAnyRoles(ROLE_DEFINITIONS.PROFESSIONALS.ADMIN)
            ]);

            this.isAdmin = isAdmin;
            this.hasCsi = hasCsi && (isCsiInspector || isAdmin);
            this.hasFog = hasFog && (isFogInspector || isAdmin);
            this.hasBackflow = hasBackflow && (isBackflowTester || isAdmin);
            this.hasFogTransportation = hasFogTransportation && (isFogTransporter || isAdmin);

            const promises: Promise<void>[] = [];

            if (this.hasCsi) {
                promises.push(this.loadRecentInspections());
            }

            if (this.hasFog) {
                promises.push(this.loadRecentFogInspections());
            }

            if (this.hasFogTransportation) {
                promises.push(this.loadSignature());
                promises.push(this.loadVehicles());
                promises.push(this.loadDisposalSites());
            }

            // The quick-view tiles read every count, so the stats are needed by any
            // role that has at least one tile — not just admins.
            if (this.isAdmin || this.hasBackflow || this.hasFogTransportation) {
                promises.push(this.loadStats());
            }

            if (this.isAdmin) {
                promises.push(
                    this.loadSubAccounts(),
                    this.loadLicenses(),
                    this.loadInsurances()
                );
            }
            if (this.hasBackflow) {
                promises.push(this.loadGauges());
                promises.push(this.loadRecentBackflowTests());
                promises.push(this.loadBackflowExpiryCounts());
            }

            await Promise.all(promises);
        } finally {
            this.isLoading = false;
        }
    }

    public openSignaturePad(): void {
        this._modalHelper.show<FogSignatureModel, string>(
            FogSignaturePadModalComponent,
            {
                title: 'Signature',
                size: ModalSize.extraLarge,
                model: { existingSignature: null }
            }
        ).result().subscribe((dataUrl: string) => {
            if (dataUrl) {
                this.saveSignature(dataUrl);
            }
        });
    }

    private async loadSignature(): Promise<void> {
        const user = await this._userService.getMyData();
        this.signatureUrl = user.signatureUrl ?? null;
    }

    private async saveSignature(dataUrl: string): Promise<void> {
        const file = this.dataUrlToFile(dataUrl, 'transporter-signature.png');
        this.signatureUrl = await this._userService.saveMySignature(file);
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

    private setupColumns(): void {
        this.recentInspections.columns = this.buildInspectionColumns();
        this.recentFogInspections.columns = this.buildFogInspectionColumns();
        this.recentBackflowTests.columns = this.buildBackflowTestColumns();
        this.subAccountsTable.columns = this.buildSubAccountsColumns();
        this.licensesTable.columns = this.buildLicensesColumns();
        this.insurancesTable.columns = this.buildInsurancesColumns();
        this.gaugesTable.columns = this.buildGaugesColumns();
        this.vehiclesTable.columns = this.buildVehiclesColumns();
        this.disposalSitesTable.columns = this.buildDisposalSitesColumns();
    }

    public setViewMode(mode: 'quick' | 'full'): void {
        this.viewMode = mode;
        localStorage.setItem(VIEW_MODE_KEY, mode);
    }

    private async loadRecentInspections(): Promise<void> {
        try {
            this.recentInspections.isLoading = true;
            const userSort = { ...this.recentInspections.query.sort };
            delete userSort['inspectionDate'];
            const queryWithSort = {
                ...this.recentInspections.query,
                sort: { inspectionDate: 'Desc' as const, ...userSort }
            };
            this.recentInspections.items = await this._inspectionService.getProfessionalInspections(
                {},
                queryWithSort,
                true
            );
        } finally {
            this.recentInspections.isLoading = false;
        }
    }

    private async loadRecentFogInspections(): Promise<void> {
        try {
            this.recentFogInspections.isLoading = true;
            const userSort = { ...this.recentFogInspections.query.sort };
            delete userSort['inspectionDate'];
            const queryWithSort = {
                ...this.recentFogInspections.query,
                sort: { inspectionDate: 'Desc' as const, ...userSort }
            };
            this.recentFogInspections.items = await this._fogInspectionService.getAll(
                {},
                queryWithSort,
                true
            );
        } finally {
            this.recentFogInspections.isLoading = false;
        }
    }

    private async loadRecentBackflowTests(): Promise<void> {
        try {
            this.recentBackflowTests.isLoading = true;
            const userSort = { ...this.recentBackflowTests.query.sort };
            delete userSort['createdTime'];
            const queryWithSort = {
                ...this.recentBackflowTests.query,
                sort: { createdTime: 'Desc' as const, ...userSort }
            };
            this.recentBackflowTests.items = await this._backflowTestService.getAllForProfessional(
                { pageSize: 30 },
                queryWithSort
            );
        } finally {
            this.recentBackflowTests.isLoading = false;
        }
    }

    private async loadBackflowExpiryCounts(): Promise<void> {
        const counts = await this._backflowTestService.getExpiryCounts();

        const defs: { key: BackflowExpiryRangeKey; cssClass: string; count: number }[] = [
            { key: 'expired', cssClass: 'btn-danger', count: counts.expired },
            { key: 'thismonth', cssClass: 'btn-warning', count: counts.thisMonth },
            { key: 'nextmonth', cssClass: 'btn-warning', count: counts.nextMonth },
            { key: 'twomonths', cssClass: 'btn-warning', count: counts.twoMonths }
        ];

        this.backflowExpiryButtons = defs.map(d => ({
            key: d.key,
            cssClass: d.cssClass,
            count: d.count,
            label: this.buildBackflowExpiryLabel(d.key, d.count)
        }));
    }

    private buildBackflowExpiryLabel(key: BackflowExpiryRangeKey, count: number): string {
        if (key === 'expired') {
            return `View Expired Tests Within the Last 6 Months (${count})`;
        }

        const offset = key === 'thismonth' ? 0 : key === 'nextmonth' ? 1 : 2;
        const date = new Date();
        date.setDate(1);
        date.setMonth(date.getMonth() + offset);
        const month = date.toLocaleString('en-US', { month: 'short' });
        return `View Tests Expiring in ${month} ${date.getFullYear()} (${count})`;
    }

    public viewExpiringTests(key: BackflowExpiryRangeKey): void {
        this._router.navigate(['/professionals/backflow/tests'], { queryParams: { expiring: key } });
    }

    public async loadStats(): Promise<void> {
        try {
            this.isStatsLoading = true;
            this.dashboardStats = await this._dashboardService.getStats();
        } finally {
            this.isStatsLoading = false;
        }
    }

    public async loadSubAccounts(): Promise<void> {
        try {
            this.subAccountsTable.isLoading = true;
            this.subAccountsTable.items = await this._userService.getAll(
                this.subAccountsTable.items?.pageInfo || {},
                this.subAccountsTable.query
            );
        } finally {
            this.subAccountsTable.isLoading = false;
        }
    }

    public async loadLicenses(): Promise<void> {
        try {
            this.licensesTable.isLoading = true;
            this.licensesTable.items = await this._licenseService.getAll(
                this.licensesTable.items?.pageInfo || {},
                this.licensesTable.query
            );
        } finally {
            this.licensesTable.isLoading = false;
        }
    }

    public async loadInsurances(): Promise<void> {
        try {
            this.insurancesTable.isLoading = true;
            this.insurancesTable.items = await this._insuranceService.getAll(
                this.insurancesTable.items?.pageInfo || {},
                this.insurancesTable.query
            );
        } finally {
            this.insurancesTable.isLoading = false;
        }
    }

    public async loadGauges(): Promise<void> {
        try {
            this.gaugesTable.isLoading = true;
            this.gaugesTable.items = await this._gaugeService.getAll(
                this.gaugesTable.items?.pageInfo || {},
                this.gaugesTable.query
            );
        } finally {
            this.gaugesTable.isLoading = false;
        }
    }

    public async loadVehicles(): Promise<void> {
        try {
            this.vehiclesTable.isLoading = true;

            const vehicles = await this._vehicleService.getAll(
                this.vehiclesTable.items?.pageInfo || {},
                this.vehiclesTable.query
            );

            this.vehiclesTable.items = {
                pageInfo: vehicles.pageInfo,
                data: vehicles.data.map(vehicle => ({
                    ...vehicle,
                    capacityDescription: this.buildCapacityDescription(vehicle)
                }))
            };
        } finally {
            this.vehiclesTable.isLoading = false;
        }
    }

    public async loadDisposalSites(): Promise<void> {
        try {
            this.disposalSitesTable.isLoading = true;

            const sites = await this._disposalSiteService.getRegistered(
                this.disposalSitesTable.items?.pageInfo || {},
                this.disposalSitesTable.query
            );

            this.disposalSitesTable.items = {
                pageInfo: sites.pageInfo,
                data: sites.data.map(site => ({
                    ...site,
                    wasteTypeDescription: site.physicalType == null
                        ? ''
                        : PHYSICAL_TYPE_LABELS[site.physicalType] ?? ''
                }))
            };
        } finally {
            this.disposalSitesTable.isLoading = false;
        }
    }

    private buildCapacityDescription(vehicle: FogVehicle): string {
        if (vehicle.capacity == null) {
            return '';
        }

        const capacityType = vehicle.capacityType == null
            ? ''
            : FOG_VEHICLE_CAPACITY_TYPE_LABELS[vehicle.capacityType] ?? '';

        return `${vehicle.capacity} ${capacityType}`.trim();
    }

    public viewInspection(inspection: CsiInspection): void {
        const url = this._router.serializeUrl(
            this._router.createUrlTree(['/professionals/csi/inspections', inspection.id])
        );
        window.open(url, '_blank');
    }

    public viewBackflowTest(test: BackflowTest): void {
        if (test?.id == null) {
            return;
        }
        const url = this._router.serializeUrl(
            this._router.createUrlTree(['/professionals/backflow/tests', test.id, 'view'])
        );
        window.open(url, '_blank');
    }

    public isExpired(date?: string): boolean {
        if (!date) {
            return false;
        }

        return new Date(date) < new Date();
    }

    private buildInspectionColumns(): TableColumn<CsiInspection>[] {
        return [
            {
                field: '',
                caption: 'Status',
                type: ColumnType.other,
                queryColumnExcluded: true,
                cellTemplate: this.statusTemplate
            },
            {
                field: 'inspectionDate',
                caption: 'Inspection Date',
                type: ColumnType.date
            },
            {
                field: 'site.accountNumber',
                caption: 'Account Number',
                type: ColumnType.text
            },
            {
                field: '',
                caption: 'Property Information',
                type: ColumnType.other,
                queryColumnExcluded: true,
                cellTemplate: this.propertyTemplate
            },
            {
                field: '',
                caption: 'Mailing / Contact Information',
                type: ColumnType.other,
                queryColumnExcluded: true,
                cellTemplate: this.mailingTemplate
            }
        ];
    }

    private buildFogInspectionColumns(): TableColumn<FogInspection>[] {
        return [
            {
                field: '',
                caption: 'Status',
                type: ColumnType.other,
                queryColumnExcluded: true,
                cellTemplate: this.fogStatusTemplate
            },
            {
                field: 'inspectionDate',
                caption: 'Inspection Date',
                type: ColumnType.date
            },
            {
                field: 'site.accountNumber',
                caption: 'Account Number',
                type: ColumnType.text
            },
            {
                field: '',
                caption: 'Property Information',
                type: ColumnType.other,
                queryColumnExcluded: true,
                cellTemplate: this.fogPropertyTemplate
            },
            {
                field: '',
                caption: 'Mailing / Contact Information',
                type: ColumnType.other,
                queryColumnExcluded: true,
                cellTemplate: this.fogMailingTemplate
            }
        ];
    }

    private buildBackflowTestColumns(): TableColumn<BackflowTest>[] {
        return [
            {
                field: '',
                caption: 'Status',
                type: ColumnType.other,
                queryColumnExcluded: true,
                cellTemplate: this.backflowStatusTemplate
            },
            {
                field: 'testDate',
                caption: 'Submission/Test/Exp Dates',
                type: ColumnType.date,
                queryColumnExcluded: true,
                cellTemplate: this.backflowDatesTemplate
            },
            {
                field: 'serialNumber',
                caption: 'Serial Number',
                type: ColumnType.other,
                queryColumnExcluded: true,
                cellTemplate: this.backflowSerialTemplate
            },
            {
                field: 'manufacturer',
                caption: 'Assembly Description',
                type: ColumnType.other,
                queryColumnExcluded: true,
                cellTemplate: this.backflowAssemblyTemplate
            },
            {
                field: '',
                caption: 'Property Information',
                type: ColumnType.other,
                queryColumnExcluded: true,
                cellTemplate: this.backflowPropertyTemplate
            },
            {
                field: '',
                caption: 'Mailing Information',
                type: ColumnType.other,
                queryColumnExcluded: true,
                cellTemplate: this.backflowMailingTemplate
            }
        ];
    }

    private buildSubAccountsColumns(): TableColumn<ProfessionalUser>[] {
        return [
            { field: 'emailAddress', caption: 'UserID', type: ColumnType.text },
            { field: 'contactName', caption: 'Contact Name', type: ColumnType.text }
        ];
    }

    private buildLicensesColumns(): TableColumn<ProfessionalUserLicense>[] {
        return [
            { field: 'licenseType.name', caption: 'Type', type: ColumnType.text },
            { field: 'licenseNumber', caption: 'License Number', type: ColumnType.text },
            {
                field: 'expirationDate',
                caption: 'Expiration Date',
                type: ColumnType.date,
                cellTemplate: this.licenseExpirationTemplate
            }
        ];
    }

    private buildInsurancesColumns(): TableColumn<ProfessionalInsurance>[] {
        return [
            { field: 'insuranceNumber', caption: 'Policy Number', type: ColumnType.text },
            {
                field: 'expirationDate',
                caption: 'Expiration Date',
                type: ColumnType.date,
                cellTemplate: this.insuranceExpirationTemplate
            }
        ];
    }

    private buildGaugesColumns(): TableColumn<BackflowGauge>[] {
        return [
            {
                field: 'manufacturer',
                caption: 'Gauge',
                type: ColumnType.text,
                cellTemplate: this.gaugeCellTemplate
            },
            {
                field: 'isPortable',
                caption: 'Type',
                type: ColumnType.other,
                queryColumnExcluded: true,
                cellTemplate: this.gaugeTypeTemplate
            },
            { field: 'serialNumber', caption: 'Serial Number', type: ColumnType.text },
            {
                field: 'lastCalibrationDate',
                caption: 'Test Date',
                type: ColumnType.date,
                cellTemplate: this.gaugeTestDateTemplate
            }
        ];
    }

    private buildVehiclesColumns(): TableColumn<FogVehicleRow>[] {
        return [
            { field: 'licensePlateNumber', caption: 'License Plate #', type: ColumnType.text },
            { field: 'manufacturer', caption: 'Manufacturer', type: ColumnType.text },
            { field: 'manufacturedYear', caption: 'Year', type: ColumnType.number },
            {
                field: 'capacityDescription',
                caption: 'Capacity',
                type: ColumnType.text,
                queryColumnExcluded: true
            },
            { field: 'stickerNumber', caption: 'Sticker #', type: ColumnType.text }
        ];
    }

    private buildDisposalSitesColumns(): TableColumn<FogDisposalSiteRow>[] {
        return [
            { field: 'name', caption: 'Disposal Facility', type: ColumnType.text },
            { field: 'registrationNumber', caption: 'Registration Number', type: ColumnType.text },
            { field: 'county', caption: 'County', type: ColumnType.text },
            {
                field: 'wasteTypeDescription',
                caption: 'Waste Types',
                type: ColumnType.text,
                queryColumnExcluded: true
            }
        ];
    }
}

// Display-only view models: the capacity and waste-type labels are pre-computed once
// per load so the templates never call a component method per row.
interface FogVehicleRow extends FogVehicle {
    capacityDescription: string;
}

interface FogDisposalSiteRow extends FogDisposalSite {
    wasteTypeDescription: string;
}
