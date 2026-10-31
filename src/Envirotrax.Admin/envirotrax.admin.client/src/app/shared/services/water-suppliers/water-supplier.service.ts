import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { InputOption, MAX_PAGE_SIZE, PagedData, PageInfo, Query, QueryHelperService, UrlResolverService } from "@envirotrax/common-ui";
import { WaterSupplier } from "../../models/water-suppliers/water-supplier";
import { WaterSupplierDetails } from "../../models/water-suppliers/water-supplier-details";
import { WaterSupplierLetterSettings } from "../../models/water-suppliers/water-supplier-letter-settings";
import { GeneratedLetters } from "../../models/water-suppliers/generated-letters";
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

    public async getAllAsOptions(): Promise<InputOption<WaterSupplier>[]> {
        const result = await this.getAll(
            { pageSize: MAX_PAGE_SIZE },
            { sort: { name: 'Asc' }, filter: [] }
        );

        return (result.data ?? []).map(supplier => ({ id: String(supplier.id), text: supplier.name, data: supplier }));
    }

    public async getAllLetterSettings(): Promise<WaterSupplierLetterSettings[]> {
        const url = this._urlResolver.resolveUrl('/api/water-suppliers/letter-settings');

        return await lastValueFrom(this._http.get<WaterSupplierLetterSettings[]>(url));
    }

    public async generateLetters(id: number): Promise<GeneratedLetters> {
        const url = this._urlResolver.resolveUrl(`/api/water-suppliers/${id}/letters`);

        return await lastValueFrom(this._http.post<GeneratedLetters>(url, null));
    }

    public async getDetails(id: number): Promise<WaterSupplierDetails> {
        const url = this._urlResolver.resolveUrl(`/api/water-suppliers/${id}`);

        return await lastValueFrom(this._http.get<WaterSupplierDetails>(url));
    }

    public async updateDetails(id: number, details: WaterSupplierDetails): Promise<WaterSupplierDetails> {
        const url = this._urlResolver.resolveUrl(`/api/water-suppliers/${id}`);

        return await lastValueFrom(this._http.put<WaterSupplierDetails>(url, details));
    }
}
