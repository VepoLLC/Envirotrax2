import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { PagedData, PageInfo, Query, QueryHelperService, UrlResolverService } from "@envirotrax/common-ui";
import { lastValueFrom } from "rxjs";
import { ProfessionalInsurance } from "../../models/professionals/professional-insurance";
import { WaterSupplierInsurance } from "../../models/professionals/water-supplier-insurance";

@Injectable({
    providedIn: 'root'
})
export class InsuranceService {
    constructor(
        private readonly _urlResolver: UrlResolverService,
        private readonly _queryHelper: QueryHelperService,
        private readonly _http: HttpClient
    ) {

    }

    public getAll(pageInfo: PageInfo, query: Query): Promise<PagedData<WaterSupplierInsurance>> {
        const url = this._urlResolver.resolveUrl('/api/insurances');

        return lastValueFrom(this._http.get<PagedData<WaterSupplierInsurance>>(url, {
            params: this._queryHelper.buildQuery(pageInfo, query)
        }));
    }

    public update(insurance: ProfessionalInsurance): Promise<ProfessionalInsurance> {
        const url = this._urlResolver.resolveUrl(`/api/insurances/${insurance.id}`);

        return lastValueFrom(this._http.put<ProfessionalInsurance>(url, insurance));
    }

    public getFileUrl(insuranceId: number): Promise<string> {
        const url = this._urlResolver.resolveUrl(`/api/insurances/${insuranceId}/file-url`);

        return lastValueFrom(this._http.get(url, { responseType: 'text' }));
    }
}
