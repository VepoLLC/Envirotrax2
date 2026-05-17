import { Component, OnInit, ViewChild, TemplateRef } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../shared/services/auth/auth.service';
import { CsiInspectionService } from '../../shared/services/csi/csi-inspection.service';
import { CsiInspection } from '../../shared/models/csi/csi-inspection';
import { FeatureType } from '../../shared/models/feature-tyype';
import { ROLE_DEFINITIONS } from '../../shared/models/role-definitions';
import { ProfessionalSupplierService } from '../../shared/services/professionals/professional-supplier.service';
import { ProfesionalUserService } from '../../shared/services/professionals/professional-user.service';
import { ProfessionalUserLicenseService } from '../../shared/services/professionals/professional-user-license.service';
import { ProfessionalInsuranceService } from '../../shared/services/professionals/professional-insurance.service';
import { BackflowGaugeService } from '../../shared/services/backflow/backflow-gauge.service';
import { ProfessionalUser } from '../../shared/models/professionals/professional-user';
import { ProfessionalUserLicense, ExpirationType } from '../../shared/models/professionals/licenses/professional-user-license';
import { ProfessionalInsurance } from '../../shared/models/professionals/professional-insurance';
import { BackflowGauge, GaugeExpirationType } from '../../shared/models/backflow/backflow-gauge';
import { TableColumn, CellTemplateData } from '../../shared/components/data-components/table/table.component';
import { ColumnType } from '../../shared/components/data-components/sorting-filtering/query-view-model';
import { TableViewModel } from '../../shared/models/table-view-model';

const VIEW_MODE_KEY = 'dashboardViewMode';

@Component({
    standalone: false,
    templateUrl: './dashboard.component.html'
})
export class DashboardComponent implements OnInit {
    @ViewChild('statusTemplate', { static: true })
    public statusTemplate!: TemplateRef<CellTemplateData<CsiInspection>>;

    @ViewChild('propertyTemplate', { static: true })
    public propertyTemplate!: TemplateRef<CellTemplateData<CsiInspection>>;

    @ViewChild('mailingTemplate', { static: true })
    public mailingTemplate!: TemplateRef<CellTemplateData<CsiInspection>>;

    public hasCsi = false;
    public hasBackflow = false;
    public isAdmin = false;
    public isLoading = true;
    public isAdminDataLoading = false;
    public isGaugeDataLoading = false;

    public viewMode: 'quick' | 'full' = 'full';

    public selectedSupplierCount = 0;
    public subAccounts: ProfessionalUser[] = [];
    public licenses: ProfessionalUserLicense[] = [];
    public insurances: ProfessionalInsurance[] = [];
    public gauges: BackflowGauge[] = [];

    public readonly ExpirationType = ExpirationType;
    public readonly GaugeExpirationType = GaugeExpirationType;

    public licenseInsuranceTab: 'licenses' | 'insurances' = 'licenses';

    public setLicenseInsuranceTab(tab: 'licenses' | 'insurances'): void {
        this.licenseInsuranceTab = tab;
    }

    public get licenseAndInsuranceCount(): number {
        return this.licenses.length + this.insurances.length;
    }

    public recentInspections: TableViewModel<CsiInspection> = {
        query: {},
        columns: []
    };

    constructor(
        private readonly _authService: AuthService,
        private readonly _inspectionService: CsiInspectionService,
        private readonly _supplierService: ProfessionalSupplierService,
        private readonly _userService: ProfesionalUserService,
        private readonly _licenseService: ProfessionalUserLicenseService,
        private readonly _insuranceService: ProfessionalInsuranceService,
        private readonly _gaugeService: BackflowGaugeService,
        private readonly _router: Router
    ) {}

    public async ngOnInit(): Promise<void> {
        this.viewMode = (localStorage.getItem(VIEW_MODE_KEY) as 'quick' | 'full') ?? 'full';

        try {
            [this.hasCsi, this.hasBackflow, this.isAdmin] = await Promise.all([
                this._authService.hasAnyFeatures(FeatureType.CsiInspection),
                this._authService.hasAnyFeatures(FeatureType.BackflowTesting),
                this._authService.hasAnyRoles(ROLE_DEFINITIONS.PROFESSIONALS.ADMIN)
            ]);

            this.recentInspections.columns = this.buildColumns();

            const promises: Promise<void>[] = [];
            if (this.hasCsi) promises.push(this.loadRecentInspections());
            if (this.isAdmin) promises.push(this.loadAdminData());
            if (this.hasBackflow) promises.push(this.loadGaugeData());
            await Promise.all(promises);
        } finally {
            this.isLoading = false;
        }
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

    private async loadAdminData(): Promise<void> {
        try {
            this.isAdminDataLoading = true;
            const [suppliers, users, licenses, insurances] = await Promise.all([
                this._supplierService.getAllMy(),
                this._userService.getAll({}, {}),
                this._licenseService.getAll({}, {}),
                this._insuranceService.getAll({}, {})
            ]);
            this.selectedSupplierCount = suppliers.pageInfo?.totalItems ?? suppliers.data.length;
            this.subAccounts = users.data;
            this.licenses = licenses.data;
            this.insurances = insurances.data;
        } finally {
            this.isAdminDataLoading = false;
        }
    }

    private async loadGaugeData(): Promise<void> {
        try {
            this.isGaugeDataLoading = true;
            const result = await this._gaugeService.getAll({}, {});
            this.gauges = result.data;
        } finally {
            this.isGaugeDataLoading = false;
        }
    }

    public viewInspection(inspection: CsiInspection): void {
        const url = this._router.serializeUrl(
            this._router.createUrlTree(['/professionals/csi/inspections', inspection.id])
        );
        window.open(url, '_blank');
    }

    private buildColumns(): TableColumn<CsiInspection>[] {
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
}
