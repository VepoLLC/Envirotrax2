import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { PagedData, PageInfo, Query, QueryHelperService, UrlResolverService } from "@envirotrax/common-ui";
import { lastValueFrom } from "rxjs";
import { BackflowTesterAccount } from "../../models/backflow/backflow-tester-account";

@Injectable({
    providedIn: 'root'
})
export class BackflowTesterService {
    constructor(
        private readonly _urlResolver: UrlResolverService,
        private readonly _queryHelper: QueryHelperService,
        private readonly _http: HttpClient
    ) {

    }

    public async getAll(
        pageInfo: PageInfo,
        query: Query,
        licenseNumber?: string | null,
        insuranceNumber?: string | null
    ): Promise<PagedData<BackflowTesterAccount>> {
        const url = this._urlResolver.resolveUrl('/api/backflow/testers');

        let params = this._queryHelper.buildQuery(pageInfo, query);

        if (licenseNumber) {
            params = params.append('licenseNumber', licenseNumber);
        }

        if (insuranceNumber) {
            params = params.append('insuranceNumber', insuranceNumber);
        }

        const observable = this._http.get<PagedData<BackflowTesterAccount>>(url, { params });

        return await lastValueFrom(observable);
    }
}
