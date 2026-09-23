import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { lastValueFrom } from "rxjs";
import { UrlResolverService } from "../helpers/url-resolver.service";
import { QueryHelperService } from "../helpers/query-helper.service";
import { PageInfo } from "../../models/page-info";
import { Query } from "../../models/query";
import { PagedData } from "../../models/paged-data";
import { DownloadEndpoint } from "../../models/download-config";
import { FogVehiclePermit, FogVehiclePermitSearch } from "../../models/fog/fog-vehicle-permit";

@Injectable({
    providedIn: 'root'
})
export class FogVehiclePermitService {
    constructor(
        private readonly _urlResolver: UrlResolverService,
        private readonly _queryHelper: QueryHelperService,
        private readonly _http: HttpClient
    ) {
    }

    public getAll(pageInfo: PageInfo, query: Query): Promise<PagedData<FogVehiclePermitSearch>> {
        const url = this._urlResolver.resolveUrl('/api/fog/vehicles');

        return lastValueFrom(this._http.get<PagedData<FogVehiclePermitSearch>>(url, {
            params: this._queryHelper.buildQuery(pageInfo, query)
        }));
    }

    public getAllEndpoint(): DownloadEndpoint {
        const url = this._urlResolver.resolveUrl('/api/fog/vehicles');

        return {
            method: 'GET',
            url: url
        };
    }

    public setPermit(vehicleId: number, permit: FogVehiclePermit): Promise<FogVehiclePermitSearch> {
        const url = this._urlResolver.resolveUrl(`/api/fog/vehicles/${vehicleId}/permit`);

        return lastValueFrom(this._http.put<FogVehiclePermitSearch>(url, permit));
    }
}
