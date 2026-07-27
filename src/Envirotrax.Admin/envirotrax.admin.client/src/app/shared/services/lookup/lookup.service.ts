import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { UrlResolverService } from "@envirotrax/common-ui";
import { lastValueFrom, Observable, shareReplay } from "rxjs";
import { State } from "../../models/lookup/state";

@Injectable({
    providedIn: 'root'
})
export class LookupService {
    private _states$?: Observable<State[]>;

    constructor(
        private readonly _urlResolver: UrlResolverService,
        private readonly _http: HttpClient
    ) {

    }

    public async getAllStates(): Promise<State[]> {
        if (!this._states$) {
            const url = this._urlResolver.resolveUrl('/api/lookup/states');
            this._states$ = this._http.get<State[]>(url).pipe(shareReplay(1));
        }

        return await lastValueFrom(this._states$);
    }
}
