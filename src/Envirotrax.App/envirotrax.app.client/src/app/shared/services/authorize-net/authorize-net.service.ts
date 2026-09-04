import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { lastValueFrom, Observable } from "rxjs";
import { UrlResolverService } from "../helpers/url-resolver.service";
import { AuthorizeNetClientConfig } from "../../models/authorize-net/authorize-net-client-config";
import { environment } from "../../../../environments/environment";

@Injectable({
    providedIn: 'root'
})
export class AuthorizeNetService {
    private _clientConfig$!: Observable<AuthorizeNetClientConfig>;
    private _scriptLoaded$?: Promise<void>;

    constructor(
        private readonly _urlResolver: UrlResolverService,
        private readonly _http: HttpClient
    ) { }

    public async getClientConfig(): Promise<AuthorizeNetClientConfig> {
        const url = this._urlResolver.resolveUrl('/api/authorize-net/client-config');

        if (!this._clientConfig$) {
            this._clientConfig$ = this._http.get<AuthorizeNetClientConfig>(url);
        }

        return await lastValueFrom(this._clientConfig$);
    }

    public ensureAcceptUiScriptLoaded(): Promise<void> {
        if (!this._scriptLoaded$) {
            this._scriptLoaded$ = new Promise<void>((resolve, reject) => {
                const script = document.createElement('script');

                script.src = environment.authorizeNetScriptUrl;
                script.async = true;
                script.onload = () => resolve();
                script.onerror = () => reject(new Error('Failed to load Authorize.Net AcceptUI script.'));

                document.head.appendChild(script);
            });
        }

        return this._scriptLoaded$;
    }
}
