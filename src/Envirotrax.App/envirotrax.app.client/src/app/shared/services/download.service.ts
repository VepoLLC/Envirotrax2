import { HttpClient, HttpHeaders } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { QueryHelperService } from "./helpers/query-helper.service";
import { lastValueFrom } from "rxjs";
import { MAX_PAGE_SIZE } from "../models/page-info";
import { DownloadConfig, FileFormat } from "../models/download-config";
import { Query } from "../models/query";
import { DownloadManagerComponent } from "../components/data-components/download-manager/download-manager.component";
import { ModalHelperService } from "@envirotrax/common-ui";

@Injectable({
    providedIn: 'root'
})
export class DownloadService {
    constructor(
        private readonly _http: HttpClient,
        private readonly _queryHelper: QueryHelperService,
        private readonly _modalHelper: ModalHelperService
    ) {

    }

    public downloadFileFromUrl(url: string, fileName?: string): void {
        const link = document.createElement('a');

        link.href = url;

        if (fileName) {
            link.download = fileName;
        } else {
            link.target = '_blank';
        }

        // this is necessary as link.click() does not work on the latest firefox
        link.dispatchEvent(new MouseEvent('click', { bubbles: true, cancelable: true, view: window }));

        window.setTimeout(function () {
            link.remove();
        }, 5000);
    }

    public downloadFileFromBlob(blob: Blob, fileName?: string): void {
        const objectUrl = URL.createObjectURL(blob);
        this.downloadFileFromUrl(objectUrl, fileName ?? 'download');

        window.setTimeout(function () {
            URL.revokeObjectURL(objectUrl);
        }, 5000);
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

    private getFileNameFromResponse(contentDisposition: string | null, fallback: string): string {
        const match = contentDisposition?.match(/filename\*?=(?:UTF-8'')?"?([^";]+)"?/i);
        return match ? decodeURIComponent(match[1].trim()) : fallback;
    }

    private async downloadPdf(config: DownloadConfig): Promise<void> {
        const observable = this._http.request(config.pdfEndpoint!.method || 'GET', config.pdfEndpoint!.url, {
            responseType: 'blob',
            observe: 'response',
            params: this._queryHelper.buildQuery(config.pdfEndpoint!.pageInfo || { pageSize: MAX_PAGE_SIZE }, config.pdfEndpoint!.query || {})
        });

        const response = await lastValueFrom(observable);
        const fileName = this.getFileNameFromResponse(response.headers.get('Content-Disposition'), config.fileName ?? 'document.pdf');

        this.downloadFileFromBlob(response.body!, fileName);
    }

    public async download(config: DownloadConfig): Promise<void> {
        if (config.selectedFormat == 'PDF') {
            if (!config.pdfEndpoint) {
                throw new Error("PDF endpoint is undefined.");
            }

            await this.downloadPdf(config);
        } else {
            const deselectedCategories = new Set(
                (config.categories ?? []).filter(c => !c.isSelected).map(c => c.name)
            );

            const downloadColumns = config.columns
                .filter(c => !c.category || !deselectedCategories.has(c.category))
                .map(c => `${c.field}=${encodeURIComponent(c.caption ?? c.field)}`)
                .join('&')

            let headerMap: { [name: string]: string } = {
                'Vp-File-Name': config.fileName!,
                'Vp-Columns': downloadColumns,
                'Accept': this.getAcceptHeader(config.selectedFormat ?? 'CSV')
            };

            if (config.selectedFormat === 'CSV' && config.csvDelimiter) {
                headerMap['Vp-Delimiter'] = config.csvDelimiter;
            }

            const headers = new HttpHeaders(headerMap);

            const observable = this._http.request(config.endpoint!.method || 'GET', config.endpoint.url, {
                headers: headers,
                responseType: 'blob',
                observe: 'response',
                params: this._queryHelper.buildQuery(config.endpoint.pageInfo || { pageSize: MAX_PAGE_SIZE }, config.endpoint.query || {})
            });

            const response = await lastValueFrom(observable);
            const fileName = this.getFileNameFromResponse(response.headers.get('Content-Disposition'), config.fileName ?? 'download');

            this.downloadFileFromBlob(response.body!, fileName);
        }
    }

    public showDownloadManager(config: DownloadConfig, query: Query): void {
        this._modalHelper.show(DownloadManagerComponent, {
            title: 'Export Results',
            model: {
                ...config,
                endpoint: {
                    ...config.endpoint,
                    query: query
                },
                pdfEndpoint: config.pdfEndpoint ? {
                    ...config.pdfEndpoint,
                    query: query
                } : undefined
            }
        });
    }
}