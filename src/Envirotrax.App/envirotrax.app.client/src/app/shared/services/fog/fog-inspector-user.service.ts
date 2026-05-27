import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { lastValueFrom } from "rxjs";
import { UrlResolverService } from "../helpers/url-resolver.service";
import { QueryHelperService } from "../helpers/query-helper.service";
import { PageInfo } from "../../models/page-info";
import { Query } from "../../models/query";
import { PagedData } from "../../models/paged-data";
import { ProfessionalUser } from "../../models/professionals/professional-user";

@Injectable({
    providedIn: 'root'
})
export class FogInspectorSubAccountsService {
    constructor(
        private readonly _urlResolver: UrlResolverService,
        private readonly _queryHelper: QueryHelperService,
        private readonly _http: HttpClient
    ) {
    }

    public async getSubAccounts(inspectorId: number, pageInfo: PageInfo, query: Query): Promise<PagedData<ProfessionalUser>> {
        const url = this._urlResolver.resolveUrl(`/api/fog/inspectors/${inspectorId}/users`);
        return await lastValueFrom(this._http.get<PagedData<ProfessionalUser>>(url, {
            params: this._queryHelper.buildQuery(pageInfo, query)
        }));
    }
}
