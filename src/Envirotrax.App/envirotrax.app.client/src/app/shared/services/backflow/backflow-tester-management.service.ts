import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { lastValueFrom } from "rxjs";
import { UrlResolverService } from "../helpers/url-resolver.service";
import { QueryHelperService } from "../helpers/query-helper.service";
import { PageInfo } from "../../models/page-info";
import { Query } from "../../models/query";
import { PagedData } from "../../models/paged-data";
import { Professional } from "../../models/professionals/professional";

/**
 * Criteria that cannot ride the generic Query pipeline because each one matches a child collection
 * (licences, insurances) or a person inside the company rather than a column on the company row.
 * Mirrors the server's BackflowTesterSearchDto.
 */
export interface BackflowTesterSearchCriteria {
    bpatLicenseNumber?: string | null;
    fireLicenseNumber?: string | null;
    insurancePolicyNumber?: string | null;
    userEmail?: string | null;
    contactName?: string | null;
    cellNumber?: string | null;
}

@Injectable({
    providedIn: 'root'
})
export class BackflowTesterManagementService {
    constructor(
        private readonly _urlResolver: UrlResolverService,
        private readonly _queryHelper: QueryHelperService,
        private readonly _http: HttpClient
    ) {
    }

    public async getAll(pageInfo: PageInfo, query: Query): Promise<PagedData<Professional>> {
        const url = this._urlResolver.resolveUrl('/api/backflow/testers');

        const observable = this._http.get<PagedData<Professional>>(url, {
            params: this._queryHelper.buildQuery(pageInfo, query)
        });

        return await lastValueFrom(observable);
    }

        public async getAccountInfo(id: number): Promise<Professional> {
        const url = this._urlResolver.resolveUrl(`/api/backflow/testers/${id}`);
        return await lastValueFrom(this._http.get<Professional>(url));
    }

    public async search(criteria: BackflowTesterSearchCriteria, pageInfo: PageInfo, query: Query): Promise<PagedData<Professional>> {
        const url = this._urlResolver.resolveUrl('/api/backflow/testers/search');

        let params = this._queryHelper.buildQuery(pageInfo, query);

        for (const [name, value] of Object.entries(criteria)) {
            if (value) {
                params = params.append(name, value);
            }
        }

        return await lastValueFrom(this._http.get<PagedData<Professional>>(url, { params }));
    }
}
