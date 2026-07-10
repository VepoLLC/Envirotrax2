import { Component, Input, OnChanges, OnInit, SimpleChanges, TemplateRef, ViewChild } from '@angular/core';
import { SiteLog } from '../../../../shared/models/sites/site-log';
import { SiteLogType } from '../../../../shared/models/sites/site-log-type.enum';
import { SiteLogReviewDateStatus } from '../../../../shared/models/sites/site-log-review-date-status.enum';
import { SiteLogService } from '../../../../shared/services/sites/site-log.service';
import { TableViewModel } from '../../../../shared/models/table-view-model';
import { ToastService } from '../../../../shared/services/toast.service';
import { AuthService } from '../../../../shared/services/auth/auth.service';
import { PermissionAction, PermissionType } from '../../../../shared/models/permission-type';
import { CellTemplateData, ColumnType, ModalHelperService, TableColumn } from '@envirotrax/common-ui';
import { ModalSize } from '@developer-partners/ngx-modal-dialog';
import { SiteLogEditComponent, SiteLogEditModel } from '../../../../shared/components/site-log/site-log-edit.component';

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

    public readonly SiteLogType = SiteLogType;
    public canModify: boolean = false;

    public readonly reviewDateStatusClasses: { [key: number]: string } = {
        [SiteLogReviewDateStatus.None]: '',
        [SiteLogReviewDateStatus.Overdue]: 'badge bg-danger',
        [SiteLogReviewDateStatus.DueSoon]: 'badge bg-warning text-dark',
        [SiteLogReviewDateStatus.Upcoming]: 'badge bg-success',
        [SiteLogReviewDateStatus.Completed]: 'badge bg-secondary'
    };

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
            this.table.items = await this._siteLogService.getAll(
                this.siteId,
                this.table.items?.pageInfo || {},
                this.table.query
            );
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
            }
        ];
    }
}
