import { Injectable } from "@angular/core";
import { UrlResolverService } from "../helpers/url-resolver.service";
import { QueryHelperService } from "../helpers/query-helper.service";
import { HttpClient } from "@angular/common/http";
import { ProfessionalUser } from "../../models/professionals/professional-user";
import { lastValueFrom, Observable } from "rxjs";
import { ProfesisonalService } from "./professional.service";
import { PageInfo } from "../../models/page-info";
import { Query } from "../../models/query";
import { PagedData } from "../../models/paged-data";

@Injectable({
    providedIn: 'root'
})
export class ProfesionalUserService {
    private _currentUser$?: Observable<ProfessionalUser>;

    constructor(
        private readonly _urlResolver: UrlResolverService,
        private readonly _queryHelper: QueryHelperService,
        private readonly _http: HttpClient,
        private readonly _professionalService: ProfesisonalService
    ) {
        _professionalService.onLoggedInPofessionalUpdated().subscribe(() => {
            this._currentUser$ = undefined;
        });
    }

    public async getMyData(): Promise<ProfessionalUser> {
        const url = this._urlResolver.resolveUrl('/api/professionals/users/my');

        if (!this._currentUser$) {
            this._currentUser$ = this._http.get<ProfessionalUser>(url);
        }

        return lastValueFrom(this._currentUser$);
    }

    public async updateMyData(user: ProfessionalUser): Promise<ProfessionalUser> {
        const url = this._urlResolver.resolveUrl('/api/professionals/users/my');

        const observable = this._http.put<ProfessionalUser>(url, user);
        this._currentUser$ = undefined;

        return lastValueFrom(observable);
    }

    public getAll(pageInfo: PageInfo, query: Query): Promise<PagedData<ProfessionalUser>> {
        const url = this._urlResolver.resolveUrl('/api/professionals/users');

        return lastValueFrom(
            this._http.get<PagedData<ProfessionalUser>>(url, {
                params: this._queryHelper.buildQuery(pageInfo, query)
            })
        );
    }

    public get(id: number): Promise<ProfessionalUser> {
        const url = this._urlResolver.resolveUrl(`/api/professionals/users/${id}`);
        return lastValueFrom(this._http.get<ProfessionalUser>(url));
    }

    public add(user: ProfessionalUser): Promise<ProfessionalUser> {
        const url = this._urlResolver.resolveUrl('/api/professionals/users');
        return lastValueFrom(this._http.post<ProfessionalUser>(url, user));
    }

    public update(user: ProfessionalUser): Promise<ProfessionalUser> {
        const url = this._urlResolver.resolveUrl(`/api/professionals/users/${user.id}`);
        return lastValueFrom(this._http.put<ProfessionalUser>(url, user));
    }

    public delete(id: number): Promise<ProfessionalUser> {
        const url = this._urlResolver.resolveUrl(`/api/professionals/users/${id}`);
        return lastValueFrom(this._http.delete<ProfessionalUser>(url));
    }

    public resendInvitation(id: number): Promise<void> {
        const url = this._urlResolver.resolveUrl(`/api/professionals/users/${id}/invitations`);
        return lastValueFrom(this._http.post<void>(url, null));
    }
}