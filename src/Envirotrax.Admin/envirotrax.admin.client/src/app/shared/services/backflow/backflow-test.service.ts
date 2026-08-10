import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { PagedData, PageInfo, Query, QueryHelperService, UrlResolverService } from "@envirotrax/common-ui";
import { BackflowPaymentStatus, BackflowTest } from "../../models/backflow/backflow-test";
import { lastValueFrom } from "rxjs";

@Injectable({
    providedIn: 'root'
})
export class BackflowTestService {
    constructor(
        private readonly _urlResolver: UrlResolverService,
        private readonly _queryHelper: QueryHelperService,
        private readonly _http: HttpClient
    ) {

    }

    public async getAll(
        pageInfo: PageInfo,
        query: Query,
        paymentStatus?: BackflowPaymentStatus | null
    ): Promise<PagedData<BackflowTest>> {
        const url = this._urlResolver.resolveUrl('/api/backflow/tests');

        let params = this._queryHelper.buildQuery(pageInfo, query);

        if (paymentStatus != null) {
            params = params.append('paymentStatus', String(paymentStatus));
        }

        const observable = this._http.get<PagedData<BackflowTest>>(url, { params });

        return await lastValueFrom(observable);
    }
}
