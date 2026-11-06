import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { PagedData, PageInfo, Query, QueryHelperService, UrlResolverService } from "@envirotrax/common-ui";
import { lastValueFrom } from "rxjs";
import { ProfessionalLicenseType, ProfessionalUserLicense } from "../../models/professionals/licenses/professional-user-license";
import { LicenseAdminCounts, LicenseFilter, ProfessionalLicense, WaterSupplierLicense } from "../../models/professionals/licenses/water-supplier-license";
import { TceqLicense } from "../../models/professionals/licenses/tceq-license";

@Injectable({
    providedIn: 'root'
})
export class LicenseService {
    constructor(
        private readonly _urlResolver: UrlResolverService,
        private readonly _queryHelper: QueryHelperService,
        private readonly _http: HttpClient
    ) {

    }

    public getAll(licenseFilter: LicenseFilter, pageInfo: PageInfo, query: Query): Promise<PagedData<WaterSupplierLicense>> {
        const url = this._urlResolver.resolveUrl('/api/licenses');

        const params = this._queryHelper.buildQuery(pageInfo, query)
            .append('licenseFilter', licenseFilter);

        return lastValueFrom(this._http.get<PagedData<WaterSupplierLicense>>(url, { params }));
    }

    public getCounts(): Promise<LicenseAdminCounts> {
        const url = this._urlResolver.resolveUrl('/api/licenses/counts');

        return lastValueFrom(this._http.get<LicenseAdminCounts>(url));
    }

    public getTypes(): Promise<ProfessionalLicenseType[]> {
        const url = this._urlResolver.resolveUrl('/api/licenses/types');

        return lastValueFrom(this._http.get<ProfessionalLicenseType[]>(url));
    }

    public update(license: ProfessionalUserLicense): Promise<ProfessionalUserLicense> {
        const url = this._urlResolver.resolveUrl(`/api/licenses/${license.id}`);

        return lastValueFrom(this._http.put<ProfessionalUserLicense>(url, license));
    }

    public check(id: number): Promise<TceqLicense | null> {
        const url = this._urlResolver.resolveUrl(`/api/licenses/${id}/checks`);

        return lastValueFrom(this._http.post<TceqLicense | null>(url, null));
    }

    public updateCompanyLicense(license: ProfessionalLicense): Promise<ProfessionalLicense> {
        const url = this._urlResolver.resolveUrl(`/api/company-licenses/${license.id}`);

        return lastValueFrom(this._http.put<ProfessionalLicense>(url, license));
    }
}
