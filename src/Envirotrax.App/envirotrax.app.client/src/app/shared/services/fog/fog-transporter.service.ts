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

export interface FogTransporterSearchCriteria {
    registrationNumber?: string | null;
    insurancePolicyNumber?: string | null;
    userEmail?: string | null;
    contactName?: string | null;
    cellNumber?: string | null;
}

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

    public async search(criteria: FogTransporterSearchCriteria, pageInfo: PageInfo, query: Query): Promise<PagedData<Professional>> {
        const url = this._urlResolver.resolveUrl('/api/fog/transporters/search');

        let params = this._queryHelper.buildQuery(pageInfo, query);

        for (const [name, value] of Object.entries(criteria)) {
            if (value) {
                params = params.append(name, value);
            }
        }

        return await lastValueFrom(this._http.get<PagedData<Professional>>(url, { params }));
    }

    public getSearchEndpoint(criteria: FogTransporterSearchCriteria): DownloadEndpoint {
        const additionalParams: { [key: string]: string } = {};

        for (const [name, value] of Object.entries(criteria)) {
            if (value) {
                additionalParams[name] = value;
            }
        }

        return {
            method: 'GET',
            url: this._urlResolver.resolveUrl('/api/fog/transporters/search'),
            additionalParams
        };
    }
}
