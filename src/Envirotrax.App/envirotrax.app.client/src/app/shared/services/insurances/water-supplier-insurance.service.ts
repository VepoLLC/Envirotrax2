import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { lastValueFrom } from 'rxjs';
import { UrlResolverService } from '../helpers/url-resolver.service';
import { QueryHelperService } from '../helpers/query-helper.service';
import { WaterSupplierInsurance, UpdateWaterSupplierInsurance, InsuranceCounts } from '../../models/professionals/water-supplier-insurance';
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

    public getInsurances(insuranceFilter: string, pageInfo: PageInfo, query: Query): Promise<PagedData<WaterSupplierInsurance>> {
        const url = this._urlResolver.resolveUrl('/api/insurances');
        const params = this._queryHelper.buildQuery(pageInfo, query).append('insuranceFilter', insuranceFilter);
        return lastValueFrom(this._http.get<PagedData<WaterSupplierInsurance>>(url, { params }));
    }

    public getCounts(): Promise<InsuranceCounts> {
        const url = this._urlResolver.resolveUrl('/api/insurances/counts');
        return lastValueFrom(this._http.get<InsuranceCounts>(url));
    }

    public getFileUrl(id: number): Promise<string> {
        const url = this._urlResolver.resolveUrl(`/api/insurances/${id}/file-url`);
        return lastValueFrom(this._http.get<string>(url));
    }

    public update(id: number, dto: UpdateWaterSupplierInsurance): Promise<WaterSupplierInsurance> {
        const url = this._urlResolver.resolveUrl(`/api/insurances/${id}`);
        return lastValueFrom(this._http.put<WaterSupplierInsurance>(url, dto));
    }

    public delete(id: number): Promise<void> {
        const url = this._urlResolver.resolveUrl(`/api/insurances/${id}`);
        return lastValueFrom(this._http.delete<void>(url));
    }
}
