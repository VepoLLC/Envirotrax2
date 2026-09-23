import { Component, Input, OnInit, TemplateRef, ViewChild } from "@angular/core";
import { ExpirationType, ProfessionalUserLicense, professionalTypeLabels, ProfessionalType } from "../../../../../shared/models/professionals/licenses/professional-user-license";
import { ProfessionalInsurance, ExpirationType as InsuranceExpirationType } from "../../../../../shared/models/professionals/professional-insurance";
import { TableViewModel } from "../../../../../shared/models/table-view-model";
import { FogTransporterLicensesService } from "../../../../../shared/services/fog/fog-transporter-licenses.service";
import { FogTransporterInsurancesService } from "../../../../../shared/services/fog/fog-transporter-insurances.service";
import { Professional } from "../../../../../shared/models/professionals/professional";
import { DownloadService } from "../../../../../shared/services/download.service";
import { AuthService } from "../../../../../shared/services/auth/auth.service";
import { FeatureType } from "../../../../../shared/models/feature-type";
import { PermissionAction, PermissionType } from "../../../../../shared/models/permission-type";
import { ModalSize } from "@developer-partners/ngx-modal-dialog";
import { EditFogTransporterLicenseComponent, FogLicenseModalData } from "../edit/edit-fog-transporter-license.component";
import { EditFogTransporterInsuranceComponent, FogInsuranceModalData } from "../edit/edit-fog-transporter-insurance.component";
import { CellTemplateData, ColumnType, ModalHelperService, TableColumn, TableCustomAction, ToastService } from "@envirotrax/common-ui";

@Component({
    selector: 'vp-fog-transporter-license-insurances',
    standalone: false,
    templateUrl: './fog-transporter-license-insurance.component.html'
})
export class FogTransporterLicenseInsuranceComponent implements OnInit {
    @Input() public transporterId!: number;
    @Input() public transporter: Professional | null = null;

    public activeTab: 'insurances' | 'licenses' = 'insurances';

    public expirationType = ExpirationType;
    public insuranceExpirationType = InsuranceExpirationType;

    public canManageLicenses: boolean = false;
    public canManageInsurances: boolean = false;

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
        private readonly _licensesService: FogTransporterLicensesService,
        private readonly _insurancesService: FogTransporterInsurancesService,
        private readonly _authService: AuthService,
        private readonly _modalHelper: ModalHelperService,
        private readonly _toastService: ToastService,
        private readonly _downloadService: DownloadService
    ) { }

    public async ngOnInit(): Promise<void> {
        await this.setPermissions();
        this.setupColumns();
        await this.loadInsurances();
    }

    private async setPermissions(): Promise<void> {
        const canEditFogTransporters = await this._authService.hasAnyPermisison(PermissionAction.CanModify, PermissionType.FogTransporters);

        this.canManageLicenses = canEditFogTransporters && await this._authService.hasAnyFeatures(FeatureType.ManageProfessionalLicenses);
        this.canManageInsurances = canEditFogTransporters && await this._authService.hasAnyFeatures(FeatureType.ManageProfessionalInsurances);

        if (this.canManageInsurances) {
            this.insuranceCustomActions = [
                {
                    text: 'View',
                    iconClass: 'fa-solid fa-eye',
                    action: (insurance: ProfessionalInsurance) => this.viewInsuranceFile(insurance)
                },
                {
                    text: 'Email',
                    iconClass: 'fa-solid fa-envelope',
                    action: (insurance: ProfessionalInsurance) => this.prepareEmail(insurance)
                }
            ];
        }
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

    public addLicense(): void {
        this._modalHelper.show<FogLicenseModalData, ProfessionalUserLicense>(EditFogTransporterLicenseComponent, {
            title: 'Add License',
            model: { transporterId: this.transporterId, license: {} },
            size: ModalSize.large
        }).result().subscribe(() => this.loadLicenses());
    }

    public editLicense(license: ProfessionalUserLicense): void {
        this._modalHelper.show<FogLicenseModalData, ProfessionalUserLicense>(EditFogTransporterLicenseComponent, {
            title: 'Edit License',
            model: { transporterId: this.transporterId, license },
            size: ModalSize.large
        }).result().subscribe(() => this.loadLicenses());
    }

    public addInsurance(): void {
        this._modalHelper.show<FogInsuranceModalData, ProfessionalInsurance>(EditFogTransporterInsuranceComponent, {
            title: 'Add Insurance Policy',
            model: { transporterId: this.transporterId, insurance: {} },
            size: ModalSize.large
        }).result().subscribe(() => this.loadInsurances());
    }

    public editInsurance(insurance: ProfessionalInsurance): void {
        this._modalHelper.show<FogInsuranceModalData, ProfessionalInsurance>(EditFogTransporterInsuranceComponent, {
            title: 'Edit Insurance Policy',
            model: { transporterId: this.transporterId, insurance },
            size: ModalSize.large
        }).result().subscribe(() => this.loadInsurances());
    }

    public deleteInsurance(insurance: ProfessionalInsurance): void {
        this._modalHelper.showDeleteConfirmation().result().subscribe(async () => {
            try {
                this.insurancesTable.isLoading = true;
                await this._insurancesService.delete(this.transporterId, insurance.id!);
                this._toastService.successFullyDeleted('Insurance');
            } finally {
                this.insurancesTable.isLoading = false;
            }
            await this.loadInsurances();
        });
    }

    public async viewInsuranceFile(insurance: ProfessionalInsurance): Promise<void> {
        try {
            this.insurancesTable.isLoading = true;
            const url = await this._insurancesService.getFileUrl(this.transporterId, insurance.id!);
            this._downloadService.downloadFileFromUrl(url);
        } finally {
            this.insurancesTable.isLoading = false;
        }
    }

    public async prepareEmail(insurance: ProfessionalInsurance): Promise<void> {
        if (!this.transporter) {
            return;
        }

        const adminEmail = await this._authService.getUserEmail();
        const body = `${this.transporter.name},%0D%0A%0D%0A` +
            `We have the Insurance: ${insurance.insuranceNumber} updated in your account. ` +
            `Please let us know if you need anything else.%0D%0A%0D%0A` +
            `Sincerely,%0D%0A${adminEmail} - Envirotrax`;
        const link = `mailto:${this.transporter.companyEmail}` +
            `?subject=${encodeURIComponent('Envirotrax - Insurance Validation')}` +
            `&body=${body}`;

        window.open(link);
    }

    public async loadLicenses(): Promise<void> {
        try {
            this.licensesTable.isLoading = true;
            this.licensesTable.items = await this._licensesService.getLicenses(
                this.transporterId,
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
                this.transporterId,
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
