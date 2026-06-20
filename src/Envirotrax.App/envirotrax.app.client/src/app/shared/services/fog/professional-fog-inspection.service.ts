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
export class ProfessionalFogInspectionService {
    constructor(
        private readonly _urlResolver: UrlResolverService,
        private readonly _queryHelper: QueryHelperService,
        private readonly _http: HttpClient
    ) {
    }

    public getAll(pageInfo: PageInfo, query: Query, latestOnly: boolean): Promise<PagedData<FogInspection>> {
        const url = this._urlResolver.resolveUrl('/api/professionals/fog/inspections');
        let params = this._queryHelper.buildQuery(pageInfo, query);
        params = params.append('latestOnly', String(latestOnly));

        return lastValueFrom(this._http.get<PagedData<FogInspection>>(url, { params }));
    }

    public getById(id: number): Promise<FogInspection> {
        const url = this._urlResolver.resolveUrl(`/api/professionals/fog/inspections/${id}`);
        return lastValueFrom(this._http.get<FogInspection>(url));
    }

    public submit(inspection: FogInspection): Promise<FogInspection> {
        const url = this._urlResolver.resolveUrl('/api/professionals/fog/inspections/submit');
        return lastValueFrom(this._http.post<FogInspection>(url, inspection));
    }
}
