import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { PagedData, PageInfo, Query, QueryHelperService, UrlResolverService } from "@envirotrax/common-ui";
import { WaterSupplier } from "../../models/water-suppliers/water-supplier";
import { WaterSupplierDetails } from "../../models/water-suppliers/water-supplier-details";
import { WaterSupplierUserAccount } from "../../models/water-suppliers/water-supplier-user-account";
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

    public async getDetails(id: number): Promise<WaterSupplierDetails> {
        const url = this._urlResolver.resolveUrl(`/api/water-suppliers/${id}/details`);

        return await lastValueFrom(this._http.get<WaterSupplierDetails>(url));
    }

    public async getUserAccounts(id: number): Promise<WaterSupplierUserAccount[]> {
        const url = this._urlResolver.resolveUrl(`/api/water-suppliers/${id}/user-accounts`);

        return await lastValueFrom(this._http.get<WaterSupplierUserAccount[]>(url));
    }

    public async updateDetails(id: number, details: WaterSupplierDetails): Promise<WaterSupplierDetails> {
        const url = this._urlResolver.resolveUrl(`/api/water-suppliers/${id}/details`);

        return await lastValueFrom(this._http.put<WaterSupplierDetails>(url, details));
    }
}
