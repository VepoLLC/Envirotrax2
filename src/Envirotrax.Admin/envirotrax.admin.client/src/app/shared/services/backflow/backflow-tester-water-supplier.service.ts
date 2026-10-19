import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { PagedData, PageInfo, Query, QueryHelperService, UrlResolverService } from "@envirotrax/common-ui";
import { lastValueFrom } from "rxjs";
import { ProfessionalWaterSupplier } from "../../models/professionals/professional-water-supplier";

@Injectable({
    providedIn: 'root'
})
export class BackflowTesterWaterSupplierService {
    constructor(
        private readonly _urlResolver: UrlResolverService,
        private readonly _queryHelper: QueryHelperService,
        private readonly _http: HttpClient
    ) {

    }

    public getAll(professionalId: number, pageInfo: PageInfo, query: Query): Promise<PagedData<ProfessionalWaterSupplier>> {
        const url = this._urlResolver.resolveUrl(`/api/backflow/testers/${professionalId}/water-suppliers`);

        return lastValueFrom(this._http.get<PagedData<ProfessionalWaterSupplier>>(url, {
            params: this._queryHelper.buildQuery(pageInfo, query)
        }));
    }

    public update(professionalId: number, registration: ProfessionalWaterSupplier): Promise<ProfessionalWaterSupplier> {
        const url = this._urlResolver.resolveUrl(`/api/backflow/testers/${professionalId}/water-suppliers/${registration.waterSupplier?.id}`);

        return lastValueFrom(this._http.put<ProfessionalWaterSupplier>(url, registration));
    }
}
