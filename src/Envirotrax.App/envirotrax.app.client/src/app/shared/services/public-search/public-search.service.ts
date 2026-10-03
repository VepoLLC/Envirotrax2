import { Injectable } from "@angular/core";
import { HttpClient, HttpParams } from "@angular/common/http";
import { lastValueFrom } from "rxjs";
import { UrlResolverService } from "../helpers/url-resolver.service";
import { QueryHelperService } from "../helpers/query-helper.service";
import { PageInfo } from "../../models/page-info";
import { PagedData } from "../../models/paged-data";
import { PublicSearchCriteria } from "../../models/public-search/public-search-criteria";
import { PublicSearchWaterSuppliers } from "../../models/public-search/public-search-water-suppliers";
import { PublicBackflowTestResult } from "../../models/public-search/public-backflow-test-result";
import { PublicCsiInspectionResult } from "../../models/public-search/public-csi-inspection-result";

@Injectable({
    providedIn: 'root'
})
export class PublicSearchService {
    constructor(
        private readonly _http: HttpClient,
        private readonly _urlResolver: UrlResolverService,
        private readonly _queryHelper: QueryHelperService
    ) {

    }

    public getWaterSuppliers(domain?: string): Promise<PublicSearchWaterSuppliers> {
        const url = this._urlResolver.resolveUrl('/api/public-search/water-suppliers');
        let params = new HttpParams();

        if (domain) {
            params = params.append('domain', domain);
        }

        return lastValueFrom(this._http.get<PublicSearchWaterSuppliers>(url, { params }));
    }

    public searchBackflowTests(criteria: PublicSearchCriteria, pageInfo: PageInfo): Promise<PagedData<PublicBackflowTestResult>> {
        const url = this._urlResolver.resolveUrl('/api/public-search/backflow-tests');

        return lastValueFrom(this._http.get<PagedData<PublicBackflowTestResult>>(url, {
            params: this.buildSearchParams(criteria, pageInfo)
        }));
    }

    public searchCsiInspections(criteria: PublicSearchCriteria, pageInfo: PageInfo): Promise<PagedData<PublicCsiInspectionResult>> {
        const url = this._urlResolver.resolveUrl('/api/public-search/csi-inspections');

        return lastValueFrom(this._http.get<PagedData<PublicCsiInspectionResult>>(url, {
            params: this.buildSearchParams(criteria, pageInfo)
        }));
    }

    private buildSearchParams(criteria: PublicSearchCriteria, pageInfo: PageInfo): HttpParams {
        let params = this._queryHelper
            .buildQuery(pageInfo, { sort: {}, filter: [] })
            .append('waterSupplierId', String(criteria.waterSupplierId));

        const addresses: Record<string, string | undefined> = {
            propertyBusinessName: criteria.propertyBusinessName,
            propertyStreetNumber: criteria.propertyStreetNumber,
            propertyStreetName: criteria.propertyStreetName,
            propertyNumber: criteria.propertyNumber
        };

        for (const [name, value] of Object.entries(addresses)) {
            if (value) {
                params = params.append(name, value);
            }
        }

        return params;
    }
}
