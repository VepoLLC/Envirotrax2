import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { PagedData, PageInfo, Query, QueryHelperService, RecordLog, UrlResolverService } from "@envirotrax/common-ui";
import { lastValueFrom } from "rxjs";

export interface RecordLogSearchResult extends RecordLog {
    waterSupplierId?: number;
    professionalId?: number;
}

@Injectable({
    providedIn: 'root'
})
export class RecordLogService {
    constructor(
        private readonly _urlResolver: UrlResolverService,
        private readonly _queryHelper: QueryHelperService,
        private readonly _http: HttpClient
    ) {

    }

    public async getAll(pageInfo: PageInfo, query: Query): Promise<PagedData<RecordLogSearchResult>> {
        const url = this._urlResolver.resolveUrl('/api/record-logs');
        const params = this._queryHelper.buildQuery(pageInfo, query);

        const observable = this._http.get<PagedData<RecordLogSearchResult>>(url, { params });

        return await lastValueFrom(observable);
    }
}
