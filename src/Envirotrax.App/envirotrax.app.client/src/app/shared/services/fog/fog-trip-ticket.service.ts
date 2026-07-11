import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { lastValueFrom } from "rxjs";
import { UrlResolverService } from "../helpers/url-resolver.service";
import { QueryHelperService } from "../helpers/query-helper.service";
import { PageInfo } from "../../models/page-info";
import { Query } from "../../models/query";
import { PagedData } from "../../models/paged-data";
import { FogTripTicket } from "../../models/fog/fog-trip-ticket";
import { InputOption } from "@envirotrax/common-ui";

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

    public async getAll(pageInfo: PageInfo, query: Query): Promise<PagedData<FogTripTicket>> {
        const url = this._urlResolver.resolveUrl('/api/fog/trip-tickets');

        const observable = this._http.get<PagedData<FogTripTicket>>(url, {
            params: this._queryHelper.buildQuery(pageInfo, query)
        });

        return await lastValueFrom(observable);
    }

    public getById(id: number): Promise<FogTripTicket> {
        const url = this._urlResolver.resolveUrl(`/api/fog/trip-tickets/${id}`);
        return lastValueFrom(this._http.get<FogTripTicket>(url));
    }

    //for professional-fog-trip-ticket-list calls
    public async searchForProfessional(pageInfo: PageInfo, query: Query, waterSupplierId?: number): Promise<PagedData<FogTripTicket>> {
        const url = this._urlResolver.resolveUrl('/api/professionals/fog/trip-tickets');
        let params = this._queryHelper.buildQuery(pageInfo, query);

        if (waterSupplierId != null) {
            params = params.append('waterSupplierId', String(waterSupplierId));
        }

        return await lastValueFrom(this._http.get<PagedData<FogTripTicket>>(url, { params }));
    }

    public getProfessionalTransporters(): Promise<InputOption[]> {
        const url = this._urlResolver.resolveUrl('/api/professionals/fog/trip-tickets/lookup/transporters');
        return lastValueFrom(this._http.get<InputOption[]>(url));
    }

    public getProfessionalVehicles(): Promise<InputOption[]> {
        const url = this._urlResolver.resolveUrl('/api/professionals/fog/trip-tickets/lookup/vehicles');
        return lastValueFrom(this._http.get<InputOption[]>(url));
    }

    public getProfessionalDisposalSites(): Promise<InputOption[]> {
        const url = this._urlResolver.resolveUrl('/api/professionals/fog/trip-tickets/lookup/disposal-sites');
        return lastValueFrom(this._http.get<InputOption[]>(url));
    }
}
