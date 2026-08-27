import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { PagedData, PageInfo, Query, QueryHelperService, UrlResolverService } from "@envirotrax/common-ui";
import { lastValueFrom } from "rxjs";
import { Professional } from "../../models/professionals/professional";

export interface CsiInspectorSearchCriteria {
    inspectorLicenseNumber?: string | null;
    insurancePolicyNumber?: string | null;
    userEmail?: string | null;
    contactName?: string | null;
}

@Injectable({
    providedIn: 'root'
})
export class CsiInspectorService {
    constructor(
        private readonly _urlResolver: UrlResolverService,
        private readonly _queryHelper: QueryHelperService,
        private readonly _http: HttpClient
    ) {

    }

    public async getAll(pageInfo: PageInfo, query: Query, criteria: CsiInspectorSearchCriteria): Promise<PagedData<Professional>> {
        const url = this._urlResolver.resolveUrl('/api/csi/inspectors');

        let params = this._queryHelper.buildQuery(pageInfo, query);

        if (criteria.inspectorLicenseNumber) {
            params = params.append('inspectorLicenseNumber', criteria.inspectorLicenseNumber);
        }

        if (criteria.insurancePolicyNumber) {
            params = params.append('insurancePolicyNumber', criteria.insurancePolicyNumber);
        }

        if (criteria.userEmail) {
            params = params.append('userEmail', criteria.userEmail);
        }

        if (criteria.contactName) {
            params = params.append('contactName', criteria.contactName);
        }

        const observable = this._http.get<PagedData<Professional>>(url, { params });

        return await lastValueFrom(observable);
    }
}
