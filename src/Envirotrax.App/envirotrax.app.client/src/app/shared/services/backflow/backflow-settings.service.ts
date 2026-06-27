import { Injectable } from "@angular/core";
import { HttpClient, HttpParams } from "@angular/common/http";
import { lastValueFrom } from "rxjs";
import { UrlResolverService } from "../helpers/url-resolver.service";
import { BackflowTestingSettings } from "../../models/backflow/backflow-testing-settings";

@Injectable({
    providedIn: 'root'
})
export class BackflowSettingsService {

    private readonly _cache = new Map<number, Promise<BackflowTestingSettings>>();

    constructor(
        private readonly _urlResolver: UrlResolverService,
        private readonly _http: HttpClient
    ) {
    }

    public getTestingSettings(waterSupplierId: number): Promise<BackflowTestingSettings> {
        const cached = this._cache.get(waterSupplierId);
        if (cached) {
            return cached;
        }

        const url = this._urlResolver.resolveUrl('/api/professionals/backflow/settings');
        const params = new HttpParams().set('waterSupplierId', waterSupplierId);

        const promise = lastValueFrom(this._http.get<BackflowTestingSettings>(url, { params }))
            .catch(error => {
                this._cache.delete(waterSupplierId);
                throw error;
            });

        this._cache.set(waterSupplierId, promise);
        return promise;
    }
}
