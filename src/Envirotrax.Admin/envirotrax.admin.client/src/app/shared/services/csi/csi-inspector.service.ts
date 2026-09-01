import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { PagedData, PageInfo, Query, QueryHelperService, UrlResolverService } from "@envirotrax/common-ui";
import { lastValueFrom } from "rxjs";
import { CsiInspectorAccount, CsiInspectorAccountDetails } from "../../models/csi/csi-inspector-account";

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

    public async getAll(
        pageInfo: PageInfo,
        query: Query,
        licenseNumber?: string | null,
        insuranceNumber?: string | null
    ): Promise<PagedData<CsiInspectorAccount>> {
        const url = this._urlResolver.resolveUrl('/api/csi/inspectors');

        let params = this._queryHelper.buildQuery(pageInfo, query);

        if (licenseNumber) {
            params = params.append('licenseNumber', licenseNumber);
        }

        if (insuranceNumber) {
            params = params.append('insuranceNumber', insuranceNumber);
        }

        const observable = this._http.get<PagedData<CsiInspectorAccount>>(url, { params });

        return await lastValueFrom(observable);
    }

    public async getDetails(professionalId: number, userId?: number | null): Promise<CsiInspectorAccountDetails> {
        const url = this._urlResolver.resolveUrl(`/api/csi/inspectors/${professionalId}`);

        const options = userId ? { params: { userId } } : {};
        const observable = this._http.get<CsiInspectorAccountDetails>(url, options);

        return await lastValueFrom(observable);
    }

    public async updateDetails(professionalId: number, details: CsiInspectorAccountDetails): Promise<CsiInspectorAccountDetails> {
        const url = this._urlResolver.resolveUrl(`/api/csi/inspectors/${professionalId}`);

        const observable = this._http.put<CsiInspectorAccountDetails>(url, details);

        return await lastValueFrom(observable);
    }
}
