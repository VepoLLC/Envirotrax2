import { Component, Input, OnInit, TemplateRef, ViewChild } from "@angular/core";
import { BackflowGauge, GaugeExpirationType } from "../../../../../shared/models/backflow/backflow-gauge";
import { BackflowTesterGaugeService } from "../../../../../shared/services/backflow/backflow-tester-gauge.service";
import { TableViewModel } from "../../../../../shared/models/table-view-model";
import { CellTemplateData, TableColumn } from "../../../../../shared/components/data-components/table/table.component";
import { ColumnType } from "../../../../../shared/components/data-components/sorting-filtering/query-view-model";

@Component({
    selector: 'vp-backflow-tester-gauge',
    standalone: false,
    templateUrl: './backflow-tester-gauge.component.html'
})
export class BackflowTesterGaugeComponent implements OnInit {
    @Input() public testerId!: number;

    public readonly gaugeExpirationType = GaugeExpirationType;

    public table: TableViewModel<BackflowGauge> = {
        columns: [],
        query: { sort: {}, filter: [] }
    };

    @ViewChild('dateCell', { static: true })
    private dateCellTemplate!: TemplateRef<CellTemplateData<BackflowGauge>>;

    @ViewChild('typeCell', { static: true })
    private typeCellTemplate!: TemplateRef<CellTemplateData<BackflowGauge>>;

    constructor(private readonly _gaugeService: BackflowTesterGaugeService) { }

    public async ngOnInit(): Promise<void> {
        this.table.columns = this.getColumns();
        await this.loadGauge();
    }

    private getColumns(): TableColumn<BackflowGauge>[] {
        return [
            {
                field: 'manufacturer',
                caption: 'Mfr.',
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
                caption: 'Serial #',
                type: ColumnType.text
            },
            {
                field: 'lastCalibrationDate',
                caption: 'Cal. Date',
                cellTemplate: this.dateCellTemplate,
                type: ColumnType.date
            }
        ];
    }

    public async loadGauge(): Promise<void> {
        try {
            this.table.isLoading = true;
            this.table.items = await this._gaugeService.getGauges(
                this.testerId,
                this.table.items?.pageInfo || {},
                this.table.query
            );
        } finally {
            this.table.isLoading = false;
        }
    }
}
