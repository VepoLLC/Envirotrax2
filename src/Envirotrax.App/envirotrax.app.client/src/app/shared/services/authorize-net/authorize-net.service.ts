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
    private static readonly ACCEPT_UI_SCRIPT_ID = 'authorize-net-accept-ui';
    private static readonly ACCEPT_UI_ELEMENT_IDS = ['AcceptUIContainer', 'AcceptUIBackground'];

    private _clientConfig$!: Observable<AuthorizeNetClientConfig>;

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

    // AcceptUI.js only binds the .AcceptUI buttons that exist when it loads, so every new button needs a fresh load.
    public reloadAcceptUiScript(): Promise<void> {
        this.removeAcceptUiElements();

        return new Promise<void>((resolve, reject) => {
            const script = document.createElement('script');

            script.id = AuthorizeNetService.ACCEPT_UI_SCRIPT_ID;
            script.src = environment.authorizeNetScriptUrl;
            script.async = true;
            script.onload = () => resolve();
            script.onerror = () => reject(new Error('Failed to load Authorize.Net AcceptUI script.'));

            document.head.appendChild(script);
        });
    }

    private removeAcceptUiElements(): void {
        document.getElementById(AuthorizeNetService.ACCEPT_UI_SCRIPT_ID)?.remove();

        AuthorizeNetService.ACCEPT_UI_ELEMENT_IDS.forEach(id => document.getElementById(id)?.remove());
    }
}
