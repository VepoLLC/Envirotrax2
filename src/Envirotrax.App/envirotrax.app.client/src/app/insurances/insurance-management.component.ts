import { Component, OnInit, TemplateRef, ViewChild } from '@angular/core';
import { Router } from '@angular/router';
import { WaterSupplierInsurance } from '../shared/models/professionals/water-supplier-insurance';
import { WaterSupplierInsuranceService } from '../shared/services/insurances/water-supplier-insurance.service';
import { TableViewModel } from '../shared/models/table-view-model';
import { ExpirationType, ProfessionalType } from '../shared/models/professionals/licenses/professional-user-license';
import { AuthService } from '../shared/services/auth/auth.service';
import { FeatureType } from '../shared/models/feature-type';
import { PermissionAction, PermissionType } from '../shared/models/permission-type';
import { CellTemplateData, ColumnType, TableColumn } from '@envirotrax/common-ui';

@Component({
    templateUrl: './insurance-management.component.html',
    standalone: false
})
export class InsuranceManagementComponent implements OnInit {
    @ViewChild('expirationDateCell', { static: true })
    private expirationDateCell!: TemplateRef<CellTemplateData<WaterSupplierInsurance>>;

    @ViewChild('actionsCell', { static: true })
    private actionsCell!: TemplateRef<CellTemplateData<WaterSupplierInsurance>>;

    public expirationType = ExpirationType;

    public table: TableViewModel<WaterSupplierInsurance> = {
        columns: [],
        query: { sort: {}, filter: [] },
        freeTextSearch: {
            searchQuery: [
                { field: 'insuranceNumber' }
            ]
        }
    };

    constructor(
        private readonly _insuranceService: WaterSupplierInsuranceService,
        private readonly _authService: AuthService,
        private readonly _router: Router
    ) { }

    public async ngOnInit(): Promise<void> {
        const hasInsuranceAccess = await this._authService.hasAnyFeatures(FeatureType.ManageProfessionalInsurances)
            || await this._authService.hasAnyPermisison(PermissionAction.CanView, PermissionType.Licenses);
        if (!hasInsuranceAccess) {
            await this._router.navigate(['auth', 'unauthorized']);
            return;
        }

        this.table.columns = this.getColumns();
        await this.loadInsurances();
    }

    private getColumns(): TableColumn<WaterSupplierInsurance>[] {
        return [
            { field: 'companyName', caption: 'Company name', type: ColumnType.text },
            { field: 'companyEmail', caption: 'Company email', type: ColumnType.text },
            { field: 'insuranceNumber', caption: 'Policy number', type: ColumnType.text },
            {
                field: 'expirationDate',
                caption: 'Expiration date',
                type: ColumnType.date,
                cellTemplate: this.expirationDateCell
            },
            { field: 'id', caption: '', type: ColumnType.text, cellTemplate: this.actionsCell }
        ];
    }

    public async loadInsurances(): Promise<void> {
        try {
            this.table.isLoading = true;
            this.table.items = await this._insuranceService.getInsurances(
                'unverified',
                this.table.items?.pageInfo || {},
                this.table.query
            );
        } finally {
            this.table.isLoading = false;
        }
    }

    public manage(insurance: WaterSupplierInsurance): void {
        const route = this.buildManageRoute(insurance);

        if (route) {
            this._router.navigate(route);
        }
    }

    private buildManageRoute(insurance: WaterSupplierInsurance): string[] | null {
        if (insurance.professionalId == null) {
            return null;
        }

        const id = String(insurance.professionalId);

        switch (insurance.professionalType) {
            case ProfessionalType.Bpat:
                return ['/backflow/testers/details', id];
            case ProfessionalType.CsiInspector:
                return ['/csi/inspectors/details', id];
            case ProfessionalType.FogTransporter:
                return ['/fog/transporters/details', id];
            case ProfessionalType.FogInspector:
                return ['/fog/inspectors/details', id];
            default:
                return null;
        }
    }
}
