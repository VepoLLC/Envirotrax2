import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { lastValueFrom } from "rxjs";
import { UrlResolverService } from "../helpers/url-resolver.service";
import { WaterSupplierDashboardStats } from "../../models/water-suppliers/water-supplier-dashboard-stats";
import { CsiSubmissionStats } from "../../models/water-suppliers/csi-submission-stats";
import { BackflowSubmissionStats } from "../../models/water-suppliers/backflow-submission-stats";
import { FogInspectionSubmissionStats } from "../../models/water-suppliers/fog-inspection-submission-stats";

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

    public getBackflowSubmissionStats(): Promise<BackflowSubmissionStats> {
        const url = this._urlResolver.resolveUrl('/api/water-suppliers/dashboard/backflow-submission-stats');
        return lastValueFrom(this._http.get<BackflowSubmissionStats>(url));
    }

    public getFogInspectionSubmissionStats(): Promise<FogInspectionSubmissionStats> {
        const url = this._urlResolver.resolveUrl('/api/water-suppliers/dashboard/fog-inspection-submission-stats');
        return lastValueFrom(this._http.get<FogInspectionSubmissionStats>(url));
    }
}
