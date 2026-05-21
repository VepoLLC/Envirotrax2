import { Component, OnInit, ViewChild, TemplateRef } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../shared/services/auth/auth.service';
import { CsiInspectionService } from '../../shared/services/csi/csi-inspection.service';
import { CsiInspection } from '../../shared/models/csi/csi-inspection';
import { FeatureType } from '../../shared/models/feature-type';
import { ROLE_DEFINITIONS } from '../../shared/models/role-definitions';
import { ProfesionalUserService } from '../../shared/services/professionals/professional-user.service';
import { ProfessionalUserLicenseService } from '../../shared/services/professionals/professional-user-license.service';
import { ProfessionalInsuranceService } from '../../shared/services/professionals/professional-insurance.service';
import { BackflowGaugeService } from '../../shared/services/backflow/backflow-gauge.service';
import { ProfessionalDashboardService } from '../../shared/services/professionals/professional-dashboard.service';
import { ProfessionalUser } from '../../shared/models/professionals/professional-user';
import { ProfessionalUserLicense, ExpirationType } from '../../shared/models/professionals/licenses/professional-user-license';
import { ProfessionalInsurance } from '../../shared/models/professionals/professional-insurance';
import { BackflowGauge, GaugeExpirationType } from '../../shared/models/backflow/backflow-gauge';
import { ProfessionalDashboardStats } from '../../shared/models/professionals/professional-dashboard-stats';
import { FreeTextSearchSettings, TableColumn, CellTemplateData } from '../../shared/components/data-components/table/table.component';
import { ColumnType } from '../../shared/components/data-components/sorting-filtering/query-view-model';
import { TableViewModel } from '../../shared/models/table-view-model';

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

    public hasCsi = false;
    public hasBackflow = false;
    public isAdmin = false;
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

    constructor(
        private readonly _authService: AuthService,
        private readonly _inspectionService: CsiInspectionService,
        private readonly _userService: ProfesionalUserService,
        private readonly _licenseService: ProfessionalUserLicenseService,
        private readonly _insuranceService: ProfessionalInsuranceService,
        private readonly _gaugeService: BackflowGaugeService,
        private readonly _dashboardService: ProfessionalDashboardService,
        private readonly _router: Router
    ) { }

    public async ngOnInit(): Promise<void> {
        this.viewMode = (localStorage.getItem(VIEW_MODE_KEY) as 'quick' | 'full') ?? 'full';

        this.setupColumns();

        try {
            const [hasCsi, hasBackflow, isCsiInspector, isBackflowTester, isAdmin] = await Promise.all([
                this._authService.hasAnyFeatures(FeatureType.CsiInspection),
                this._authService.hasAnyFeatures(FeatureType.BackflowTesting),
                this._authService.hasAnyRoles(ROLE_DEFINITIONS.PROFESSIONALS.CSI_INSPECTOR),
                this._authService.hasAnyRoles(ROLE_DEFINITIONS.PROFESSIONALS.BACKFLOW_TESTER),
                this._authService.hasAnyRoles(ROLE_DEFINITIONS.PROFESSIONALS.ADMIN)
            ]);

            this.isAdmin = isAdmin;
            this.hasCsi = hasCsi && (isCsiInspector || isAdmin);
            this.hasBackflow = hasBackflow && (isBackflowTester || isAdmin);

            const promises: Promise<void>[] = [];

            if (this.hasCsi) {
                promises.push(this.loadRecentInspections());
            }

            if (this.isAdmin) {
                promises.push(
                    this.loadStats(),
                    this.loadSubAccounts(),
                    this.loadLicenses(),
                    this.loadInsurances()
                );
            }
            if (this.hasBackflow) {
                promises.push(this.loadGauges());
            }

            await Promise.all(promises);
        } finally {
            this.isLoading = false;
        }
    }

    private setupColumns(): void {
        this.recentInspections.columns = this.buildInspectionColumns();
        this.subAccountsTable.columns = this.buildSubAccountsColumns();
        this.licensesTable.columns = this.buildLicensesColumns();
        this.insurancesTable.columns = this.buildInsurancesColumns();
        this.gaugesTable.columns = this.buildGaugesColumns();
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

    public viewInspection(inspection: CsiInspection): void {
        const url = this._router.serializeUrl(
            this._router.createUrlTree(['/professionals/csi/inspections', inspection.id])
        );
        window.open(url, '_blank');
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
}
