import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { PagedData, PageInfo, Query, QueryHelperService, UrlResolverService } from "@envirotrax/common-ui";
import { lastValueFrom } from "rxjs";
import { CompanyContact } from "../../models/crm/company-contact";

@Injectable({
    providedIn: 'root'
})
export class CompanyContactService {
    constructor(
        private readonly _urlResolver: UrlResolverService,
        private readonly _queryHelper: QueryHelperService,
        private readonly _http: HttpClient
    ) {

    }

    public getAll(companyId: number, pageInfo: PageInfo, query: Query): Promise<PagedData<CompanyContact>> {
        const url = this._urlResolver.resolveUrl(`/api/crm/companies/${companyId}/contacts`);

        return lastValueFrom(this._http.get<PagedData<CompanyContact>>(url, {
            params: this._queryHelper.buildQuery(pageInfo, query)
        }));
    }

    public add(companyId: number, contact: CompanyContact): Promise<CompanyContact> {
        const url = this._urlResolver.resolveUrl(`/api/crm/companies/${companyId}/contacts`);

        return lastValueFrom(this._http.post<CompanyContact>(url, contact));
    }

    public update(companyId: number, contact: CompanyContact): Promise<CompanyContact> {
        const url = this._urlResolver.resolveUrl(`/api/crm/companies/${companyId}/contacts/${contact.id}`);

        return lastValueFrom(this._http.put<CompanyContact>(url, contact));
    }

    public delete(companyId: number, contactId: number): Promise<void> {
        const url = this._urlResolver.resolveUrl(`/api/crm/companies/${companyId}/contacts/${contactId}`);

        return lastValueFrom(this._http.delete<void>(url));
    }
}
