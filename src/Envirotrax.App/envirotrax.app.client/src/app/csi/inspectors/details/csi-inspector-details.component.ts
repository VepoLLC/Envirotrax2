import { Component, OnInit, TemplateRef, ViewChild } from "@angular/core";
import { ActivatedRoute } from "@angular/router";
import { CsiInspectorAccount } from "../../../shared/models/csi/csi-inspector-account";
import { CsiInspectoreManagementService } from "../../../shared/services/csi/csi-inspector-management.service";
import { ExpirationType, ProfessionalUserLicense, professionalTypeLabels, ProfessionalType } from "../../../shared/models/professionals/licenses/professional-user-license";
import { ProfessionalInsurance, ExpirationType as InsuranceExpirationType } from "../../../shared/models/professionals/professional-insurance";
import { ProfessionalWaterSupplier } from "../../../shared/models/professionals/professional-water-supplier";
import { ProfessionalUser } from "../../../shared/models/professionals/professional-user";
import { TableViewModel } from "../../../shared/models/table-view-model";
import { CellTemplateData, TableColumn } from "../../../shared/components/data-components/table/table.component";
import { ColumnType } from "../../../shared/components/data-components/sorting-filtering/query-view-model";

@Component({
    selector: 'app-csi-inspector-details',
    standalone: false,
    styleUrls: ['./csi-inspector-details.component.css'],
    templateUrl: './csi-inspector-details.component.html'
})
export class CsiInspectorDetailsComponent implements OnInit {
    public id: number | null = null;
    public accountInfo: CsiInspectorAccount | null = null;
    public isAccountLoading: boolean = false;
    public activeTab: 'insurances' | 'licenses' = 'insurances';

    public expirationType = ExpirationType;
    public insuranceExpirationType = InsuranceExpirationType;

    public waterSuppliersTable: TableViewModel<ProfessionalWaterSupplier> = {
        columns: [],
        query: { sort: {}, filter: [] }
    };

    public subAccountsTable: TableViewModel<ProfessionalUser> = {
        columns: [],
        query: { sort: {}, filter: [] }
    };

    public licensesTable: TableViewModel<ProfessionalUserLicense> = {
        columns: [],
        query: { sort: {}, filter: [] }
    };

    public insurancesTable: TableViewModel<ProfessionalInsurance> = {
        columns: [],
        query: { sort: {}, filter: [] }
    };

    @ViewChild('supplierNameCell', { static: true })
    private supplierNameCellTemplate!: TemplateRef<CellTemplateData<ProfessionalWaterSupplier>>;

    @ViewChild('bannedCell', { static: true })
    private bannedCellTemplate!: TemplateRef<CellTemplateData<ProfessionalWaterSupplier>>;

    @ViewChild('licenseTypeCell', { static: true })
    private licenseTypeCellTemplate!: TemplateRef<CellTemplateData<ProfessionalUserLicense>>;

    @ViewChild('professionalTypeCell', { static: true })
    private professionalTypeCellTemplate!: TemplateRef<CellTemplateData<ProfessionalUserLicense>>;

    @ViewChild('userEmailCell', { static: true })
    private userEmailCellTemplate!: TemplateRef<CellTemplateData<ProfessionalUserLicense>>;

    @ViewChild('licenseExpirationCell', { static: true })
    private licenseExpirationCellTemplate!: TemplateRef<CellTemplateData<ProfessionalUserLicense>>;

    @ViewChild('insuranceExpirationCell', { static: true })
    private insuranceExpirationCellTemplate!: TemplateRef<CellTemplateData<ProfessionalInsurance>>;

    constructor(
        private readonly _activatedRoute: ActivatedRoute,
        private readonly _csiInspectorService: CsiInspectoreManagementService
    ) {}

    public async ngOnInit(): Promise<void> {
        this.setIdFromRoute();
        if (this.id !== null) {
            this.setupColumns();
            await Promise.all([
                this.loadAccountInfo(),
                this.loadWaterSuppliers(),
                this.loadSubAccounts(),
                this.loadLicenses(),
                this.loadInsurances()
            ]);
        }
    }

    private setIdFromRoute(): void {
        const idParam = this._activatedRoute.snapshot.paramMap.get('id');
        this.id = idParam ? Number(idParam) : null;
    }

    private setupColumns(): void {
        this.waterSuppliersTable.columns = this.getWaterSupplierColumns();
        this.subAccountsTable.columns = this.getSubAccountColumns();
        this.licensesTable.columns = this.getLicenseColumns();
        this.insurancesTable.columns = this.getInsuranceColumns();
    }

    private getWaterSupplierColumns(): TableColumn<ProfessionalWaterSupplier>[] {
        return [
            {
                field: 'waterSupplierId',
                caption: 'Water Supplier',
                cellTemplate: this.supplierNameCellTemplate,
                type: ColumnType.text
            },
            {
                field: 'isBanned',
                caption: 'Suspended',
                cellTemplate: this.bannedCellTemplate,
                type: ColumnType.text
            }
        ];
    }

