import { Injectable } from "@angular/core";
import { UrlResolverService } from "../helpers/url-resolver.service";
import { QueryHelperService } from "../helpers/query-helper.service";
import { HttpClient } from "@angular/common/http";
import { PageInfo } from "../../models/page-info";
import { Query } from "../../models/query";
import { PagedData } from "../../models/paged-data";
import { Professional } from "../../models/professionals/professional";
import { lastValueFrom, Observable, shareReplay } from "rxjs";

@Injectable({
    providedIn: 'root'
})
export class ProfesisonalService {
    private _currentProfessional$?: Observable<Professional>;

    constructor(
        private readonly _urlResolver: UrlResolverService,
        private readonly _queryHelper: QueryHelperService,
        private readonly _http: HttpClient
    ) {

    }

    public getAllMy(pageInfo: PageInfo, query: Query): Promise<PagedData<Professional>> {
        const url = this._urlResolver.resolveUrl('/api/professionals/my');

        const observable = this._http.get<PagedData<Professional>>(url, {
            params: this._queryHelper.buildQuery(pageInfo, query)
        });

        return lastValueFrom(observable);
    }

    public getLoggedInProfessional(): Promise<Professional> {
        const url = this._urlResolver.resolveUrl('/api/professionals/my/current');

        if (!this._currentProfessional$) {
            this._currentProfessional$ = this._http.get<Professional>(url).pipe(shareReplay(1));
        }

        return lastValueFrom(this._currentProfessional$);
    }

    public addMy(professional: Professional): Promise<Professional> {
        const url = this._urlResolver.resolveUrl('/api/professionals/my');

        const observable = this._http.post<Professional>(url, professional);
        this._currentProfessional$ = undefined;

        return lastValueFrom(observable);
    }
}