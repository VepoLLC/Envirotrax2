import { Injectable } from "@angular/core";
import { UrlResolverService } from "../helpers/url-resolver.service";
import { QueryHelperService } from "../helpers/query-helper.service";
import { HttpClient } from "@angular/common/http";
import { MAX_PAGE_SIZE, PageInfo } from "../../models/page-info";
import { Query } from "../../models/query";
import { PagedData } from "../../models/paged-data";
import { lastValueFrom } from "rxjs";
import { AvailableWaterSupplier, ProfessionalWaterSupplier } from "../../models/professionals/professional-water-supplier";
import { InputOption } from "../../components/input/input.component";

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

    public async getMyAsOptions(hasCsiInspection = false): Promise<InputOption[]> {
        const suppliers = await this.getAllMy(hasCsiInspection);
        return [
            { id: '', text: 'Select a water supplier' },
            ...suppliers.data
                .filter(s => s.waterSupplier?.id)
                .map(s => ({ id: String(s.waterSupplier!.id!), text: s.waterSupplier!.name ?? '' }))
        ];
    }

    public getAllMy(hasCsiInspection = false): Promise<PagedData<ProfessionalWaterSupplier>> {
        const url = this._urlResolver.resolveUrl('/api/professionals/water-suppliers');
        const pageInfo: PageInfo = {pageSize: MAX_PAGE_SIZE };

        const query: Query = hasCsiInspection
         ? {
            filter:[
                {
                    columnName: 'hasCsiInspection',
                    comparisonOperator: 'Eq',
                    value: 'true'
                }
            ]
         }: {};
       return lastValueFrom(
            this._http.get<PagedData<ProfessionalWaterSupplier>>(url, {
                params: this._queryHelper.buildQuery(pageInfo, query)
            })
        );
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

    public delete(supplierId: number): Promise<ProfessionalWaterSupplier> {
        const url = this._urlResolver.resolveUrl(`/api/professionals/water-suppliers/${supplierId}`);
        const observable = this._http.delete<ProfessionalWaterSupplier>(url);

        return lastValueFrom(observable);
    }
}