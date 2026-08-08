import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { Observable, lastValueFrom, catchError, shareReplay, throwError } from "rxjs";
import { UrlResolverService } from "../helpers/url-resolver.service";
import { ProfessionalFogSettings } from "../../models/fog/professional-fog-settings";

@Injectable({
    providedIn: 'root'
})
export class FogSettingsService {
    private readonly _cache = new Map<number, Observable<ProfessionalFogSettings>>();

    constructor(
        private readonly _urlResolver: UrlResolverService,
        private readonly _http: HttpClient
    ) {
    }

    public getSettings(waterSupplierId: number): Promise<ProfessionalFogSettings> {
        let cached = this._cache.get(waterSupplierId);

        if (!cached) {
            const url = this._urlResolver.resolveUrl(`/api/professionals/fog/settings/${waterSupplierId}`);

            cached = this._http.get<ProfessionalFogSettings>(url).pipe(
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
