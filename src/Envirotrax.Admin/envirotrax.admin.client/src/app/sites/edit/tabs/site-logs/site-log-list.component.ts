import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, OnInit, Output, TemplateRef, ViewChild } from '@angular/core';
import { CellTemplateData, ColumnType, ModalHelperService, TableColumn, TableViewModel, ToastService } from '@envirotrax/common-ui';
import { SharedComponentsModule } from '../../../../shared/components/shared.components.module';
import { SiteLog, SiteLogEditModel, SiteLogRow, SiteLogType } from '../../../../shared/models/sites/site-log';
import { SiteLogService } from '../../../../shared/services/sites/site-log.service';
import { SiteLogEditComponent } from './site-log-edit.component';

@Component({
    selector: 'app-site-log-list',
    templateUrl: './site-log-list.component.html',
    imports: [CommonModule, SharedComponentsModule],
})
export class SiteLogListComponent implements OnInit {
    @Input()
    public siteId: number = 0;

    @Input()
    public waterSupplierId: number = 0;

    @Output()
    public logsChanged: EventEmitter<void> = new EventEmitter();

    @ViewChild('logTypeCell', { static: true })
    public logTypeCell?: TemplateRef<CellTemplateData<SiteLogRow>>;

    @ViewChild('logDateCell', { static: true })
    public logDateCell?: TemplateRef<CellTemplateData<SiteLogRow>>;

    @ViewChild('descriptionCell', { static: true })
    public descriptionCell?: TemplateRef<CellTemplateData<SiteLogRow>>;

    @ViewChild('reviewDateCell', { static: true })
    public reviewDateCell?: TemplateRef<CellTemplateData<SiteLogRow>>;

    public readonly siteLogType = SiteLogType;

    public table: TableViewModel<SiteLogRow> = {
        query: {
            sort: {},
            filter: []
        }
    };

    constructor(
        private readonly _siteLogService: SiteLogService,
        private readonly _modalHelper: ModalHelperService,
        private readonly _toastService: ToastService
    ) {

    }

    public async ngOnInit(): Promise<void> {
        this.table.columns = this.getColumns();

        await this.load();
    }

    public async load(): Promise<void> {
        if (this.table.items) {
            this.table.items.pageInfo.pageNumber = 1;
        }

        await this.getLogs();
    }

    public async getLogs(): Promise<void> {
        try {
            this.table.isLoading = true;

            const logs = await this._siteLogService.getAll(this.siteId, this.table.items?.pageInfo || {}, this.table.query);

            const rows: SiteLogRow[] = [];

            for (const log of logs.data) {
                rows.push({
                    ...log,
                    assemblyDescription: this.buildAssemblyDescription(log)
                });
            }

            this.table.items = {
                pageInfo: logs.pageInfo,
                data: rows
            };
        } finally {
            this.table.isLoading = false;
        }
    }

    public addLog(): void {
        this.openEditModal({
            siteId: this.siteId,
            waterSupplierId: this.waterSupplierId
        });
    }

    public editLog(log: SiteLogRow): void {
        this.openEditModal({
            siteId: this.siteId,
            waterSupplierId: this.waterSupplierId,
            id: log.id
        });
    }

    public deleteLog(log: SiteLogRow): void {
        this._modalHelper
            .confirm({
                title: 'Confirm Delete',
                messages: ['Are you sure you want to delete this log?']
            })
            .result()
            .subscribe(() => this.processDelete(log));
    }

    private openEditModal(model: SiteLogEditModel): void {
        this._modalHelper
            .show<SiteLogEditModel, SiteLog>(SiteLogEditComponent, {
                title: 'Site Log Edit',
                model
            })
            .result()
            .subscribe(() => this.reloadAfterChange());
    }

    private async processDelete(log: SiteLogRow): Promise<void> {
        if (log.id == null) {
            return;
        }

        try {
            this.table.isLoading = true;

            await this._siteLogService.delete(this.siteId, log.id);
        } finally {
            this.table.isLoading = false;
        }

        this._toastService.successFullyDeleted('Site Log');

        await this.reloadAfterChange();
    }

    private async reloadAfterChange(): Promise<void> {
        await this.getLogs();

        this.logsChanged.emit();
    }

    private buildAssemblyDescription(log: SiteLog): string {
        const assembly = log.assembly;

        if (assembly?.id == null) {
            return '';
        }

        const description = `${assembly.manufacturer ?? ''} ${assembly.model ?? ''} ${assembly.size ?? ''} ${assembly.deviceType ?? ''}`.trim();

        return `${description} - SN#: ${assembly.serialNumber ?? ''}`;
    }

    private getColumns(): TableColumn<SiteLogRow>[] {
        return [
            { field: 'logType', caption: '', type: ColumnType.other, cellTemplate: this.logTypeCell, queryColumnExcluded: true },
            { field: 'createdTime', caption: 'Log Date', type: ColumnType.other, cellTemplate: this.logDateCell, queryColumnExcluded: true },
            { field: 'noteText', caption: 'Description', type: ColumnType.other, cellTemplate: this.descriptionCell, queryColumnExcluded: true },
            { field: 'reviewDate', caption: 'Review Date', type: ColumnType.other, cellTemplate: this.reviewDateCell, queryColumnExcluded: true }
        ];
    }
}
