import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { InputOption, PagedData, PageInfo, Query, QueryHelperService, UrlResolverService } from "@envirotrax/common-ui";
import { lastValueFrom } from "rxjs";
import { ProfessionalLicenseType, ProfessionalUserLicense } from "../../models/professionals/licenses/professional-user-license";

@Injectable({
    providedIn: 'root'
})
export class CsiInspectorLicenseService {
    constructor(
        private readonly _urlResolver: UrlResolverService,
        private readonly _queryHelper: QueryHelperService,
        private readonly _http: HttpClient
    ) {

    }

    public async getTypesAsOptions(): Promise<InputOption<ProfessionalLicenseType>[]> {
        const url = this._urlResolver.resolveUrl('/api/csi/inspectors/licenses/types');

        const types = await lastValueFrom(this._http.get<ProfessionalLicenseType[]>(url));

        return types.map(type => ({ id: type.id, text: type.name ?? '', data: type }));
    }

    public getAll(professionalId: number, pageInfo: PageInfo, query: Query): Promise<PagedData<ProfessionalUserLicense>> {
        const url = this._urlResolver.resolveUrl(`/api/csi/inspectors/${professionalId}/licenses`);

        return lastValueFrom(this._http.get<PagedData<ProfessionalUserLicense>>(url, {
            params: this._queryHelper.buildQuery(pageInfo, query)
        }));
    }

    public add(professionalId: number, license: ProfessionalUserLicense): Promise<ProfessionalUserLicense> {
        const url = this._urlResolver.resolveUrl(`/api/csi/inspectors/${professionalId}/licenses`);

        return lastValueFrom(this._http.post<ProfessionalUserLicense>(url, license));
    }

    public update(professionalId: number, license: ProfessionalUserLicense): Promise<ProfessionalUserLicense> {
        const url = this._urlResolver.resolveUrl(`/api/csi/inspectors/${professionalId}/licenses/${license.id}`);

        return lastValueFrom(this._http.put<ProfessionalUserLicense>(url, license));
    }

    public delete(professionalId: number, licenseId: number): Promise<void> {
        const url = this._urlResolver.resolveUrl(`/api/csi/inspectors/${professionalId}/licenses/${licenseId}`);

        return lastValueFrom(this._http.delete<void>(url));
    }
}
