import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { lastValueFrom } from 'rxjs';
import { UrlResolverService } from '../helpers/url-resolver.service';
import { QueryHelperService } from '../helpers/query-helper.service';
import { WaterSupplierGauge } from '../../models/backflow/water-supplier-gauge';
import { PagedData } from '../../models/paged-data';
import { PageInfo } from '../../models/page-info';
import { Query } from '../../models/query';

@Injectable({ providedIn: 'root' })
export class WaterSupplierGaugeService {
    constructor(
        private readonly _urlResolver: UrlResolverService,
        private readonly _queryHelper: QueryHelperService,
        private readonly _http: HttpClient
    ) {}

    public getGauges(pageInfo: PageInfo, query: Query): Promise<PagedData<WaterSupplierGauge>> {
        const url = this._urlResolver.resolveUrl('/api/gauges');
        const params = this._queryHelper.buildQuery(pageInfo, query);
        return lastValueFrom(this._http.get<PagedData<WaterSupplierGauge>>(url, { params }));
    }
}
