import { Injectable } from "@angular/core";
import { UrlResolverService } from "../helpers/url-resolver.service";
import { QueryHelperService } from "../helpers/query-helper.service";
import { HttpClient } from "@angular/common/http";
import { PageInfo } from "../../models/page-info";
import { Query } from "../../models/query";
import { PagedData } from "../../models/paged-data";
import { WaterSupplierUser } from "../../models/users/water-supplier-user";
import { lastValueFrom } from "rxjs";

@Injectable({
    providedIn: 'root'
})
export class UserService {
    constructor(
        private readonly _urlResolver: UrlResolverService,
        private readonly _queryHelper: QueryHelperService,
        private readonly _http: HttpClient
    ) {

    }

    public async getAll(pageInfo: PageInfo, query: Query): Promise<PagedData<WaterSupplierUser>> {
        const url = this._urlResolver.resolveUrl('/api/users');

        const observable = this._http.get<PagedData<WaterSupplierUser>>(url, {
            params: this._queryHelper.buildQuery(pageInfo, query)
        });

        return await lastValueFrom(observable);
    }

    public get(id: number): Promise<WaterSupplierUser> {
        const url = this._urlResolver.resolveUrl(`/api/users/${id}`);
        const observable = this._http.get(url);

        return lastValueFrom(observable);
    }

    public add(user: WaterSupplierUser): Promise<WaterSupplierUser> {
        const url = this._urlResolver.resolveUrl('/api/users');
        const observable = this._http.post(url, user);

        return lastValueFrom(observable);
    }

    public update(user: WaterSupplierUser): Promise<WaterSupplierUser | undefined> {
        const url = this._urlResolver.resolveUrl(`/api/users/${user.id}`);
        const observable = this._http.put(url, user);

        return lastValueFrom(observable);
    }

    public delete(id: number): Promise<WaterSupplierUser | undefined> {
        const url = this._urlResolver.resolveUrl(`/api/users/${id}`);
        const observable = this._http.delete(url);

        return lastValueFrom(observable);
    }
}