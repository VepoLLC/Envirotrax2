import { Component, OnInit, TemplateRef, ViewChild } from "@angular/core";
import { ExpirationType, ProfessionalType, ProfessionalUserLicense, professionalTypeLabels } from "../../shared/models/professionals/licenses/professional-user-license";
import { ProfessionalUserLicenseService } from "../../shared/services/professionals/professional-user-license.service";
import { ToastService, ToastType, CellTemplateData, ColumnType, InputOption, ModalHelperService, TableColumn } from '@envirotrax/common-ui';
import { TableViewModel } from "../../shared/models/table-view-model";
import { CreateEditLicenseComponent } from "../users/edit/licenses/create-edit/create-edit-license.component";
import { ModalSize } from "@developer-partners/ngx-modal-dialog";
import { NgForm } from "@angular/forms";
import { LicenseScope, ProfessionalLicenseType } from "../../shared/models/professionals/licenses/professional-license-type";
import { ProfesisonalService } from "../../shared/services/professionals/professional.service";
import { ProfessionalUser } from "../../shared/models/professionals/professional-user";
import { ProfessionalUserLookupComponent } from "../../shared/components/lookups/professional-user-lookup/professional-user-lookup.component";
import { Professional } from "../../shared/models/professionals/professional";
import { ProfessionalLicense } from "../../shared/models/professionals/licenses/professional-license";
import { ProfessionalLicenseService } from "../../shared/services/professionals/professional-license.service";
import { ProfessionalDashboardLicenseInsurance, ProfessionalDashboardRowType } from "../../shared/models/professionals/professional-dashboard-license-insurance";
import { EditCompanyLicenseComponent } from "./edit/edit-company-license.component";
import { HelperService } from "../../shared/services/helpers/helper.service";

@Component({
    selector: 'vp-license-list',
    templateUrl: './license-list.component.html',
    standalone: false
})
export class LicenseListComponent implements OnInit {
    private _allLicenseTypes: InputOption<ProfessionalLicenseType>[] = [];

    @ViewChild('typeCell', { static: true })
    private typeCellTemplate!: TemplateRef<CellTemplateData<ProfessionalDashboardLicenseInsurance>>;

    @ViewChild('dateCell', { static: true })
    private dateCellTemplate!: TemplateRef<CellTemplateData<ProfessionalDashboardLicenseInsurance>>;

    public expirationType = ExpirationType;
    public professionalType = ProfessionalType;
    public professionalTypeLabels = professionalTypeLabels;
    public hasReadHelp: boolean = false;
    public newLicense: ProfessionalUserLicense = {};
    public newLicenseValidationErrors: string[] = [];
    public licenseTypes: InputOption<ProfessionalLicenseType>[] = [];
    public isNewLicenseLoading: boolean = false;
    public isCompanyLicense: boolean = false;

    public professions: InputOption<ProfessionalType>[] = [];
    public activeProfession?: ProfessionalType;

    public table: TableViewModel<ProfessionalDashboardLicenseInsurance> = {
        columns: [],
        query: {
            sort: {},
            filter: []
        },
        freeTextSearch: {
            searchQuery: [
                { field: 'assignedTo' },
                { field: 'typeName' },
                { field: 'number' }
            ]
        }
    };

    constructor(
        private readonly _licenseService: ProfessionalUserLicenseService,
        private readonly _companyLicenseService: ProfessionalLicenseService,
        private readonly _modalHelper: ModalHelperService,
        private readonly _toastService: ToastService,
        private readonly _professionalService: ProfesisonalService,
        private readonly _helper: HelperService
    ) {

    }

    public async ngOnInit(): Promise<void> {
        this.table.columns = this.getColumns();

        const [_, professional, licenseTypes] = await Promise.all([
            this.getLicenses(),
            this._professionalService.getLoggedInProfessional(),
            this._licenseService.getAllTypesAsOptions({ sort: {}, filter: [] }, false)
        ]);

        this._allLicenseTypes = licenseTypes.filter(l => l.data?.state?.id == professional.state?.id);
        this.professions = this.getProfessions(professional);

        if (this.professions.length > 0) {
            this.selectProfession(this.professions[0].data!);
        }
    }

    private getProfessions(professional: Professional): InputOption<ProfessionalType>[] {
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

        const professions: InputOption<ProfessionalType>[] = [];

        for (const providedType of providedTypes) {
            const hasLicenseTypes = this._allLicenseTypes.some(licenseType => licenseType.data?.professionalType == providedType);

            if (hasLicenseTypes) {
                professions.push({
                    id: providedType,
                    text: professionalTypeLabels[providedType],
                    data: providedType
                });
            }
        }

        return professions;
    }

