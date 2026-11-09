import { HttpClient, HttpHeaders, HttpParams } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { QueryHelperService } from "../helpers/query-helper.service";
import { lastValueFrom } from "rxjs";
import { MAX_PAGE_SIZE } from "../../models/page-info";
import { DownloadConfig, DownloadEndpoint, FileFormat } from "../../models/download-config";
import { Query } from "../../models/query";
import { DownloadManagerComponent } from "../../components/data-components/download-manager/download-manager.component";
import { ModalHelperService } from "../helpers/modal-helper.service";

const LOADING_WINDOW_TITLE = 'Loading...';

const LOADING_WINDOW_STYLES = `
    <style>
        body {
            display: flex;
            align-items: center;
            justify-content: center;
            height: 100vh;
            margin: 0;
            font-family: Arial, sans-serif;
            color: #212529;
        }

        .vp-loader-container {
            display: flex;
            flex-direction: row-reverse;
            justify-content: space-between;
            align-items: center;
            min-width: 250px;
            padding: 0.5rem 1rem;
            border: 1px solid #dee2e6;
            border-radius: 28px;
            box-shadow: 0 0.125rem 0.25rem rgba(0, 0, 0, 0.075);
        }

        .vp-loader-container p {
            margin: 0 1rem 0 0;
            font-size: 18px;
        }

        .vp-loader {
            width: 55px;
            aspect-ratio: 1;
            display: grid;
            -webkit-mask: conic-gradient(from 15deg, #0000, #000);
            animation: l26 1s infinite steps(12);
        }

        .vp-loader,
        .vp-loader:before,
        .vp-loader:after {
            background:
                radial-gradient(closest-side at 50% 12.5%, #008fd4 96%, #0000) 50% 0/20% 80% repeat-y,
                radial-gradient(closest-side at 12.5% 50%, #008fd4 96%, #0000) 0 50%/80% 20% repeat-x;
        }

        .vp-loader:before,
        .vp-loader:after {
            content: "";
            grid-area: 1/1;
            transform: rotate(30deg);
        }

        .vp-loader:after {
            transform: rotate(60deg);
        }

        @keyframes l26 {
            100% {
                transform: rotate(1turn);
            }
        }
    </style>`;

const LOADING_WINDOW_BODY = '<div class="vp-loader-container"><div class="vp-loader"></div><p>Loading Data...</p></div>';

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

    public async openBlobInNewWindow(loadBlob: () => Promise<Blob>): Promise<void> {
        const newWindow = window.open('', '_blank');

        if (!newWindow) {
            this.downloadFileFromBlob(await loadBlob());
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

    public async openUrlInNewWindow(loadUrl: () => Promise<string>): Promise<void> {
        const newWindow = window.open('', '_blank');

        if (!newWindow) {
            this.downloadFileFromUrl(await loadUrl());
            return;
        }

        newWindow.document.title = LOADING_WINDOW_TITLE;
        newWindow.document.head.innerHTML = LOADING_WINDOW_STYLES;
        newWindow.document.body.innerHTML = LOADING_WINDOW_BODY;

        let isOpened = false;

        try {
            const url = await loadUrl();

            newWindow.location.href = url;
            isOpened = true;
        } finally {
            if (!isOpened) {
                newWindow.close();
            }
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

    private async downloadPdf(config: DownloadConfig): Promise<void> {
        const observable = this._http.request(config.pdfEndpoint!.method || 'GET', config.pdfEndpoint!.url, {
            responseType: 'blob',
            observe: 'response',
            params: this.buildEndpointParams(config.pdfEndpoint!)
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