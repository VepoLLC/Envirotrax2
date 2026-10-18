import { CommonModule } from '@angular/common';
import { Component, Input, OnInit, TemplateRef, ViewChild } from '@angular/core';
import { ModalSize } from '@developer-partners/ngx-modal-dialog';
import {
    CellTemplateData,
    ColumnType,
    ModalHelperService,
    TableColumn,
    TableCustomAction,
    TableViewModel,
    ToastService,
    ToastType
} from '@envirotrax/common-ui';
import { SharedComponentsModule } from '../../../../../shared/components/shared.components.module';
import { BackflowGauge } from '../../../../../shared/models/backflow/backflow-gauge';
import {
    expirationTypeCssClasses,
    unvalidatedExpirationCssClass
} from '../../../../../shared/models/professionals/licenses/professional-user-license';
import { BackflowTesterGaugeService } from '../../../../../shared/services/backflow/backflow-tester-gauge.service';
import {
    AddEditBackflowTesterGaugeComponent,
    BackflowTesterGaugeModalData
} from '../edit/add-edit-backflow-tester-gauge.component';

interface BackflowTesterGaugeVm extends BackflowGauge {
    waterSystemType: string;
    expirationCssClass: string;
}

@Component({
    selector: 'vp-backflow-tester-gauges',
    templateUrl: './backflow-tester-gauge-list.component.html',
    imports: [
        CommonModule,
        SharedComponentsModule
    ],
})
export class BackflowTesterGaugeListComponent implements OnInit {
    @Input()
    public professionalId!: number;

    @ViewChild('waterSystemCell', { static: true })
    public waterSystemCell?: TemplateRef<CellTemplateData<BackflowTesterGaugeVm>>;

    @ViewChild('calibrationCell', { static: true })
    public calibrationCell?: TemplateRef<CellTemplateData<BackflowTesterGaugeVm>>;

    public table: TableViewModel<BackflowTesterGaugeVm> = {
        query: {
            sort: { manufacturer: 'Asc' },
            filter: []
        }
    };

    public readonly customActions: TableCustomAction<BackflowTesterGaugeVm>[] = [
        {
            text: 'View',
            iconClass: 'fa-solid fa-eye',
            action: gauge => this.viewCertificate(gauge)
        }
    ];

    constructor(
        private readonly _gaugeService: BackflowTesterGaugeService,
        private readonly _modalHelper: ModalHelperService,
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
                this.professionalId,
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

    public addGauge(): void {
        this.showEditor('Add Gauge', {});
    }

    public editGauge(row: BackflowTesterGaugeVm): void {
        const { waterSystemType, expirationCssClass, ...gauge } = row;

        this.showEditor('Edit Gauge', gauge);
    }

    public deleteGauge(gauge: BackflowGauge): void {
        this._modalHelper.showDeleteConfirmation().result().subscribe(async () => {
            try {
                this.table.isLoading = true;

                await this._gaugeService.delete(this.professionalId, gauge.id!);

                this._toastService.successFullyDeleted('Gauge');
            } catch (error) {
                this._toastService.show({ text: 'Failed to delete Gauge.', type: ToastType.Error });

                return;
            } finally {
                this.table.isLoading = false;
            }

            await this.getGauges();
        });
    }

    public async viewCertificate(gauge: BackflowGauge): Promise<void> {
        // The tab has to be opened synchronously inside the click, otherwise the await moves the
        // window.open out of the user-gesture task and pop-up blockers suppress it.
        const certificateWindow = window.open('', '_blank');

        try {
            this.table.isLoading = true;

            const url = await this._gaugeService.getFileUrl(this.professionalId, gauge.id!);

            if (certificateWindow) {
                certificateWindow.location.href = url;
            }
        } catch (error) {
            certificateWindow?.close();

            this._toastService.show({ text: 'Could not open the calibration certificate.', type: ToastType.Error });
        } finally {
            this.table.isLoading = false;
        }
    }

    private showEditor(title: string, gauge: BackflowGauge): void {
        this._modalHelper.show<BackflowTesterGaugeModalData, BackflowGauge>(AddEditBackflowTesterGaugeComponent, {
            title,
            model: { professionalId: this.professionalId, gauge },
            size: ModalSize.medium,
            mode: 'disableFullScreen'
        }).result().subscribe(() => this.getGauges());
    }

    private toViewModel(gauge: BackflowGauge): BackflowTesterGaugeVm {
        return {
            ...gauge,
            waterSystemType: gauge.isPortable ? 'Potable' : 'Non-Potable',
            expirationCssClass: gauge.lastCalibrationDate && gauge.expirationType != null
                ? expirationTypeCssClasses[gauge.expirationType]
                : unvalidatedExpirationCssClass
        };
    }

    private getColumns(): TableColumn<BackflowTesterGaugeVm>[] {
        return [
            { field: 'manufacturer', caption: 'Manufacturer', type: ColumnType.text },
            { field: 'model', caption: 'Model', type: ColumnType.text },
            { field: 'isPortable', caption: 'Type', type: ColumnType.other, cellTemplate: this.waterSystemCell },
            { field: 'serialNumber', caption: 'Serial Number', type: ColumnType.text },
            { field: 'lastCalibrationDate', caption: 'Calibration Date', type: ColumnType.date, cellTemplate: this.calibrationCell }
        ];
    }
}
