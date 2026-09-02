import { Injectable } from "@angular/core";
import { HttpClient, HttpParams } from "@angular/common/http";
import { lastValueFrom } from "rxjs";
import { InputOption } from "@envirotrax/common-ui";
import { UrlResolverService } from "../helpers/url-resolver.service";
import { QueryHelperService } from "../helpers/query-helper.service";
import { PageInfo } from "../../models/page-info";
import { PagedData } from "../../models/paged-data";
import { Query } from "../../models/query";
import { ProfessionalType } from "../../models/professionals/licenses/professional-user-license";
import { RegisteredProfessional, RegisteredProfessionalSupplier } from "../../models/professionals/registered-professional";

/**
 * Public "Registered Professionals" directory. Every endpoint here is anonymous, so this service is
 * the only one in the app that is safe to call before a user signs in.
 */
@Injectable({
    providedIn: 'root'
})
export class RegisteredProfessionalService {
    constructor(
        private readonly _http: HttpClient,
        private readonly _urlResolver: UrlResolverService,
        private readonly _queryHelper: QueryHelperService
    ) {

    }

    public getSearchEndpoint(): string {
        return this._urlResolver.resolveUrl('/api/registered-professionals');
    }

    public getWaterSuppliers(professionalType: ProfessionalType): Promise<RegisteredProfessionalSupplier[]> {
        const url = this._urlResolver.resolveUrl('/api/registered-professionals/water-suppliers');
        const params = new HttpParams().append('professionalType', String(professionalType));

        return lastValueFrom(this._http.get<RegisteredProfessionalSupplier[]>(url, { params }));
    }

    public async getWaterSupplierOptions(professionalType: ProfessionalType): Promise<InputOption[]> {
        const suppliers = await this.getWaterSuppliers(professionalType);

        return [
            { id: '', text: 'Select a water supplier' },
            ...suppliers
                .filter(supplier => supplier.id)
                .map(supplier => ({ id: String(supplier.id), text: supplier.name ?? '' }))
        ];
    }

    public search(
        waterSupplierId: number,
        professionalType: ProfessionalType,
        pageInfo: PageInfo,
        query: Query
    ): Promise<PagedData<RegisteredProfessional>> {
        const params = this._queryHelper
            .buildQuery(pageInfo, query)
            .append('waterSupplierId', String(waterSupplierId))
            .append('professionalType', String(professionalType));

        return lastValueFrom(
            this._http.get<PagedData<RegisteredProfessional>>(this.getSearchEndpoint(), { params })
        );
    }
}
