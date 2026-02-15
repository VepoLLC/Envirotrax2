import { Injectable } from "@angular/core";
import { UrlResolverService } from "../helpers/url-resolver.service";
import { QueryHelperService } from "../helpers/query-helper.service";
import { HttpClient } from "@angular/common/http";
import { MAX_PAGE_SIZE, PageInfo } from "../../models/page-info";
import { Query } from "../../models/query";
import { PagedData } from "../../models/paged-data";
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

    public getAllMy(): Promise<PagedData<ProfessionalWaterSupplier>> {
        const url = this._urlResolver.resolveUrl('/api/professionals/water-suppliers');

        const observable = this._http.get<PagedData<ProfessionalWaterSupplier>>(url, {
            params: {
                pageSize: MAX_PAGE_SIZE
            }
        });

        return lastValueFrom(observable);
    }

    public get(supplierId: number): Promise<ProfessionalWaterSupplier> {
        const url = this._urlResolver.resolveUrl(`/api/professionals/water-suppliers/${supplierId}`);
        const observable = this._http.get<ProfessionalWaterSupplier>(url);

        return lastValueFrom(observable);
    }

    public add(proSupplier: ProfessionalWaterSupplier): Promise<ProfessionalWaterSupplier> {
        const url = this._urlResolver.resolveUrl('/api/professionals/water-suppliers');
        const observable = this._http.post<ProfessionalWaterSupplier>(url, proSupplier);

        return lastValueFrom(observable);
    }

    public update(proSupplier: ProfessionalWaterSupplier): Promise<ProfessionalWaterSupplier> {
        const url = this._urlResolver.resolveUrl(`/api/professionals/water-suppliers/${proSupplier.waterSupplier?.id}`);
        const observable = this._http.put<ProfessionalWaterSupplier>(url, proSupplier);

        return lastValueFrom(observable);
    }
}