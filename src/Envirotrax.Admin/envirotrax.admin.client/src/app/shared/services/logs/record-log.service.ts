import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { PagedData, PageInfo, Query, QueryHelperService, RecordLog, RecordLogType, UrlResolverService } from "@envirotrax/common-ui";
import { lastValueFrom } from "rxjs";

export interface RecordLogSearchCriteria {
    tableName?: string | null;
    waterSupplierId?: string | null;
    recordId?: number | null;
    userEmail?: string | null;
    logType?: RecordLogType | null;
}

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

    public async search(pageInfo: PageInfo, query: Query, criteria: RecordLogSearchCriteria): Promise<PagedData<RecordLogSearchResult>> {
        const url = this._urlResolver.resolveUrl('/api/record-logs');

        let params = this._queryHelper.buildQuery(pageInfo, query);

        for (const [name, value] of Object.entries(criteria)) {
            if (value != null && value !== '') {
                params = params.append(name, value);
            }
        }

        const observable = this._http.get<PagedData<RecordLogSearchResult>>(url, { params });

        return await lastValueFrom(observable);
    }
}
