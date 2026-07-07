import { Injectable } from "@angular/core";
import { HttpClient, HttpParams } from "@angular/common/http";
import { lastValueFrom } from "rxjs";
import { UrlResolverService } from "../helpers/url-resolver.service";
import { BackflowTest } from "../../models/backflow/backflow-test";
import { BackflowOutOfServiceRequest } from "../../models/backflow/backflow-out-of-service-request";

@Injectable({
    providedIn: 'root'
})
export class BackflowOutOfServiceRequestService {
    constructor(
        private readonly _urlResolver: UrlResolverService,
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
}