    private getColumns(): TableColumn<ProfessionalDashboardLicenseInsurance>[] {
        return [
            {
                field: 'assignedTo',
                caption: 'User',
                type: ColumnType.text
            },
            {
                field: 'professionalType',
                caption: 'Profession',
                type: ColumnType.text,
                cellTemplate: this.typeCellTemplate
            },
            {
                field: 'typeName',
                caption: 'Type',
                type: ColumnType.text
            },
            {
                field: 'number',
                caption: 'License Number',
                type: ColumnType.text
            },
            {
                field: 'expirationDate',
                caption: 'Expiration Date',
                type: ColumnType.date,
                cellTemplate: this.dateCellTemplate
            }
        ];
    }

    public async getLicenses(): Promise<void> {
        try {
            this.table.isLoading = true;
            this.table.items = await this._companyLicenseService.getAllWithUserLicenses(
                this.table.items?.pageInfo || {},
                this.table.query
            );
        } finally {
            this.table.isLoading = false;
        }
    }

    public lookupUser(): void {
        this._modalHelper.show<ProfessionalUser>(ProfessionalUserLookupComponent, {
            title: 'Accounts',
            size: ModalSize.large
        }).result()
            .subscribe(selectedUser => {
                this.newLicense.user = selectedUser;
            });
    }

    public selectProfession(professionalType: ProfessionalType): void {
        this.newLicense.professionalType = professionalType;
        this.professionChange();
    }

    public professionChange(): void {
        this.activeProfession = this.newLicense.professionalType;
        this.licenseTypes = this._allLicenseTypes.filter(t => t.data?.professionalType == this.newLicense.professionalType);
        this.newLicense.licenseType = undefined;

        this.updateLicenseScope();
    }

    public licenseTypeChange(typeId: number): void {
        if (typeId) {
            this.newLicense.licenseType = {
                id: typeId
            };
        } else {
            this.newLicense.licenseType = undefined;
        }

        this.updateLicenseScope();
    }

    private updateLicenseScope(): void {
        const selectedType = this.licenseTypes.find(type => type.data?.id == this.newLicense.licenseType?.id);

        if (selectedType && selectedType.data) {
            this.isCompanyLicense = selectedType.data.licenseScope == LicenseScope.Company;
            return;
        }

        const hasUserLicenseTypes = this.licenseTypes.some(type => type.data?.licenseScope == LicenseScope.User);

        this.isCompanyLicense = !hasUserLicenseTypes;
    }

    public async add(form: NgForm): Promise<void> {
        if (form.valid) {
            try {
                this.isNewLicenseLoading = true;
                this.newLicenseValidationErrors = [];

                if (this.isCompanyLicense) {
                    await this.addCompanyLicense();
                } else {
                    await this._licenseService.add(this.newLicense);
                }

                const licenseType = this.licenseTypes.find(type => type.data?.id == this.newLicense.licenseType?.id);

                this._toastService.show({
                    type: ToastType.Success,
                    text: `Your ${licenseType?.text} License has been submitted for validation. The validation process may take anywhere from an hour up to 1 business day to process.`
                });

                const professionalType = this.newLicense.professionalType!;

                this.newLicense = {};
                this.hasReadHelp = false;

                form.resetForm({ professionalType: professionalType });
                this.selectProfession(professionalType);

                this.getLicenses();
            } catch (error) {
                if (!this._helper.parseValidationErrors(error, this.newLicenseValidationErrors)) {
                    throw error;
                }
            } finally {
                this.isNewLicenseLoading = false;
            }
        }
    }

    private async addCompanyLicense(): Promise<void> {
        const license: ProfessionalLicense = {
            professionalType: this.newLicense.professionalType,
            licenseType: this.newLicense.licenseType,
            licenseNumber: this.newLicense.licenseNumber
        };

        await this._companyLicenseService.add(license);
    }

    public edit(row: ProfessionalDashboardLicenseInsurance): void {
        if (row.rowType == ProfessionalDashboardRowType.CompanyLicense) {
            this._modalHelper.show<ProfessionalLicense, ProfessionalLicense>(EditCompanyLicenseComponent, {
                title: 'Edit License',
                model: { id: row.id },
                size: ModalSize.large,
                mode: 'disableFullScreen'
            }).result().subscribe(() => this.getLicenses());
        } else {
            this._modalHelper.show<ProfessionalUserLicense, ProfessionalUserLicense>(CreateEditLicenseComponent, {
                title: 'Edit License',
                model: { id: row.id },
                size: ModalSize.large,
                mode: 'disableFullScreen'
            }).result().subscribe(() => this.getLicenses());
        }
    }

    public delete(row: ProfessionalDashboardLicenseInsurance): void {
        this._modalHelper.showDeleteConfirmation()
            .result()
            .subscribe(async () => {
                try {
                    this.table.isLoading = true;

                    if (row.rowType == ProfessionalDashboardRowType.CompanyLicense) {
                        await this._companyLicenseService.delete(row.id!);
                    } else {
                        await this._licenseService.delete(row.id!);
                    }

                    this._toastService.successFullyDeleted('License');
                } finally {
                    this.table.isLoading = false;
                }

                await this.getLicenses();
            });
    }
}
