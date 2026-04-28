import { Component, Input, OnInit, TemplateRef, ViewChild } from "@angular/core";
import { ExpirationType, ProfessionalUserLicense, professionalTypeLabels, ProfessionalType } from "../../../../shared/models/professionals/licenses/professional-user-license";
import { ProfessionalInsurance, ExpirationType as InsuranceExpirationType } from "../../../../shared/models/professionals/professional-insurance";
import { TableViewModel } from "../../../../shared/models/table-view-model";
import { CellTemplateData, TableColumn } from "../../../../shared/components/data-components/table/table.component";
import { ColumnType } from "../../../../shared/components/data-components/sorting-filtering/query-view-model";
import { CsiInspectorLicensesService } from "../../../../shared/services/csi/csi-inspector-licenses.service";
import { CsiInspectorInsurancesService } from "../../../../shared/services/csi/csi-inspector-insurances.service";
import { AuthService } from "../../../../shared/services/auth/auth.service";
import { FeatureType } from "../../../../shared/models/feature-tyype";
import { PermissionAction, PermissionType } from "../../../../shared/models/permission-type";
import { ModalHelperService } from "../../../../shared/services/helpers/modal-helper.service";
import { ModalSize } from "@developer-partners/ngx-modal-dialog";
import { ToastService } from "../../../../shared/services/toast.service";
import { CsiInspectorAddEditInsuranceComponent } from "./modals/add-edit-csi-inspector-insurance.component";
import { CsiInspectorAddEditLicenseComponent } from "./modals/add-edit-csi-inspector-license.component";


@Component({
    selector: 'vp-csi-inspector-license-insurances',
    standalone: false,
    templateUrl: './csi-inspector-license-insurance.component.html'
})
export class CsiInspectorLicenseInsuranceComponent implements OnInit {
    @Input() public inspectorId!: number;

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
        private readonly _insurancesService: CsiInspectorInsurancesService,
        private readonly _authService: AuthService,
        private readonly _modalHelper: ModalHelperService,
        private readonly _toastService: ToastService,
    ) { }

    public async ngOnInit(): Promise<void> {
        await this.setPermissions();

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

    private async setPermissions(): Promise<void> {
        const canEditCsiInspectors = await this._authService.hasAnyPermisison(PermissionAction.CanEdit, PermissionType.CsiInspectors);

        this.canManageLicenses = canEditCsiInspectors && await this._authService.hasAnyFeatures(FeatureType.ManageProfessionalLicenses);
        this.canManageInsurances = canEditCsiInspectors && await this._authService.hasAnyFeatures(FeatureType.ManageProfessionalInsurances);
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

    public addInsurance(): void {
        this._modalHelper.show<any, ProfessionalInsurance>(CsiInspectorAddEditInsuranceComponent, {
            title: 'Add Insurance Policy',
            model: { inspectorId: this.inspectorId, insurance: {} },
            size: ModalSize.large
        }).result().subscribe(() => this.loadInsurances());
    }

    public addLicense(): void {
        this._modalHelper.show<any, ProfessionalUserLicense>(CsiInspectorAddEditLicenseComponent, {
            title: 'Add License',
            model: { inspectorId: this.inspectorId, license: {} },
            size: ModalSize.large
        }).result().subscribe(() => this.loadLicenses());
    }

    public editLicense(license: ProfessionalUserLicense): void {
        this._modalHelper.show<any, ProfessionalUserLicense>(CsiInspectorAddEditLicenseComponent, {
            title: 'Edit License',
            model: { inspectorId: this.inspectorId, license },
            size: ModalSize.large
        }).result().subscribe(() => this.loadLicenses());
    }

    public deleteLicense(license: ProfessionalUserLicense): void {
        this._modalHelper.showDeleteConfirmation().result().subscribe(async () => {
            try {
                this.licensesTable.isLoading = true;
                await this._licensesService.delete(this.inspectorId, license.id!);
                this._toastService.successFullyDeleted('License');
            } finally {
                this.licensesTable.isLoading = false;
            }
            await this.loadLicenses();
        });
    }

    public editInsurance(insurance: ProfessionalInsurance): void {
        this._modalHelper.show<any, ProfessionalInsurance>(CsiInspectorAddEditInsuranceComponent, {
            title: 'Edit Insurance Policy',
            model: { inspectorId: this.inspectorId, insurance },
            size: ModalSize.large
        }).result().subscribe(() => this.loadInsurances());
    }

    public deleteInsurance(insurance: ProfessionalInsurance): void {
        this._modalHelper.showDeleteConfirmation().result().subscribe(async () => {
            try {
                this.insurancesTable.isLoading = true;
                await this._insurancesService.delete(this.inspectorId, insurance.id!);
                this._toastService.successFullyDeleted('Insurance');
            } finally {
                this.insurancesTable.isLoading = false;
            }
            await this.loadInsurances();
        });
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
