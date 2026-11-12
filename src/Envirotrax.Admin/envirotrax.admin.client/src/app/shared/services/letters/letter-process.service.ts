import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { UrlResolverService } from "@envirotrax/common-ui";
import { lastValueFrom } from "rxjs";
import { LetterEmail } from "../../models/letters/letter-email";
import { LetterFile } from "../../models/letters/letter-file";
import { LetterProcess } from "../../models/letters/letter-process";

@Injectable({
    providedIn: 'root'
})
export class LetterProcessService {
    constructor(
        private readonly _urlResolver: UrlResolverService,
        private readonly _http: HttpClient
    ) {

    }

    public getAll(): Promise<LetterProcess[]> {
        const url = this._urlResolver.resolveUrl('/api/letter-processes');

        return lastValueFrom(this._http.get<LetterProcess[]>(url));
    }

    public getFiles(processId: string): Promise<LetterFile[]> {
        const url = this._urlResolver.resolveUrl(`/api/letter-processes/${encodeURIComponent(processId)}/files`);

        return lastValueFrom(this._http.get<LetterFile[]>(url));
    }

    public getEmails(processId: string): Promise<LetterEmail[]> {
        const url = this._urlResolver.resolveUrl(`/api/letter-processes/${encodeURIComponent(processId)}/emails`);

        return lastValueFrom(this._http.get<LetterEmail[]>(url));
    }

    public getFileUrl(processId: string, fileName: string): Promise<string> {
        const url = this._urlResolver.resolveUrl(`/api/letter-processes/${encodeURIComponent(processId)}/file-url`);

        return lastValueFrom(this._http.get(url, {
            params: { fileName },
            responseType: 'text'
        }));
    }

    public delete(processId: string): Promise<void> {
        const url = this._urlResolver.resolveUrl(`/api/letter-processes/${encodeURIComponent(processId)}`);

        return lastValueFrom(this._http.delete<void>(url));
    }
}
