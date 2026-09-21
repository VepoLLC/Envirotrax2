import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { lastValueFrom } from 'rxjs';
import { UrlResolverService } from '../helpers/url-resolver.service';
import { QueryHelperService } from '../helpers/query-helper.service';
import { WaterSupplierLicense, UpdateWaterSupplierLicense, LicenseCounts } from '../../models/professionals/licenses/water-supplier-license';
import { PagedData } from '../../models/paged-data';
import { PageInfo } from '../../models/page-info';
import { Query } from '../../models/query';

@Injectable({ providedIn: 'root' })
export class WaterSupplierLicenseService {
    constructor(
        private readonly _urlResolver: UrlResolverService,
        private readonly _queryHelper: QueryHelperService,
        private readonly _http: HttpClient
    ) {}

    public getLicenses(licenseFilter: string, pageInfo: PageInfo, query: Query): Promise<PagedData<WaterSupplierLicense>> {
        const url = this._urlResolver.resolveUrl('/api/licenses');
        const params = this._queryHelper.buildQuery(pageInfo, query).append('licenseFilter', licenseFilter);
        return lastValueFrom(this._http.get<PagedData<WaterSupplierLicense>>(url, { params }));
    }

    public getCounts(): Promise<LicenseCounts> {
        const url = this._urlResolver.resolveUrl('/api/licenses/counts');
        return lastValueFrom(this._http.get<LicenseCounts>(url));
    }

    public update(id: number, dto: UpdateWaterSupplierLicense): Promise<WaterSupplierLicense> {
        const url = this._urlResolver.resolveUrl(`/api/licenses/${id}`);
        return lastValueFrom(this._http.put<WaterSupplierLicense>(url, dto));
    }

    public delete(id: number): Promise<void> {
        const url = this._urlResolver.resolveUrl(`/api/licenses/${id}`);
        return lastValueFrom(this._http.delete<void>(url));
    }
}
