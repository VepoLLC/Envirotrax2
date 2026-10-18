import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { PagedData, PageInfo, Query, QueryHelperService, UrlResolverService } from "@envirotrax/common-ui";
import { lastValueFrom } from "rxjs";
import { BackflowGauge } from "../../models/backflow/backflow-gauge";

@Injectable({
    providedIn: 'root'
})
export class BackflowTesterGaugeService {
    constructor(
        private readonly _urlResolver: UrlResolverService,
        private readonly _queryHelper: QueryHelperService,
        private readonly _http: HttpClient
    ) {

    }

    public getAll(professionalId: number, pageInfo: PageInfo, query: Query): Promise<PagedData<BackflowGauge>> {
        const url = this._urlResolver.resolveUrl(`/api/backflow/testers/${professionalId}/gauges`);

        return lastValueFrom(this._http.get<PagedData<BackflowGauge>>(url, {
            params: this._queryHelper.buildQuery(pageInfo, query)
        }));
    }

    public add(professionalId: number, gauge: BackflowGauge, file: File): Promise<BackflowGauge> {
        const url = this._urlResolver.resolveUrl(`/api/backflow/testers/${professionalId}/gauges`);

        const formData = new FormData();
        formData.append('manufacturer', gauge.manufacturer ?? '');
        formData.append('model', gauge.model ?? '');
        formData.append('serialNumber', gauge.serialNumber ?? '');
        formData.append('isPortable', (gauge.isPortable ?? false).toString());

        if (gauge.lastCalibrationDate) {
            formData.append('lastCalibrationDate', gauge.lastCalibrationDate);
        }

        formData.append('file', file);

        return lastValueFrom(this._http.post<BackflowGauge>(url, formData));
    }

    public update(professionalId: number, gauge: BackflowGauge): Promise<BackflowGauge> {
        const url = this._urlResolver.resolveUrl(`/api/backflow/testers/${professionalId}/gauges/${gauge.id}`);

        return lastValueFrom(this._http.put<BackflowGauge>(url, gauge));
    }

    public getFileUrl(professionalId: number, gaugeId: number): Promise<string> {
        const url = this._urlResolver.resolveUrl(`/api/backflow/testers/${professionalId}/gauges/${gaugeId}/file-url`);

        return lastValueFrom(this._http.get<string>(url));
    }

    public delete(professionalId: number, gaugeId: number): Promise<void> {
        const url = this._urlResolver.resolveUrl(`/api/backflow/testers/${professionalId}/gauges/${gaugeId}`);

        return lastValueFrom(this._http.delete<void>(url));
    }
}
