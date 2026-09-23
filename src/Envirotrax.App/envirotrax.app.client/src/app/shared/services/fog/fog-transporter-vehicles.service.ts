import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { lastValueFrom } from "rxjs";
import { UrlResolverService } from "../helpers/url-resolver.service";
import { QueryHelperService } from "../helpers/query-helper.service";
import { PageInfo } from "../../models/page-info";
import { Query } from "../../models/query";
import { PagedData } from "../../models/paged-data";
import { FogVehicle } from "../../models/fog/fog-vehicle";

@Injectable({
    providedIn: 'root'
})
export class FogTransporterVehiclesService {
    constructor(
        private readonly _urlResolver: UrlResolverService,
        private readonly _queryHelper: QueryHelperService,
        private readonly _http: HttpClient
    ) {
    }

    public getVehicles(transporterId: number, pageInfo: PageInfo, query: Query): Promise<PagedData<FogVehicle>> {
        const url = this._urlResolver.resolveUrl(`/api/fog/transporters/${transporterId}/vehicles`);
        return lastValueFrom(this._http.get<PagedData<FogVehicle>>(url, {
            params: this._queryHelper.buildQuery(pageInfo, query)
        }));
    }

    public add(transporterId: number, vehicle: FogVehicle): Promise<FogVehicle> {
        const url = this._urlResolver.resolveUrl(`/api/fog/transporters/${transporterId}/vehicles`);
        const payload: FogVehicle = { ...vehicle, professional: { id: transporterId } };
        return lastValueFrom(this._http.post<FogVehicle>(url, payload));
    }

    public update(transporterId: number, vehicle: FogVehicle): Promise<FogVehicle> {
        const url = this._urlResolver.resolveUrl(`/api/fog/transporters/${transporterId}/vehicles/${vehicle.id}`);
        return lastValueFrom(this._http.put<FogVehicle>(url, vehicle));
    }

    public delete(transporterId: number, vehicleId: number): Promise<void> {
        const url = this._urlResolver.resolveUrl(`/api/fog/transporters/${transporterId}/vehicles/${vehicleId}`);
        return lastValueFrom(this._http.delete<void>(url));
    }
}
