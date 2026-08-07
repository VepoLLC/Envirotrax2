import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { InputOption, UrlResolverService } from "@envirotrax/common-ui";
import { State } from "../../models/lookup/state";
import { lastValueFrom, Observable, shareReplay } from "rxjs";

@Injectable({
    providedIn: 'root'
})
export class LookupService {
    private _states: Observable<State[]> | null = null;

    constructor(
        private readonly _urlResolver: UrlResolverService,
        private readonly _http: HttpClient
    ) {

    }

    public async getStates(): Promise<State[]> {
        const url = this._urlResolver.resolveUrl('/api/lookup/states');

        if (!this._states) {
            this._states = this._http.get<State[]>(url).pipe(
                shareReplay(1)
            );
        }

        return await lastValueFrom(this._states);
    }

    public async getStatesAsOptions(): Promise<InputOption<State>[]> {
        const states = await this.getStates();

        return states.map(state => ({ id: String(state.id), text: state.code ?? '', data: state }));
    }
}
