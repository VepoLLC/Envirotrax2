import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { PagedData, PageInfo, Query, QueryHelperService, UrlResolverService } from "@envirotrax/common-ui";
import {
    BackflowPaymentStatus,
    BackflowTest,
    BackflowTestCounts,
    BackflowTestDetails
} from "../../models/backflow/backflow-test";
import { RecordLog } from "../../models/logs/record-log";
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

    public async get(id: number): Promise<BackflowTestDetails> {
        const url = this._urlResolver.resolveUrl(`/api/backflow/tests/${id}`);

        const observable = this._http.get<BackflowTestDetails>(url);

        return await lastValueFrom(observable);
    }

    public async update(id: number, waterSupplierId: number, test: BackflowTestDetails): Promise<BackflowTestDetails> {
        const url = this._urlResolver.resolveUrl(`/api/backflow/tests/${id}?waterSupplierId=${waterSupplierId}`);

        const observable = this._http.put<BackflowTestDetails>(url, test);

        return await lastValueFrom(observable);
    }

    public async uploadImage(id: number, waterSupplierId: number, imageType: string, file: File): Promise<BackflowTestDetails> {
        const url = this._urlResolver.resolveUrl(`/api/backflow/tests/${id}/images/${imageType}?waterSupplierId=${waterSupplierId}`);

        const formData = new FormData();
        formData.append('file', file, file.name);

        const observable = this._http.post<BackflowTestDetails>(url, formData);

        return await lastValueFrom(observable);
    }

    public async getCounts(id: number): Promise<BackflowTestCounts> {
        const url = this._urlResolver.resolveUrl(`/api/backflow/tests/${id}/counts`);

        const observable = this._http.get<BackflowTestCounts>(url);

        return await lastValueFrom(observable);
    }

    public async getLogs(id: number): Promise<RecordLog[]> {
        const url = this._urlResolver.resolveUrl(`/api/backflow/tests/${id}/logs`);

        const observable = this._http.get<RecordLog[]>(url);

        return await lastValueFrom(observable);
    }
}
