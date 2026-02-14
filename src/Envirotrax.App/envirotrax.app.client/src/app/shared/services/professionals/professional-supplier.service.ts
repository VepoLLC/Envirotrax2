import { Injectable } from "@angular/core";
import { UrlResolverService } from "../helpers/url-resolver.service";
import { QueryHelperService } from "../helpers/query-helper.service";
import { HttpClient } from "@angular/common/http";
import { PageInfo } from "../../models/page-info";
import { Query } from "../../models/query";
import { PagedData } from "../../models/paged-data";
import { WaterSupplier } from "../../models/water-suppliers/water-supplier";
import { lastValueFrom } from "rxjs";
import { AvailableWaterSupplier, ProfessionalWaterSupplier } from "../../models/professionals/professional-water-supplier";

@Injectable({
    providedIn: 'root'
})
export class ProfessionalSupplierService {
    constructor(
        private readonly _urlResolver: UrlResolverService,
        private readonly _queryHelper: QueryHelperService,
        private readonly _http: HttpClient
    ) {

    }

    public getAllAvailableSuppliers(pageInfo: PageInfo, query: Query): Promise<PagedData<AvailableWaterSupplier>> {
        const url = this._urlResolver.resolveUrl('/api/professionals/water-suppliers/available');

        const obsertvable = this._http.get<PagedData<AvailableWaterSupplier>>(url, {
            params: this._queryHelper.buildQuery(pageInfo, query)
        });

        return lastValueFrom(obsertvable);
    }

    public getAllMy(): Promise<ProfessionalWaterSupplier[]> {
        const url = this._urlResolver.resolveUrl('/api/professionals/water-suppliers/my');
        const observable = this._http.get<ProfessionalWaterSupplier[]>(url);

        return lastValueFrom(observable);
    }

    public addOrUpdate(proSupplier: ProfessionalWaterSupplier): Promise<ProfessionalWaterSupplier> {
        const url = this._urlResolver.resolveUrl('/api/professionals/water-suppliers/my/add-or-update');
        const observable = this._http.put<ProfessionalWaterSupplier>(url, proSupplier);

        return lastValueFrom(observable);
    }
}