import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { PagedData, PageInfo, Query, QueryHelperService, UrlResolverService } from "@envirotrax/common-ui";
import { FogCompliancyStatus, Site } from "../../models/sites/site";
import { lastValueFrom } from "rxjs";

@Injectable({
    providedIn: 'root'
})
export class SiteService {
    constructor(
        private readonly _urlResolver: UrlResolverService,
        private readonly _queryHelper: QueryHelperService,
        private readonly _http: HttpClient
    ) {

    }

    public async getAll(pageInfo: PageInfo, query: Query, fogCompliancyStatus?: FogCompliancyStatus | null): Promise<PagedData<Site>> {
        const url = this._urlResolver.resolveUrl('/api/sites');

        let params = this._queryHelper.buildQuery(pageInfo, query);

        if (fogCompliancyStatus != null) {
            params = params.append('fogCompliancyStatus', String(fogCompliancyStatus));
        }

        const observable = this._http.get<PagedData<Site>>(url, { params });

        return await lastValueFrom(observable);
    }
}
