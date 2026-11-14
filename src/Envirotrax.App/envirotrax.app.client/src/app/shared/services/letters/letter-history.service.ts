import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { lastValueFrom } from "rxjs";
import { UrlResolverService } from "../helpers/url-resolver.service";
import { LetterHistory } from "../../models/letters/letter-history";

@Injectable({
    providedIn: 'root'
})
export class LetterHistoryService {
    constructor(
        private readonly _urlResolver: UrlResolverService,
        private readonly _http: HttpClient
    ) {
    }

    public getAll(): Promise<LetterHistory[]> {
        const url = this._urlResolver.resolveUrl('/api/letter-history');
        return lastValueFrom(this._http.get<LetterHistory[]>(url));
    }

    public getFileUrl(processId: string, fileName: string): Promise<string> {
        const url = this._urlResolver.resolveUrl(`/api/letter-history/${encodeURIComponent(processId)}/file-url`);
        return lastValueFrom(this._http.get<string>(url, { params: { fileName } }));
    }
}
