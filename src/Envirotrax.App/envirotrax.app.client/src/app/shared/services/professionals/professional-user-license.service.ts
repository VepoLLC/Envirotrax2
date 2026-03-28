import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { lastValueFrom } from "rxjs";
import { UrlResolverService } from "../helpers/url-resolver.service";
import { QueryHelperService } from "../helpers/query-helper.service";
import { ProfessionalUserLicense } from "../../models/professionals/licenses/professional-user-license";
import { PagedData } from "../../models/paged-data";
import { PageInfo } from "../../models/page-info";
import { Query } from "../../models/query";
import { ProfessionalLicenseType } from "../../models/professionals/licenses/professional-license-type";
import { InputOption } from "../../components/input/input.component";

@Injectable({
    providedIn: 'root'
})
export class ProfessionalUserLicenseService {
    constructor(
        private readonly _urlResolver: UrlResolverService,
        private readonly _queryHelper: QueryHelperService,
        private readonly _http: HttpClient
    ) {

    }

    public getAllTypes(query: Query): Promise<ProfessionalLicenseType[]> {
        const url = this._urlResolver.resolveUrl('/api/professionals/licenses/types');

        const observable = this._http.get<ProfessionalLicenseType[]>(url, {
            params: this._queryHelper.buildQuery({}, query)
        });

        return lastValueFrom(observable);
    }

    public async getAllTypesAsOptions(query: Query, includeEmpty: boolean): Promise<InputOption<ProfessionalLicenseType>[]> {
        const types = await this.getAllTypes(query);

        const options: InputOption<ProfessionalLicenseType>[] = types.map(t => ({
            id: t.id,
            text: t.name,
            data: t
        }));

        if (includeEmpty) {
            options.splice(0, 0, { id: '', text: '' });
        }

        return options;
    }

    public getForUser(userId: number, pageInfo: PageInfo, query: Query): Promise<PagedData<ProfessionalUserLicense>> {
        const url = this._urlResolver.resolveUrl(`/api/professionals/${userId}/licenses`);

        const observable = this._http.get<PagedData<ProfessionalUserLicense>>(url, {
            params: this._queryHelper.buildQuery(pageInfo, query)
        });

        return lastValueFrom(observable);
    }

    public get(id: number): Promise<ProfessionalUserLicense> {
        const url = this._urlResolver.resolveUrl(`/api/professionals/licenses/${id}`);
        const observable = this._http.get<ProfessionalUserLicense>(url);

        return lastValueFrom(observable);
    }

    public add(license: ProfessionalUserLicense): Promise<ProfessionalUserLicense> {
        const url = this._urlResolver.resolveUrl('/api/professionals/licenses');
        return lastValueFrom(this._http.post<ProfessionalUserLicense>(url, license));
    }

    public update(license: ProfessionalUserLicense): Promise<ProfessionalUserLicense> {
        const url = this._urlResolver.resolveUrl(`/api/professionals/licenses/${license.id}`);
        const observable = this._http.put<ProfessionalUserLicense>(url, license);

        return lastValueFrom(observable);
    }

    public delete(id: number): Promise<ProfessionalUserLicense> {
        const url = this._urlResolver.resolveUrl(`/api/professionals/licenses/${id}`);
        return lastValueFrom(this._http.delete<ProfessionalUserLicense>(url));
    }
}
