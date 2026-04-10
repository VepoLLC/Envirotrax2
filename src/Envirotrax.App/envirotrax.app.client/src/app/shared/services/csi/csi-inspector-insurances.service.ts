import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { lastValueFrom } from "rxjs";
import { UrlResolverService } from "../helpers/url-resolver.service";
import { QueryHelperService } from "../helpers/query-helper.service";
import { PageInfo } from "../../models/page-info";
import { Query } from "../../models/query";
import { PagedData } from "../../models/paged-data";
import { ProfessionalInsurance } from "../../models/professionals/professional-insurance";

@Injectable({
    providedIn: 'root'
})
export class CsiInspectorInsurancesService {
    constructor(
        private readonly _urlResolver: UrlResolverService,
        private readonly _queryHelper: QueryHelperService,
        private readonly _http: HttpClient
    ) {
    }

    public async getInsurances(id: number, pageInfo: PageInfo, query: Query): Promise<PagedData<ProfessionalInsurance>> {
        const url = this._urlResolver.resolveUrl(`/api/csi/inspectors/${id}/insurances`);
        return await lastValueFrom(this._http.get<PagedData<ProfessionalInsurance>>(url, {
            params: this._queryHelper.buildQuery(pageInfo, query)
        }));
    }
}
