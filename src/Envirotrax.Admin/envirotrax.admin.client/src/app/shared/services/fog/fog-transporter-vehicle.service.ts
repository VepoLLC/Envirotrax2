import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { PagedData, PageInfo, Query, QueryHelperService, UrlResolverService } from "@envirotrax/common-ui";
import { lastValueFrom } from "rxjs";
import { FogVehicle } from "../../models/fog/fog-vehicle";

@Injectable({
    providedIn: 'root'
})
export class FogTransporterVehicleService {
    constructor(
        private readonly _urlResolver: UrlResolverService,
        private readonly _queryHelper: QueryHelperService,
        private readonly _http: HttpClient
    ) {

    }

    public getAll(professionalId: number, pageInfo: PageInfo, query: Query): Promise<PagedData<FogVehicle>> {
        const url = this._urlResolver.resolveUrl(`/api/fog/transporters/${professionalId}/vehicles`);

        return lastValueFrom(this._http.get<PagedData<FogVehicle>>(url, {
            params: this._queryHelper.buildQuery(pageInfo, query)
        }));
    }

    public add(professionalId: number, vehicle: FogVehicle): Promise<FogVehicle> {
        const url = this._urlResolver.resolveUrl(`/api/fog/transporters/${professionalId}/vehicles`);

        return lastValueFrom(this._http.post<FogVehicle>(url, vehicle));
    }

    public update(professionalId: number, vehicle: FogVehicle): Promise<FogVehicle> {
        const url = this._urlResolver.resolveUrl(`/api/fog/transporters/${professionalId}/vehicles/${vehicle.id}`);

        return lastValueFrom(this._http.put<FogVehicle>(url, vehicle));
    }

    public delete(professionalId: number, vehicleId: number): Promise<void> {
        const url = this._urlResolver.resolveUrl(`/api/fog/transporters/${professionalId}/vehicles/${vehicleId}`);

        return lastValueFrom(this._http.delete<void>(url));
    }
}
