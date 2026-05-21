import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { lastValueFrom } from "rxjs";
import { UrlResolverService } from "../helpers/url-resolver.service";
import { QueryHelperService } from "../helpers/query-helper.service";
import { PageInfo } from "../../models/page-info";
import { Query } from "../../models/query";
import { PagedData } from "../../models/paged-data";
import { ProfessionalUser } from "../../models/professionals/professional-user";

@Injectable({
    providedIn: 'root'
})
export class CsiInspectorSubAccountsService {
    constructor(
        private readonly _urlResolver: UrlResolverService,
        private readonly _queryHelper: QueryHelperService,
        private readonly _http: HttpClient
    ) {
    }

    public async getSubAccounts(inspectorId: number, pageInfo: PageInfo, query: Query): Promise<PagedData<ProfessionalUser>> {
        const url = this._urlResolver.resolveUrl(`/api/csi/inspectors/${inspectorId}/users`);
        return await lastValueFrom(this._http.get<PagedData<ProfessionalUser>>(url, {
            params: this._queryHelper.buildQuery(pageInfo, query)
        }));
    }

    public add(inspectorId: number, user: ProfessionalUser): Promise<ProfessionalUser> {
        const url = this._urlResolver.resolveUrl(`/api/csi/inspectors/${inspectorId}/users`);
        return lastValueFrom(this._http.post<ProfessionalUser>(url, user));
    }

    public update(inspectorId: number, user: ProfessionalUser): Promise<ProfessionalUser> {
        const url = this._urlResolver.resolveUrl(`/api/csi/inspectors/${inspectorId}/users/${user.id}`);
        return lastValueFrom(this._http.put<ProfessionalUser>(url, user));
    }

    public delete(inspectorId: number, userId: number): Promise<void> {
        const url = this._urlResolver.resolveUrl(`/api/csi/inspectors/${inspectorId}/users/${userId}`);
        return lastValueFrom(this._http.delete<void>(url));
    }
}