    private getSubAccountColumns(): TableColumn<ProfessionalUser>[] {
        return [
            {
                field: 'emailAddress',
                caption: 'Email Address',
                type: ColumnType.text
            },
            {
                field: 'contactName',
                caption: 'Contact Name',
                type: ColumnType.text
            },
            {
                field: 'jobTitle',
                caption: 'Job Title',
                type: ColumnType.text
            }
        ];
    }

    private getLicenseColumns(): TableColumn<ProfessionalUserLicense>[] {
        return [
            {
                field: 'licenseNumber',
                caption: 'License Number',
                type: ColumnType.text
            },
            {
                field: 'licenseType.name',
                caption: 'Type',
                cellTemplate: this.licenseTypeCellTemplate,
                type: ColumnType.text
            },
            {
                field: 'professionalType',
                caption: 'Professional Type',
                cellTemplate: this.professionalTypeCellTemplate,
                type: ColumnType.text
            },
            {
                field: 'user.emailAddress',
                caption: 'Email Address',
                cellTemplate: this.userEmailCellTemplate,
                type: ColumnType.text
            },
            {
                field: 'expirationDate',
                caption: 'Expiration Date',
                cellTemplate: this.licenseExpirationCellTemplate,
                type: ColumnType.date
            }
        ];
    }

    private getInsuranceColumns(): TableColumn<ProfessionalInsurance>[] {
        return [
            {
                field: 'insuranceNumber',
                caption: 'Policy Number',
                type: ColumnType.text
            },
            {
                field: 'expirationDate',
                caption: 'Expiration Date',
                cellTemplate: this.insuranceExpirationCellTemplate,
                type: ColumnType.date
            }
        ];
    }

    private async loadAccountInfo(): Promise<void> {
        if (this.id === null){
            return;
        }
        try {
            this.isAccountLoading = true;
            this.accountInfo = await this._csiInspectorService.getAccountInfo(this.id);
        } finally {
            this.isAccountLoading = false;
        }
    }

    public async loadWaterSuppliers(): Promise<void> {
        if (this.id === null){
            return;
        }
        try {
            this.waterSuppliersTable.isLoading = true;
            this.waterSuppliersTable.items = await this._csiInspectorService.getWaterSuppliers(
                this.id,
                this.waterSuppliersTable.items?.pageInfo || {},
                this.waterSuppliersTable.query
            );
        } finally {
            this.waterSuppliersTable.isLoading = false;
        }
    }

    public async loadSubAccounts(): Promise<void> {
        if (this.id === null){
            return;
        }
        try {
            this.subAccountsTable.isLoading = true;
            this.subAccountsTable.items = await this._csiInspectorService.getSubAccounts(
                this.id,
                this.subAccountsTable.items?.pageInfo || {},
                this.subAccountsTable.query
            );
        } finally {
            this.subAccountsTable.isLoading = false;
        }
    }

    public async loadLicenses(): Promise<void> {
        if (this.id === null){
            return;
        }
        try {
            this.licensesTable.isLoading = true;
            this.licensesTable.items = await this._csiInspectorService.getLicenses(
                this.id,
                this.licensesTable.items?.pageInfo || {},
                this.licensesTable.query
            );
        } finally {
            this.licensesTable.isLoading = false;
        }
    }

    public async loadInsurances(): Promise<void> {
        if (this.id === null){
            return;
        }
        try {
            this.insurancesTable.isLoading = true;
            this.insurancesTable.items = await this._csiInspectorService.getInsurances(
                this.id,
                this.insurancesTable.items?.pageInfo || {},
                this.insurancesTable.query
            );
        } finally {
            this.insurancesTable.isLoading = false;
        }
    }

    public getProfessionalTypeLabel(type?: number): string {
        if (type === undefined || type === null){
            return '';
        }
        return professionalTypeLabels[type as ProfessionalType] ?? '';
    }

    public getFormattedAddress(): string {
        if (!this.accountInfo){
            return '';
        }
        const parts = [
            this.accountInfo.address,
            this.accountInfo.city,
            this.accountInfo.state?.name,
            this.accountInfo.zipCode
        ].filter(p => p);
        return parts.join(', ');
    }

    public getLicenseExpirationClass(license: ProfessionalUserLicense): string {
        if (!license.expirationDate) {
            return 'text-bg-primary';
        }
        if (license.expirationType === ExpirationType.Expired){
             return 'text-bg-danger';
        }
        if (license.expirationType === ExpirationType.AboutToExpire){
            return 'text-bg-warning';
        }
        return 'text-bg-success';
    }

    public getInsuranceExpirationClass(insurance: ProfessionalInsurance): string {
        if (!insurance.expirationDate) {
            return 'text-bg-primary';
        }
        if (insurance.expirationType === InsuranceExpirationType.Expired){
             return 'text-bg-danger';
        }
        if (insurance.expirationType === InsuranceExpirationType.AboutToExpire){
            return 'text-bg-warning';
        }
        return 'text-bg-success';
    }
}
