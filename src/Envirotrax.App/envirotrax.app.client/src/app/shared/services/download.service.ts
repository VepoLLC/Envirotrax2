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

    private downloadBlob(fileName: string, blob: Blob): void {
        let data = window.URL.createObjectURL(blob);

        let link = window.document.createElement('a');
        link.href = data;
        link.download = fileName;

        link.dispatchEvent(new MouseEvent('click', { bubbles: true, cancelable: true, view: window }));

        window.setTimeout(function () {
            window.URL.revokeObjectURL(data);
            link.remove();
        }, 100);
    }

    private getAcceptHeader(format: FileFormat): string {
        switch (format) {
            case 'CSV':
                return 'text/csv';
            case 'Excel':
                return 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet';
            case 'PDF':
                return 'application/pdf';
        }
    }

    public async download(config: DownloadConfig): Promise<void> {
        const downloadColumns = config.columns
            .filter(c => c.isSelected)
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

        this.downloadBlob(fileName, response.body!);
    }
}