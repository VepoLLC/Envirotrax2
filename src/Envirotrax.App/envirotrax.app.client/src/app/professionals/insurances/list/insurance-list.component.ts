import { Component, OnInit, TemplateRef, ViewChild } from "@angular/core";
import { ProfessionalInsurance } from "../../../shared/models/professionals/professional-insurance";
import { ProfessionalInsuranceService } from "../../../shared/services/professionals/professional-insurance.service";
import { ToastService, ToastType, CellTemplateData, ColumnType, InputOption, ModalHelperService, TableColumn } from '@envirotrax/common-ui';
import { TableViewModel } from "../../../shared/models/table-view-model";
import { ProfesisonalService } from "../../../shared/services/professionals/professional.service";
import { NgForm } from "@angular/forms";
import { EditInsuranceComponent } from "../edit/edit-insurance.component";
import { EditCompanyLicenseComponent } from "../edit/edit-company-license.component";
import { ModalSize } from "@developer-partners/ngx-modal-dialog";
import { ExpirationType, ProfessionalDashboardLicenseInsurance, ProfessionalDashboardRowType } from "../../../shared/models/professionals/professional-dashboard-license-insurance";
import { ProfessionalLicense } from "../../../shared/models/professionals/licenses/professional-license";
import { LicenseScope, ProfessionalLicenseType } from "../../../shared/models/professionals/licenses/professional-license-type";
import { ProfessionalType } from "../../../shared/models/professionals/licenses/professional-user-license";
import { Professional } from "../../../shared/models/professionals/professional";
import { ProfessionalLicenseService } from "../../../shared/services/professionals/professional-license.service";
import { ProfessionalUserLicenseService } from "../../../shared/services/professionals/professional-user-license.service";
import { HelperService } from "../../../shared/services/helpers/helper.service";

const INSURANCE_POLICY_TYPE_ID = 'insurance';

@Component({
    selector: 'vp-insurance-list',
    templateUrl: './insurance-list.component.html',
    standalone: false
})
export class InsuranceListComponent implements OnInit {
    public table: TableViewModel<ProfessionalDashboardLicenseInsurance> = {
        columns: [],
        query: {
            sort: {},
            filter: []
        },
        freeTextSearch: {
            searchQuery: [
                { field: 'typeName' },
                { field: 'number' }
            ]
        }
    };

    public newInsurance?: ProfessionalInsurance;
    public newLicense: ProfessionalLicense = {};
    public certificateFile: File | null = null;
    public hasReadHelp: boolean = false;
    public newInsuranceValidationErrors: string[] = [];
    public isNewInsuranceLoading: boolean = false;
    public expirationType = ExpirationType;

    public documentTypes: InputOption<ProfessionalLicenseType>[] = [];
    public documentTypeId: number | string = INSURANCE_POLICY_TYPE_ID;
    public hasCompanyLicenseTypes: boolean = false;
    public isCompanyLicense: boolean = false;

    public showBackflowHelp: boolean = false;
    public showCsiHelp: boolean = false;
    public showFogInspectionHelp: boolean = false;
    public activeHelpTab: 'backflow' | 'csi' | 'fogInspection' = 'backflow';

    @ViewChild('dateCell', { static: true })
    private dateCellTemplate!: TemplateRef<CellTemplateData<ProfessionalDashboardLicenseInsurance>>;

    constructor(
        private readonly _insuranceService: ProfessionalInsuranceService,
        private readonly _licenseService: ProfessionalLicenseService,
        private readonly _userLicenseService: ProfessionalUserLicenseService,
        private readonly _modalHelper: ModalHelperService,
        private readonly _toastService: ToastService,
        private readonly _professionalService: ProfesisonalService,
        private readonly _helper: HelperService
    ) { }

    public async ngOnInit(): Promise<void> {
        this.table.columns = this.getColumns();

        this.getRows();
        this.resetNewInsurance();

        const [professional, companyLicenseTypes] = await Promise.all([
            this._professionalService.getLoggedInProfessional(),
            this._userLicenseService.getAllTypesAsOptions({ sort: {}, filter: [{ columnName: 'licenseScope', comparisonOperator: 'Eq', value: LicenseScope.Company.toString() }] }, false)
        ]);

        this.setHelpTabs(professional);
        this.setDocumentTypes(professional, companyLicenseTypes);
    }

    private setHelpTabs(professional: Professional): void {
        this.showBackflowHelp = professional.hasBackflowTesting == true;
        this.showCsiHelp = professional.hasCsiInspection == true;
        this.showFogInspectionHelp = professional.hasFogInspection == true;

        if (this.showBackflowHelp) {
            this.activeHelpTab = 'backflow';
        } else if (this.showCsiHelp) {
            this.activeHelpTab = 'csi';
        } else if (this.showFogInspectionHelp) {
            this.activeHelpTab = 'fogInspection';
        }
    }

