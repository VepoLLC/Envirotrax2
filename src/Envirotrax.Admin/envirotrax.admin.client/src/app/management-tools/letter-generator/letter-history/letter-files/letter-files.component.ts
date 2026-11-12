import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { ModalReference } from '@developer-partners/ngx-modal-dialog';
import { DownloadService } from '@envirotrax/common-ui';
import { SharedComponentsModule } from '../../../../shared/components/shared.components.module';
import { LetterEmail } from '../../../../shared/models/letters/letter-email';
import { LetterFile } from '../../../../shared/models/letters/letter-file';
import { LetterProcessService } from '../../../../shared/services/letters/letter-process.service';

export interface LetterFilesModalData {
    processId: string;
}

type LetterFilesTab = 'files' | 'emails';

@Component({
    imports: [
        CommonModule,
        SharedComponentsModule
    ],
    templateUrl: './letter-files.component.html'
})
export class LetterFilesComponent implements OnInit {
    public files: LetterFile[] = [];
    public emails: LetterEmail[] = [];

    public activeTab: LetterFilesTab = 'files';

    public isLoading: boolean = false;

    private readonly _processId: string;

    constructor(
        private readonly _modalReference: ModalReference<LetterFilesModalData, void>,
        private readonly _letterProcessService: LetterProcessService,
        private readonly _downloadService: DownloadService
    ) {
        this._processId = this._modalReference.config.model!.processId;
    }

    public async ngOnInit(): Promise<void> {
        try {
            this.isLoading = true;

            const filesPromise = this._letterProcessService.getFiles(this._processId);
            const emailsPromise = this._letterProcessService.getEmails(this._processId);

            this.files = await filesPromise;
            this.emails = await emailsPromise;
        } finally {
            this.isLoading = false;
        }
    }

    public setActiveTab(tab: LetterFilesTab): void {
        this.activeTab = tab;
    }

    public async viewFile(fileName: string): Promise<void> {
        try {
            this.isLoading = true;

            await this._downloadService.openUrlInNewWindow(() => this._letterProcessService.getFileUrl(this._processId, fileName));
        } finally {
            this.isLoading = false;
        }
    }

    public close(): void {
        this._modalReference.cancel();
    }
}
