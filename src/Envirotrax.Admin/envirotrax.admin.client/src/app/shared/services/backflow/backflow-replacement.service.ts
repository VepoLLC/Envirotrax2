import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { PagedData, PageInfo, Query, QueryHelperService, UrlResolverService } from "@envirotrax/common-ui";
import { BackflowReplacement } from "../../models/backflow/backflow-replacement";
import { lastValueFrom } from "rxjs";

@Injectable({
    providedIn: 'root'
})
export class BackflowReplacementService {
    constructor(
        private readonly _urlResolver: UrlResolverService,
        private readonly _queryHelper: QueryHelperService,
        private readonly _http: HttpClient
    ) {

    }

    public async getAll(pageInfo: PageInfo, query: Query, onHold: boolean): Promise<PagedData<BackflowReplacement>> {
        const url = this._urlResolver.resolveUrl('/api/backflow/replacements');

        const params = this._queryHelper.buildQuery(pageInfo, query).append('onHold', String(onHold));

        const observable = this._http.get<PagedData<BackflowReplacement>>(url, { params });

        return await lastValueFrom(observable);
    }

    public async getReplacedAssembly(id: number): Promise<BackflowReplacement | null> {
        const url = this._urlResolver.resolveUrl(`/api/backflow/replacements/${id}/replaced-assembly`);

        const observable = this._http.get<BackflowReplacement | null>(url);

        return await lastValueFrom(observable);
    }

    public async updateHold(id: number, waterSupplierId: number, onHold: boolean): Promise<BackflowReplacement> {
        const url = this._urlResolver.resolveUrl(`/api/backflow/replacements/${id}/hold?waterSupplierId=${waterSupplierId}`);

        const observable = this._http.put<BackflowReplacement>(url, onHold);

        return await lastValueFrom(observable);
    }

    public async updateCleared(id: number, waterSupplierId: number, cleared: boolean): Promise<BackflowReplacement> {
        const url = this._urlResolver.resolveUrl(`/api/backflow/replacements/${id}/cleared?waterSupplierId=${waterSupplierId}`);

        const observable = this._http.put<BackflowReplacement>(url, cleared);

        return await lastValueFrom(observable);
    }
}
