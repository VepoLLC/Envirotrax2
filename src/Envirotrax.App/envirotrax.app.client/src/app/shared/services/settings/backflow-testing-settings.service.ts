import { Injectable } from "@angular/core";
import { UrlResolverService } from "../helpers/url-resolver.service";
import { HttpClient } from "@angular/common/http";
import { BackflowSettings } from "../../models/settings/backflow-settings";
import { PagedData } from "../../models/paged-data";
import { lastValueFrom } from "rxjs";


@Injectable({
    providedIn: 'root'
})

export class BackflowTestingSettingsService {
    constructor(        
        private readonly _urlResolver: UrlResolverService,
        private readonly _http: HttpClient) {
    }

      public async get(): Promise<BackflowSettings> {
            const url = this._urlResolver.resolveUrl('/api/backflow-settings');
            const observable = this._http.get<PagedData<BackflowSettings>>(url);
            const response = await lastValueFrom(observable);
    
            return response.data?.[0] ?? {};
        }
    
        public add(settings: BackflowSettings): Promise<BackflowSettings> {
            const url = this._urlResolver.resolveUrl('/api/backflow-settings');
            const observable = this._http.post<BackflowSettings>(url, settings);
    
            return lastValueFrom(observable);
        }
    
        public update(settings: BackflowSettings): Promise<BackflowSettings | undefined> {
            const url = this._urlResolver.resolveUrl(`/api/backflow-settings/${settings.id}`);
            const observable = this._http.put<BackflowSettings>(url, settings);
    
            return lastValueFrom(observable);
        }
}