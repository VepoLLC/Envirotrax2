import { Component, OnInit, TemplateRef, ViewChild } from "@angular/core";
import { BackflowGauge } from "../../../shared/models/backflow/backflow-gauge";
import { BackflowGaugeService } from "../../../shared/services/backflow/backflow-gauge.service";
import { ModalHelperService } from "../../../shared/services/helpers/modal-helper.service";
import { ToastService, ToastType } from "../../../shared/services/toast.service";
import { TableViewModel } from "../../../shared/models/table-view-model";
import { CellTemplateData, TableColumn } from "../../../shared/components/data-components/table/table.component";
import { ColumnType } from "../../../shared/components/data-components/sorting-filtering/query-view-model";
import { ProfesisonalService } from "../../../shared/services/professionals/professional.service";
import { NgForm } from "@angular/forms";
import { HelperService } from "../../../shared/services/helpers/helper.service";
import { EditGaugeComponent } from "./edit/edit-gauge.component";
import { ModalSize } from "@developer-partners/ngx-modal-dialog";

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

    public newGauge?: BackflowGauge;
    public tfaFile: File | null = null;
    public hasReadHelp: boolean = false;
    public newGaugeValidationErrors: string[] = [];
    public isNewGaugeLoading: boolean = false;

    @ViewChild('dateCell', { static: true })
    private dateCellTemplate!: TemplateRef<CellTemplateData<BackflowGauge>>;

    @ViewChild('typeCell', { static: true })
    private typeCellTemplate!: TemplateRef<CellTemplateData<BackflowGauge>>;

    constructor(
        private readonly _gaugeService: BackflowGaugeService,
        private readonly _modalHelper: ModalHelperService,
        private readonly _toastService: ToastService,
        private readonly _professionalService: ProfesisonalService,
        private readonly _helperService: HelperService
    ) { }

    public async ngOnInit(): Promise<void> {
        this.table.columns = this.getColumns();

        this.getGauges();
        this.resetNewGauge();
    }

    private async newGaugeObject(): Promise<BackflowGauge> {
        const pro = await this._professionalService.getLoggedInProfessional();

        return {
            professionalId: pro.id
        };
    }

    private async resetNewGauge(): Promise<void> {
        this.newGauge = await this.newGaugeObject();
        this.tfaFile = null;
        this.hasReadHelp = false;
    }

    private getColumns(): TableColumn<BackflowGauge>[] {
        return [
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
                field: 'isPortable',
                caption: 'Type',
                cellTemplate: this.typeCellTemplate,
                type: ColumnType.text
            },
            {
                field: 'serialNumber',
                caption: 'Serial number',
                type: ColumnType.text
            },
            {
                field: 'lastCalibrationDate',
                caption: 'Test date',
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

        const url = await this._gaugeService.getFileUrl(gauge.id!);
        window.open(url, '_blank');
    }

    public async getGauges(): Promise<void> {
        try {
            this.table.isLoading = true;
            this.table.items = await this._gaugeService.getAll(
                this.table.items?.pageInfo || {},
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

                await this._gaugeService.add(this.newGauge!, this.tfaFile);

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
