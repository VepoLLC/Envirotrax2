import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { PagedData, PageInfo, Query, QueryHelperService, UrlResolverService } from "@envirotrax/common-ui";
import { lastValueFrom } from "rxjs";
import { Professional } from "../../models/professionals/professional";

/**
 * Criteria that cannot ride the generic Query pipeline because they match a person inside the company
 * rather than a column on the company row.
 */
export interface FogInspectorSearchCriteria {
    userEmail?: string | null;
    contactName?: string | null;
    cellNumber?: string | null;
}

@Injectable({
    providedIn: 'root'
})
export class FogInspectorService {
    constructor(
        private readonly _urlResolver: UrlResolverService,
        private readonly _queryHelper: QueryHelperService,
        private readonly _http: HttpClient
    ) {

    }

    public async getAll(pageInfo: PageInfo, query: Query, criteria: FogInspectorSearchCriteria): Promise<PagedData<Professional>> {
        const url = this._urlResolver.resolveUrl('/api/fog/inspectors');

        let params = this._queryHelper.buildQuery(pageInfo, query);

        for (const [name, value] of Object.entries(criteria)) {
            if (value) {
                params = params.append(name, value);
            }
        }

        const observable = this._http.get<PagedData<Professional>>(url, { params });

        return await lastValueFrom(observable);
    }
}
