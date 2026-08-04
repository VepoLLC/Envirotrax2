import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { PagedData, PageInfo, Query, QueryHelperService, UrlResolverService } from "@envirotrax/common-ui";
import { CsiInspection, CsiPaymentStatus } from "../../models/csi/csi-inspection";
import { lastValueFrom } from "rxjs";

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

    public async getAll(
        pageInfo: PageInfo,
        query: Query,
        paymentStatus?: CsiPaymentStatus | null,
        inspectorId?: number | null
    ): Promise<PagedData<CsiInspection>> {
        const url = this._urlResolver.resolveUrl('/api/csi/inspections');

        let params = this._queryHelper.buildQuery(pageInfo, query);

        if (paymentStatus != null) {
            params = params.append('paymentStatus', String(paymentStatus));
        }

        if (inspectorId != null) {
            params = params.append('inspectorId', String(inspectorId));
        }

        const observable = this._http.get<PagedData<CsiInspection>>(url, { params });

        return await lastValueFrom(observable);
    }
}
