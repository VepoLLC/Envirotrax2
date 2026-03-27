import { Component, Input, OnInit, ViewChild, TemplateRef } from "@angular/core";
import { ProfessionalUserLicense, professionalTypeLabels } from "../../../../../shared/models/professionals/professional-user-license";
import { ProfessionalUserLicenseService } from "../../../../../shared/services/professionals/professional-user-license.service";
import { ModalHelperService } from "../../../../../shared/services/helpers/modal-helper.service";
import { ToastService } from "../../../../../shared/services/toast.service";
import { TableViewModel } from "../../../../../shared/models/table-view-model";
import { CellTemplateData, TableColumn } from "../../../../../shared/components/data-components/table/table.component";
import { ColumnType } from "../../../../../shared/components/data-components/sorting-filtering/query-view-model";
import { CreateProfessionalUserLicenseComponent } from "../create/create-professional-user-license.component";
import { EditProfessionalUserLicenseComponent } from "../edit/edit-professional-user-license.component";

@Component({
    selector: 'vp-professional-user-license-list',
    templateUrl: './professional-user-license-list.component.html',
    standalone: false
})
export class ProfessionalUserLicenseListComponent implements OnInit {
    @Input() public userId!: number;

    @ViewChild('typeCell', { static: true })
    private typeCellTemplate!: TemplateRef<CellTemplateData<ProfessionalUserLicense>>;

    @ViewChild('dateCell', { static: true })
    private dateCellTemplate!: TemplateRef<CellTemplateData<ProfessionalUserLicense>>;

    public table: TableViewModel<ProfessionalUserLicense> = {
        columns: [],
        query: {
            sort: {},
            filter: []
        }
    };

    constructor(
        private readonly _licenseService: ProfessionalUserLicenseService,
        private readonly _modalHelper: ModalHelperService,
        private readonly _toastService: ToastService
    ) { }

    public ngOnInit(): void {
        this.table.columns = this.getColumns();
        this.getLicenses();
    }

    private getColumns(): TableColumn<ProfessionalUserLicense>[] {
        return [
            {
                field: 'professionalType',
                caption: 'Type',
                type: ColumnType.text,
                cellTemplate: this.typeCellTemplate
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
        if (type === undefined || type === null) return '';
        return professionalTypeLabels[type as keyof typeof professionalTypeLabels] ?? '';
    }

    public async getLicenses(): Promise<void> {
        try {
            this.table.isLoading = true;
            this.table.items = await this._licenseService.getForUser(
                this.userId,
                this.table.items?.pageInfo || {},
                this.table.query
            );
        } finally {
            this.table.isLoading = false;
        }
    }

    public add(): void {
        this._modalHelper.show<number, ProfessionalUserLicense>(CreateProfessionalUserLicenseComponent, {
            title: 'Add License',
            model: this.userId
        }).result().subscribe(() => {
            this._toastService.successfullySaved("License");
            this.getLicenses();
        });
    }

    public edit(license: ProfessionalUserLicense): void {
        this._modalHelper.show<ProfessionalUserLicense, ProfessionalUserLicense>(EditProfessionalUserLicenseComponent, {
            title: 'Edit License',
            model: license
        }).result().subscribe(() => {
            this._toastService.successfullySaved("License");
            this.getLicenses();
        });
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
