import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { lastValueFrom } from "rxjs";
import { UrlResolverService } from "../helpers/url-resolver.service";
import { QueryHelperService } from "../helpers/query-helper.service";
import { PageInfo } from "../../models/page-info";
import { Query } from "../../models/query";
import { PagedData } from "../../models/paged-data";
import { ProfessionalUserLicense } from "../../models/professionals/licenses/professional-user-license";

@Injectable({
    providedIn: 'root'
})
export class CsiInspectorLicensesService {
    constructor(
        private readonly _urlResolver: UrlResolverService,
        private readonly _queryHelper: QueryHelperService,
        private readonly _http: HttpClient
    ) {
    }

    public async getLicenses(id: number, pageInfo: PageInfo, query: Query): Promise<PagedData<ProfessionalUserLicense>> {
        const url = this._urlResolver.resolveUrl(`/api/csi/inspectors/${id}/licenses`);
        return await lastValueFrom(this._http.get<PagedData<ProfessionalUserLicense>>(url, {
            params: this._queryHelper.buildQuery(pageInfo, query)
        }));
    }
}
