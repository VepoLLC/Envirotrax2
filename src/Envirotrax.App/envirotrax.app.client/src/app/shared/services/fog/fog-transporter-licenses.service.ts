import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { lastValueFrom } from "rxjs";
import { UrlResolverService } from "../helpers/url-resolver.service";
import { QueryHelperService } from "../helpers/query-helper.service";
import { PageInfo } from "../../models/page-info";
import { Query } from "../../models/query";
import { PagedData } from "../../models/paged-data";
import { ProfessionalUserLicense } from "../../models/professionals/licenses/professional-user-license";
import { ProfessionalLicenseType } from "../../models/professionals/licenses/professional-license-type";
import { InputOption } from "@envirotrax/common-ui";

@Injectable({
    providedIn: 'root'
})
export class FogTransporterLicensesService {
    constructor(
        private readonly _urlResolver: UrlResolverService,
        private readonly _queryHelper: QueryHelperService,
        private readonly _http: HttpClient
    ) {
    }

    public async getLicenseTypes(query: Query = {}): Promise<InputOption<ProfessionalLicenseType>[]> {
        const url = this._urlResolver.resolveUrl('/api/fog/transporters/licenses/types');
        const types = await lastValueFrom(this._http.get<ProfessionalLicenseType[]>(url, {
            params: this._queryHelper.buildQuery({}, query)
        }));
        return types.map(t => ({ id: t.id, text: t.name, data: t }));
    }

    public async getLicenses(id: number, pageInfo: PageInfo, query: Query): Promise<PagedData<ProfessionalUserLicense>> {
        const url = this._urlResolver.resolveUrl(`/api/fog/transporters/${id}/licenses`);
        return await lastValueFrom(this._http.get<PagedData<ProfessionalUserLicense>>(url, {
            params: this._queryHelper.buildQuery(pageInfo, query)
        }));
    }

    public add(transporterId: number, license: ProfessionalUserLicense): Promise<ProfessionalUserLicense> {
        const url = this._urlResolver.resolveUrl(`/api/fog/transporters/${transporterId}/licenses`);
        return lastValueFrom(this._http.post<ProfessionalUserLicense>(url, license));
    }

    public update(transporterId: number, license: ProfessionalUserLicense): Promise<ProfessionalUserLicense> {
        const url = this._urlResolver.resolveUrl(`/api/fog/transporters/${transporterId}/licenses/${license.id}`);
        return lastValueFrom(this._http.put<ProfessionalUserLicense>(url, license));
    }
}
