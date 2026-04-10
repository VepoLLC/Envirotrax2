import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { lastValueFrom } from "rxjs";
import { UrlResolverService } from "../helpers/url-resolver.service";
import { QueryHelperService } from "../helpers/query-helper.service";
import { PageInfo } from "../../models/page-info";
import { Query } from "../../models/query";
import { PagedData } from "../../models/paged-data";
import { ProfessionalWaterSupplier } from "../../models/professionals/professional-water-supplier";

@Injectable({
    providedIn: 'root'
})
export class CsiInspectorWaterSuppliersService {
    constructor(
        private readonly _urlResolver: UrlResolverService,
        private readonly _queryHelper: QueryHelperService,
        private readonly _http: HttpClient
    ) {
    }

    public async getWaterSuppliers(id: number, pageInfo: PageInfo, query: Query): Promise<PagedData<ProfessionalWaterSupplier>> {
        const url = this._urlResolver.resolveUrl(`/api/csi/inspectors/${id}/water-suppliers`);
        return await lastValueFrom(this._http.get<PagedData<ProfessionalWaterSupplier>>(url, {
            params: this._queryHelper.buildQuery(pageInfo, query)
        }));
    }
}
