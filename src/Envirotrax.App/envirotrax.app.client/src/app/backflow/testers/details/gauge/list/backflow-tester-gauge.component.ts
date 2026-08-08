import { Component, Input, OnInit, TemplateRef, ViewChild } from "@angular/core";
import { BackflowGauge, GaugeExpirationType } from "../../../../../shared/models/backflow/backflow-gauge";
import { BackflowTesterGaugeService } from "../../../../../shared/services/backflow/backflow-tester-gauge.service";
import { TableViewModel } from "../../../../../shared/models/table-view-model";
import { AuthService } from "../../../../../shared/services/auth/auth.service";
import { PermissionAction, PermissionType } from "../../../../../shared/models/permission-type";
import { ToastService, CellTemplateData, ColumnType, ModalHelperService, TableColumn, TableCustomAction } from '@envirotrax/common-ui';
import { ModalSize } from "@developer-partners/ngx-modal-dialog";
import { AddEditBackflowTesterGaugeComponent, BackflowGaugeModalData } from "../edit/add-edit-backflow-tester-gauge.component";
import { Professional } from "../../../../../shared/models/professionals/professional";
import { DownloadService } from "../../../../../shared/services/download.service";

@Component({
    selector: 'vp-backflow-tester-gauge',
    standalone: false,
    templateUrl: './backflow-tester-gauge.component.html'
})
export class BackflowTesterGaugeComponent implements OnInit {
    @Input() public testerId!: number;
    @Input() public tester: Professional | null = null;

    public readonly gaugeExpirationType = GaugeExpirationType;
    public canManage: boolean = false;
    public gaugeCustomActions: TableCustomAction<BackflowGauge>[] = [];

    public table: TableViewModel<BackflowGauge> = {
        columns: [],
        query: { sort: {}, filter: [] }
    };

    @ViewChild('dateCell', { static: true })
    private dateCellTemplate!: TemplateRef<CellTemplateData<BackflowGauge>>;

    @ViewChild('typeCell', { static: true })
    private typeCellTemplate!: TemplateRef<CellTemplateData<BackflowGauge>>;

    constructor(
        private readonly _gaugeService: BackflowTesterGaugeService,
        private readonly _authService: AuthService,
        private readonly _modalHelper: ModalHelperService,
        private readonly _toastService: ToastService,
        private readonly _downloadService: DownloadService
    ) { }

    public async ngOnInit(): Promise<void> {
        await this.setPermissions();
        this.table.columns = this.getColumns();
        await this.loadGauge();
    }

    private async setPermissions(): Promise<void> {
        this.canManage = await this._authService.hasAnyPermisison(PermissionAction.CanModify, PermissionType.BackflowTesters);

        if (this.canManage) {
            this.gaugeCustomActions = [
                {
                    text: 'View',
                    iconClass: 'fa-solid fa-eye',
                    action: (gauge: BackflowGauge) => this.viewGaugeFile(gauge)
                },
                {
                    text: 'Email',
                    iconClass: 'fa-solid fa-envelope',
                    action: (gauge: BackflowGauge) => this.prepareEmail(gauge)
                }
            ];
        }
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

    public addGauge(): void {
        this._modalHelper.show<BackflowGaugeModalData, BackflowGauge>(AddEditBackflowTesterGaugeComponent, {
            title: 'Add Gauge',
            model: { testerId: this.testerId, gauge: {} },
            size: ModalSize.large
        }).result().subscribe(() => this.loadGauge());
    }

    public editGauge(gauge: BackflowGauge): void {
        this._modalHelper.show<BackflowGaugeModalData, BackflowGauge>(AddEditBackflowTesterGaugeComponent, {
            title: 'Edit Gauge',
            model: { testerId: this.testerId, gauge },
            size: ModalSize.large
        }).result().subscribe(() => this.loadGauge());
    }

    public deleteGauge(gauge: BackflowGauge): void {
        this._modalHelper.showDeleteConfirmation().result().subscribe(async () => {
            try {
                this.table.isLoading = true;
                await this._gaugeService.delete(this.testerId, gauge.id!);
                this._toastService.successFullyDeleted('Gauge');
            } finally {
                this.table.isLoading = false;
            }
            await this.loadGauge();
        });
    }

    public async viewGaugeFile(gauge: BackflowGauge): Promise<void> {
        try {
            this.table.isLoading = true;
            const url = await this._gaugeService.getFileUrl(this.testerId, gauge.id!);
            this._downloadService.downloadFileFromUrl(url);
        } finally {
            this.table.isLoading = false;
        }
    }

    public async prepareEmail(gauge: BackflowGauge): Promise<void> {
        if (!this.tester) {
            return;
        }

        const adminEmail = await this._authService.getUserEmail();
        const body = `${this.tester.name},%0D%0A%0D%0A` +
            `We have the Gauge: ${gauge.serialNumber} updated in your account. ` +
            `Please let us know if you need anything else.%0D%0A%0D%0A` +
            `Sincerely,%0D%0A${adminEmail} - Envirotrax`;
        const link = `mailto:${this.tester.companyEmail}` +
            `?subject=${encodeURIComponent('Envirotrax - Gauge Calibration Certificate')}` +
            `&body=${body}`;

        window.open(link);
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
