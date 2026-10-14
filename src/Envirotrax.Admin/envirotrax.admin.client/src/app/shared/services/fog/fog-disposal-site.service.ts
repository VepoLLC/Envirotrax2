import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { PagedData, PageInfo, Query, QueryHelperService, UrlResolverService } from "@envirotrax/common-ui";
import { lastValueFrom } from "rxjs";
import { FogDisposalSite } from "../../models/fog/fog-disposal-site";

@Injectable({
    providedIn: 'root'
})
export class FogDisposalSiteService {
    constructor(
        private readonly _urlResolver: UrlResolverService,
        private readonly _queryHelper: QueryHelperService,
        private readonly _http: HttpClient
    ) {

    }

    public getAll(pageInfo: PageInfo, query: Query): Promise<PagedData<FogDisposalSite>> {
        const url = this._urlResolver.resolveUrl('/api/fog/disposal-sites');

        return lastValueFrom(this._http.get<PagedData<FogDisposalSite>>(url, {
            params: this._queryHelper.buildQuery(pageInfo, query)
        }));
    }

    public add(site: FogDisposalSite): Promise<FogDisposalSite> {
        const url = this._urlResolver.resolveUrl('/api/fog/disposal-sites');

        return lastValueFrom(this._http.post<FogDisposalSite>(url, site));
    }

    public update(site: FogDisposalSite): Promise<FogDisposalSite> {
        const url = this._urlResolver.resolveUrl(`/api/fog/disposal-sites/${site.id}`);

        return lastValueFrom(this._http.put<FogDisposalSite>(url, site));
    }

    public delete(id: number): Promise<void> {
        const url = this._urlResolver.resolveUrl(`/api/fog/disposal-sites/${id}`);

        return lastValueFrom(this._http.delete<void>(url));
    }
}
