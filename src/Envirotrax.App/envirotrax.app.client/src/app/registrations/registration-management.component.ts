import { Component, OnInit, TemplateRef, ViewChild } from '@angular/core';
import { Router } from '@angular/router';
import { CellTemplateData, ColumnType, TableColumn } from '@envirotrax/common-ui';
import { WaterSupplierLicense } from '../shared/models/professionals/licenses/water-supplier-license';
import { WaterSupplierLicenseService } from '../shared/services/licenses/water-supplier-license.service';
import { TableViewModel } from '../shared/models/table-view-model';
import { AuthService } from '../shared/services/auth/auth.service';
import { FeatureType } from '../shared/models/feature-type';
import { PermissionAction, PermissionType } from '../shared/models/permission-type';

@Component({
    templateUrl: './registration-management.component.html',
    standalone: false
})
export class RegistrationManagementComponent implements OnInit {
    @ViewChild('expirationDateCell', { static: true })
    private expirationDateCell!: TemplateRef<CellTemplateData<WaterSupplierLicense>>;

    @ViewChild('actionsCell', { static: true })
    private actionsCell!: TemplateRef<CellTemplateData<WaterSupplierLicense>>;

    public canManage = false;

    public table: TableViewModel<WaterSupplierLicense> = {
        columns: [],
        query: { sort: {}, filter: [] },
        freeTextSearch: {
            searchQuery: [
                { field: 'licenseNumber' }
            ]
        }
    };

    constructor(
        private readonly _licenseService: WaterSupplierLicenseService,
        private readonly _authService: AuthService,
        private readonly _router: Router
    ) { }

    public async ngOnInit(): Promise<void> {
        const hasRegistrationAccess = await this._authService.hasAnyFeatures(FeatureType.ManageProfessionalLicenses)
            || await this._authService.hasAnyPermisison(PermissionAction.CanView, PermissionType.Licenses);

        if (!hasRegistrationAccess) {
            await this._router.navigate(['auth', 'unauthorized']);
            return;
        }

        this.canManage = await this._authService.hasAnyPermisison(PermissionAction.CanView, PermissionType.FogTransporters);

        this.table.columns = this.getColumns();

        await this.loadRegistrations();
    }

    private getColumns(): TableColumn<WaterSupplierLicense>[] {
        const cols: TableColumn<WaterSupplierLicense>[] = [
            { field: 'submittedOn', caption: 'Submitted On', type: ColumnType.date },
            { field: 'userEmail', caption: 'User ID', type: ColumnType.text },
            { field: 'companyName', caption: 'Company Name', type: ColumnType.text },
            { field: 'contactName', caption: 'Contact Name', type: ColumnType.text },
            { field: 'licenseNumber', caption: 'Registration Number', type: ColumnType.text },
            {
                field: 'expirationDate',
                caption: '',
                type: ColumnType.date,
                cellTemplate: this.expirationDateCell,
                queryColumnExcluded: true
            }
        ];

        if (this.canManage) {
            cols.push({ field: 'id', caption: '', type: ColumnType.text, cellTemplate: this.actionsCell, queryColumnExcluded: true });
        }

        return cols;
    }

    public async loadRegistrations(): Promise<void> {
        try {
            this.table.isLoading = true;
            this.table.items = await this._licenseService.getUnverifiedRegistrations(
                this.table.items?.pageInfo || {},
                this.table.query
            );
        } finally {
            this.table.isLoading = false;
        }
    }

    public async manage(registration: WaterSupplierLicense): Promise<void> {
        await this._router.navigate(['/fog/transporters/details', registration.professionalId]);
    }
}
