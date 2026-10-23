import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { lastValueFrom } from "rxjs";
import { UrlResolverService } from "../helpers/url-resolver.service";
import { QueryHelperService } from "../helpers/query-helper.service";
import { PageInfo } from "../../models/page-info";
import { PagedData } from "../../models/paged-data";
import { Query } from "../../models/query";
import { ProfessionalTransaction } from "../../models/payments/professional-transaction";

@Injectable({
    providedIn: 'root'
})
export class ProfessionalTransactionService {
    constructor(
        private readonly _urlResolver: UrlResolverService,
        private readonly _queryHelper: QueryHelperService,
        private readonly _http: HttpClient
    ) {
    }

    public getAll(pageInfo: PageInfo, query: Query): Promise<PagedData<ProfessionalTransaction>> {
        const url = this._urlResolver.resolveUrl('/api/professionals/transactions');

        const observable = this._http.get<PagedData<ProfessionalTransaction>>(url, {
            params: this._queryHelper.buildQuery(pageInfo, query)
        });

        return lastValueFrom(observable);
    }

    public async getReceiptPdf(transactionId: string): Promise<Blob> {
        const url = this._urlResolver.resolveUrl(`/api/professionals/transactions/${encodeURIComponent(transactionId)}/receipt`);
        return await lastValueFrom(this._http.get(url, { responseType: 'blob' }));
    }
}
