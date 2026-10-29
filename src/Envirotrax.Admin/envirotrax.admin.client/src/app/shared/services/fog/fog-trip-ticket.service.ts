import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { PagedData, PageInfo, Query, QueryHelperService, UrlResolverService } from "@envirotrax/common-ui";
import { lastValueFrom } from "rxjs";
import { FogTripTicket } from "../../models/fog/fog-trip-ticket";

export interface FogTripTicketSearchCriteria {
    userEmail?: string | null;
}

@Injectable({
    providedIn: 'root'
})
export class FogTripTicketService {
    constructor(
        private readonly _urlResolver: UrlResolverService,
        private readonly _queryHelper: QueryHelperService,
        private readonly _http: HttpClient
    ) {

    }

    public async getAll(pageInfo: PageInfo, query: Query, criteria: FogTripTicketSearchCriteria): Promise<PagedData<FogTripTicket>> {
        const url = this._urlResolver.resolveUrl('/api/fog/trip-tickets');

        let params = this._queryHelper.buildQuery(pageInfo, query);

        for (const [name, value] of Object.entries(criteria)) {
            if (value) {
                params = params.append(name, value);
            }
        }

        const observable = this._http.get<PagedData<FogTripTicket>>(url, { params });

        return await lastValueFrom(observable);
    }

    public async updateValidationHold(id: number, waterSupplierId: number, onHold: boolean): Promise<FogTripTicket> {
        const url = this._urlResolver.resolveUrl(`/api/fog/trip-tickets/${id}/validation-hold?waterSupplierId=${waterSupplierId}`);

        const observable = this._http.put<FogTripTicket>(url, onHold);

        return await lastValueFrom(observable);
    }

    public async updateNeedsValidation(id: number, waterSupplierId: number, needsValidation: boolean): Promise<FogTripTicket> {
        const url = this._urlResolver.resolveUrl(`/api/fog/trip-tickets/${id}/needs-validation?waterSupplierId=${waterSupplierId}`);

        const observable = this._http.put<FogTripTicket>(url, needsValidation);

        return await lastValueFrom(observable);
    }
}
