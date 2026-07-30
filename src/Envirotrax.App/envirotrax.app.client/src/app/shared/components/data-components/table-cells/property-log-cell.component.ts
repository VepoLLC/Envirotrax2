import { Component } from "@angular/core";
import { ModalHelperService, TableRowData } from "@envirotrax/common-ui";
import { ModalSize } from "@developer-partners/ngx-modal-dialog";
import { SiteLog } from "../../../models/sites/site-log";
import { SiteLogType } from "../../../models/sites/site-log-type.enum";
import { SiteLogReviewDateStatus } from "../../../models/sites/site-log-review-date-status.enum";
import { MAX_PAGE_SIZE } from "../../../models/page-info";
import { SiteLogService } from "../../../services/sites/site-log.service";
import { DownloadService } from "../../../services/download.service";
import { ToastService } from "../../../services/toast.service";
import { SiteLogEditComponent, SiteLogEditModel } from "../../site-log/site-log-edit.component";

// Contract the hosting grid's row must satisfy (read off rowData; every field is optional/decorated):
//   - the site whose logs to manage, exposed either as `site.id` (assembly-rooted rows, e.g. backflow) or
//     as `id` (site-rooted rows, e.g. CSI);
//   - `logs`: the site's logs (the backend attaches these to the row);
//   - `canModify`: whether edit controls show (decorated by the page from its Sites-modify permission);
//   - `isFirstOfGroup`: for grouped grids, render only on the first row of a same-site run (absent = always).
export interface PropertyLogCellRow {
    id?: number;
    site?: { id?: number } | null;
    logs?: SiteLog[];
    canModify?: boolean;
    isFirstOfGroup?: boolean;
}

// Reusable app-level table cell (used via TableColumn.cellComponent, not cellTemplate) that renders a site's
// property logs with add/edit/delete + attachment download, self-contained via injected services. Declared in
// SharedComponentsModule so any feature can drop it onto a column. Consumers: Backflow / CSI Compliance
// Management. Mirrors the log rendering those pages previously inlined.
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

    public readonly row: PropertyLogCellRow;
    public logsExpanded: boolean = false;

    constructor(
        tableRowData: TableRowData<PropertyLogCellRow>,
        private readonly _modalHelper: ModalHelperService,
        private readonly _siteLogService: SiteLogService,
        private readonly _downloadService: DownloadService,
        private readonly _toastService: ToastService
    ) {
        this.row = tableRowData.rowData;
    }

    // Site the logs belong to: `site.id` for assembly-rooted rows, else the row's own `id` for site-rooted rows.
    public get siteId(): number | undefined {
        return this.row?.site?.id ?? this.row?.id;
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
        const siteId = this.siteId;

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
        const siteId = this.siteId;

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
        const siteId = this.siteId;

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
        const siteId = this.siteId;

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
