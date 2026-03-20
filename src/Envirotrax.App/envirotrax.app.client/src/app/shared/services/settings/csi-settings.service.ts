import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { lastValueFrom } from "rxjs";
import { UrlResolverService } from "../helpers/url-resolver.service";
import { CsiSettings } from "../../models/settings/csi-settings";
import { PagedData } from "../../models/paged-data";

@Injectable({
    providedIn: 'root'
})
export class CsiSettingsService {
    constructor(
        private readonly _urlResolver: UrlResolverService,
        private readonly _http: HttpClient
    ) {
    }

    public async get(): Promise<CsiSettings> {
        const url = this._urlResolver.resolveUrl('/api/csi-settings');
        const observable = this._http.get<PagedData<CsiSettings>>(url);
        const response = await lastValueFrom(observable);

        return response.data?.[0] ?? {
            newlyCreatedBackflowTestExpirationDays: 0,
            impendingNotice1: 0,
            impendingNotice2: 0,
            pastDueNotice1: 0,
            pastDueNotice2: 0,
            nonCompliant1: 0,
            nonCompliant2: 0,
            impendingLettersBackgroundColor: '#d3d3d3',
            impendingLettersForegroundColor: '#000000',
            impendingLettersBorderColor: '#000000',
            pastDueLettersBackgroundColor: '#d3d3d3',
            pastDueLettersForegroundColor: '#000000',
            pastDueLettersBorderColor: '#000000',
            nonCompliantLettersBackgroundColor: '#d3d3d3',
            nonCompliantLettersForegroundColor: '#000000',
            nonCompliantLettersBorderColor: '#000000',
        };
    }

    public add(settings: CsiSettings): Promise<CsiSettings> {
        const url = this._urlResolver.resolveUrl('/api/csi-settings');
        const observable = this._http.post<CsiSettings>(url, settings);

        return lastValueFrom(observable);
    }

    public update(settings: CsiSettings): Promise<CsiSettings | undefined> {
        const url = this._urlResolver.resolveUrl(`/api/csi-settings/${settings.id}`);
        const observable = this._http.put<CsiSettings>(url, settings);

        return lastValueFrom(observable);
    }
}
