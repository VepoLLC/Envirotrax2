import { Component, OnInit, TemplateRef, ViewChild } from "@angular/core";
import { ExpirationType, ProfessionalInsurance } from "../../../shared/models/professionals/professional-insurance";
import { ProfessionalInsuranceService } from "../../../shared/services/professionals/professional-insurance.service";
import { ModalHelperService } from "../../../shared/services/helpers/modal-helper.service";
import { ToastService, ToastType } from "../../../shared/services/toast.service";
import { TableViewModel } from "../../../shared/models/table-view-model";
import { CellTemplateData, TableColumn } from "../../../shared/components/data-components/table/table.component";
import { ColumnType } from "../../../shared/components/data-components/sorting-filtering/query-view-model";
import { ProfesisonalService } from "../../../shared/services/professionals/professional.service";
import { NgForm } from "@angular/forms";
import { HelperService } from "../../../shared/services/helpers/helper.service";
import { EditInsuranceComponent } from "../edit/edit-insurance.component";
import { ModalSize } from "@developer-partners/ngx-modal-dialog";

@Component({
    selector: 'vp-insurance-list',
    templateUrl: './insurance-list.component.html',
    standalone: false
})
export class InsuranceListComponent implements OnInit {
    public table: TableViewModel<ProfessionalInsurance> = {
        columns: [],
        query: {
            sort: {},
            filter: []
        },
        freeTextSearch: {
            searchQuery: [
                { field: 'insuranceNumber' }
            ]
        }
    };

    public newInsurance?: ProfessionalInsurance;
    public certificateFile: File | null = null;
    public hasReadHelp: boolean = false;
    public newInsuranceValidationErrors: string[] = [];
    public isNewInsuranceLoading: boolean = false;
    public expirationType = ExpirationType;

    @ViewChild('dateCell', { static: true })
    private dateCellTemplate!: TemplateRef<CellTemplateData<ProfessionalInsurance>>;

    constructor(
        private readonly _insuranceService: ProfessionalInsuranceService,
        private readonly _modalHelper: ModalHelperService,
        private readonly _toastService: ToastService,
        private readonly _professionalService: ProfesisonalService,
        private readonly _helperService: HelperService
    ) { }

    public async ngOnInit(): Promise<void> {
        this.table.columns = this.getColumns();

        this.getInsurances();
        this.resetNewInsurance();
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

    private getColumns(): TableColumn<ProfessionalInsurance>[] {
        return [
            {
                field: 'insuranceNumber',
                caption: 'Insurance Number',
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

    public async getInsurances(): Promise<void> {
        try {
            this.table.isLoading = true;
            this.table.items = await this._insuranceService.getAll(
                this.table.items?.pageInfo || {},
                this.table.query
            );
        } finally {
            this.table.isLoading = false;
        }
    }

    public edit(insurance: ProfessionalInsurance): void {
        this._modalHelper.show<ProfessionalInsurance>(EditInsuranceComponent, {
            title: 'Edit Insurance Policy',
            model: insurance,
            size: ModalSize.large
        }).result().subscribe(() => this.getInsurances());
    }

    public delete(insurance: ProfessionalInsurance): void {
        this._modalHelper.showDeleteConfirmation()
            .result()
            .subscribe(async () => {
                try {
                    this.table.isLoading = true;
                    await this._insuranceService.delete(insurance.id!);
                    this._toastService.successFullyDeleted('Insurance');
                } finally {
                    this.table.isLoading = false;
                }

                await this.getInsurances();
            });
    }

    public certificateFileSelected(file: File | null): void {
        this.certificateFile = file;
    }

    public async saveInsurance(form: NgForm): Promise<void> {
        if (form.valid) {
            try {
                this.isNewInsuranceLoading = true;
                this.newInsuranceValidationErrors = [];

                await this._insuranceService.add(this.newInsurance!, this.certificateFile!);

                this._toastService.show({
                    type: ToastType.Success,
                    text: 'Your insurance has been submitted for validation. The validation process may take anywhere from an hour up to 1 business day to process.'
                });

                this.resetNewInsurance();
                this.getInsurances();
                form.resetForm();
            } catch (e) {

            } finally {
                this.isNewInsuranceLoading = false;
            }
        }
    }
}