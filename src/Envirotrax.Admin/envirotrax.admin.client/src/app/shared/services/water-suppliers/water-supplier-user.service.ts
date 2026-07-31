import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { UrlResolverService } from "@envirotrax/common-ui";
import { WaterSupplierUser } from "../../models/water-suppliers/water-supplier-user";
import { lastValueFrom } from "rxjs";

@Injectable({
    providedIn: 'root'
})
export class WaterSupplierUserService {
    constructor(
        private readonly _urlResolver: UrlResolverService,
        private readonly _http: HttpClient
    ) {

    }

    public async getAll(waterSupplierId: number): Promise<WaterSupplierUser[]> {
        const url = this._urlResolver.resolveUrl(`/api/water-suppliers/${waterSupplierId}/users`);

        return await lastValueFrom(this._http.get<WaterSupplierUser[]>(url));
    }
}
