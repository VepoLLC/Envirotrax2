import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { lastValueFrom } from 'rxjs';
import { UrlResolverService } from '../helpers/url-resolver.service';
import { QueryHelperService } from '../helpers/query-helper.service';
import { ProfessionalDashboardStats } from '../../models/professionals/professional-dashboard-stats';
import { ProfessionalDashboardLicenseInsurance } from '../../models/professionals/professional-dashboard-license-insurance';
import { PagedData } from '../../models/paged-data';
import { PageInfo } from '../../models/page-info';
import { Query } from '../../models/query';

@Injectable({
    providedIn: 'root'
})
export class ProfessionalDashboardService {
    constructor(
        private readonly _http: HttpClient,
        private readonly _urlResolver: UrlResolverService,
        private readonly _queryHelper: QueryHelperService
    ) {}

    public getStats(): Promise<ProfessionalDashboardStats> {
        const url = this._urlResolver.resolveUrl('/api/professionals/dashboard/stats');
        return lastValueFrom(this._http.get<ProfessionalDashboardStats>(url));
    }

    public getLicensesAndInsurances(pageInfo: PageInfo, query: Query): Promise<PagedData<ProfessionalDashboardLicenseInsurance>> {
        const url = this._urlResolver.resolveUrl('/api/professionals/dashboard/licenses-and-insurances');

        const observable = this._http.get<PagedData<ProfessionalDashboardLicenseInsurance>>(url, {
            params: this._queryHelper.buildQuery(pageInfo, query)
        });

        return lastValueFrom(observable);
    }
}
