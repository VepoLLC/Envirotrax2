import { Component, OnInit } from "@angular/core";
import { LetterHistory } from "../shared/models/letters/letter-history";
import { LetterFile } from "../shared/models/letters/letter-file";
import { LetterHistoryService } from "../shared/services/letters/letter-history.service";
import { DownloadService } from "../shared/services/download.service";

const BACKFLOW_FILE_MARKER = '- BF -';

@Component({
    templateUrl: './letter-history.component.html',
    standalone: false
})
export class LetterHistoryComponent implements OnInit {
    public history: LetterHistory[] = [];

    public isLoading: boolean = false;

    constructor(
        private readonly _letterHistoryService: LetterHistoryService,
        private readonly _downloadService: DownloadService
    ) {
    }

    public async ngOnInit(): Promise<void> {
        await this.loadHistory();
    }

    public isBackflowFile(file: LetterFile): boolean {
        return file.fileName?.includes(BACKFLOW_FILE_MARKER) ?? false;
    }

    public async openFile(process: LetterHistory, file: LetterFile): Promise<void> {
        try {
            this.isLoading = true;

            await this._downloadService.openUrlInNewWindow(() => this._letterHistoryService.getFileUrl(process.id!, file.fileName!));
        } finally {
            this.isLoading = false;
        }
    }

    private async loadHistory(): Promise<void> {
        try {
            this.isLoading = true;

            this.history = await this._letterHistoryService.getAll();
        } finally {
            this.isLoading = false;
        }
    }
}
