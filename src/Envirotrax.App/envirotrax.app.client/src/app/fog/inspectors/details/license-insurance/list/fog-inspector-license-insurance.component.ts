import { Component, Input, OnInit, TemplateRef, ViewChild } from "@angular/core";
import { ExpirationType, ProfessionalUserLicense, professionalTypeLabels, ProfessionalType } from "../../../../../shared/models/professionals/licenses/professional-user-license";
import { ProfessionalInsurance, ExpirationType as InsuranceExpirationType } from "../../../../../shared/models/professionals/professional-insurance";
import { TableViewModel } from "../../../../../shared/models/table-view-model";
import { FogInspectorLicensesService } from "../../../../../shared/services/fog/fog-inspector-licenses.service";
import { FogInspectorInsurancesService } from "../../../../../shared/services/fog/fog-inspector-insurances.service";
import { DownloadService } from "../../../../../shared/services/download.service";
import { CellTemplateData, ColumnType, TableColumn, TableCustomAction } from "@envirotrax/common-ui";

@Component({
    selector: 'vp-fog-inspector-license-insurances',
    standalone: false,
    templateUrl: './fog-inspector-license-insurance.component.html'
})
export class FogInspectorLicenseInsuranceComponent implements OnInit {
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

    public insuranceCustomActions: TableCustomAction<ProfessionalInsurance>[] = [];

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
        private readonly _licensesService: FogInspectorLicensesService,
        private readonly _insurancesService: FogInspectorInsurancesService,
        private readonly _downloadService: DownloadService
    ) { }

    public async ngOnInit(): Promise<void> {
        this.insuranceCustomActions = [
            {
                text: 'View',
                iconClass: 'fa-solid fa-eye',
                action: (insurance: ProfessionalInsurance) => this.viewInsuranceFile(insurance)
            }
        ];

        this.setupColumns();
        await this.loadInsurances();
    }

    public async setActiveTab(tab: 'insurances' | 'licenses'): Promise<void> {
        this.activeTab = tab;
        if (tab === 'licenses' && !this.licensesTable.items) {
            await this.loadLicenses();
        } else if (tab === 'insurances' && !this.insurancesTable.items) {
            await this.loadInsurances();
        }
    }

    private setupColumns(): void {
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

    public async viewInsuranceFile(insurance: ProfessionalInsurance): Promise<void> {
        try {
            this.insurancesTable.isLoading = true;
            const url = await this._insurancesService.getFileUrl(this.inspectorId, insurance.id!);
            this._downloadService.downloadFileFromUrl(url);
        } finally {
            this.insurancesTable.isLoading = false;
        }
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
}
