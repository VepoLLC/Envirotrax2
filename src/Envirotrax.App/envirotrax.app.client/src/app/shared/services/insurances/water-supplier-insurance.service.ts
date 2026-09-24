import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { lastValueFrom } from 'rxjs';
import { UrlResolverService } from '../helpers/url-resolver.service';
import { QueryHelperService } from '../helpers/query-helper.service';
import { WaterSupplierInsurance } from '../../models/professionals/water-supplier-insurance';
import { PagedData } from '../../models/paged-data';
import { PageInfo } from '../../models/page-info';
import { Query } from '../../models/query';

@Injectable({ providedIn: 'root' })
export class WaterSupplierInsuranceService {
    constructor(
        private readonly _urlResolver: UrlResolverService,
        private readonly _queryHelper: QueryHelperService,
        private readonly _http: HttpClient
    ) {}

    public getUnverifiedInsurances(pageInfo: PageInfo, query: Query): Promise<PagedData<WaterSupplierInsurance>> {
        const url = this._urlResolver.resolveUrl('/api/insurances');
        const params = this._queryHelper.buildQuery(pageInfo, query);
        return lastValueFrom(this._http.get<PagedData<WaterSupplierInsurance>>(url, { params }));
    }
}
