import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { PagedData, PageInfo, Query, QueryHelperService, UrlResolverService } from "@envirotrax/common-ui";
import { lastValueFrom } from "rxjs";
import { BackflowGauge } from "../../models/backflow/backflow-gauge";
import { WaterSupplierGauge } from "../../models/backflow/water-supplier-gauge";

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

    public getAll(pageInfo: PageInfo, query: Query): Promise<PagedData<WaterSupplierGauge>> {
        const url = this._urlResolver.resolveUrl('/api/backflow/gauges');

        return lastValueFrom(this._http.get<PagedData<WaterSupplierGauge>>(url, {
            params: this._queryHelper.buildQuery(pageInfo, query)
        }));
    }

    public update(gauge: BackflowGauge): Promise<BackflowGauge> {
        const url = this._urlResolver.resolveUrl(`/api/backflow/gauges/${gauge.id}`);

        return lastValueFrom(this._http.put<BackflowGauge>(url, gauge));
    }

    public getFileUrl(id: number): Promise<string> {
        const url = this._urlResolver.resolveUrl(`/api/backflow/gauges/${id}/file-url`);

        return lastValueFrom(this._http.get<string>(url));
    }
}
