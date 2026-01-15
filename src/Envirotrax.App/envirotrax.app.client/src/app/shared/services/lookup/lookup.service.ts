import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { State } from "../../models/states/state";
import { UrlResolverService } from "../helpers/url-resolver.service";
import { lastValueFrom } from "rxjs";

@Injectable({
    providedIn: 'root'
})
export class LookupService {

    constructor(
        private readonly _urlResolver: UrlResolverService,
        private readonly _http: HttpClient
    ) {
    }

    public async getAllStates(): Promise<State[]> {
        const url = this._urlResolver.resolveUrl('/api/lookup/states');

        return await lastValueFrom(
            this._http.get<State[]>(url)
        );
    }
}
