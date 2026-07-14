import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { lastValueFrom } from "rxjs";
import { UrlResolverService } from "../helpers/url-resolver.service";
import { QueryHelperService } from "../helpers/query-helper.service";
import { PageInfo } from "../../models/page-info";
import { Query } from "../../models/query";
import { PagedData } from "../../models/paged-data";
import { FogVehicle } from "../../models/fog/fog-vehicle";
import { InputOption, MAX_PAGE_SIZE } from "@envirotrax/common-ui";

@Injectable({
    providedIn: 'root'
})
export class ProfessionalFogVehicleService {
    constructor(
        private readonly _urlResolver: UrlResolverService,
        private readonly _queryHelper: QueryHelperService,
        private readonly _http: HttpClient
    ) {
    }

    public getAll(pageInfo: PageInfo, query: Query): Promise<PagedData<FogVehicle>> {
        const url = this._urlResolver.resolveUrl('/api/professionals/fog/vehicles');
        return lastValueFrom(this._http.get<PagedData<FogVehicle>>(url, {
            params: this._queryHelper.buildQuery(pageInfo, query)
        }));
    }

    public async getAllAsOptions(includeEmpty: Boolean, emptyOptionText: string): Promise<InputOption<FogVehicle>[]> {
        const vehicles = await this.getAll({ pageSize: MAX_PAGE_SIZE }, {});

        const options: InputOption<FogVehicle>[] = vehicles.data.map(v => ({
            id: v.id,
            text: `${v.manufacturedYear ?? ''} ${v.manufacturer ?? ''} ${v.licensePlateNumber ?? ''}`.trim(),
            data: v
        }));

        if (includeEmpty) {
            options.splice(0, 0, { id: '', text: emptyOptionText ?? '' });
        }

        return options;
    }

    public add(vehicle: FogVehicle): Promise<FogVehicle> {
        const url = this._urlResolver.resolveUrl('/api/professionals/fog/vehicles');
        return lastValueFrom(this._http.post<FogVehicle>(url, vehicle));
    }

    public update(id: number, vehicle: FogVehicle): Promise<FogVehicle> {
        const url = this._urlResolver.resolveUrl(`/api/professionals/fog/vehicles/${id}`);
        return lastValueFrom(this._http.put<FogVehicle>(url, vehicle));
    }

    public delete(id: number): Promise<FogVehicle> {
        const url = this._urlResolver.resolveUrl(`/api/professionals/fog/vehicles/${id}`);
        return lastValueFrom(this._http.delete<FogVehicle>(url));
    }
}
