import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { PagedData, PageInfo, Query, QueryHelperService, UrlResolverService } from "@envirotrax/common-ui";
import { lastValueFrom } from "rxjs";
import { FogInspection, FogPaymentStatus, FogTotalCapacityRange } from "../../models/fog/fog-inspection";

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

    public async getAll(
        pageInfo: PageInfo,
        query: Query,
        paymentStatus?: FogPaymentStatus | null,
        totalCapacityRange?: FogTotalCapacityRange | null
    ): Promise<PagedData<FogInspection>> {
        const url = this._urlResolver.resolveUrl('/api/fog/inspections');

        let params = this._queryHelper.buildQuery(pageInfo, query);

        if (paymentStatus != null) {
            params = params.append('paymentStatus', String(paymentStatus));
        }

        if (totalCapacityRange != null) {
            params = params.append('totalCapacityRange', String(totalCapacityRange));
        }

        const observable = this._http.get<PagedData<FogInspection>>(url, { params });

        return await lastValueFrom(observable);
    }
}
