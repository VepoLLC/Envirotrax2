import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { lastValueFrom } from "rxjs";
import { UrlResolverService } from "../helpers/url-resolver.service";
import { Permission } from "../../models/users/permission";
import { RolePermission } from "../../models/users/role-permission";

@Injectable({
    providedIn: 'root'
})
export class RolePermissionService {
    constructor(
        private readonly _urlResolver: UrlResolverService,
        private readonly _httpClient: HttpClient
    ) {

    }

    public getAllPermissions(): Promise<Permission[]> {
        const url = this._urlResolver.resolveUrl('/api/users/roles/permissions');
        const observable = this._httpClient.get<Permission[]>(url);

        return lastValueFrom(observable);
    }

    public getAll(roleId: number): Promise<RolePermission[]> {
        const url = this._urlResolver.resolveUrl(`/api/users/roles/${roleId}/permissions`);
        const observable = this._httpClient.get<RolePermission[]>(url);

        return lastValueFrom(observable);
    }

    public addOrUpdate(rolePermission: RolePermission): Promise<RolePermission> {
        const url = this._urlResolver.resolveUrl(`/api/users/roles/${rolePermission.role!.id}/permissions/add-or-update`);
        const observable = this._httpClient.put<RolePermission>(url, rolePermission);

        return lastValueFrom(observable);
    }

    public bulkUpdate(roleId: number, rolePermissions: RolePermission[]): Promise<RolePermission[]> {
        const url = this._urlResolver.resolveUrl(`/api/users/roles/${roleId}/permissions/bulk-update`);
        const observable = this._httpClient.put<RolePermission[]>(url, rolePermissions);

        return lastValueFrom(observable);
    }
}