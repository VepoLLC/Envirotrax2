import { Component, Input, OnInit, TemplateRef, ViewChild } from "@angular/core";
import { ExpirationType, ProfessionalUserLicense, professionalTypeLabels, ProfessionalType } from "../../../../shared/models/professionals/licenses/professional-user-license";
import { ProfessionalInsurance, ExpirationType as InsuranceExpirationType } from "../../../../shared/models/professionals/professional-insurance";
import { TableViewModel } from "../../../../shared/models/table-view-model";
import { CellTemplateData, TableColumn } from "../../../../shared/components/data-components/table/table.component";
import { ColumnType } from "../../../../shared/components/data-components/sorting-filtering/query-view-model";
import { CsiInspectorLicensesService } from "../../../../shared/services/csi/csi-inspector-licenses.service";
import { CsiInspectorInsurancesService } from "../../../../shared/services/csi/csi-inspector-insurances.service";

@Component({
    selector: 'app-csi-inspector-license-insurance',
    standalone: false,
    templateUrl: './csi-inspector-license-insurance.component.html'
})
export class CsiInspectorLicenseInsuranceComponent implements OnInit {
    @Input() public inspectorId!: number;

    public activeTab: 'insurances' | 'licenses' = 'insurances';

    public expirationType = ExpirationType;
    public insuranceExpirationType = InsuranceExpirationType;

    public licensesTable: TableViewModel<ProfessionalUserLicense> = {
        columns: [],
        query: { sort: {}, filter: [] }
    };

    public insurancesTable: TableViewModel<ProfessionalInsurance> = {
        columns: [],
        query: { sort: {}, filter: [] }
    };

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
        private readonly _licensesService: CsiInspectorLicensesService,
        private readonly _insurancesService: CsiInspectorInsurancesService
    ) {}

    public async ngOnInit(): Promise<void> {
        this.setupColumns();
        await this.loadInsurances();
    }

    public async setActiveTab(tab: 'insurances' | 'licenses'): Promise<void>{
        this.activeTab = tab;
        if (tab === 'licenses' && !this.licensesTable.items) {
            await this.loadLicenses();
        } else if (tab === 'insurances' && !this.insurancesTable.items) {
            await this.loadInsurances();
        }
    }

    setupColumns(): void {
        this.licensesTable.columns = this.getLicenseColumns();
        this.insurancesTable.columns = this.getInsuranceColumns();
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

    public async loadLicenses(): Promise<void> {
        try {
            this.licensesTable.isLoading = true;
            this.licensesTable.items = await this._licensesService.getLicenses(
                this.inspectorId,
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
            this.insurancesTable.items = await this._insurancesService.getInsurances(
                this.inspectorId,
                this.insurancesTable.items?.pageInfo || {},
                this.insurancesTable.query
            );
        } finally {
            this.insurancesTable.isLoading = false;
        }
    }

    public getProfessionalTypeLabel(type?: number): string {
        if (type === undefined || type === null) {
            return '';
        }
        return professionalTypeLabels[type as ProfessionalType] ?? '';
    }

    public getLicenseExpirationClass(license: ProfessionalUserLicense): string {
        if (!license.expirationDate) {
            return 'text-bg-primary';
        }
        if (license.expirationType === ExpirationType.Expired) {
            return 'text-bg-danger';
        }
        if (license.expirationType === ExpirationType.AboutToExpire) {
            return 'text-bg-warning';
        }
        return 'text-bg-success';
    }

    public getInsuranceExpirationClass(insurance: ProfessionalInsurance): string {
        if (!insurance.expirationDate) {
            return 'text-bg-primary';
        }
        if (insurance.expirationType === InsuranceExpirationType.Expired) {
            return 'text-bg-danger';
        }
        if (insurance.expirationType === InsuranceExpirationType.AboutToExpire) {
            return 'text-bg-warning';
        }
        return 'text-bg-success';
    }
}
