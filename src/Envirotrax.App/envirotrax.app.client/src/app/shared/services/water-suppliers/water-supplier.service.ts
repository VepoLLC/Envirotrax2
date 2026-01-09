import { Injectable } from "@angular/core";
import { UrlResolverService } from "../helpers/url-resolver.service";
import { PageInfo } from "../../models/page-info";
import { Query } from "../../models/query";
import { QueryHelperService } from "../helpers/query-helper.service";
import { HttpClient } from "@angular/common/http";
import { PagedData } from "../../models/paged-data";
import { WaterSupplier } from "../../models/water-suppliers/water-supplier";
import { lastValueFrom } from "rxjs";

@Injectable({
    providedIn: 'root'
})
export class WaterSupplierService {
    constructor(
        private readonly _urlResolver: UrlResolverService,
        private readonly _queryHelper: QueryHelperService,
        private readonly _http: HttpClient
    ) {

    }

    public async getAll(pageInfo: PageInfo, query: Query): Promise<PagedData<WaterSupplier>> {
        const url = this._urlResolver.resolveUrl('/api/water-suppliers');

        const observable = this._http.get<PagedData<WaterSupplier>>(url, {
            params: this._queryHelper.buildQuery(pageInfo, query)
        });

        return await lastValueFrom(observable);
    }

    public get(id: number): Promise<WaterSupplier> {
        const url = this._urlResolver.resolveUrl(`/api/water-suppliers/${id}`);

        return lastValueFrom(
            this._http.get<WaterSupplier>(url)
        );
    }

    public add(supplier: WaterSupplier): Promise<WaterSupplier> {
        const url = this._urlResolver.resolveUrl('/api/water-suppliers');

        return lastValueFrom(
            this._http.post<WaterSupplier>(url, supplier)
        );
    }


    public update(supplier: WaterSupplier): Promise<WaterSupplier> {
        const url = this._urlResolver.resolveUrl(`/api/water-suppliers/${supplier.id}`);

        return lastValueFrom(
            this._http.put<WaterSupplier>(url, supplier)
        );
    }

    public delete(id: number): Promise<WaterSupplier> {
        const url = this._urlResolver.resolveUrl(`/api/water-suppliers/${id}`);

        return lastValueFrom(
            this._http.delete<WaterSupplier>(url)
        );
    }
}
