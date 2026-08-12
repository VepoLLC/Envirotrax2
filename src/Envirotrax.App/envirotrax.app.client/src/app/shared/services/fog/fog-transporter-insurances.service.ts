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
export class FogTransporterInsurancesService {
    constructor(
        private readonly _urlResolver: UrlResolverService,
        private readonly _queryHelper: QueryHelperService,
        private readonly _http: HttpClient
    ) {
    }

    public async getInsurances(id: number, pageInfo: PageInfo, query: Query): Promise<PagedData<ProfessionalInsurance>> {
        const url = this._urlResolver.resolveUrl(`/api/fog/transporters/${id}/insurances`);
        return await lastValueFrom(this._http.get<PagedData<ProfessionalInsurance>>(url, {
            params: this._queryHelper.buildQuery(pageInfo, query)
        }));
    }

    public add(transporterId: number, insurance: ProfessionalInsurance, file: File | null): Promise<ProfessionalInsurance> {
        const url = this._urlResolver.resolveUrl(`/api/fog/transporters/${transporterId}/insurances`);
        const formData = new FormData();

        formData.append('professional.id', transporterId.toString());
        formData.append('insuranceNumber', insurance.insuranceNumber ?? '');

        if (insurance.expirationDate) {
            formData.append('expirationDate', insurance.expirationDate.toString());
        }

        if (file) {
            formData.append('file', file);
        }

        return lastValueFrom(this._http.post<ProfessionalInsurance>(url, formData));
    }

    public update(transporterId: number, insurance: ProfessionalInsurance): Promise<ProfessionalInsurance> {
        const url = this._urlResolver.resolveUrl(`/api/fog/transporters/${transporterId}/insurances/${insurance.id}`);
        return lastValueFrom(this._http.put<ProfessionalInsurance>(url, insurance));
    }

    public async getFileUrl(transporterId: number, insuranceId: number): Promise<string> {
        const url = this._urlResolver.resolveUrl(`/api/fog/transporters/${transporterId}/insurances/${insuranceId}/file-url`);
        return await lastValueFrom(this._http.get<string>(url));
    }

    public delete(transporterId: number, insuranceId: number): Promise<void> {
        const url = this._urlResolver.resolveUrl(`/api/fog/transporters/${transporterId}/insurances/${insuranceId}`);
        return lastValueFrom(this._http.delete<void>(url));
    }
}
