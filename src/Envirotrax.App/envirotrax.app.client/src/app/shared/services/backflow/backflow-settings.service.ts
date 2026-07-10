import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { Observable, lastValueFrom, catchError, shareReplay, throwError } from "rxjs";
import { UrlResolverService } from "../helpers/url-resolver.service";
import { BackflowTestingSettings } from "../../models/backflow/backflow-testing-settings";

@Injectable({
    providedIn: 'root'
})
export class BackflowSettingsService {
    private readonly _cache = new Map<number, Observable<BackflowTestingSettings>>();

    constructor(
        private readonly _urlResolver: UrlResolverService,
        private readonly _http: HttpClient
    ) {
    }

    public getTestingSettings(waterSupplierId: number): Promise<BackflowTestingSettings> {
        let cached = this._cache.get(waterSupplierId);

        if (!cached) {
            const url = this._urlResolver.resolveUrl(`/api/professionals/backflow/settings/${waterSupplierId}`);

            cached = this._http.get<BackflowTestingSettings>(url).pipe(
                shareReplay(1),
                catchError(error => {
                    this._cache.delete(waterSupplierId);
                    return throwError(() => error);
                })
            );

            this._cache.set(waterSupplierId, cached);
        }

        return lastValueFrom(cached);
    }
}
