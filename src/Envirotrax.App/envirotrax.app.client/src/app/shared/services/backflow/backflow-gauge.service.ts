import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { lastValueFrom } from "rxjs";
import { UrlResolverService } from "../helpers/url-resolver.service";
import { QueryHelperService } from "../helpers/query-helper.service";
import { BackflowGauge } from "../../models/backflow/backflow-gauge";
import { PagedData } from "../../models/paged-data";
import { PageInfo } from "../../models/page-info";
import { Query } from "../../models/query";

@Injectable({
    providedIn: 'root'
})
export class BackflowGaugeService {
    constructor(
        private readonly _urlResolver: UrlResolverService,
        private readonly _queryHelper: QueryHelperService,
        private readonly _http: HttpClient
    ) {
    }

    public getAll(pageInfo: PageInfo, query: Query): Promise<PagedData<BackflowGauge>> {
        const url = this._urlResolver.resolveUrl('/api/professionals/backflow/gauges');

        const observable = this._http.get<PagedData<BackflowGauge>>(url, {
            params: this._queryHelper.buildQuery(pageInfo, query)
        });

        return lastValueFrom(observable);
    }

    public add(gauge: BackflowGauge, file: File | null): Promise<BackflowGauge> {
        const url = this._urlResolver.resolveUrl('/api/professionals/backflow/gauges');

        if (file) {
            const formData = new FormData();
            formData.append('file', file);
            if (gauge.professionalId) formData.append('professionalId', gauge.professionalId.toString());
            if (gauge.manufacturer) formData.append('manufacturer', gauge.manufacturer);
            if (gauge.model) formData.append('model', gauge.model);
            if (gauge.serialNumber) formData.append('serialNumber', gauge.serialNumber);
            if (gauge.lastCalibrationDate) formData.append('lastCalibrationDate', new Date(gauge.lastCalibrationDate).toISOString());
            formData.append('isPortable', gauge.isPortable ? 'true' : 'false');
            formData.append('isManaged', gauge.isManaged ? 'true' : 'false');
            return lastValueFrom(this._http.post<BackflowGauge>(url, formData));
        }

        return lastValueFrom(this._http.post<BackflowGauge>(url, gauge));
    }

    public update(gauge: BackflowGauge): Promise<BackflowGauge> {
        const url = this._urlResolver.resolveUrl(`/api/professionals/backflow/gauges/${gauge.id}`);
        return lastValueFrom(this._http.put<BackflowGauge>(url, gauge));
    }

    public delete(id: number): Promise<BackflowGauge> {
        const url = this._urlResolver.resolveUrl(`/api/professionals/backflow/gauges/${id}`);
        return lastValueFrom(this._http.delete<BackflowGauge>(url));
    }

    public async getFileUrl(id: number): Promise<string> {
        const url = this._urlResolver.resolveUrl(`/api/professionals/backflow/gauges/${id}/file`);
        return lastValueFrom(this._http.get<string>(url));
    }
}
