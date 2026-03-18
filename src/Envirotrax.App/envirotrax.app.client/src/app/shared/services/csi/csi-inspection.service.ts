import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { lastValueFrom } from "rxjs";
import { UrlResolverService } from "../helpers/url-resolver.service";
import { QueryHelperService } from "../helpers/query-helper.service";
import { PageInfo } from "../../models/page-info";
import { Query } from "../../models/query";
import { PagedData } from "../../models/paged-data";
import { CsiInspection } from "../../models/csi/csi-inspection";

@Injectable({
    providedIn: 'root'
})
export class CsiInspectionService {
    constructor(
        private readonly _urlResolver: UrlResolverService,
        private readonly _queryHelper: QueryHelperService,
        private readonly _http: HttpClient
    ) {
    }

    public async getAll(pageInfo: PageInfo, query: Query): Promise<PagedData<CsiInspection>> {
        const url = this._urlResolver.resolveUrl('/api/csi/inspections');

        const observable = this._http.get<PagedData<CsiInspection>>(url, {
            params: this._queryHelper.buildQuery(pageInfo, query)
        });

        return await lastValueFrom(observable);
    }

    public get(id: number): Promise<CsiInspection> {
        const url = this._urlResolver.resolveUrl(`/api/csi/inspections/${id}`);

        return lastValueFrom(
            this._http.get<CsiInspection>(url)
        );
    }

    public add(inspection: CsiInspection): Promise<CsiInspection> {
        const url = this._urlResolver.resolveUrl('/api/csi/inspections');

        return lastValueFrom(
            this._http.post<CsiInspection>(url, inspection)
        );
    }

    public update(inspection: CsiInspection): Promise<CsiInspection> {
        const url = this._urlResolver.resolveUrl(`/api/csi/inspections/${inspection.id}`);

        return lastValueFrom(
            this._http.put<CsiInspection>(url, inspection)
        );
    }

    public delete(id: number): Promise<CsiInspection> {
        const url = this._urlResolver.resolveUrl(`/api/csi/inspections/${id}`);

        return lastValueFrom(
            this._http.delete<CsiInspection>(url)
        );
    }
}
