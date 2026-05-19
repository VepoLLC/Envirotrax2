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
}
