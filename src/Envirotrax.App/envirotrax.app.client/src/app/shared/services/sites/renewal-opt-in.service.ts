import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { lastValueFrom } from "rxjs";
import { UrlResolverService } from "../helpers/url-resolver.service";
import { RenewalOptInRequest, RenewalOptInResult, RenewalOptInSite, RenewalOptInTokenRequest } from "../../models/sites/renewal-opt-in";

@Injectable({
    providedIn: 'root'
})
export class RenewalOptInService {
    constructor(
        private readonly _http: HttpClient,
        private readonly _urlResolver: UrlResolverService
    ) {

    }

    public getSite(siteId: number): Promise<RenewalOptInSite> {
        const url = this._urlResolver.resolveUrl(`/api/renewal-opt-in/sites/${siteId}`);

        return lastValueFrom(this._http.get<RenewalOptInSite>(url));
    }

    public save(siteId: number, request: RenewalOptInRequest): Promise<RenewalOptInResult> {
        const url = this._urlResolver.resolveUrl(`/api/renewal-opt-in/sites/${siteId}`);

        return lastValueFrom(this._http.post<RenewalOptInResult>(url, request));
    }

    public verifyEmail(request: RenewalOptInTokenRequest): Promise<RenewalOptInResult> {
        const url = this._urlResolver.resolveUrl('/api/renewal-opt-in/verify-email');

        return lastValueFrom(this._http.post<RenewalOptInResult>(url, request));
    }

    public unsubscribe(request: RenewalOptInTokenRequest): Promise<RenewalOptInResult> {
        const url = this._urlResolver.resolveUrl('/api/renewal-opt-in/unsubscribe');

        return lastValueFrom(this._http.post<RenewalOptInResult>(url, request));
    }
}
