import { Component } from "@angular/core";
import { NgForm } from "@angular/forms";
import { ModalReference, ModalSize } from "@developer-partners/ngx-modal-dialog";
import { DownloadConfig } from "../../../models/download-config";
import { DownloadService } from "../../../services/download.service";
import { MAX_PAGE_SIZE } from "../../../models/page-info";
import { InputOption } from "@envirotrax/common-ui";

@Component({
    templateUrl: './download-manager.component.html',
    standalone: false
})
export class DownloadManagerComponent {
    public isLoading: boolean = false;
    public downloadConfig: DownloadConfig;
    public formatOptions: InputOption[];
    public delimiterOptions: InputOption[] = [
        { id: ',', text: 'Comma' },
        { id: '|', text: 'Pipe' },
        { id: 'tab', text: 'Tab' }
    ];

    constructor(
        private readonly _modalReference: ModalReference<DownloadConfig, void>,
        private readonly _downloadService: DownloadService
    ) {
        _modalReference.config.size = ModalSize.large;

        this.downloadConfig = { ..._modalReference.config.model! };

        this.downloadConfig.endpoint.pageInfo = {
            pageNumber: 1,
            pageSize: MAX_PAGE_SIZE
        };

        if (this.downloadConfig.pdfEndpoint) {
            this.downloadConfig.pdfEndpoint.pageInfo = {
                pageNumber: 1,
                pageSize: MAX_PAGE_SIZE
            };
        }

        this.downloadConfig.selectedFormat = 'CSV';
        this.downloadConfig.csvDelimiter = ',';

        this.formatOptions = (this.downloadConfig.suppoertedFormats ?? ['CSV']).map(f => ({
            id: f,
            text: f
        }));
    }

    public async download(form: NgForm): Promise<void> {
        if (form.valid) {
            try {
                this.isLoading = true;

                await this._downloadService.download(this.downloadConfig);

                this._modalReference.closeSuccess();
            } finally {
                this.isLoading = false;
            }
        }
    }
}