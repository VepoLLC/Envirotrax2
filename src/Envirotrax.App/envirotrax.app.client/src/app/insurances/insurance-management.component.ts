import { Component, OnInit, TemplateRef, ViewChild } from '@angular/core';
import { Router } from '@angular/router';
import { CellTemplateData, ColumnType, TableColumn } from '@envirotrax/common-ui';
import { WaterSupplierInsurance } from '../shared/models/professionals/water-supplier-insurance';
import { WaterSupplierInsuranceService } from '../shared/services/insurances/water-supplier-insurance.service';
import { TableViewModel } from '../shared/models/table-view-model';
import { ProfessionalType } from '../shared/models/professionals/licenses/professional-user-license';
import { AuthService } from '../shared/services/auth/auth.service';
import { FeatureType } from '../shared/models/feature-type';
import { PermissionAction, PermissionType } from '../shared/models/permission-type';

@Component({
    templateUrl: './insurance-management.component.html',
    standalone: false
})
export class InsuranceManagementComponent implements OnInit {
    @ViewChild('expirationDateCell', { static: true })
    private expirationDateCell!: TemplateRef<CellTemplateData<WaterSupplierInsurance>>;

    @ViewChild('actionsCell', { static: true })
    private actionsCell!: TemplateRef<CellTemplateData<WaterSupplierInsurance>>;

    private readonly detailsRoutes: Partial<Record<ProfessionalType, string>> = {
        [ProfessionalType.Bpat]: '/backflow/testers/details',
        [ProfessionalType.CsiInspector]: '/csi/inspectors/details',
        [ProfessionalType.FogTransporter]: '/fog/transporters/details',
        [ProfessionalType.FogInspector]: '/fog/inspectors/details'
    };

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
            { field: 'submittedOn', caption: 'Submitted On', type: ColumnType.date },
            { field: 'userEmail', caption: 'User ID', type: ColumnType.text },
            { field: 'companyName', caption: 'Company Name', type: ColumnType.text },
            { field: 'contactName', caption: 'Contact Name', type: ColumnType.text, queryColumnExcluded: true },
            { field: 'insuranceNumber', caption: 'Policy Number', type: ColumnType.text },
            {
                field: 'expirationDate',
                caption: '',
                type: ColumnType.date,
                cellTemplate: this.expirationDateCell,
                queryColumnExcluded: true
            },
            { field: 'id', caption: '', type: ColumnType.text, cellTemplate: this.actionsCell, queryColumnExcluded: true }
        ];
    }

    public async loadInsurances(): Promise<void> {
        try {
            this.table.isLoading = true;
            this.table.items = await this._insuranceService.getUnverifiedInsurances(
                this.table.items?.pageInfo || {},
                this.table.query
            );
        } finally {
            this.table.isLoading = false;
        }
    }

    public async manage(insurance: WaterSupplierInsurance): Promise<void> {
        const route = insurance.professionalType != null ? this.detailsRoutes[insurance.professionalType] : undefined;

        if (route) {
            await this._router.navigate([route, insurance.professionalId]);
        }
    }
}
