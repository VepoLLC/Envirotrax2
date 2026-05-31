import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { lastValueFrom } from "rxjs";
import { UrlResolverService } from "../helpers/url-resolver.service";
import { QueryHelperService } from "../helpers/query-helper.service";
import { PageInfo } from "../../models/page-info";
import { Query } from "../../models/query";
import { PagedData } from "../../models/paged-data";
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

    public getGauges(testerId: number, pageInfo: PageInfo, query: Query): Promise<PagedData<BackflowGauge>> {
        const url = this._urlResolver.resolveUrl(`/api/backflow/testers/${testerId}/gauges`);
        return lastValueFrom(this._http.get<PagedData<BackflowGauge>>(url, {
            params: this._queryHelper.buildQuery(pageInfo, query)
        }));
    }

    public add(testerId: number, gauge: BackflowGauge, file: File): Promise<BackflowGauge> {
        const url = this._urlResolver.resolveUrl(`/api/backflow/testers/${testerId}/gauges`);
        const formData = new FormData();
        formData.append('professional.id', testerId.toString());
        formData.append('manufacturer', gauge.manufacturer ?? '');
        formData.append('model', gauge.model ?? '');
        formData.append('serialNumber', gauge.serialNumber ?? '');
        formData.append('isPortable', (gauge.isPortable ?? false).toString());
        if (gauge.lastCalibrationDate) {
            formData.append('lastCalibrationDate', gauge.lastCalibrationDate.toString());
        }
        formData.append('file', file);
        return lastValueFrom(this._http.post<BackflowGauge>(url, formData));
    }

    public update(testerId: number, gauge: BackflowGauge): Promise<BackflowGauge> {
        const url = this._urlResolver.resolveUrl(`/api/backflow/testers/${testerId}/gauges/${gauge.id}`);
        return lastValueFrom(this._http.put<BackflowGauge>(url, gauge));
    }

    public async getFileUrl(testerId: number, gaugeId: number): Promise<string> {
        const url = this._urlResolver.resolveUrl(`/api/backflow/testers/${testerId}/gauges/${gaugeId}/file-url`);
        return await lastValueFrom(this._http.get<string>(url));
    }

    public delete(testerId: number, gaugeId: number): Promise<void> {
        const url = this._urlResolver.resolveUrl(`/api/backflow/testers/${testerId}/gauges/${gaugeId}`);
        return lastValueFrom(this._http.delete<void>(url));
    }
}
