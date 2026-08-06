import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { InputOption, UrlResolverService } from "@envirotrax/common-ui";
import { State } from "../../models/lookup/state";
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

    public async getStates(): Promise<State[]> {
        const url = this._urlResolver.resolveUrl('/api/lookup/states');

        const observable = this._http.get<State[]>(url);

        return await lastValueFrom(observable);
    }

    public async getStatesAsOptions(): Promise<InputOption<State>[]> {
        const states = await this.getStates();

        return states.map(state => ({ id: String(state.id), text: state.code ?? '', data: state }));
    }
}
