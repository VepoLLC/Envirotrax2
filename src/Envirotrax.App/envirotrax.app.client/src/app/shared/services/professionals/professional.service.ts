import { Injectable } from "@angular/core";
import { UrlResolverService } from "../helpers/url-resolver.service";
import { QueryHelperService } from "../helpers/query-helper.service";
import { HttpClient } from "@angular/common/http";
import { PageInfo } from "../../models/page-info";
import { Query } from "../../models/query";
import { PagedData } from "../../models/paged-data";
import { Professional } from "../../models/professionals/professional";
import { lastValueFrom } from "rxjs";

@Injectable({
    providedIn: 'root'
})
export class ProfesisonalService {
    constructor(
        private readonly _urlResolver: UrlResolverService,
        private readonly _queryHelper: QueryHelperService,
        private readonly _http: HttpClient
    ) {

    }

    public getAllMy(pageInfo: PageInfo, query: Query): Promise<PagedData<Professional>> {
        const url = this._urlResolver.resolveUrl('/api/professionals/my');

        const observable = this._http.get<PagedData<Professional>>(url, {
            params: this._queryHelper.buildQuery(pageInfo, query)
        });

        return lastValueFrom(observable);
    }
}