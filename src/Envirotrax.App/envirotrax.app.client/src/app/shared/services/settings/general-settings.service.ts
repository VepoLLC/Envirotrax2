import { Injectable } from "@angular/core";
import { UrlResolverService } from "../helpers/url-resolver.service";
import { PageInfo } from "../../models/page-info";
import { Query } from "../../models/query";
import { QueryHelperService } from "../helpers/query-helper.service";
import { HttpClient } from "@angular/common/http";
import { PagedData } from "../../models/paged-data";
import { GeneralSettings } from "../../models/settings/general-settings";
import { lastValueFrom } from "rxjs";

@Injectable({
    providedIn: 'root'
})
export class GeneralSettingsService {
    constructor(
        private readonly _urlResolver: UrlResolverService,
        private readonly _queryHelper: QueryHelperService,
        private readonly _http: HttpClient
    ) {

    }

    public async getAll(pageInfo: PageInfo, query: Query): Promise<PagedData<GeneralSettings>> {
        const url = this._urlResolver.resolveUrl('/api/general-settings');

        const observable = this._http.get<PagedData<GeneralSettings>>(url, {
            params: this._queryHelper.buildQuery(pageInfo, query)
        });

        return await lastValueFrom(observable);
    }

    public get(waterSupplierId: number): Promise<GeneralSettings> {
        const url = this._urlResolver.resolveUrl(`/api/general-settings/${waterSupplierId}`);
        const observable = this._http.get(url);

        return lastValueFrom(observable);
    }

    public getCurrent(): Promise<GeneralSettings> {
        const url = this._urlResolver.resolveUrl('/api/general-settings/current');
        const observable = this._http.get(url);

        return lastValueFrom(observable);
    }

    public add(settings: GeneralSettings): Promise<GeneralSettings> {
        const url = this._urlResolver.resolveUrl('/api/general-settings');
        const observable = this._http.post(url, settings);

        return lastValueFrom(observable);
    }

    public update(settings: GeneralSettings): Promise<GeneralSettings | undefined> {
        const url = this._urlResolver.resolveUrl(`/api/general-settings/${settings.id}`);
        const observable = this._http.put(url, settings);

        return lastValueFrom(observable);
    }

    public updateCurrent(settings: GeneralSettings): Promise<GeneralSettings | undefined> {
        const url = this._urlResolver.resolveUrl('/api/general-settings/current');
        const observable = this._http.put(url, settings);

        return lastValueFrom(observable);
    }

    public delete(waterSupplierId: number): Promise<GeneralSettings | undefined> {
        const url = this._urlResolver.resolveUrl(`/api/general-settings/${waterSupplierId}`);
        const observable = this._http.delete(url);

        return lastValueFrom(observable);
    }
}