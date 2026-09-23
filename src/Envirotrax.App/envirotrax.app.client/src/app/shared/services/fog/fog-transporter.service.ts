import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { lastValueFrom } from "rxjs";
import { UrlResolverService } from "../helpers/url-resolver.service";
import { QueryHelperService } from "../helpers/query-helper.service";
import { PageInfo } from "../../models/page-info";
import { Query } from "../../models/query";
import { PagedData } from "../../models/paged-data";
import { Professional } from "../../models/professionals/professional";
import { DownloadEndpoint } from "../../models/download-config";

@Injectable({
    providedIn: 'root'
})
export class FogTransporterService {
    constructor(
        private readonly _urlResolver: UrlResolverService,
        private readonly _queryHelper: QueryHelperService,
        private readonly _http: HttpClient
    ) {
    }

    public async getAll(pageInfo: PageInfo, query: Query): Promise<PagedData<Professional>> {
        const url = this._urlResolver.resolveUrl('/api/fog/transporters');

        const observable = this._http.get<PagedData<Professional>>(url, {
            params: this._queryHelper.buildQuery(pageInfo, query)
        });

        return await lastValueFrom(observable);
    }

    public getAllEndpoint(): DownloadEndpoint {
        const url = this._urlResolver.resolveUrl('/api/fog/transporters');

        return {
            method: 'GET',
            url: url
        };
    }

    public async search(registrationNumber: string | undefined, insurancePolicyNumber: string | undefined, pageInfo: PageInfo): Promise<PagedData<Professional>> {
        const url = this._urlResolver.resolveUrl('/api/fog/transporters/search');
        let params = this._queryHelper.pageInfoToQueryString(pageInfo);

        if (registrationNumber) {
            params = params.append('registrationNumber', registrationNumber);
        }
        if (insurancePolicyNumber) {
            params = params.append('insurancePolicyNumber', insurancePolicyNumber);
        }

        return await lastValueFrom(this._http.get<PagedData<Professional>>(url, { params }));
    }
}
