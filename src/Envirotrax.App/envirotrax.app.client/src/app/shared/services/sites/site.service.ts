import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { lastValueFrom } from "rxjs";
import { UrlResolverService } from "../helpers/url-resolver.service";
import { QueryHelperService } from "../helpers/query-helper.service";
import { PageInfo } from "../../models/page-info";
import { Query } from "../../models/query";
import { PagedData } from "../../models/paged-data";
import { Site } from "../../models/sites/site";

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

    public async getAll(pageInfo: PageInfo, query: Query): Promise<PagedData<Site>> {
        const url = this._urlResolver.resolveUrl('/api/sites');

        const observable = this._http.get<PagedData<Site>>(url, {
            params: this._queryHelper.buildQuery(pageInfo, query)
        });

        return await lastValueFrom(observable);
    }

    public get(id: number): Promise<Site> {
        const url = this._urlResolver.resolveUrl(`/api/sites/${id}`);

        return lastValueFrom(
            this._http.get<Site>(url)
        );
    }

    public add(site: Site): Promise<Site> {
        const url = this._urlResolver.resolveUrl('/api/sites');

        return lastValueFrom(
            this._http.post<Site>(url, site)
        );
    }

    public update(site: Site): Promise<Site> {
        const url = this._urlResolver.resolveUrl(`/api/sites/${site.id}`);

        return lastValueFrom(
            this._http.put<Site>(url, site)
        );
    }

    public delete(id: number): Promise<Site> {
        const url = this._urlResolver.resolveUrl(`/api/sites/${id}`);

        return lastValueFrom(
            this._http.delete<Site>(url)
        );
    }
}
