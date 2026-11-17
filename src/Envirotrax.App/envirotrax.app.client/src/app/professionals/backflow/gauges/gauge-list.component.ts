import { Component, OnInit, TemplateRef, ViewChild } from "@angular/core";
import { BackflowGauge, GaugeExpirationType } from "../../../shared/models/backflow/backflow-gauge";
import { BackflowGaugeService } from "../../../shared/services/backflow/backflow-gauge.service";
import { ToastService, ToastType, CellTemplateData, ColumnType, InputOption, ModalHelperService, TableColumn } from '@envirotrax/common-ui';
import { TableViewModel } from "../../../shared/models/table-view-model";
import { ProfesisonalService } from "../../../shared/services/professionals/professional.service";
import { NgForm } from "@angular/forms";
import { HelperService } from "../../../shared/services/helpers/helper.service";
import { EditGaugeComponent } from "./edit/edit-gauge.component";
import { ModalSize } from "@developer-partners/ngx-modal-dialog";
import { MAX_PAGE_SIZE, STANDALONE_GRID_PAGE_SIZE } from "../../../shared/models/page-info";
import { ProfesionalUserService } from "../../../shared/services/professionals/professional-user.service";

const COMPANY_OPTION_ID = 0;

@Component({
    standalone: false,
    templateUrl: './gauge-list.component.html'
})
export class GaugeListComponent implements OnInit {
    public table: TableViewModel<BackflowGauge> = {
        columns: [],
        query: {
            sort: {},
            filter: []
        },
        freeTextSearch: {
            searchQuery: [
                { field: 'manufacturer' },
                { field: 'model' },
                { field: 'serialNumber' }
            ]
        }
    };

    public readonly gaugeExpirationType = GaugeExpirationType;

    public isAdmin = false;
    public companyName = '';
    public assignedToOptions: InputOption[] = [];
    public assignedUserId = COMPANY_OPTION_ID;

    public newGauge?: BackflowGauge;
    public tfaFile: File | null = null;
    public hasReadHelp: boolean = false;
    public newGaugeValidationErrors: string[] = [];
    public isNewGaugeLoading: boolean = false;
    public isFileLoading: boolean = false;

    @ViewChild('dateCell', { static: true })
    private dateCellTemplate!: TemplateRef<CellTemplateData<BackflowGauge>>;

    @ViewChild('typeCell', { static: true })
    private typeCellTemplate!: TemplateRef<CellTemplateData<BackflowGauge>>;

    @ViewChild('assignedToCell', { static: true })
    private assignedToCellTemplate!: TemplateRef<CellTemplateData<BackflowGauge>>;

    constructor(
        private readonly _gaugeService: BackflowGaugeService,
        private readonly _modalHelper: ModalHelperService,
        private readonly _toastService: ToastService,
        private readonly _professionalService: ProfesisonalService,
        private readonly _userService: ProfesionalUserService,
        private readonly _helperService: HelperService
    ) { }

    public async ngOnInit(): Promise<void> {
        this.table.isLoading = true;

        const [myUser, professional] = await Promise.all([
            this._userService.getMyData(),
            this._professionalService.getLoggedInProfessional()
        ]);

        this.isAdmin = !!myUser.isAdmin;
        this.companyName = professional.name ?? '';
        this.table.columns = this.getColumns();

        this.getGauges();
        this.resetNewGauge();

        if (this.isAdmin) {
            await this.loadAssignedToOptions();
        }
    }

    // V1 parity: an admin registers a gauge for the company or for one tester; a tester only for themselves.
    private async loadAssignedToOptions(): Promise<void> {
        try {
            this.isNewGaugeLoading = true;

            const testersPage = await this._userService.getAll(
                { pageSize: MAX_PAGE_SIZE },
                { sort: {}, filter: [{ columnName: 'isBackflowTester', comparisonOperator: 'Eq', value: 'true' }] }
            );

            this.assignedToOptions = [
                { id: COMPANY_OPTION_ID, text: this.companyName },
                ...(testersPage.data ?? []).map(tester => ({ id: tester.id, text: tester.contactName ?? tester.emailAddress ?? '' }))
            ];
        } finally {
            this.isNewGaugeLoading = false;
        }
    }

