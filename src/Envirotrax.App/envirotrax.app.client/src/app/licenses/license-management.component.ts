import { Component, OnInit, TemplateRef, ViewChild } from '@angular/core';
import { WaterSupplierLicense, LicenseCounts } from '../shared/models/professionals/licenses/water-supplier-license';
import { WaterSupplierLicenseService } from '../shared/services/licenses/water-supplier-license.service';
import { TableViewModel } from '../shared/models/table-view-model';
import { ExpirationType } from '../shared/models/professionals/licenses/professional-user-license';
import { AuthService } from '../shared/services/auth/auth.service';
import { FeatureType } from '../shared/models/feature-type';
import { PermissionAction, PermissionType } from '../shared/models/permission-type';
import { ModalSize } from '@developer-partners/ngx-modal-dialog';
import { ToastService } from '../shared/services/toast.service';
import { EditWaterSupplierLicenseComponent, WaterSupplierLicenseModalData } from './edit/edit-water-supplier-license.component';
import { CellTemplateData, ColumnType, ModalHelperService, TableColumn } from '@envirotrax/common-ui';

@Component({
    templateUrl: './license-management.component.html',
    standalone: false
})
export class LicenseManagementComponent implements OnInit {
    @ViewChild('expirationDateCell', { static: true })
    private expirationDateCell!: TemplateRef<CellTemplateData<WaterSupplierLicense>>;

    @ViewChild('actionsCell', { static: true })
    private actionsCell!: TemplateRef<CellTemplateData<WaterSupplierLicense>>;

    public activeFilter: string = 'unverified';
    public counts: LicenseCounts = { unverifiedCount: 0, expiredCount: 0, expiringCount: 0 };
    public expirationType = ExpirationType;
    public canModify = false;

    public get expiredMonthRange(): string {
        const now = new Date();
        const firstDayLastMonth = new Date(now.getFullYear(), now.getMonth() - 1, 1);
        const lastDayLastMonth = new Date(now.getFullYear(), now.getMonth(), 0);
        const fmt = (d: Date) => `${d.getMonth() + 1}/${d.getDate()}/${d.getFullYear()}`;
        return `${fmt(firstDayLastMonth)} - ${fmt(lastDayLastMonth)}`;
    }

    public get expiringMonthRange(): string {
        const now = new Date();
        const firstDayThisMonth = new Date(now.getFullYear(), now.getMonth(), 1);
        const lastDayThisMonth = new Date(now.getFullYear(), now.getMonth() + 1, 0);
        const fmt = (d: Date) => `${d.getMonth() + 1}/${d.getDate()}/${d.getFullYear()}`;
        return `${fmt(firstDayThisMonth)} - ${fmt(lastDayThisMonth)}`;
    }

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
        private readonly _modalHelper: ModalHelperService,
        private readonly _toastService: ToastService
    ) { }

    public async ngOnInit(): Promise<void> {
        this.canModify = await this._authService.hasAnyFeatures(FeatureType.ManageProfessionalLicenses)
            && await this._authService.hasAnyPermisison(PermissionAction.CanModify, PermissionType.Licenses);
        this.table.columns = this.getColumns();
        await Promise.all([this.loadLicenses(), this.loadCounts()]);
    }

    private getColumns(): TableColumn<WaterSupplierLicense>[] {
        const cols: TableColumn<WaterSupplierLicense>[] = [
            { field: 'userEmail', caption: 'User ID', type: ColumnType.text },
            { field: 'companyName', caption: 'Company name', type: ColumnType.text },
            { field: 'contactName', caption: 'Contact name', type: ColumnType.text },
            { field: 'licenseTypeName', caption: 'Type', type: ColumnType.text },
            { field: 'licenseNumber', caption: 'License number', type: ColumnType.text },
            {
                field: 'expirationDate',
                caption: 'Expiration date',
                type: ColumnType.date,
                cellTemplate: this.expirationDateCell
            }
        ];
        if (this.canModify) {
            cols.push({ field: 'id', caption: '', type: ColumnType.text, cellTemplate: this.actionsCell });
        }
        return cols;
    }

    public async setFilter(filter: string): Promise<void> {
        this.activeFilter = filter;
        this.table.items = undefined;
        this.table.query = { sort: {}, filter: [] };
        await this.loadLicenses();
    }

    public async loadLicenses(): Promise<void> {
        try {
            this.table.isLoading = true;
            this.table.items = await this._licenseService.getLicenses(
                this.activeFilter,
                this.table.items?.pageInfo || {},
                this.table.query
            );
        } finally {
            this.table.isLoading = false;
        }
    }

    public editLicense(license: WaterSupplierLicense): void {
        this._modalHelper.show<WaterSupplierLicenseModalData, WaterSupplierLicense>(EditWaterSupplierLicenseComponent, {
            title: `Edit License - ${license.contactName ?? license.userEmail}`,
            model: { license },
            size: ModalSize.large
        }).result().subscribe(() => this.loadLicenses());
    }

    public deleteLicense(license: WaterSupplierLicense): void {
        this._modalHelper.showDeleteConfirmation().result().subscribe(async () => {
            try {
                this.table.isLoading = true;
                await this._licenseService.delete(license.id!);
                this._toastService.successFullyDeleted('License');
            } finally {
                this.table.isLoading = false;
            }
            await Promise.all([this.loadLicenses(), this.loadCounts()]);
        });
    }

    private async loadCounts(): Promise<void> {
        this.counts = await this._licenseService.getCounts();
    }
}
