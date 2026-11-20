import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { PagedData, PageInfo, Query, QueryHelperService, UrlResolverService } from "@envirotrax/common-ui";
import { lastValueFrom } from "rxjs";
import { SiteLog } from "../../models/sites/site-log";

@Injectable({
    providedIn: 'root'
})
export class SiteLogService {
    constructor(
        private readonly _urlResolver: UrlResolverService,
        private readonly _queryHelper: QueryHelperService,
        private readonly _http: HttpClient
    ) {

    }

    public async getAll(siteId: number, pageInfo: PageInfo, query: Query): Promise<PagedData<SiteLog>> {
        const url = this._urlResolver.resolveUrl(`/api/sites/${siteId}/site-logs`);

        const observable = this._http.get<PagedData<SiteLog>>(url, {
            params: this._queryHelper.buildQuery(pageInfo, query)
        });

        return await lastValueFrom(observable);
    }

    public async get(siteId: number, id: number): Promise<SiteLog> {
        const url = this._urlResolver.resolveUrl(`/api/sites/${siteId}/site-logs/${id}`);

        const observable = this._http.get<SiteLog>(url);

        return await lastValueFrom(observable);
    }

    public async add(siteId: number, waterSupplierId: number, log: SiteLog): Promise<SiteLog> {
        const url = this._urlResolver.resolveUrl(`/api/sites/${siteId}/site-logs?waterSupplierId=${waterSupplierId}`);

        const observable = this._http.post<SiteLog>(url, log);

        return await lastValueFrom(observable);
    }

    public async update(siteId: number, id: number, waterSupplierId: number, log: SiteLog): Promise<SiteLog> {
        const url = this._urlResolver.resolveUrl(`/api/sites/${siteId}/site-logs/${id}?waterSupplierId=${waterSupplierId}`);

        const observable = this._http.put<SiteLog>(url, log);

        return await lastValueFrom(observable);
    }

    public async delete(siteId: number, id: number): Promise<void> {
        const url = this._urlResolver.resolveUrl(`/api/sites/${siteId}/site-logs/${id}`);

        const observable = this._http.delete<void>(url);

        await lastValueFrom(observable);
    }
}
