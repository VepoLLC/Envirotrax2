import { HttpClient, HttpParams } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { lastValueFrom } from "rxjs";
import { UrlResolverService } from "./url-resolver.service";

interface PdfViewLink {
    url: string;
}

@Injectable({
    providedIn: 'root'
})
export class PdfViewService {
    constructor(
        private readonly _urlResolver: UrlResolverService,
        private readonly _http: HttpClient
    ) {
    }

    public async createViewUrl(blob: Blob, fileName: string): Promise<string> {
        const url = this._urlResolver.resolveUrl('/api/pdf-views');
        const params = new HttpParams().set('fileName', fileName);

        const link = await lastValueFrom(this._http.post<PdfViewLink>(url, blob, {
            params,
            headers: { 'Content-Type': 'application/pdf' }
        }));

        return this._urlResolver.resolveUrl(link.url);
    }
}
