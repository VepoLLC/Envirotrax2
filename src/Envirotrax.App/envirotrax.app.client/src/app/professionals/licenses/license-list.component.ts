import { Component, OnInit, TemplateRef, ViewChild } from "@angular/core";
import { ExpirationType, ProfessionalType, ProfessionalUserLicense, professionalTypeLabels } from "../../shared/models/professionals/licenses/professional-user-license";
import { ProfessionalUserLicenseService } from "../../shared/services/professionals/professional-user-license.service";
import { ModalHelperService } from "../../shared/services/helpers/modal-helper.service";
import { ToastService, ToastType } from "../../shared/services/toast.service";
import { TableViewModel } from "../../shared/models/table-view-model";
import { CellTemplateData, TableColumn } from "../../shared/components/data-components/table/table.component";
import { ColumnType } from "../../shared/components/data-components/sorting-filtering/query-view-model";
import { CreateEditLicenseComponent } from "../users/edit/licenses/create-edit/create-edit-license.component";
import { ModalSize } from "@developer-partners/ngx-modal-dialog";
import { NgForm } from "@angular/forms";
import { InputOption } from "../../shared/components/input/input.component";
import { ProfessionalLicenseType } from "../../shared/models/professionals/licenses/professional-license-type";
import { ProfesisonalService } from "../../shared/services/professionals/professional.service";
import { ProfessionalUser } from "../../shared/models/professionals/professional-user";
import { ProfessionalUserLookupComponent } from "../../shared/components/lookups/professional-user-lookup/professional-user-lookup.component";

@Component({
    selector: 'vp-license-list',
    templateUrl: './license-list.component.html',
    standalone: false
})
export class LicenseListComponent implements OnInit {
    private _allLicenseTypes: InputOption<ProfessionalLicenseType>[] = [];

    @ViewChild('typeCell', { static: true })
    private typeCellTemplate!: TemplateRef<CellTemplateData<ProfessionalUserLicense>>;

    @ViewChild('dateCell', { static: true })
    private dateCellTemplate!: TemplateRef<CellTemplateData<ProfessionalUserLicense>>;

    public expirationType = ExpirationType;
    public hasReadHelp: boolean = false;
    public newLicense: ProfessionalUserLicense = {};
    public newLicenseValidationErrors: string[] = [];
    public licenseTypes: InputOption<ProfessionalLicenseType>[] = [];
    public isNewLicenseLoading: boolean = false;

    public readonly professionalTypeOptions: InputOption<ProfessionalType>[] = Object.values(ProfessionalType)
        .filter(v => typeof v === 'number')
        .map(v => ({
            id: v as ProfessionalType,
            text: professionalTypeLabels[v as ProfessionalType],
            data: v as ProfessionalType
        }));

    public table: TableViewModel<ProfessionalUserLicense> = {
        columns: [],
        query: {
            sort: {},
            filter: []
        },
        freeTextSearch: {
            searchQuery: [
                { field: 'user.contactName' },
                { field: 'licenseType.name' },
                { field: 'licenseNumber' }
            ]
        }
    };

    constructor(
        private readonly _licenseService: ProfessionalUserLicenseService,
        private readonly _modalHelper: ModalHelperService,
        private readonly _toastService: ToastService,
        private readonly _professionalService: ProfesisonalService
    ) {
        this.professionalTypeOptions.splice(0, 0, { id: '', text: '' });
    }

    public async ngOnInit(): Promise<void> {
        this.table.columns = this.getColumns();

        const [_, professional, licenseTypes] = await Promise.all([
            this.getLicenses(),
            this._professionalService.getLoggedInProfessional(),
            this._licenseService.getAllTypesAsOptions({}, true)
        ]);

        this._allLicenseTypes = licenseTypes;
        this.licenseTypes = this._allLicenseTypes.filter(l => l.data?.state?.id == professional.state?.id || !l.id);
    }

    private getColumns(): TableColumn<ProfessionalUserLicense>[] {
        return [
            {
                field: 'user.contactName',
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
                field: 'licenseType.name',
                caption: 'Type',
                type: ColumnType.text
            },
            {
                field: 'licenseNumber',
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

    public getProfessionalTypeLabel(type?: number): string {
        if (type === undefined || type === null) {
            return '';
        }

        return professionalTypeLabels[type as keyof typeof professionalTypeLabels] ?? '';
    }

    public async getLicenses(): Promise<void> {
        try {
            this.table.isLoading = true;
            this.table.items = await this._licenseService.getAll(
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

    public licenseTypeChange(typeId: number): void {
        if (typeId) {
            this.newLicense.licenseType = {
                id: typeId
            };
        } else {
            this.newLicense.licenseType = undefined;
        }
    }

    public professionChange(): void {
        this.licenseTypes = this._allLicenseTypes.filter(t => t.data?.professionalType == this.newLicense.professionalType || !t.id);
        this.newLicense.licenseType = undefined;
    }

    public async add(form: NgForm): Promise<void> {
        if (form.valid) {
            try {
                this.isNewLicenseLoading = true;
                this.newLicenseValidationErrors = [];

                await this._licenseService.add(this.newLicense!);

                const licenseType = this.licenseTypes.find(type => type.data?.id == this.newLicense.licenseType?.id);

                this._toastService.show({
                    type: ToastType.Success,
                    text: `Your ${licenseType?.text} License has been submitted for validation. The validation process may take anywhere from an hour up to 1 business day to process.`
                })

                this.newLicense = {};
                this.getLicenses();
                form.resetForm();
            } catch (e) {

            } finally {
                this.isNewLicenseLoading = false;
            }
        }
    }

    public edit(license: ProfessionalUserLicense): void {
        this._modalHelper.show<ProfessionalUserLicense, ProfessionalUserLicense>(CreateEditLicenseComponent, {
            title: 'Edit License',
            model: license,
            size: ModalSize.large
        }).result().subscribe(() => this.getLicenses());
    }

    public delete(license: ProfessionalUserLicense): void {
        this._modalHelper.showDeleteConfirmation()
            .result()
            .subscribe(async () => {
                try {
                    this.table.isLoading = true;
                    await this._licenseService.delete(license.id!);
                    this._toastService.successFullyDeleted('License');
                } finally {
                    this.table.isLoading = false;
                }

                await this.getLicenses();
            });
    }
}
