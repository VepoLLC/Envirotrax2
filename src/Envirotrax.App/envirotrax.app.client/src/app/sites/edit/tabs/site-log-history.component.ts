import { Component, Input, OnChanges, OnInit, SimpleChanges, TemplateRef, ViewChild } from '@angular/core';
import { SiteLog } from '../../../shared/models/sites/site-log';
import { SiteLogType } from '../../../shared/models/sites/site-log-type.enum';
import { SiteLogService } from '../../../shared/services/sites/site-log.service';
import { TableViewModel } from '../../../shared/models/table-view-model';
import { ComparisonOperator } from '../../../shared/models/query';
import { ToastService } from '../../../shared/services/toast.service';
import { AuthService } from '../../../shared/services/auth/auth.service';
import { PermissionAction, PermissionType } from '../../../shared/models/permission-type';
import { CellTemplateData, ColumnType, ModalHelperService, TableColumn } from '@envirotrax/common-ui';
import { ModalSize } from '@developer-partners/ngx-modal-dialog';
import { SiteLogEditComponent, SiteLogEditModel } from './site-log-edit.component';

@Component({
    selector: 'app-site-log-history',
    standalone: false,
    templateUrl: './site-log-history.component.html'
})
export class SiteLogHistoryComponent implements OnInit, OnChanges {
    @Input() public siteId?: number;

    @ViewChild('logTypeTemplate', { static: true })
    public logTypeTemplate!: TemplateRef<CellTemplateData<SiteLog>>;

    @ViewChild('descriptionTemplate', { static: true })
    public descriptionTemplate!: TemplateRef<CellTemplateData<SiteLog>>;

    @ViewChild('reviewDateTemplate', { static: true })
    public reviewDateTemplate!: TemplateRef<CellTemplateData<SiteLog>>;

    @ViewChild('actionsTemplate', { static: true })
    public actionsTemplate!: TemplateRef<CellTemplateData<SiteLog>>;

    public readonly SiteLogType = SiteLogType;
    public canModify: boolean = false;

    public reviewDateClasses = new Map<number, string>();

    public table: TableViewModel<SiteLog> = {
        columns: [],
        query: {
            sort: { id: 'Desc' },
            filter: []
        }
    };

    constructor(
        private readonly _siteLogService: SiteLogService,
        private readonly _toastService: ToastService,
        private readonly _authService: AuthService,
        private readonly _modalHelper: ModalHelperService
    ) {}

    public ngOnChanges(changes: SimpleChanges): void {
        if (changes['siteId'] && !changes['siteId'].firstChange && this.siteId) {
            this.loadLogs();
        }
    }

    public async ngOnInit(): Promise<void> {
        this.canModify = await this._authService.hasAnyPermisison(PermissionAction.CanModify, PermissionType.Sites);
        this.table.columns = this.getColumns();
        await this.loadLogs();
    }

    public async loadLogs(): Promise<void> {
        if (!this.siteId) return;
        try {
            this.table.isLoading = true;
            this.table.query.filter = [
                { columnName: 'site.id', value: this.siteId!.toString(), comparisonOperator: 'Eq' as ComparisonOperator }
            ];
            this.table.items = await this._siteLogService.getAll(
                this.siteId,
                this.table.items?.pageInfo || {},
                this.table.query
            );
            this.computeReviewDateClasses(this.table.items.data);
        } finally {
            this.table.isLoading = false;
        }
    }

    public openAddModal(): void {
        this._modalHelper.show<SiteLogEditModel, SiteLog>(SiteLogEditComponent, {
            title: 'Add Log Record',
            model: { siteId: this.siteId!, log: { logType: SiteLogType.Note } },
            size: ModalSize.large
        }).result().subscribe(() => this.loadLogs());
    }

    public openEditModal(log: SiteLog): void {
        this._modalHelper.show<SiteLogEditModel, SiteLog>(SiteLogEditComponent, {
            title: 'Edit Log Record',
            model: { siteId: this.siteId!, log },
            size: ModalSize.large
        }).result().subscribe(() => this.loadLogs());
    }

    public deleteLog(log: SiteLog): void {
        this._modalHelper.showDeleteConfirmation().result().subscribe(async () => {
            try {
                this.table.isLoading = true;
                await this._siteLogService.delete(this.siteId!, log.id!);
                this._toastService.successfullySaved('Log Record');
                await this.loadLogs();
            } finally {
                this.table.isLoading = false;
            }
        });
    }

    private computeReviewDateClasses(logs: SiteLog[]): void {
        this.reviewDateClasses.clear();
        for (const log of logs) {
            if (!log.reviewDate || !log.id) continue;
            if (log.logType === SiteLogType.CompletedReminder) {
                this.reviewDateClasses.set(log.id, 'badge bg-secondary');
                continue;
            }
            const reviewDate = new Date(log.reviewDate);
            const now = new Date();
            const thirtyDaysFromNow = new Date();
            thirtyDaysFromNow.setDate(now.getDate() + 30);
            if (reviewDate < now) {
                this.reviewDateClasses.set(log.id, 'badge bg-danger');
            } else if (reviewDate <= thirtyDaysFromNow) {
                this.reviewDateClasses.set(log.id, 'badge bg-warning text-dark');
            } else {
                this.reviewDateClasses.set(log.id, 'badge bg-success');
            }
        }
    }

    private getColumns(): TableColumn<SiteLog>[] {
        return [
            {
                field: '',
                caption: '',
                type: ColumnType.other,
                queryColumnExcluded: true,
                cellTemplate: this.logTypeTemplate
            },
            { field: 'createdTime', caption: 'Log Date', type: ColumnType.date },
            { field: 'createdBy.email', caption: 'User', type: ColumnType.text },
            {
                field: '',
                caption: 'Description',
                type: ColumnType.other,
                queryColumnExcluded: true,
                cellTemplate: this.descriptionTemplate
            },
            {
                field: '',
                caption: 'Review Date',
                type: ColumnType.other,
                queryColumnExcluded: true,
                cellTemplate: this.reviewDateTemplate
            },
            {
                field: '',
                caption: '',
                type: ColumnType.other,
                queryColumnExcluded: true,
                cellTemplate: this.actionsTemplate
            }
        ];
    }
}
