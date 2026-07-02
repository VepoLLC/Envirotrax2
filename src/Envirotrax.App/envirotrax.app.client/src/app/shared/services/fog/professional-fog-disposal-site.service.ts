import { Injectable } from "@angular/core";
import { HttpClient, HttpParams } from "@angular/common/http";
import { lastValueFrom } from "rxjs";
import { UrlResolverService } from "../helpers/url-resolver.service";
import { QueryHelperService } from "../helpers/query-helper.service";
import { PageInfo } from "../../models/page-info";
import { Query } from "../../models/query";
import { PagedData } from "../../models/paged-data";
import { FogDisposalSite } from "../../models/fog/fog-disposal-site";

@Injectable({
    providedIn: 'root'
})
export class ProfessionalFogDisposalSiteService {
    private readonly _baseUrl = '/api/professionals/fog/transportation/disposal-sites';

    constructor(
        private readonly _urlResolver: UrlResolverService,
        private readonly _queryHelper: QueryHelperService,
        private readonly _http: HttpClient
    ) {
    }

    public getAll(pageInfo: PageInfo, query: Query): Promise<PagedData<FogDisposalSite>> {
        const url = this._urlResolver.resolveUrl(this._baseUrl);
        return lastValueFrom(this._http.get<PagedData<FogDisposalSite>>(url, {
            params: this._queryHelper.buildQuery(pageInfo, query)
        }));
    }

    public getRegistered(pageInfo: PageInfo, query: Query): Promise<PagedData<FogDisposalSite>> {
        const url = this._urlResolver.resolveUrl(`${this._baseUrl}/registered`);
        return lastValueFrom(this._http.get<PagedData<FogDisposalSite>>(url, {
            params: this._queryHelper.buildQuery(pageInfo, query)
        }));
    }

    public setRegistration(disposalSiteId: number, isActive: boolean): Promise<void> {
        const url = this._urlResolver.resolveUrl(`${this._baseUrl}/${disposalSiteId}/registration`);
        const params = new HttpParams().set('isActive', String(isActive));
        return lastValueFrom(this._http.put<void>(url, null, { params }));
    }
}
