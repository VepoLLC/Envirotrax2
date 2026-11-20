import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { ModalReference } from '@developer-partners/ngx-modal-dialog';
import { HelperService, InputOption, ToastService, allSettled } from '@envirotrax/common-ui';
import { SharedComponentsModule } from '../../../../shared/components/shared.components.module';
import { BackflowTest } from '../../../../shared/models/backflow/backflow-test';
import { SiteLog, SiteLogAssembly, SiteLogEditModel, SiteLogType } from '../../../../shared/models/sites/site-log';
import { BackflowTestService } from '../../../../shared/services/backflow/backflow-test.service';
import { SiteLogService } from '../../../../shared/services/sites/site-log.service';

@Component({
    templateUrl: './site-log-edit.component.html',
    imports: [CommonModule, FormsModule, SharedComponentsModule],
})
export class SiteLogEditComponent implements OnInit {
    public isLoading: boolean = false;

    public validationErrors: string[] = [];

    public log: SiteLog = {};

    public logType: string = String(SiteLogType.Note);

    public assemblyId: string = '';

    public showReviewDate: boolean = false;

    public readonly isEditMode: boolean;

    public readonly logTypeOptions: InputOption[] = [
        { id: String(SiteLogType.Note), text: 'Note' },
        { id: String(SiteLogType.Reminder), text: 'Reminder' },
        { id: String(SiteLogType.CompletedReminder), text: 'Completed Reminder' }
    ];

    public assemblyOptions: InputOption[] = [{ id: '', text: '' }];

    private readonly _model: SiteLogEditModel;

    constructor(
        private readonly _modalReference: ModalReference<SiteLogEditModel, SiteLog>,
        private readonly _siteLogService: SiteLogService,
        private readonly _backflowTestService: BackflowTestService,
        private readonly _helper: HelperService,
        private readonly _toastService: ToastService
    ) {
        this._model = this._modalReference.config.model!;
        this.isEditMode = this._model.id != null;
    }

    public async ngOnInit(): Promise<void> {
        try {
            this.isLoading = true;

            await allSettled([
                this.loadLog(),
                this.loadAssemblyOptions()
            ]);
        } finally {
            this.isLoading = false;
        }
    }

    public onLogTypeChange(): void {
        this.showReviewDate = this.logType !== String(SiteLogType.Note);
    }

    public async save(form: NgForm): Promise<void> {
        this.validationErrors = [];

        if (!form.valid) {
            return;
        }

        try {
            this.isLoading = true;

            const log = this.buildLog();

            let saved: SiteLog;

            if (this.isEditMode) {
                saved = await this._siteLogService.update(this._model.siteId, this._model.id!, this._model.waterSupplierId, log);
            } else {
                saved = await this._siteLogService.add(this._model.siteId, this._model.waterSupplierId, log);
            }

            this._toastService.successfullySaved('Site Log');
            this._modalReference.closeSuccess(saved);
        } catch (error) {
            if (!this._helper.parseValidationErrors(error, this.validationErrors)) {
                throw error;
            }

            this._toastService.failedToSave('Site Log');
        } finally {
            this.isLoading = false;
        }
    }

    public cancel(): void {
        this._modalReference.cancel();
    }

    private async loadLog(): Promise<void> {
        if (this._model.id == null) {
            return;
        }

        this.log = await this._siteLogService.get(this._model.siteId, this._model.id);
        this.logType = String(this.log.logType ?? SiteLogType.Note);

        if (this.log.assembly?.id != null) {
            this.assemblyId = String(this.log.assembly.id);
        }

        this.onLogTypeChange();
    }

    private async loadAssemblyOptions(): Promise<void> {
        const tests = await this._backflowTestService.getAll(
            { pageNumber: 1, pageSize: 999999 },
            {
                filter: [{ columnName: 'site.id', value: String(this._model.siteId), comparisonOperator: 'Eq' }],
                sort: { testDate: 'Desc' }
            }
        );

        const options: InputOption[] = [{ id: '', text: '' }];

        for (const test of tests.data) {
            options.push({ id: String(test.id), text: this.buildAssemblyLabel(test) });
        }

        this.assemblyOptions = options;
    }

    private buildAssemblyLabel(test: BackflowTest): string {
        const description = `${test.manufacturer ?? ''} ${test.model ?? ''} ${test.size ?? ''} ${test.deviceType ?? ''}`.trim();

        return `${test.id} - SN#: ${test.serialNumber ?? ''} - ${description}`;
    }

    private buildLog(): SiteLog {
        let assembly: SiteLogAssembly | null = null;

        if (this.assemblyId) {
            assembly = { id: Number(this.assemblyId) };
        }

        let reviewDate: string | null = null;

        if (this.log.reviewDate) {
            reviewDate = this.log.reviewDate;
        }

        return {
            id: this.log.id,
            logType: Number(this.logType) as SiteLogType,
            noteText: this.log.noteText,
            reviewDate,
            assembly,
            skipFile: this.log.skipFile
        };
    }
}
