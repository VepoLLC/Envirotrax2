import { HttpClient, HttpHeaders } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { QueryHelperService } from "./helpers/query-helper.service";
import { lastValueFrom } from "rxjs";
import { MAX_PAGE_SIZE } from "../models/page-info";
import { DownloadConfig, FileFormat } from "../models/download-config";

@Injectable({
    providedIn: 'root'
})
export class DownloadService {
    constructor(
        private readonly _http: HttpClient,
        private readonly _queryHelper: QueryHelperService
    ) {

    }

    public downloadFileFromUrl(url: string, fileName?: string): void {
        const link = document.createElement('a');

        link.href = url;
        link.target = '_blank';

        if (fileName) {
            link.download = fileName;
        }

        // this is necessary as link.click() does not work on the latest firefox
        link.dispatchEvent(new MouseEvent('click', { bubbles: true, cancelable: true, view: window }));

        window.setTimeout(function () {
            link.remove();
        }, 5000);
    }

    public downloadFileFromBlob(blob: Blob, fileName?: string): void {
        const objectUrl = URL.createObjectURL(blob);
        this.downloadFileFromUrl(objectUrl, fileName);
    }

    private getAcceptHeader(format: FileFormat): string {
        switch (format) {
            case 'CSV':
                return 'text/csv';
            case 'Excel':
                return 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet';
            case 'PDF':
                return 'application/pdf';
            case 'XML':
                return 'application/xml';
        }
    }

    private async downloadPdf(config: DownloadConfig): Promise<void> {
        const observable = this._http.request(config.pdfEndpoint!.method || 'GET', config.pdfEndpoint!.url, {
            responseType: 'blob',
            observe: 'response',
            params: this._queryHelper.buildQuery(config.pdfEndpoint!.pageInfo || { pageSize: MAX_PAGE_SIZE }, config.pdfEndpoint!.query || {})
        });

        const response = await lastValueFrom(observable);

        this.downloadFileFromBlob(response.body!);
    }

    public async download(config: DownloadConfig): Promise<void> {
        if (config.selectedFormat == 'PDF') {
            if (!config.pdfEndpoint) {
                throw new Error("PDF endpoint is undefined.");
            }

            await this.downloadPdf(config);
        } else {
            const downloadColumns = config.columns
                .map(c => `${c.field}=${encodeURIComponent(c.caption ?? c.field)}`)
                .join('&')

            const headers = new HttpHeaders({
                'Vp-File-Name': config.fileName!,
                'Vp-Columns': downloadColumns,
                'Accept': this.getAcceptHeader(config.selectedFormat ?? 'CSV')
            });

            const observable = this._http.request(config.endpoint!.method || 'GET', config.endpoint.url, {
                headers: headers,
                responseType: 'blob',
                observe: 'response',
                params: this._queryHelper.buildQuery(config.endpoint.pageInfo || { pageSize: MAX_PAGE_SIZE }, config.endpoint.query || {})
            });

            const response = await lastValueFrom(observable);
            const fileName = response.headers.get('Content-Disposition')!.split('filename=')[1].replace(/^"|"$/g, '');

            this.downloadFileFromBlob(response.body!, fileName);
        }
    }
}