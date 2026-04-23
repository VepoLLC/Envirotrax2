import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { lastValueFrom } from "rxjs";
import { UrlResolverService } from "../helpers/url-resolver.service";
import { QueryHelperService } from "../helpers/query-helper.service";
import { PageInfo } from "../../models/page-info";
import { Query } from "../../models/query";
import { PagedData } from "../../models/paged-data";
import { DefaultGisMapView, GisArea } from "../../models/gis-areas/gis-area";

@Injectable({
    providedIn: 'root'
})
export class GisAreaService {
    constructor(
        private readonly _http: HttpClient,
        private readonly _urlResolver: UrlResolverService,
        private readonly _queryHelper: QueryHelperService,
    ) { }

    public getAll(pageInfo: PageInfo, query: Query): Promise<PagedData<GisArea>> {
        const url = this._urlResolver.resolveUrl('/api/gis-areas');
        return lastValueFrom(this._http.get<PagedData<GisArea>>(url, {
            params: this._queryHelper.buildQuery(pageInfo, query)
        }));
    }

    public get(id: number): Promise<GisArea> {
        const url = this._urlResolver.resolveUrl(`/api/gis-areas/${id}`);
        return lastValueFrom(this._http.get<GisArea>(url));
    }

    public add(dto: GisArea): Promise<GisArea> {
        const url = this._urlResolver.resolveUrl('/api/gis-areas');
        return lastValueFrom(this._http.post<GisArea>(url, dto));
    }

    public update(dto: GisArea): Promise<GisArea> {
        const url = this._urlResolver.resolveUrl(`/api/gis-areas/${dto.id}`);
        return lastValueFrom(this._http.put<GisArea>(url, dto));
    }

    public delete(id: number): Promise<GisArea> {
        const url = this._urlResolver.resolveUrl(`/api/gis-areas/${id}`);
        return lastValueFrom(this._http.delete<GisArea>(url));
    }

    public reactivate(id: number): Promise<GisArea> {
        const url = this._urlResolver.resolveUrl(`/api/gis-areas/${id}/reactivate`);
        return lastValueFrom(this._http.delete<GisArea>(url));
    }

    public getDefaultView(): Promise<DefaultGisMapView> {
        const url = this._urlResolver.resolveUrl('/api/gis-areas/default-view');
        return lastValueFrom(this._http.get<DefaultGisMapView>(url));
    }

    public updateDefaultView(dto: DefaultGisMapView): Promise<DefaultGisMapView> {
        const url = this._urlResolver.resolveUrl('/api/gis-areas/default-view');
        return lastValueFrom(this._http.put<DefaultGisMapView>(url, dto));
    }
}
