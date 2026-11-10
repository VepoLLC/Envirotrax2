import { CommonModule } from '@angular/common';
import { Component, OnInit, TemplateRef, ViewChild } from '@angular/core';
import { FormsModule } from '@angular/forms';
import {
    CellTemplateData,
    ColumnType,
    DownloadService,
    HelperService,
    InputOption,
    TableColumn,
    TableViewModel,
    ToastService
} from '@envirotrax/common-ui';
import { SharedComponentsModule } from '../../../shared/components/shared.components.module';
import { BackflowGauge } from '../../../shared/models/backflow/backflow-gauge';
import {
    ExpirationType,
    expirationTypeCssClasses,
    unvalidatedExpirationCssClass
} from '../../../shared/models/professionals/licenses/professional-user-license';
import { BackflowGaugeService } from '../../../shared/services/backflow/backflow-gauge.service';

const POTABLE = 'potable';
const NON_POTABLE = 'nonPotable';

interface GaugeVm extends BackflowGauge {
    waterSystemType: string;
    expirationCssClass: string;
    isSelected: boolean;
    isSaved: boolean;
}

@Component({
    selector: 'vp-gauge-list',
    templateUrl: './gauge-list.component.html',
    imports: [
        CommonModule,
        FormsModule,
        SharedComponentsModule
    ],
})
export class GaugeListComponent implements OnInit {
    @ViewChild('manufacturerCell', { static: true })
    public manufacturerCell?: TemplateRef<CellTemplateData<GaugeVm>>;

    @ViewChild('modelCell', { static: true })
    public modelCell?: TemplateRef<CellTemplateData<GaugeVm>>;

    @ViewChild('waterSystemCell', { static: true })
    public waterSystemCell?: TemplateRef<CellTemplateData<GaugeVm>>;

    @ViewChild('serialNumberCell', { static: true })
    public serialNumberCell?: TemplateRef<CellTemplateData<GaugeVm>>;

    @ViewChild('calibrationCell', { static: true })
    public calibrationCell?: TemplateRef<CellTemplateData<GaugeVm>>;

    @ViewChild('actionsCell', { static: true })
    public actionsCell?: TemplateRef<CellTemplateData<GaugeVm>>;

    public readonly waterSystemOptions: InputOption[] = [
        { id: POTABLE, text: 'Potable' },
        { id: NON_POTABLE, text: 'Non-Potable' }
    ];

    public table: TableViewModel<GaugeVm> = {
        query: {
            sort: {},
            filter: []
        }
    };

    constructor(
        private readonly _gaugeService: BackflowGaugeService,
        private readonly _downloadService: DownloadService,
        private readonly _helper: HelperService,
        private readonly _toastService: ToastService
    ) {

    }

    public async ngOnInit(): Promise<void> {
        this.table.columns = this.getColumns();

        await this.getGauges();
    }

    public async getGauges(): Promise<void> {
        try {
            this.table.isLoading = true;

            const gauges = await this._gaugeService.getAll(
                this.table.items?.pageInfo || {},
                this.table.query
            );

            this.table.items = {
                pageInfo: gauges.pageInfo,
                data: gauges.data.map(gauge => this.toViewModel(gauge))
            };
        } finally {
            this.table.isLoading = false;
        }
    }

    public selectGauge(selected: GaugeVm): void {
        if (!this.table.items || !this.table.items.data.includes(selected)) {
            return;
        }

        for (const gauge of this.table.items.data) {
            gauge.isSelected = gauge === selected;
        }
    }

    public async saveGauge(gauge: GaugeVm): Promise<void> {
        try {
            this.table.isLoading = true;

            const saved = await this._gaugeService.update(this.toBackflowGauge(gauge));

            gauge.lastCalibrationDate = saved.lastCalibrationDate;
            gauge.expirationCssClass = this.getExpirationCssClass(saved.lastCalibrationDate, saved.expirationType);
            gauge.isSaved = true;

            this._toastService.successfullySaved('Gauge');
        } finally {
            this.table.isLoading = false;
        }
    }

    public async viewCertificate(gauge: GaugeVm): Promise<void> {
        try {
            this.table.isLoading = true;

            const url = await this._gaugeService.getFileUrl(gauge.id!);

            this._downloadService.downloadFileFromUrl(url);
        } finally {
            this.table.isLoading = false;
        }
    }

    private toBackflowGauge(gauge: GaugeVm): BackflowGauge {
        return {
            id: gauge.id,
            manufacturer: gauge.manufacturer,
            model: gauge.model,
            serialNumber: gauge.serialNumber,
            isPortable: gauge.waterSystemType === POTABLE,
            lastCalibrationDate: this._helper.toTextOrUndefined(gauge.lastCalibrationDate)
        };
    }

    private toViewModel(gauge: BackflowGauge): GaugeVm {
        let waterSystemType = NON_POTABLE;

        if (gauge.isPortable) {
            waterSystemType = POTABLE;
        }

        return {
            ...gauge,
            waterSystemType,
            expirationCssClass: this.getExpirationCssClass(gauge.lastCalibrationDate, gauge.expirationType),
            isSelected: false,
            isSaved: false
        };
    }

    private getExpirationCssClass(lastCalibrationDate?: string, expirationType?: ExpirationType): string {
        if (!lastCalibrationDate || expirationType == null) {
            return unvalidatedExpirationCssClass;
        }

        return expirationTypeCssClasses[expirationType];
    }

    private getColumns(): TableColumn<GaugeVm>[] {
        return [
            { field: 'manufacturer', caption: 'Manufacturer', type: ColumnType.text, cellTemplate: this.manufacturerCell },
            { field: 'model', caption: 'Model', type: ColumnType.text, cellTemplate: this.modelCell },
            { field: 'isPortable', caption: 'Type', type: ColumnType.other, cellTemplate: this.waterSystemCell },
            { field: 'serialNumber', caption: 'Serial Number', type: ColumnType.text, cellTemplate: this.serialNumberCell },
            { field: 'lastCalibrationDate', caption: 'Last Cal Date', type: ColumnType.date, cellTemplate: this.calibrationCell },
            { field: 'id', caption: '', type: ColumnType.other, cellTemplate: this.actionsCell, queryColumnExcluded: true }
        ];
    }
}
