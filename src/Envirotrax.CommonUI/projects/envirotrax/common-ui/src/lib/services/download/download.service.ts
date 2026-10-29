import { HttpClient, HttpHeaders, HttpParams, HttpResponse } from "@angular/common/http";
import { Inject, Injectable, Optional } from "@angular/core";
import { QueryHelperService } from "../helpers/query-helper.service";
import { lastValueFrom } from "rxjs";
import { MAX_PAGE_SIZE } from "../../models/page-info";
import { DownloadConfig, DownloadEndpoint, FileFormat } from "../../models/download-config";
import { Query } from "../../models/query";
import { DownloadManagerComponent } from "../../components/data-components/download-manager/download-manager.component";
import { ModalHelperService } from "../helpers/modal-helper.service";
import { PDF_VIEW_URL_FACTORY, PdfViewUrlFactory } from "../../tokens/pdf-view-url-factory.token";

const LOADING_WINDOW_TITLE = 'Loading...';

const LOADING_WINDOW_STYLES = `
    <style>
        body {
            display: flex;
            flex-direction: column;
            align-items: center;
            justify-content: center;
            height: 100vh;
            margin: 0;
            font-family: Arial, sans-serif;
            color: #555;
        }

        .spinner {
            width: 48px;
            height: 48px;
            border: 5px solid #ddd;
            border-top-color: #0084c3;
            border-radius: 50%;
            animation: spin 1s linear infinite;
        }

        @keyframes spin {
            to {
                transform: rotate(360deg);
            }
        }
    </style>`;

const LOADING_WINDOW_BODY = '<div class="spinner"></div><p>Loading...</p>';

@Injectable({
    providedIn: 'root'
})
export class DownloadService {
    constructor(
        private readonly _http: HttpClient,
        private readonly _queryHelper: QueryHelperService,
        private readonly _modalHelper: ModalHelperService,
        @Optional() @Inject(PDF_VIEW_URL_FACTORY) private readonly _pdfViewUrlFactory: PdfViewUrlFactory | null
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

    public async openBlobInNewWindow(loadBlob: () => Promise<Blob>, fileName?: string): Promise<void> {
        if (fileName && this._pdfViewUrlFactory) {
            await this.openNamedBlobInNewWindow(loadBlob, fileName, this._pdfViewUrlFactory);
            return;
        }

        const newWindow = window.open('', '_blank');

        if (!newWindow) {
            this.downloadFileFromBlob(await loadBlob(), fileName);
            return;
        }

        newWindow.document.title = LOADING_WINDOW_TITLE;
        newWindow.document.head.innerHTML = LOADING_WINDOW_STYLES;
        newWindow.document.body.innerHTML = LOADING_WINDOW_BODY;

        let isOpened = false;

        try {
            const blob = await loadBlob();
            const objectUrl = URL.createObjectURL(blob);

            newWindow.location.href = objectUrl;
            isOpened = true;

            window.setTimeout(function () {
                URL.revokeObjectURL(objectUrl);
            }, 5000);
        } finally {
            if (!isOpened) {
                newWindow.close();
            }
        }
    }

    // The window is opened straight at the final URL (no intermediate about:blank window), like a plain link would,
    // so the PDF viewer shows the file name as its title. If the browser blocks the late popup, the file is downloaded.
    private async openNamedBlobInNewWindow(loadBlob: () => Promise<Blob>, fileName: string, createViewUrl: PdfViewUrlFactory): Promise<void> {
        const blob = await loadBlob();
        const viewUrl = await createViewUrl(blob, fileName);

        if (!window.open(viewUrl, '_blank')) {
            this.downloadFileFromBlob(blob, fileName);
        }
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

    private buildEndpointParams(endpoint: DownloadEndpoint): HttpParams {
        let params = this._queryHelper.buildQuery(endpoint.pageInfo || { pageSize: MAX_PAGE_SIZE }, endpoint.query || {});

        for (const key of Object.keys(endpoint.additionalParams ?? {})) {
            params = params.append(key, endpoint.additionalParams![key]);
        }

        return params;
    }

    private toPdfFileName(fileName?: string): string {
        const name = fileName ?? 'document';

        return name.toLowerCase().endsWith('.pdf') ? name : `${name}.pdf`;
    }

    private requestPdf(config: DownloadConfig): Promise<HttpResponse<Blob>> {
        const observable = this._http.request(config.pdfEndpoint!.method || 'GET', config.pdfEndpoint!.url, {
            responseType: 'blob',
            observe: 'response',
            params: this.buildEndpointParams(config.pdfEndpoint!)
        });

        return lastValueFrom(observable);
    }

    private async downloadPdf(config: DownloadConfig): Promise<void> {
        if (config.openPdfInNewWindow) {
            await this.openBlobInNewWindow(async () => (await this.requestPdf(config)).body!, this.toPdfFileName(config.fileName));

            return;
        }

        const response = await this.requestPdf(config);
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
                params: this.buildEndpointParams(config.endpoint)
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