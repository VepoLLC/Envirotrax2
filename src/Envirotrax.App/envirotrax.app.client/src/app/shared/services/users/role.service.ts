import { Injectable } from "@angular/core";
import { UrlResolverService } from "../helpers/url-resolver.service";
import { QueryHelperService } from "../helpers/query-helper.service";
import { HttpClient } from "@angular/common/http";
import { PageInfo } from "../../models/page-info";
import { PagedData } from "../../models/paged-data";
import { Role } from "../../models/users/role";
import { lastValueFrom } from "rxjs";
import { Query } from "../../models/query";

@Injectable()
export class RoleService {
    constructor(
        private readonly _urlResolver: UrlResolverService,
        private readonly _queryHelper: QueryHelperService,
        private readonly _httpClient: HttpClient
    ) {

    }

    public getAll(pageInfo: PageInfo, query: Query): Promise<PagedData<Role>> {
        const url = this._urlResolver.resolveUrl('/api/users/roles');

        const observable = this._httpClient.get<PagedData<Role>>(url, {
            params: this._queryHelper.buildQuery(pageInfo, query)
        });

        return lastValueFrom(observable)
    }

    public get(id: number): Promise<Role> {
        const url = this._urlResolver.resolveUrl(`/api/users/roles/${id}`);
        const observable = this._httpClient.get<Role>(url);

        return lastValueFrom(observable);
    }

    public add(role: Role): Promise<Role> {
        const url = this._urlResolver.resolveUrl('/api/users/roles');
        const observable = this._httpClient.post<Role>(url, role);

        return lastValueFrom(observable);
    }

    public update(role: Role): Promise<Role> {
        const url = this._urlResolver.resolveUrl(`/api/users/roles/${role.id}`);
        const observable = this._httpClient.put<Role>(url, role);

        return lastValueFrom(observable);
    }

    public delete(id: number): Promise<Role> {
        const url = this._urlResolver.resolveUrl(`/api/users/roles/${id}`);
        const observable = this._httpClient.delete<Role>(url);

        return lastValueFrom(observable);
    }

    public reactivate(id: number): Promise<Role> {
        const url = this._urlResolver.resolveUrl(`/api/users/roles/${id}/reactivate`);
        const observable = this._httpClient.delete<Role>(url);

        return lastValueFrom(observable);
    }
}