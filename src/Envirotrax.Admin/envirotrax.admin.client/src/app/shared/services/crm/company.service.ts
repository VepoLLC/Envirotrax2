import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { PagedData, PageInfo, Query, QueryHelperService, UrlResolverService } from "@envirotrax/common-ui";
import { lastValueFrom } from "rxjs";
import { Company } from "../../models/crm/company";
import { WaterSupplierUser } from "../../models/water-suppliers/water-supplier-user";

@Injectable({
    providedIn: 'root'
})
export class CompanyService {
    constructor(
        private readonly _urlResolver: UrlResolverService,
        private readonly _queryHelper: QueryHelperService,
        private readonly _http: HttpClient
    ) {

    }

    public getAll(pageInfo: PageInfo, query: Query): Promise<PagedData<Company>> {
        const url = this._urlResolver.resolveUrl('/api/crm/companies');

        return lastValueFrom(this._http.get<PagedData<Company>>(url, {
            params: this._queryHelper.buildQuery(pageInfo, query)
        }));
    }

    public get(id: number): Promise<Company> {
        const url = this._urlResolver.resolveUrl(`/api/crm/companies/${id}`);

        return lastValueFrom(this._http.get<Company>(url));
    }

    public add(company: Company): Promise<Company> {
        const url = this._urlResolver.resolveUrl('/api/crm/companies');

        return lastValueFrom(this._http.post<Company>(url, company));
    }

    public update(company: Company): Promise<Company> {
        const url = this._urlResolver.resolveUrl(`/api/crm/companies/${company.id}`);

        return lastValueFrom(this._http.put<Company>(url, company));
    }

    public delete(id: number): Promise<void> {
        const url = this._urlResolver.resolveUrl(`/api/crm/companies/${id}`);

        return lastValueFrom(this._http.delete<void>(url));
    }

    /**
     * The users a company's Last Contacted By can name: the users of the Vepo LLC water supplier.
     */
    public getCrmUsers(): Promise<PagedData<WaterSupplierUser>> {
        const url = this._urlResolver.resolveUrl('/api/crm/users');

        return lastValueFrom(this._http.get<PagedData<WaterSupplierUser>>(url, {
            params: this._queryHelper.buildQuery({ pageNumber: 1, pageSize: 500 }, { sort: { emailAddress: 'Asc' }, filter: [] })
        }));
    }
}
