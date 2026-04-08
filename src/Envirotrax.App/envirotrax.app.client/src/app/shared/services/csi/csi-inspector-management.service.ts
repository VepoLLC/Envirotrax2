import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { lastValueFrom } from "rxjs";
import { UrlResolverService } from "../helpers/url-resolver.service";
import { QueryHelperService } from "../helpers/query-helper.service";
import { PageInfo } from "../../models/page-info";
import { Query } from "../../models/query";
import { PagedData } from "../../models/paged-data";
import { Professional } from "../../models/professionals/professional";
import { CsiInspectorAccount} from "../../models/csi/csi-inspector-account";
import { ProfessionalWaterSupplier } from "../../models/professionals/professional-water-supplier";
import { ProfessionalUser } from "../../models/professionals/professional-user";
import { ProfessionalUserLicense } from "../../models/professionals/licenses/professional-user-license";
import { ProfessionalInsurance } from "../../models/professionals/professional-insurance";

@Injectable({
    providedIn: 'root'
})
export class CsiInspectoreManagementService {
    constructor(
        private readonly _urlResolver: UrlResolverService,
        private readonly _queryHelper: QueryHelperService,
        private readonly _http: HttpClient
    ) {
    }

    public async getAll(pageInfo: PageInfo, query: Query): Promise<PagedData<Professional>> {
        const url = this._urlResolver.resolveUrl('/api/csi/inspectors');

        const observable = this._http.get<PagedData<Professional>>(url, {
            params: this._queryHelper.buildQuery(pageInfo, query)
        });

        return await lastValueFrom(observable);
    }

    public async getAccountInfo(id: number): Promise<CsiInspectorAccount> {
        const url = this._urlResolver.resolveUrl(`/api/csi/inspectors/${id}/account-info`);
        return await lastValueFrom(this._http.get<CsiInspectorAccount>(url));
    }

    public async getWaterSuppliers(id: number, pageInfo: PageInfo, query: Query): Promise<PagedData<ProfessionalWaterSupplier>> {
        const url = this._urlResolver.resolveUrl(`/api/csi/inspectors/${id}/water-suppliers`);
        return await lastValueFrom(this._http.get<PagedData<ProfessionalWaterSupplier>>(url, {
            params: this._queryHelper.buildQuery(pageInfo, query)
        }));
    }

    public async getSubAccounts(id: number, pageInfo: PageInfo, query: Query): Promise<PagedData<ProfessionalUser>> {
        const url = this._urlResolver.resolveUrl(`/api/csi/inspectors/${id}/sub-accounts`);
        return await lastValueFrom(this._http.get<PagedData<ProfessionalUser>>(url, {
            params: this._queryHelper.buildQuery(pageInfo, query)
        }));
    }

    public async getLicenses(id: number, pageInfo: PageInfo, query: Query): Promise<PagedData<ProfessionalUserLicense>> {
        const url = this._urlResolver.resolveUrl(`/api/csi/inspectors/${id}/licenses`);
        return await lastValueFrom(this._http.get<PagedData<ProfessionalUserLicense>>(url, {
            params: this._queryHelper.buildQuery(pageInfo, query)
        }));
    }

    public async getInsurances(id: number, pageInfo: PageInfo, query: Query): Promise<PagedData<ProfessionalInsurance>> {
        const url = this._urlResolver.resolveUrl(`/api/csi/inspectors/${id}/insurances`);
        return await lastValueFrom(this._http.get<PagedData<ProfessionalInsurance>>(url, {
            params: this._queryHelper.buildQuery(pageInfo, query)
        }));
    }
}
