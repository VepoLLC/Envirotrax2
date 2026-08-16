import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { lastValueFrom } from "rxjs";
import { UrlResolverService } from "../helpers/url-resolver.service";
import { QueryHelperService } from "../helpers/query-helper.service";
import { PageInfo } from "../../models/page-info";
import { Query } from "../../models/query";
import { PagedData } from "../../models/paged-data";
import { FogInspection } from "../../models/fog/fog-inspection";

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

    public async getAll(pageInfo: PageInfo, query: Query, subAccountWaterSupplierId?: number | null): Promise<PagedData<FogInspection>> {
        const url = this._urlResolver.resolveUrl('/api/fog/inspections');

        let params = this._queryHelper.buildQuery(pageInfo, query);

        if (subAccountWaterSupplierId != null) {
            params = params.append('subAccountWaterSupplierId', String(subAccountWaterSupplierId));
        }

        const observable = this._http.get<PagedData<FogInspection>>(url, { params });

        return await lastValueFrom(observable);
    }

    public getById(id: number): Promise<FogInspection> {
        const url = this._urlResolver.resolveUrl(`/api/fog/inspections/${id}`);
        return lastValueFrom(this._http.get<FogInspection>(url));
    }
}
