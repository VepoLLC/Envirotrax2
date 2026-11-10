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
export class FogInspectorService {
    constructor(
        private readonly _urlResolver: UrlResolverService,
        private readonly _queryHelper: QueryHelperService,
        private readonly _http: HttpClient
    ) {
    }

    public async getAll(pageInfo: PageInfo, query: Query): Promise<PagedData<Professional>> {
        const url = this._urlResolver.resolveUrl('/api/fog/inspectors');

        const observable = this._http.get<PagedData<Professional>>(url, {
            params: this._queryHelper.buildQuery(pageInfo, query)
        });

        return await lastValueFrom(observable);
    }

    public async search(inspectorLicenseNumber: string | undefined, insurancePolicyNumber: string | undefined, pageInfo: PageInfo): Promise<PagedData<Professional>> {
        const url = this._urlResolver.resolveUrl('/api/fog/inspectors/search');
        let params = this._queryHelper.pageInfoToQueryString(pageInfo);

        if (inspectorLicenseNumber) {
            params = params.append('inspectorLicenseNumber', inspectorLicenseNumber);
        }
        if (insurancePolicyNumber) {
            params = params.append('insurancePolicyNumber', insurancePolicyNumber);
        }

        return await lastValueFrom(this._http.get<PagedData<Professional>>(url, { params }));
    }

    public getAllEndpoint(): DownloadEndpoint {
        return {
            method: 'GET',
            url: this._urlResolver.resolveUrl('/api/fog/inspectors')
        };
    }

    public getSearchEndpoint(inspectorLicenseNumber: string | undefined, insurancePolicyNumber: string | undefined): DownloadEndpoint {
        const additionalParams: { [key: string]: string } = {};

        if (inspectorLicenseNumber) {
            additionalParams['inspectorLicenseNumber'] = inspectorLicenseNumber;
        }
        if (insurancePolicyNumber) {
            additionalParams['insurancePolicyNumber'] = insurancePolicyNumber;
        }

        return {
            method: 'GET',
            url: this._urlResolver.resolveUrl('/api/fog/inspectors/search'),
            additionalParams
        };
    }
}
