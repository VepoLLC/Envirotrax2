import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { lastValueFrom } from "rxjs";
import { UrlResolverService } from "../helpers/url-resolver.service";
import { QueryHelperService } from "../helpers/query-helper.service";
import { BackflowRenewalRequirement } from "../../models/settings/backflow-renewal-requirement";
import { PagedData } from "../../models/paged-data";
import { PageInfo } from "../../models/page-info";
import { Query } from "../../models/query";

@Injectable({
    providedIn: 'root'
})
export class BackflowRenewalRequirementService {
    constructor(
        private readonly _urlResolver: UrlResolverService,
        private readonly _queryHelper: QueryHelperService,
        private readonly _http: HttpClient
    ) {
    }

    public getAll(pageInfo: PageInfo, query: Query): Promise<PagedData<BackflowRenewalRequirement>> {
        const url = this._urlResolver.resolveUrl('/api/backflow-renewal-requirements');

        const observable = this._http.get<PagedData<BackflowRenewalRequirement>>(url, {
            params: this._queryHelper.buildQuery(pageInfo, query)
        });

        return lastValueFrom(observable);
    }

    public add(requirement: BackflowRenewalRequirement): Promise<BackflowRenewalRequirement> {
        const url = this._urlResolver.resolveUrl('/api/backflow-renewal-requirements');
        return lastValueFrom(this._http.post<BackflowRenewalRequirement>(url, requirement));
    }

    public update(requirement: BackflowRenewalRequirement): Promise<BackflowRenewalRequirement> {
        const url = this._urlResolver.resolveUrl(`/api/backflow-renewal-requirements/${requirement.id}`);
        return lastValueFrom(this._http.put<BackflowRenewalRequirement>(url, requirement));
    }

    public delete(id: number): Promise<BackflowRenewalRequirement> {
        const url = this._urlResolver.resolveUrl(`/api/backflow-renewal-requirements/${id}`);
        return lastValueFrom(this._http.delete<BackflowRenewalRequirement>(url));
    }
}
