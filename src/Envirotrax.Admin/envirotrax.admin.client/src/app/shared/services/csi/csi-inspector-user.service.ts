import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { InputOption, MAX_PAGE_SIZE, PagedData, PageInfo, Query, QueryHelperService, UrlResolverService } from "@envirotrax/common-ui";
import { lastValueFrom } from "rxjs";
import { ProfessionalUser } from "../../models/professionals/professional";

@Injectable({
    providedIn: 'root'
})
export class CsiInspectorUserService {
    constructor(
        private readonly _urlResolver: UrlResolverService,
        private readonly _queryHelper: QueryHelperService,
        private readonly _http: HttpClient
    ) {

    }

    public getAll(professionalId: number, pageInfo: PageInfo, query: Query): Promise<PagedData<ProfessionalUser>> {
        const url = this._urlResolver.resolveUrl(`/api/csi/inspectors/${professionalId}/users`);

        return lastValueFrom(this._http.get<PagedData<ProfessionalUser>>(url, {
            params: this._queryHelper.buildQuery(pageInfo, query)
        }));
    }

    public async getAllAsOptions(professionalId: number): Promise<InputOption<ProfessionalUser>[]> {
        const users = await this.getAll(
            professionalId,
            { pageNumber: 1, pageSize: MAX_PAGE_SIZE },
            { sort: { contactName: 'Asc' }, filter: [] }
        );

        return users.data.map(user => ({
            id: user.id,
            text: user.emailAddress ?? user.contactName ?? '',
            data: user
        }));
    }

    public add(professionalId: number, user: ProfessionalUser): Promise<ProfessionalUser> {
        const url = this._urlResolver.resolveUrl(`/api/csi/inspectors/${professionalId}/users`);

        return lastValueFrom(this._http.post<ProfessionalUser>(url, user));
    }

    public update(professionalId: number, user: ProfessionalUser): Promise<ProfessionalUser> {
        const url = this._urlResolver.resolveUrl(`/api/csi/inspectors/${professionalId}/users/${user.id}`);

        return lastValueFrom(this._http.put<ProfessionalUser>(url, user));
    }

    public delete(professionalId: number, userId: number): Promise<void> {
        const url = this._urlResolver.resolveUrl(`/api/csi/inspectors/${professionalId}/users/${userId}`);

        return lastValueFrom(this._http.delete<void>(url));
    }
}
