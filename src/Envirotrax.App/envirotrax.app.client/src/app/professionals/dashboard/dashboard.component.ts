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
import { ProfessionalUser } from '../../shared/models/professionals/professional-user';
import { ProfessionalUserLicense, ExpirationType } from '../../shared/models/professionals/licenses/professional-user-license';
import { ProfessionalInsuranceService } from '../../shared/services/professionals/professional-insurance.service';
import { ProfessionalInsurance } from '../../shared/models/professionals/professional-insurance';
import { TableColumn, CellTemplateData } from '../../shared/components/data-components/table/table.component';
import { ColumnType } from '../../shared/components/data-components/sorting-filtering/query-view-model';
import { TableViewModel } from '../../shared/models/table-view-model';

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

    public selectedSupplierCount = 0;
    public subAccounts: ProfessionalUser[] = [];
    public licenses: ProfessionalUserLicense[] = [];
    public insurances: ProfessionalInsurance[] = [];

    public readonly ExpirationType = ExpirationType;

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
        private readonly _router: Router
    ) {}

    public async ngOnInit(): Promise<void> {
        try {
            [this.hasCsi, this.hasBackflow, this.isAdmin] = await Promise.all([
                this._authService.hasAnyFeatures(FeatureType.CsiInspection),
                this._authService.hasAnyFeatures(FeatureType.BackflowTesting),
                this._authService.hasAnyRoles(ROLE_DEFINITIONS.PROFESSIONALS.ADMIN)
            ]);

            this.recentInspections.columns = this.buildColumns();

            const promises: Promise<void>[] = [];
            if (this.hasCsi) {
                promises.push(this.loadRecentInspections());
            }
            if (this.isAdmin) {
                promises.push(this.loadAdminData());
            }
            await Promise.all(promises);
        } finally {
            this.isLoading = false;
        }
    }

    private async loadRecentInspections(): Promise<void> {
        try {
            this.recentInspections.isLoading = true;
            this.recentInspections.items = await this._inspectionService.getProfessionalInspections(
                { pageSize: 5 },
                this.recentInspections.query,
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
                this._userService.getAll({ pageSize: 1000 }, {}),
                this._licenseService.getAll({ pageSize: 1000 }, {}),
                this._insuranceService.getAll({ pageSize: 1000 }, {})
            ]);
            this.selectedSupplierCount = suppliers.pageInfo?.totalItems ?? suppliers.data.length;
            this.subAccounts = users.data;
            this.licenses = licenses.data;
            this.insurances = insurances.data;
        } finally {
            this.isAdminDataLoading = false;
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
