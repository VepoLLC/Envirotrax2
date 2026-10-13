import { Component, OnInit, TemplateRef, ViewChild } from "@angular/core";
import { ProfessionalInsurance } from "../../../shared/models/professionals/professional-insurance";
import { ProfessionalInsuranceService } from "../../../shared/services/professionals/professional-insurance.service";
import { ToastService, ToastType, CellTemplateData, ColumnType, ModalHelperService, TableColumn } from '@envirotrax/common-ui';
import { TableViewModel } from "../../../shared/models/table-view-model";
import { ProfesisonalService } from "../../../shared/services/professionals/professional.service";
import { NgForm } from "@angular/forms";
import { EditInsuranceComponent } from "../edit/edit-insurance.component";
import { ModalSize } from "@developer-partners/ngx-modal-dialog";
import { ExpirationType } from "../../../shared/models/professionals/licenses/professional-user-license";
import { Professional } from "../../../shared/models/professionals/professional";
import { HelperService } from "../../../shared/services/helpers/helper.service";

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

    public showBackflowHelp: boolean = false;
    public showCsiHelp: boolean = false;
    public showFogInspectionHelp: boolean = false;
    public activeHelpTab: 'backflow' | 'csi' | 'fogInspection' = 'backflow';

    @ViewChild('dateCell', { static: true })
    private dateCellTemplate!: TemplateRef<CellTemplateData<ProfessionalInsurance>>;

    constructor(
        private readonly _insuranceService: ProfessionalInsuranceService,
        private readonly _modalHelper: ModalHelperService,
        private readonly _toastService: ToastService,
        private readonly _professionalService: ProfesisonalService,
        private readonly _helper: HelperService
    ) { }

    public async ngOnInit(): Promise<void> {
        this.table.columns = this.getColumns();

        this.getRows();
        this.resetNewInsurance();

        const professional = await this._professionalService.getLoggedInProfessional();

        this.setHelpTabs(professional);
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
                caption: 'Policy Number',
                type: ColumnType.text
            },
            {
                field: 'coverageAmount',
                caption: 'Coverage $',
                type: ColumnType.number
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
            this.table.items = await this._insuranceService.getAll(
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

    public edit(insurance: ProfessionalInsurance): void {
        this._modalHelper.show<ProfessionalInsurance>(EditInsuranceComponent, {
            title: 'Edit Insurance Policy',
            model: { id: insurance.id },
            size: ModalSize.large,
            mode: 'disableFullScreen'
        }).result().subscribe(() => this.getRows());
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

                await this.getRows();
            });
    }

    public async save(form: NgForm): Promise<void> {
        if (form.valid) {
            try {
                this.isNewInsuranceLoading = true;
                this.newInsuranceValidationErrors = [];

                await this._insuranceService.add(this.newInsurance!, this.certificateFile!);

                this._toastService.show({
                    type: ToastType.Success,
                    text: 'Your insurance has been submitted for validation. The validation process may take anywhere from an hour up to 1 business day to process.'
                });

                form.resetForm();

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
}
