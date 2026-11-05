import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { lastValueFrom } from "rxjs";
import { UrlResolverService } from "../helpers/url-resolver.service";
import { QueryHelperService } from "../helpers/query-helper.service";
import { PageInfo } from "../../models/page-info";
import { Query } from "../../models/query";
import { PagedData } from "../../models/paged-data";
import { FogInspection } from "../../models/fog/fog-inspection";
import { DownloadEndpoint } from "../../models/download-config";

@Injectable({
    providedIn: 'root'
})
export class FogInspectionService {
    constructor(
        private readonly _urlResolver: UrlResolverService,
        private readonly _queryHelper: QueryHelperService,
        private readonly _http: HttpClient
    ) {
    }

    public async getAll(pageInfo: PageInfo, query: Query): Promise<PagedData<FogInspection>> {
        const url = this._urlResolver.resolveUrl('/api/fog/inspections');

        const params = this._queryHelper.buildQuery(pageInfo, query);

        const observable = this._http.get<PagedData<FogInspection>>(url, { params });

        return await lastValueFrom(observable);
    }

    public getAllEndpoint(): DownloadEndpoint {
        return {
            method: 'GET',
            url: this._urlResolver.resolveUrl('/api/fog/inspections')
        };
    }

    public getAllPdfEndpoint(): DownloadEndpoint {
        return {
            method: 'GET',
            url: this._urlResolver.resolveUrl('/api/fog/inspections/pdf')
        };
    }

    public getById(id: number): Promise<FogInspection> {
        const url = this._urlResolver.resolveUrl(`/api/fog/inspections/${id}`);
        return lastValueFrom(this._http.get<FogInspection>(url));
    }

    public getPdf(id: number): Promise<Blob> {
        const url = this._urlResolver.resolveUrl(`/api/fog/inspections/${id}/pdf`);
        return lastValueFrom(this._http.get(url, { responseType: 'blob' }));
    }
}
