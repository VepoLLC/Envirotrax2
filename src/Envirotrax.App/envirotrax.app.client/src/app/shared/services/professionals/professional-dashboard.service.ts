import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { lastValueFrom } from 'rxjs';
import { UrlResolverService } from '../helpers/url-resolver.service';
import { ProfessionalDashboardStats } from '../../models/professionals/professional-dashboard-stats';

@Injectable({
    providedIn: 'root'
})
export class ProfessionalDashboardService {
    constructor(
        private readonly _http: HttpClient,
        private readonly _urlResolver: UrlResolverService
    ) {}

    public getStats(): Promise<ProfessionalDashboardStats> {
        const url = this._urlResolver.resolveUrl('/api/professionals/dashboard/stats');
        return lastValueFrom(this._http.get<ProfessionalDashboardStats>(url));
    }
}
