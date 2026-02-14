import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { State } from "../../models/states/state";
import { UrlResolverService } from "../helpers/url-resolver.service";
import { lastValueFrom, Observable } from "rxjs";

@Injectable({
    providedIn: 'root'
})
export class LookupService {
    private _states$!: Observable<State[]>

    constructor(
        private readonly _urlResolver: UrlResolverService,
        private readonly _http: HttpClient
    ) {
    }

    public async getAllStates(): Promise<State[]> {
        const url = this._urlResolver.resolveUrl('/api/lookup/states');

        if (!this._states$) {
            this._states$ = this._http.get<State[]>(url);
        }

        return await lastValueFrom(this._states$);
    }
}
