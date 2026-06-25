import { Injectable } from "@angular/core";
import { HttpClient, HttpParams } from "@angular/common/http";
import { lastValueFrom } from "rxjs";
import { UrlResolverService } from "../helpers/url-resolver.service";
import { QueryHelperService } from "../helpers/query-helper.service";
import { PageInfo } from "../../models/page-info";
import { Query } from "../../models/query";
import { PagedData } from "../../models/paged-data";
import { FogDisposalSiteCandidate } from "../../models/fog/fog-disposal-site-candidate";

@Injectable({
    providedIn: 'root'
})
export class ProfessionalFogDisposalSiteService {
    constructor(
        private readonly _urlResolver: UrlResolverService,
        private readonly _queryHelper: QueryHelperService,
        private readonly _http: HttpClient
    ) {
    }

    public getAvailable(pageInfo: PageInfo, query: Query): Promise<PagedData<FogDisposalSiteCandidate>> {
        const url = this._urlResolver.resolveUrl('/api/professionals/fog/transportation/disposal-sites/available');
        return lastValueFrom(this._http.get<PagedData<FogDisposalSiteCandidate>>(url, {
            params: this._queryHelper.buildQuery(pageInfo, query)
        }));
    }

    public setRegistration(disposalSiteId: number, isActive: boolean): Promise<FogDisposalSiteCandidate> {
        const url = this._urlResolver.resolveUrl(`/api/professionals/fog/transportation/disposal-sites/available/${disposalSiteId}`);
        const params = new HttpParams().set('isActive', String(isActive));
        return lastValueFrom(this._http.put<FogDisposalSiteCandidate>(url, null, { params }));
    }
}
