import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { lastValueFrom } from "rxjs";
import { UrlResolverService } from "../helpers/url-resolver.service";
import { QueryHelperService } from "../helpers/query-helper.service";
import { ProfessionalInsurance } from "../../models/professionals/professional-insurance";
import { PagedData } from "../../models/paged-data";
import { PageInfo } from "../../models/page-info";
import { Query } from "../../models/query";

@Injectable({
    providedIn: 'root'
})
export class ProfessionalInsuranceService {
    constructor(
        private readonly _urlResolver: UrlResolverService,
        private readonly _queryHelper: QueryHelperService,
        private readonly _http: HttpClient
    ) {

    }

    public getAll(pageInfo: PageInfo, query: Query): Promise<PagedData<ProfessionalInsurance>> {
        const url = this._urlResolver.resolveUrl('/api/professionals/insurances');

        const observable = this._http.get<PagedData<ProfessionalInsurance>>(url, {
            params: this._queryHelper.buildQuery(pageInfo, query)
        });

        return lastValueFrom(observable);
    }

    public get(id: number): Promise<ProfessionalInsurance> {
        const url = this._urlResolver.resolveUrl(`/api/professionals/insurances/${id}`);
        return lastValueFrom(this._http.get<ProfessionalInsurance>(url));
    }

    public add(insurance: ProfessionalInsurance, file: File): Promise<ProfessionalInsurance> {
        const url = this._urlResolver.resolveUrl('/api/professionals/insurances');

        const formData = new FormData();
        formData.append('file', file);

        if (insurance.id) formData.append('id', insurance.id.toString());
        if (insurance.professional?.id) formData.append('professional.id', insurance.professional.id.toString());
        if (insurance.insuranceNumber) formData.append('insuranceNumber', insurance.insuranceNumber);
        if (insurance.expirationDate) formData.append('expirationDate', insurance.expirationDate.toISOString());

        return lastValueFrom(this._http.post<ProfessionalInsurance>(url, formData));
    }

    public update(insurance: ProfessionalInsurance): Promise<ProfessionalInsurance> {
        const url = this._urlResolver.resolveUrl(`/api/professionals/insurances/${insurance.id}`);
        return lastValueFrom(this._http.put<ProfessionalInsurance>(url, insurance));
    }

    public delete(id: number): Promise<ProfessionalInsurance> {
        const url = this._urlResolver.resolveUrl(`/api/professionals/insurances/${id}`);
        return lastValueFrom(this._http.delete<ProfessionalInsurance>(url));
    }

    public reactivate(id: number): Promise<ProfessionalInsurance> {
        const url = this._urlResolver.resolveUrl(`/api/professionals/insurances/${id}/reactivate`);
        return lastValueFrom(this._http.delete<ProfessionalInsurance>(url));
    }
}
