import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { lastValueFrom } from "rxjs";
import { UrlResolverService } from "../helpers/url-resolver.service";
import { WaterSupplierDashboardStats } from "../../models/water-suppliers/water-supplier-dashboard-stats";
import { CsiSubmissionStats } from "../../models/water-suppliers/csi-submission-stats";

@Injectable({
    providedIn: 'root'
})
export class WaterSupplierDashboardService {
    constructor(
        private readonly _http: HttpClient,
        private readonly _urlResolver: UrlResolverService
    ) { }

    public getStats(): Promise<WaterSupplierDashboardStats> {
        const url = this._urlResolver.resolveUrl('/api/water-suppliers/dashboard/stats');
        return lastValueFrom(this._http.get<WaterSupplierDashboardStats>(url));
    }

    public getCsiSubmissionStats(): Promise<CsiSubmissionStats> {
        const url = this._urlResolver.resolveUrl('/api/water-suppliers/dashboard/csi-submission-stats');
        return lastValueFrom(this._http.get<CsiSubmissionStats>(url));
    }
}
