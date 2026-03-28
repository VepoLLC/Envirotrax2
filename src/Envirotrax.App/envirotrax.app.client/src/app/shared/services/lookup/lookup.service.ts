import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { State } from "../../models/lookup/state";
import { UrlResolverService } from "../helpers/url-resolver.service";
import { lastValueFrom, Observable } from "rxjs";
import { InputOption } from "../../components/input/input.component";

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

    public async getAllStatesAsOptions(includeEmpty: boolean): Promise<InputOption<State>[]> {
        const states = await this.getAllStates();

        const options: InputOption<State>[] = states.map(s => ({
            id: s.id,
            text: s.name,
            data: s
        }));

        if (includeEmpty) {
            options.splice(0, 0, { id: '', text: '' });
        }

        return options;
    }
}
