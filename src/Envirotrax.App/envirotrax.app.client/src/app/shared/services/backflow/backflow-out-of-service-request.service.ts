import { Injectable } from "@angular/core";
import { HttpClient, HttpParams } from "@angular/common/http";
import { lastValueFrom } from "rxjs";
import { UrlResolverService } from "../helpers/url-resolver.service";
import { QueryHelperService } from "../helpers/query-helper.service";
import { PageInfo } from "../../models/page-info";
import { Query } from "../../models/query";
import { PagedData } from "../../models/paged-data";
import { BackflowTest } from "../../models/backflow/backflow-test";
import { BackflowOutOfServiceRequest } from "../../models/backflow/backflow-out-of-service-request";
import { OutOfServiceRequestStatusFilter } from "../../models/backflow/out-of-service-request-status-filter.enum";
import { OutOfServiceType } from "../../models/backflow/out-of-service-type.enum";

@Injectable({
    providedIn: 'root'
})
export class BackflowOutOfServiceRequestService {
    constructor(
        private readonly _urlResolver: UrlResolverService,
        private readonly _queryHelper: QueryHelperService,
        private readonly _http: HttpClient
    ) {
    }

    public async getReplacementCandidates(testId: number): Promise<BackflowTest[]> {
        const url = this._urlResolver.resolveUrl('/api/professionals/backflow/out-of-service-requests/replacement-candidates');
        const params = new HttpParams().set('testId', String(testId));

        return await lastValueFrom(this._http.get<BackflowTest[]>(url, { params }));
    }

    public async submit(request: BackflowOutOfServiceRequest): Promise<BackflowOutOfServiceRequest> {
        const url = this._urlResolver.resolveUrl('/api/professionals/backflow/out-of-service-requests');

        return await lastValueFrom(this._http.post<BackflowOutOfServiceRequest>(url, request));
    }

    public async getAllForWaterSupplier(
        pageInfo: PageInfo,
        query: Query,
        status: OutOfServiceRequestStatusFilter,
        type?: OutOfServiceType
    ): Promise<PagedData<BackflowOutOfServiceRequest>> {
        const url = this._urlResolver.resolveUrl('/api/backflow/out-of-service-requests');
        let params = this._queryHelper.buildQuery(pageInfo, query);
        params = params.append('status', String(status));

        if (type != null) {
            params = params.append('type', String(type));
        }

        return await lastValueFrom(this._http.get<PagedData<BackflowOutOfServiceRequest>>(url, { params }));
    }

    public async clear(id: number): Promise<void> {
        const url = this._urlResolver.resolveUrl('/api/backflow/out-of-service-requests/' + id + '/clear');

        await lastValueFrom(this._http.put<void>(url, {}));
    }
}
