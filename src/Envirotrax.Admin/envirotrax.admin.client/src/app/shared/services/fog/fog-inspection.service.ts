import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { PagedData, PageInfo, Query, QueryHelperService, RecordLog, UrlResolverService } from "@envirotrax/common-ui";
import { lastValueFrom } from "rxjs";
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

    public async getAll(pageInfo: PageInfo, query: Query): Promise<PagedData<FogInspection>> {
        const url = this._urlResolver.resolveUrl('/api/fog/inspections');

        const observable = this._http.get<PagedData<FogInspection>>(url, {
            params: this._queryHelper.buildQuery(pageInfo, query)
        });

        return await lastValueFrom(observable);
    }

    public async get(id: number): Promise<FogInspection> {
        const url = this._urlResolver.resolveUrl(`/api/fog/inspections/${id}`);

        return await lastValueFrom(this._http.get<FogInspection>(url));
    }

    public async getLogs(id: number): Promise<RecordLog[]> {
        const url = this._urlResolver.resolveUrl(`/api/fog/inspections/${id}/logs`);

        return await lastValueFrom(this._http.get<RecordLog[]>(url));
    }
}
