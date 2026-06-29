import { Component, OnInit } from "@angular/core";
import { NgForm } from "@angular/forms";
import { ModalReference } from "@developer-partners/ngx-modal-dialog";
import { InputOption } from "@envirotrax/common-ui";
import { SiteLog } from "../../../shared/models/sites/site-log";
import { SiteLogType } from "../../../shared/models/sites/site-log-type.enum";
import { ComparisonOperator } from "../../../shared/models/query";
import { BackflowTest } from "../../../shared/models/backflow/backflow-test";
import { SiteLogService } from "../../../shared/services/sites/site-log.service";
import { BackflowTestService } from "../../../shared/services/backflow/backflow-test.service";
import { HelperService } from "../../../shared/services/helpers/helper.service";

export interface SiteLogModalModel {
    siteId: number;
    canModify: boolean;
}

type SiteLogRow = SiteLog & {
    typeLabel?: string;
    reviewClass?: string;
};

@Component({
    standalone: false,
    templateUrl: './site-log-modal.component.html'
})
export class SiteLogModalComponent implements OnInit {
    public readonly SiteLogType = SiteLogType;

    public logs: SiteLogRow[] = [];
    public assemblyOptions: InputOption[] = [];
    public isLoading: boolean = false;
    public validationErrors: string[] = [];

    public showForm: boolean = false;
    public editingId: number | null = null;
    public formType: SiteLogType = SiteLogType.Note;
    public formNote: string = '';
    public formReviewDate: string = '';
    public formAssemblyId: string = '';
    public formFile: File | null = null;

    constructor(
        private readonly _modalReference: ModalReference<SiteLogModalModel>,
        private readonly _siteLogService: SiteLogService,
        private readonly _backflowTestService: BackflowTestService,
        private readonly _helper: HelperService
    ) { }

    public get siteId(): number {
        return this._modalReference.config.model!.siteId;
    }

    public get canModify(): boolean {
        return this._modalReference.config.model!.canModify;
    }

    public async ngOnInit(): Promise<void> {
        await this.load();

        if (this.canModify) {
            await this.loadAssemblyOptions();
        }
    }

    private async load(): Promise<void> {
        try {
            this.isLoading = true;
            const result = await this._siteLogService.getAll(
                this.siteId,
                { pageNumber: 1, pageSize: 999999 },
                { sort: { id: 'Desc' }, filter: [] }
            );

            const logs = (result.data ?? []) as SiteLogRow[];

            for (const log of logs) {
                this.decorate(log);
            }

            this.logs = logs;
        } finally {
            this.isLoading = false;
        }
    }

    private async loadAssemblyOptions(): Promise<void> {
        const result = await this._backflowTestService.getAll(
            { pageNumber: 1, pageSize: 999999 },
            {
                filter: [{ columnName: 'site.id', value: this.siteId.toString(), comparisonOperator: 'Eq' as ComparisonOperator }],
                sort: {}
            }
        );

        this.assemblyOptions = result.data.map((t: BackflowTest) => ({ id: t.id, text: this.buildAssemblyLabel(t) }));
    }

    private buildAssemblyLabel(test: BackflowTest): string {
        let label = `SN: ${test.serialNumber || 'Unknown'}`;

        const deviceInfo = [test.manufacturer, test.model, test.size].filter(Boolean).join(' ');
        if (deviceInfo) {
            label += ` - ${deviceInfo}`;
        }
        if (test.deviceType) {
            label += ` - ${test.deviceType}`;
        }

        return label;
    }

    private decorate(log: SiteLogRow): void {
        switch (log.logType) {
            case SiteLogType.Reminder:
                log.typeLabel = 'Scheduled Review';
                break;
            case SiteLogType.CompletedReminder:
                log.typeLabel = 'Completed Review';
                break;
            default:
                log.typeLabel = 'Note';
                break;
        }

        log.reviewClass = this.reviewBadgeClass(log);
    }

    private reviewBadgeClass(log: SiteLog): string {
        if (log.logType === SiteLogType.CompletedReminder) {
            return 'bg-secondary';
        }

        if (!log.reviewDate) {
            return 'bg-success';
        }

        const review = new Date(log.reviewDate).setHours(0, 0, 0, 0);
        const now = new Date().setHours(0, 0, 0, 0);
        const in30Days = now + 30 * 86400000;

        if (review < now) {
            return 'bg-danger';
        }

        if (review < in30Days) {
            return 'bg-warning text-dark';
        }

        return 'bg-success';
    }

    public startAdd(): void {
        this.editingId = null;
        this.formType = SiteLogType.Note;
        this.formNote = '';
        this.formReviewDate = '';
        this.formAssemblyId = '';
        this.formFile = null;
        this.validationErrors = [];
        this.showForm = true;
    }

    public startEdit(log: SiteLog): void {
        this.editingId = log.id ?? null;
        this.formType = log.logType ?? SiteLogType.Note;
        this.formNote = log.noteText ?? '';
        this.formReviewDate = log.reviewDate ? log.reviewDate.substring(0, 10) : '';
        this.formAssemblyId = log.assemblyId ? String(log.assemblyId) : '';
        this.formFile = null;
        this.validationErrors = [];
        this.showForm = true;
    }

    public cancelForm(): void {
        this.showForm = false;
    }

    public async save(form: NgForm): Promise<void> {
        if (!form.valid) {
            return;
        }

        const log: SiteLog = {
            id: this.editingId ?? undefined,
            logType: this.formType,
            noteText: this.formNote || undefined,
            reviewDate: this.formType === SiteLogType.Note ? null : (this.formReviewDate || null),
            assemblyId: this.formAssemblyId ? Number(this.formAssemblyId) : null,
            skipFile: false
        };

        try {
            this.isLoading = true;
            this.validationErrors = [];

            if (this.editingId) {
                await this._siteLogService.update(this.siteId, log, this.formFile);
            } else {
                await this._siteLogService.add(this.siteId, log, this.formFile);
            }

            this.showForm = false;
            await this.load();
        } catch (e) {
            if (!this._helper.parseValidationErrors(e, this.validationErrors)) {
                throw e;
            }
        } finally {
            this.isLoading = false;
        }
    }

    public async deleteLog(log: SiteLog): Promise<void> {
        if (!log.id || !confirm('Delete this log record?')) {
            return;
        }

        try {
            this.isLoading = true;
            await this._siteLogService.delete(this.siteId, log.id);
            await this.load();
        } finally {
            this.isLoading = false;
        }
    }

    public close(): void {
        this._modalReference.cancel();
    }
}
