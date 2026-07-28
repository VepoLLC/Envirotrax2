import { Component } from "@angular/core";
import { ModalHelperService, TableRowData } from "@envirotrax/common-ui";
import { ModalSize } from "@developer-partners/ngx-modal-dialog";
import { BackflowCompliance } from "../../../shared/models/backflow/backflow-compliance";
import { SiteLog } from "../../../shared/models/sites/site-log";
import { SiteLogType } from "../../../shared/models/sites/site-log-type.enum";
import { SiteLogReviewDateStatus } from "../../../shared/models/sites/site-log-review-date-status.enum";
import { MAX_PAGE_SIZE } from "../../../shared/models/page-info";
import { SiteLogService } from "../../../shared/services/sites/site-log.service";
import { DownloadService } from "../../../shared/services/download.service";
import { ToastService } from "../../../shared/services/toast.service";
import { SiteLogEditComponent, SiteLogEditModel } from "../../../shared/components/site-log/site-log-edit.component";

// The row a compliance grid supplies: an assembly carrying its site + the site's logs, plus the grid's
// grouping flag (so site data renders once per contiguous same-site run) and the modify permission.
type PropertyLogRow = BackflowCompliance & { isFirstOfGroup?: boolean; canModify?: boolean };

// App-level table cell (used via TableColumn.cellComponent, not cellTemplate) that renders a site's property
// logs with add/edit/delete + attachment download, self-contained via injected services. `canModify` and the
// grouping flag are read off the row. Reused-shape sibling: the CSI Compliance Management log rendering.
@Component({
    standalone: false,
    templateUrl: './property-log-cell.component.html'
})
export class PropertyLogCellComponent {
    public readonly SiteLogType = SiteLogType;

    public readonly reviewDateStatusClasses: { [key: number]: string } = {
        [SiteLogReviewDateStatus.None]: '',
        [SiteLogReviewDateStatus.Overdue]: 'badge bg-danger',
        [SiteLogReviewDateStatus.DueSoon]: 'badge bg-warning text-dark',
        [SiteLogReviewDateStatus.Upcoming]: 'badge bg-success',
        [SiteLogReviewDateStatus.Completed]: 'badge bg-secondary'
    };

    public readonly row: PropertyLogRow;
    public logsExpanded: boolean = false;

    constructor(
        tableRowData: TableRowData<BackflowCompliance>,
        private readonly _modalHelper: ModalHelperService,
        private readonly _siteLogService: SiteLogService,
        private readonly _downloadService: DownloadService,
        private readonly _toastService: ToastService
    ) {
        this.row = tableRowData.rowData as PropertyLogRow;
    }

    // Logs render once per group; when the grouping flag is absent (non-grouped usage) the cell always shows.
    public get show(): boolean {
        return this.row?.isFirstOfGroup !== false;
    }

    public get canModify(): boolean {
        return this.row?.canModify === true;
    }

    public get logs(): SiteLog[] {
        return this.row?.logs ?? [];
    }

    public addLog(): void {
        const siteId = this.row?.site?.id;

        if (siteId == null) {
            return;
        }

        this._modalHelper.show<SiteLogEditModel, SiteLog>(SiteLogEditComponent, {
            title: 'Add Log Record',
            model: { siteId, log: { logType: SiteLogType.Note } },
            size: ModalSize.large
        }).result().subscribe(() => this.reloadLogs(siteId));
    }

    public editLog(log: SiteLog): void {
        const siteId = this.row?.site?.id;

        if (siteId == null) {
            return;
        }

        this._modalHelper.show<SiteLogEditModel, SiteLog>(SiteLogEditComponent, {
            title: 'Edit Log Record',
            model: { siteId, log },
            size: ModalSize.large
        }).result().subscribe(() => this.reloadLogs(siteId));
    }

    public deleteLog(log: SiteLog): void {
        const siteId = this.row?.site?.id;

        if (siteId == null) {
            return;
        }

        this._modalHelper.showDeleteConfirmation().result().subscribe(async () => {
            await this._siteLogService.delete(siteId, log.id!);
            this._toastService.successfullySaved('Log Record');
            await this.reloadLogs(siteId);
        });
    }

    public async openAttachment(log: SiteLog): Promise<void> {
        const siteId = this.row?.site?.id;

        if (siteId == null || log.id == null) {
            return;
        }

        const url = await this._siteLogService.getAttachmentUrl(siteId, log.id);

        if (url) {
            this._downloadService.downloadFileFromUrl(url);
        }
    }

    private async reloadLogs(siteId: number): Promise<void> {
        const result = await this._siteLogService.getAll(
            siteId,
            { pageNumber: 1, pageSize: MAX_PAGE_SIZE },
            { sort: { id: 'Desc' }, filter: [] }
        );

        this.row.logs = result.data;
    }
}
