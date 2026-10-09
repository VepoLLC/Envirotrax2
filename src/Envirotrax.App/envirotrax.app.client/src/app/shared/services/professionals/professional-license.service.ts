import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { lastValueFrom } from "rxjs";
import { UrlResolverService } from "../helpers/url-resolver.service";
import { QueryHelperService } from "../helpers/query-helper.service";
import { ProfessionalLicense } from "../../models/professionals/licenses/professional-license";
import { ProfessionalDashboardLicenseInsurance } from "../../models/professionals/professional-dashboard-license-insurance";
import { PagedData } from "../../models/paged-data";
import { PageInfo } from "../../models/page-info";
import { Query } from "../../models/query";

@Injectable({
    providedIn: 'root'
})
export class ProfessionalLicenseService {
    constructor(
        private readonly _urlResolver: UrlResolverService,
        private readonly _queryHelper: QueryHelperService,
        private readonly _http: HttpClient
    ) {

    }

    public getAllWithInsurances(pageInfo: PageInfo, query: Query): Promise<PagedData<ProfessionalDashboardLicenseInsurance>> {
        const url = this._urlResolver.resolveUrl('/api/professionals/company-licenses-and-insurances');

        const observable = this._http.get<PagedData<ProfessionalDashboardLicenseInsurance>>(url, {
            params: this._queryHelper.buildQuery(pageInfo, query)
        });

        return lastValueFrom(observable);
    }

    public get(id: number): Promise<ProfessionalLicense> {
        const url = this._urlResolver.resolveUrl(`/api/professionals/company-licenses/${id}`);
        return lastValueFrom(this._http.get<ProfessionalLicense>(url));
    }

    public add(license: ProfessionalLicense): Promise<ProfessionalLicense> {
        const url = this._urlResolver.resolveUrl('/api/professionals/company-licenses');
        return lastValueFrom(this._http.post<ProfessionalLicense>(url, license));
    }

    public update(license: ProfessionalLicense): Promise<ProfessionalLicense> {
        const url = this._urlResolver.resolveUrl(`/api/professionals/company-licenses/${license.id}`);
        return lastValueFrom(this._http.put<ProfessionalLicense>(url, license));
    }

    public delete(id: number): Promise<ProfessionalLicense> {
        const url = this._urlResolver.resolveUrl(`/api/professionals/company-licenses/${id}`);
        return lastValueFrom(this._http.delete<ProfessionalLicense>(url));
    }
}
