import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { PagedData, PageInfo, Query, QueryHelperService, UrlResolverService } from "@envirotrax/common-ui";
import { FogCompliancyStatus, Site } from "../../models/sites/site";
import { SiteDetail } from "../../models/sites/site-detail";
import { SiteGisUpdate, SiteUpdate, SiteWaterSupplierUpdate } from "../../models/sites/site-update";
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

    public async getById(siteId: number): Promise<SiteDetail> {
        const url = this._urlResolver.resolveUrl(`/api/sites/${siteId}`);

        const observable = this._http.get<SiteDetail>(url);

        return await lastValueFrom(observable);
    }

    public async update(siteId: number, waterSupplierId: number, site: SiteUpdate): Promise<void> {
        const url = this._urlResolver.resolveUrl(`/api/sites/${siteId}?waterSupplierId=${waterSupplierId}`);

        const observable = this._http.put<void>(url, site);

        await lastValueFrom(observable);
    }

    public async updateGis(siteId: number, waterSupplierId: number, gis: SiteGisUpdate): Promise<void> {
        const url = this._urlResolver.resolveUrl(`/api/sites/${siteId}/gis-data?waterSupplierId=${waterSupplierId}`);

        const observable = this._http.put<void>(url, gis);

        await lastValueFrom(observable);
    }

    public async updateWaterSupplier(siteId: number, currentWaterSupplierId: number, waterSupplierId: number): Promise<void> {
        const url = this._urlResolver.resolveUrl(`/api/sites/${siteId}/water-supplier?waterSupplierId=${currentWaterSupplierId}`);

        const payload: SiteWaterSupplierUpdate = { waterSupplierId };

        const observable = this._http.put<void>(url, payload);

        await lastValueFrom(observable);
    }
}
