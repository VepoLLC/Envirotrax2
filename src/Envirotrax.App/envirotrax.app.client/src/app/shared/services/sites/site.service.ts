import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { lastValueFrom } from "rxjs";
import { UrlResolverService } from "../helpers/url-resolver.service";
import { QueryHelperService } from "../helpers/query-helper.service";
import { PageInfo } from "../../models/page-info";
import { Query } from "../../models/query";
import { PagedData } from "../../models/paged-data";
import { Site } from "../../models/sites/site";
import { DownloadEndpoint } from "../../models/download-config";

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

    public getAllEndpoint(): DownloadEndpoint {
        return {
            method: 'GET',
            url: this._urlResolver.resolveUrl('/api/sites'),
        };
    }

    public getAllForProfessionalEndpoint(): DownloadEndpoint {
        return {
            method: 'GET',
            url: this._urlResolver.resolveUrl('/api/professionals/sites'),
        };
    }

    public getCsiComplianceEndpoint(): DownloadEndpoint {
        return {
            method: 'GET',
            url: this._urlResolver.resolveUrl('/api/sites/csi-compliance'),
        };
    }

    public async getCsiCompliance(pageInfo: PageInfo, query: Query): Promise<PagedData<Site>> {
        const url = this._urlResolver.resolveUrl('/api/sites/csi-compliance');

        const observable = this._http.get<PagedData<Site>>(url, {
            params: this._queryHelper.buildQuery(pageInfo, query)
        });

        return await lastValueFrom(observable);
    }

    public updateCsiAssignment(siteId: number, userId: number | null): Promise<void> {
        const url = this._urlResolver.resolveUrl(`/api/sites/${siteId}/csi-assignment`);

        return lastValueFrom(
            this._http.put<void>(url, { userId })
        );
    }

    public updateBackflowAssignment(siteId: number, userId: number | null): Promise<void> {
        const url = this._urlResolver.resolveUrl(`/api/sites/${siteId}/backflow-assignment`);

        return lastValueFrom(
            this._http.put<void>(url, { userId })
        );
    }

    public getFogTripTicketComplianceEndpoint(): DownloadEndpoint {
        return {
            method: 'GET',
            url: this._urlResolver.resolveUrl('/api/sites/fog-trip-ticket-compliance'),
        };
    }

    public async getFogTripTicketCompliance(pageInfo: PageInfo, query: Query): Promise<PagedData<Site>> {
        const url = this._urlResolver.resolveUrl('/api/sites/fog-trip-ticket-compliance');

        const observable = this._http.get<PagedData<Site>>(url, {
            params: this._queryHelper.buildQuery(pageInfo, query)
        });

        return await lastValueFrom(observable);
    }

    public updateFogAssignment(siteId: number, userId: number | null): Promise<void> {
        const url = this._urlResolver.resolveUrl(`/api/sites/${siteId}/fog-assignment`);

        return lastValueFrom(
            this._http.put<void>(url, { userId })
        );
    }

    public async getAll(pageInfo: PageInfo, query: Query): Promise<PagedData<Site>> {
        const url = this._urlResolver.resolveUrl('/api/sites');

        const observable = this._http.get<PagedData<Site>>(url, {
            params: this._queryHelper.buildQuery(pageInfo, query)
        });

        return await lastValueFrom(observable);
    }

    public async getAllForProfessional(pageInfo: PageInfo, query: Query): Promise<PagedData<Site>> {
        const url = this._urlResolver.resolveUrl('/api/professionals/sites');
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

    public getForProfessional(id: number): Promise<Site> {
        const url = this._urlResolver.resolveUrl(`/api/professionals/sites/${id}`);

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

    public updateGisData(siteId: number, data: { gisLatitude?: number | null, gisLongitude?: number | null, gisStatus?: number }): Promise<void> {
        const url = this._urlResolver.resolveUrl(`/api/sites/${siteId}/gis-data`);
        return lastValueFrom(
            this._http.put<void>(url, {
                latitude: data.gisLatitude,
                longitude: data.gisLongitude,
                status: data.gisStatus
            })
        );
    }

    public delete(id: number): Promise<Site> {
        const url = this._urlResolver.resolveUrl(`/api/sites/${id}`);

        return lastValueFrom(
            this._http.delete<Site>(url)
        );
    }
}
