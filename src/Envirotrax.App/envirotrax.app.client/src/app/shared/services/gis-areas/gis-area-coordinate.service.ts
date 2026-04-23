import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { lastValueFrom } from "rxjs";
import { UrlResolverService } from "../helpers/url-resolver.service";
import { GisAreaCoordinate } from "../../models/gis-areas/gis-area";

@Injectable({
    providedIn: 'root'
})
export class GisAreaCoordinateService {
    constructor(
        private readonly _http: HttpClient,
        private readonly _urlResolver: UrlResolverService,
    ) { }

    public getAll(): Promise<GisAreaCoordinate[]> {
        const url = this._urlResolver.resolveUrl('/api/gis-areas/coordinates');
        return lastValueFrom(this._http.get<GisAreaCoordinate[]>(url));
    }

    public getByAreaId(areaId: number): Promise<GisAreaCoordinate[]> {
        const url = this._urlResolver.resolveUrl(`/api/gis-areas/${areaId}/coordinates`);
        return lastValueFrom(this._http.get<GisAreaCoordinate[]>(url));
    }

    public addOrUpdate(areaId: number, coordinates: GisAreaCoordinate[]): Promise<GisAreaCoordinate[]> {
        const url = this._urlResolver.resolveUrl(`/api/gis-areas/${areaId}/coordinates`);
        return lastValueFrom(this._http.put<GisAreaCoordinate[]>(url, coordinates));
    }

    public deleteByArea(areaId: number): Promise<void> {
        const url = this._urlResolver.resolveUrl(`/api/gis-areas/${areaId}/coordinates`);
        const observable = this._http.delete<void>(url);

        return lastValueFrom(observable);
    }
}
