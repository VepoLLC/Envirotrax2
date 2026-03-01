import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { lastValueFrom } from "rxjs";
import { UrlResolverService } from "../helpers/url-resolver.service";
import { UserRole } from "../../models/users/user-role";

@Injectable({
    providedIn: 'root'
})
export class UserRoleService {
    constructor(
        private readonly _urlResolver: UrlResolverService,
        private readonly _httpClient: HttpClient
    ) {

    }

    public getAll(userId: number): Promise<UserRole[]> {
        const url = this._urlResolver.resolveUrl(`/api/users/${userId}/roles`);
        const observable = this._httpClient.get<UserRole[]>(url);

        return lastValueFrom(observable);
    }

    public add(userRole: UserRole): Promise<UserRole> {
        const url = this._urlResolver.resolveUrl(`/api/users/${userRole.user!.id}/roles`);
        const observable = this._httpClient.post<UserRole>(url, userRole);

        return lastValueFrom(observable);
    }

    public delete(userId: number, roleId: number): Promise<UserRole> {
        const url = this._urlResolver.resolveUrl(`/api/users/${userId}/roles/${roleId}`);
        const observable = this._httpClient.delete<UserRole>(url);

        return lastValueFrom(observable);
    }
}