    private async newGaugeObject(): Promise<BackflowGauge> {
        const pro = await this._professionalService.getLoggedInProfessional();

        return {
            professionalId: pro.id
        };
    }

    private async resetNewGauge(): Promise<void> {
        this.newGauge = await this.newGaugeObject();
        this.assignedUserId = COMPANY_OPTION_ID;
        this.tfaFile = null;
        this.hasReadHelp = false;
    }

    private getColumns(): TableColumn<BackflowGauge>[] {
        const assignedToColumn: TableColumn<BackflowGauge>[] = this.isAdmin
            ? [{ field: 'professionalUser.contactName', caption: 'Assigned To', cellTemplate: this.assignedToCellTemplate, type: ColumnType.text }]
            : [];

        return [
            ...assignedToColumn,
            {
                field: 'manufacturer',
                caption: 'Manufacturer',
                type: ColumnType.text
            },
            {
                field: 'model',
                caption: 'Model',
                type: ColumnType.text
            },
            {
                field: 'serialNumber',
                caption: 'Serial Number',
                type: ColumnType.text
            },
            {
                field: 'isPortable',
                caption: 'Type',
                cellTemplate: this.typeCellTemplate,
                type: ColumnType.text
            },
            {
                field: 'lastCalibrationDate',
                caption: 'Test Date',
                cellTemplate: this.dateCellTemplate,
                type: ColumnType.date
            }
        ];
    }

    public async viewFile(gauge: BackflowGauge): Promise<void> {
        if (!gauge.filePath) {
            this._toastService.show({ text: 'No file has been uploaded for this gauge.', type: ToastType.Warning });
            return;
        }

        try {
            this.isFileLoading = true;
            const url = await this._gaugeService.getFileUrl(gauge.id!);
            window.open(url, '_blank');
        } finally {
            this.isFileLoading = false;
        }
    }

    public async getGauges(): Promise<void> {
        try {
            this.table.isLoading = true;
            this.table.items = await this._gaugeService.getAll(
                this.table.items?.pageInfo || { pageSize: STANDALONE_GRID_PAGE_SIZE },
                this.table.query
            );
        } finally {
            this.table.isLoading = false;
        }
    }

    public edit(gauge: BackflowGauge): void {
        this._modalHelper.show<BackflowGauge>(EditGaugeComponent, {
            title: 'Edit Gauge',
            model: gauge,
            size: ModalSize.large
        }).result().subscribe(() => this.getGauges());
    }

    public delete(gauge: BackflowGauge): void {
        this._modalHelper.showDeleteConfirmation()
            .result()
            .subscribe(async () => {
                try {
                    this.table.isLoading = true;
                    await this._gaugeService.delete(gauge.id!);
                    this._toastService.successFullyDeleted('Gauge');
                } finally {
                    this.table.isLoading = false;
                }

                await this.getGauges();
            });
    }

    public async saveGauge(form: NgForm): Promise<void> {
        if (form.valid) {
            try {
                this.isNewGaugeLoading = true;
                this.newGaugeValidationErrors = [];

                this.newGauge!.professionalUser = this.assignedUserId !== COMPANY_OPTION_ID ? { id: this.assignedUserId } : null;

                await this._gaugeService.add(this.newGauge!, this.tfaFile!);

                this._toastService.successfullySaved('Gauge');

                this.resetNewGauge();
                this.getGauges();
                form.resetForm();
            } catch (error) {
                if (!this._helperService.parseValidationErrors(error, this.newGaugeValidationErrors)) {
                    throw error;
                }

                this._toastService.failedToSave('Gauge');
            } finally {
                this.isNewGaugeLoading = false;
            }
        }
    }
}
