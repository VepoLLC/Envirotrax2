import { Injectable } from "@angular/core";
import { UrlResolverService } from "../helpers/url-resolver.service";
import { HttpClient } from "@angular/common/http";
import { GeneralSettings } from "../../models/settings/general-settings";
import { PagedData } from "../../models/paged-data";
import { lastValueFrom } from "rxjs";

@Injectable({
    providedIn: 'root'
})
export class GeneralSettingsService {
    constructor(
        private readonly _urlResolver: UrlResolverService,
        private readonly _http: HttpClient
    ) {
    }

    public async get(): Promise<GeneralSettings> {
        const url = this._urlResolver.resolveUrl('/api/general-settings');
        const observable = this._http.get<PagedData<GeneralSettings>>(url);
        const response = await lastValueFrom(observable);

        return response.data?.[0] ?? {};
    }

    public add(settings: GeneralSettings): Promise<GeneralSettings> {
        const url = this._urlResolver.resolveUrl('/api/general-settings');
        const observable = this._http.post<GeneralSettings>(url, settings);

        return lastValueFrom(observable);
    }

    public update(settings: GeneralSettings): Promise<GeneralSettings | undefined> {
        const url = this._urlResolver.resolveUrl(`/api/general-settings/${settings.id}`);
        const observable = this._http.put<GeneralSettings>(url, settings);

        return lastValueFrom(observable);
    }
}
