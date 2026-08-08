import { Component, OnInit } from '@angular/core';
import { NgForm } from '@angular/forms';
import { ModalReference } from '@developer-partners/ngx-modal-dialog';
import { SiteLog } from '../../models/sites/site-log';
import { SiteLogType } from '../../models/sites/site-log-type.enum';
import { SiteLogService } from '../../services/sites/site-log.service';
import { BackflowTestService } from '../../services/backflow/backflow-test.service';
import { BackflowTest } from '../../models/backflow/backflow-test';
import { ComparisonOperator } from '../../models/query';
import { ToastService, InputOption } from '@envirotrax/common-ui';
import { HelperService } from '../../services/helpers/helper.service';

export interface SiteLogEditModel {
    siteId: number;
    log: SiteLog;
}

@Component({
    standalone: false,
    templateUrl: './site-log-edit.component.html'
})
export class SiteLogEditComponent implements OnInit {
    public readonly SiteLogType = SiteLogType;

    public isLoading: boolean = false;
    public validationErrors: string[] = [];
    public editingLog: SiteLog;
    public selectedFile: File | null = null;
    public assemblyOptions: InputOption[] = [];

    public readonly logTypeOptions: InputOption[] = [
        { id: SiteLogType.Note, text: 'Informational Note' },
        { id: SiteLogType.Reminder, text: 'Scheduled Review' },
        { id: SiteLogType.CompletedReminder, text: 'Completed Review' }
    ];

    public get isEditMode(): boolean {
        return !!this.editingLog.id;
    }

    public get showReviewDate(): boolean {
        const type = Number(this.editingLog.logType);
        return type === SiteLogType.Reminder || type === SiteLogType.CompletedReminder;
    }

    private get siteId(): number {
        return this._modalReference.config.model!.siteId;
    }

    constructor(
        private readonly _modalReference: ModalReference<SiteLogEditModel, SiteLog>,
        private readonly _siteLogService: SiteLogService,
        private readonly _backflowTestService: BackflowTestService,
        private readonly _toastService: ToastService,
        private readonly _helper: HelperService
    ) {
        const model = this._modalReference.config.model!;
        this.editingLog = { ...model.log };
        if (!this.editingLog.id) {
            this.editingLog.assemblyId = null;
        }
    }

    public async ngOnInit(): Promise<void> {
        await this.loadAssemblyOptions();
    }

    private async loadAssemblyOptions(): Promise<void> {
        const result = await this._backflowTestService.getAll(
            { pageNumber: 1, pageSize: 999999 },
            {
                filter: [{ columnName: 'site.id', value: this.siteId.toString(), comparisonOperator: 'Eq' as ComparisonOperator }],
                sort: {}
            }
        );

        this.assemblyOptions = [
            { id: null, text: 'N/A' },
            ...result.data.map((t: BackflowTest) => ({ id: t.id, text: this.buildAssemblyLabel(t) }))
        ];
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

    public onFileChange(file: File | null): void {
        this.selectedFile = file;
    }

    public async save(form: NgForm): Promise<void> {
        if (!form.valid) return;

        try {
            this.isLoading = true;
            this.validationErrors = [];

            const result = this.isEditMode
                ? await this._siteLogService.update(this.siteId, this.editingLog, this.selectedFile)
                : await this._siteLogService.add(this.siteId, this.editingLog, this.selectedFile);

            this._toastService.successfullySaved('Log Record');
            this._modalReference.closeSuccess(result);
        } catch (error) {
            if (!this._helper.parseValidationErrors(error, this.validationErrors)) throw error;
            this._toastService.failedToSave('Log Record');
        } finally {
            this.isLoading = false;
        }
    }

    public cancel(): void {
        this._modalReference.cancel();
    }
}
