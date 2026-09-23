import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { PagedData, PageInfo, Query, QueryHelperService, UrlResolverService } from "@envirotrax/common-ui";
import { lastValueFrom } from "rxjs";
import { ProfessionalInsurance } from "../../models/professionals/professional-insurance";

@Injectable({
    providedIn: 'root'
})
export class CsiInspectorInsuranceService {
    constructor(
        private readonly _urlResolver: UrlResolverService,
        private readonly _queryHelper: QueryHelperService,
        private readonly _http: HttpClient
    ) {

    }

    public getAll(professionalId: number, pageInfo: PageInfo, query: Query): Promise<PagedData<ProfessionalInsurance>> {
        const url = this._urlResolver.resolveUrl(`/api/csi/inspectors/${professionalId}/insurances`);

        return lastValueFrom(this._http.get<PagedData<ProfessionalInsurance>>(url, {
            params: this._queryHelper.buildQuery(pageInfo, query)
        }));
    }

    public add(professionalId: number, insurance: ProfessionalInsurance, file: File): Promise<ProfessionalInsurance> {
        const url = this._urlResolver.resolveUrl(`/api/csi/inspectors/${professionalId}/insurances`);

        const formData = new FormData();
        formData.append('insuranceNumber', insurance.insuranceNumber ?? '');

        if (insurance.expirationDate) {
            formData.append('expirationDate', insurance.expirationDate);
        }

        formData.append('file', file);

        return lastValueFrom(this._http.post<ProfessionalInsurance>(url, formData));
    }

    public update(professionalId: number, insurance: ProfessionalInsurance): Promise<ProfessionalInsurance> {
        const url = this._urlResolver.resolveUrl(`/api/csi/inspectors/${professionalId}/insurances/${insurance.id}`);

        return lastValueFrom(this._http.put<ProfessionalInsurance>(url, insurance));
    }

    public getFileUrl(professionalId: number, insuranceId: number): Promise<string> {
        const url = this._urlResolver.resolveUrl(`/api/csi/inspectors/${professionalId}/insurances/${insuranceId}/file-url`);

        return lastValueFrom(this._http.get<string>(url));
    }

    public delete(professionalId: number, insuranceId: number): Promise<void> {
        const url = this._urlResolver.resolveUrl(`/api/csi/inspectors/${professionalId}/insurances/${insuranceId}`);

        return lastValueFrom(this._http.delete<void>(url));
    }
}
