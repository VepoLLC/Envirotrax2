import { Component, OnInit, TemplateRef, ViewChild } from '@angular/core';
import { Router } from '@angular/router';
import { WaterSupplierLicense } from '../shared/models/professionals/licenses/water-supplier-license';
import { WaterSupplierLicenseService } from '../shared/services/licenses/water-supplier-license.service';
import { TableViewModel } from '../shared/models/table-view-model';
import { ExpirationType } from '../shared/models/professionals/licenses/professional-user-license';
import { AuthService } from '../shared/services/auth/auth.service';
import { FeatureType } from '../shared/models/feature-type';
import { PermissionAction, PermissionType } from '../shared/models/permission-type';
import { CellTemplateData, ColumnType, TableColumn } from '@envirotrax/common-ui';

@Component({
    templateUrl: './registration-management.component.html',
    standalone: false
})
export class RegistrationManagementComponent implements OnInit {
    @ViewChild('expirationDateCell', { static: true })
    private expirationDateCell!: TemplateRef<CellTemplateData<WaterSupplierLicense>>;

    @ViewChild('actionsCell', { static: true })
    private actionsCell!: TemplateRef<CellTemplateData<WaterSupplierLicense>>;

    public expirationType = ExpirationType;

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
        const hasLicenseAccess = await this._authService.hasAnyFeatures(FeatureType.ManageProfessionalLicenses)
            || await this._authService.hasAnyPermisison(PermissionAction.CanView, PermissionType.Licenses);
        if (!hasLicenseAccess) {
            await this._router.navigate(['auth', 'unauthorized']);
            return;
        }

        this.table.columns = this.getColumns();
        await this.loadRegistrations();
    }

    private getColumns(): TableColumn<WaterSupplierLicense>[] {
        return [
            { field: 'companyName', caption: 'Company name', type: ColumnType.text },
            { field: 'userEmail', caption: 'Contact email', type: ColumnType.text },
            { field: 'licenseNumber', caption: 'Registration number', type: ColumnType.text },
            {
                field: 'expirationDate',
                caption: 'Expiration date',
                type: ColumnType.date,
                cellTemplate: this.expirationDateCell
            },
            { field: 'id', caption: '', type: ColumnType.text, cellTemplate: this.actionsCell }
        ];
    }

    public async loadRegistrations(): Promise<void> {
        try {
            this.table.isLoading = true;
            this.table.items = await this._licenseService.getRegistrations(
                this.table.items?.pageInfo || {},
                this.table.query
            );
        } finally {
            this.table.isLoading = false;
        }
    }

    public manage(registration: WaterSupplierLicense): void {
        if (registration.professionalId == null) {
            return;
        }

        this._router.navigate(['/fog/transporters/details', String(registration.professionalId)]);
    }
}