    private setDocumentTypes(professional: Professional, companyLicenseTypes: InputOption<ProfessionalLicenseType>[]): void {
        const providedTypes: ProfessionalType[] = [];

        if (professional.hasBackflowTesting) {
            providedTypes.push(ProfessionalType.Bpat);
        }

        if (professional.hasCsiInspection) {
            providedTypes.push(ProfessionalType.CsiInspector);
        }

        if (professional.hasFogInspection) {
            providedTypes.push(ProfessionalType.FogInspector);
        }

        if (professional.hasFogTransportation) {
            providedTypes.push(ProfessionalType.FogTransporter);
        }

        const documentTypes: InputOption<ProfessionalLicenseType>[] = [{ id: INSURANCE_POLICY_TYPE_ID, text: 'Insurance Policy' }];

        for (const option of companyLicenseTypes) {
            const licenseType = option.data;

            if (licenseType && licenseType.state?.id == professional.state?.id && providedTypes.some(providedType => providedType == licenseType.professionalType)) {
                documentTypes.push(option);
            }
        }

        this.documentTypes = documentTypes;
        this.hasCompanyLicenseTypes = documentTypes.length > 1;
    }

    private async newInsuranceObject(): Promise<ProfessionalInsurance> {
        const pro = await this._professionalService.getLoggedInProfessional();

        return {
            professional: {
                id: pro.id
            }
        };
    }

    private async resetNewInsurance(): Promise<void> {
        this.newInsurance = await this.newInsuranceObject()
        this.hasReadHelp = false;
        this.certificateFile = null;
    }

    private getColumns(): TableColumn<ProfessionalDashboardLicenseInsurance>[] {
        return [
            {
                field: 'typeName',
                caption: 'Type',
                type: ColumnType.text
            },
            {
                field: 'number',
                caption: 'License / Insurance Policy Number',
                type: ColumnType.text
            },
            {
                field: 'expirationDate',
                caption: 'Expiration Date',
                cellTemplate: this.dateCellTemplate,
                type: ColumnType.date
            }
        ];
    }

    public async getRows(): Promise<void> {
        try {
            this.table.isLoading = true;
            this.table.items = await this._licenseService.getAllWithInsurances(
                this.table.items?.pageInfo || {},
                this.table.query
            );
        } finally {
            this.table.isLoading = false;
        }
    }

    public setHelpTab(tab: 'backflow' | 'csi' | 'fogInspection'): void {
        this.activeHelpTab = tab;
    }

    public documentTypeChange(typeId: number | string): void {
        this.documentTypeId = typeId;
        this.isCompanyLicense = typeId != INSURANCE_POLICY_TYPE_ID;

        this.newLicense.licenseType = undefined;
        this.newLicense.professionalType = undefined;

        const option = this.documentTypes.find(type => type.id == typeId);

        if (this.isCompanyLicense && option && option.data) {
            this.newLicense.licenseType = { id: option.data.id };
            this.newLicense.professionalType = option.data.professionalType;
        }
    }

    public edit(row: ProfessionalDashboardLicenseInsurance): void {
        if (row.rowType == ProfessionalDashboardRowType.CompanyLicense) {
            this._modalHelper.show<ProfessionalLicense, ProfessionalLicense>(EditCompanyLicenseComponent, {
                title: 'Edit License',
                model: { id: row.id },
                size: ModalSize.large,
                mode: 'disableFullScreen'
            }).result().subscribe(() => this.getRows());
        } else {
            this._modalHelper.show<ProfessionalInsurance>(EditInsuranceComponent, {
                title: 'Edit Insurance Policy',
                model: { id: row.id },
                size: ModalSize.large,
                mode: 'disableFullScreen'
            }).result().subscribe(() => this.getRows());
        }
    }

    public delete(row: ProfessionalDashboardLicenseInsurance): void {
        this._modalHelper.showDeleteConfirmation()
            .result()
            .subscribe(async () => {
                try {
                    this.table.isLoading = true;

                    if (row.rowType == ProfessionalDashboardRowType.CompanyLicense) {
                        await this._licenseService.delete(row.id!);
                        this._toastService.successFullyDeleted('License');
                    } else {
                        await this._insuranceService.delete(row.id!);
                        this._toastService.successFullyDeleted('Insurance');
                    }
                } finally {
                    this.table.isLoading = false;
                }

                await this.getRows();
            });
    }

    public async save(form: NgForm): Promise<void> {
        if (form.valid) {
            try {
                this.isNewInsuranceLoading = true;
                this.newInsuranceValidationErrors = [];

                if (this.isCompanyLicense) {
                    await this.addCompanyLicense();
                } else {
                    await this.addInsurance();
                }

                form.resetForm({ documentType: this.documentTypeId });

                this.newLicense = {};
                this.documentTypeChange(this.documentTypeId);
                this.resetNewInsurance();

                this.getRows();
            } catch (error) {
                if (!this._helper.parseValidationErrors(error, this.newInsuranceValidationErrors)) {
                    throw error;
                }
            } finally {
                this.isNewInsuranceLoading = false;
            }
        }
    }

    private async addInsurance(): Promise<void> {
        await this._insuranceService.add(this.newInsurance!, this.certificateFile!);

        this._toastService.show({
            type: ToastType.Success,
            text: 'Your insurance has been submitted for validation. The validation process may take anywhere from an hour up to 1 business day to process.'
        });
    }

    private async addCompanyLicense(): Promise<void> {
        await this._licenseService.add(this.newLicense);

        let licenseTypeName = '';
        const licenseType = this.documentTypes.find(type => type.id == this.documentTypeId);

        if (licenseType && licenseType.text) {
            licenseTypeName = licenseType.text;
        }

        this._toastService.show({
            type: ToastType.Success,
            text: `Your ${licenseTypeName} License has been submitted for validation. The validation process may take anywhere from an hour up to 1 business day to process.`
        });
    }
}
