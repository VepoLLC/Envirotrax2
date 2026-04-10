import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { lastValueFrom } from "rxjs";
import { UrlResolverService } from "../helpers/url-resolver.service";
import { Professional } from "../../models/professionals/professional";

@Injectable({
    providedIn: 'root'
})
export class CsiInspectorAccountInfoService {
    constructor(
        private readonly _urlResolver: UrlResolverService,
        private readonly _http: HttpClient
    ) {
    }

    public async getAccountInfo(id: number): Promise<Professional> {
        const url = this._urlResolver.resolveUrl(`/api/csi/inspectors/${id}/account-info`);
        return await lastValueFrom(this._http.get<Professional>(url));
    }
}
