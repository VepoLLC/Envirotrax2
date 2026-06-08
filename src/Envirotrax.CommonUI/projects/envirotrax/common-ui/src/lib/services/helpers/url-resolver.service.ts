import { Inject, Injectable } from "@angular/core";
import { API_BASE_URL } from "../../tokens/api-url.token";

@Injectable({
    providedIn: 'root'
})
export class UrlResolverService {
    constructor(@Inject(API_BASE_URL) private readonly _apiBaseUrl: string) { }

    public resolveUrl(relativePath: string): string {
        if (relativePath[0] == '/') {
            relativePath = relativePath.substring(1);
        }

        return this._apiBaseUrl + '/' + relativePath;
    }
}