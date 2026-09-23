import { Injectable } from "@angular/core";
import { HttpClient, HttpParams } from "@angular/common/http";
import { lastValueFrom } from "rxjs";
import { UrlResolverService } from "../helpers/url-resolver.service";
import { FogSystemReport, FogTripTicketReportDateType } from "../../models/fog/fog-system-report";

@Injectable({
    providedIn: 'root'
})
export class FogSystemReportService {
    constructor(
        private readonly _urlResolver: UrlResolverService,
        private readonly _http: HttpClient
    ) {
    }

    public getTripTicketReport(dateType: FogTripTicketReportDateType, fromDate: string, toDate: string): Promise<FogSystemReport> {
        const url = this._urlResolver.resolveUrl('/api/fog/reports/trip-tickets');

        let params = new HttpParams().set('dateType', dateType);

        if (fromDate) {
            params = params.set('fromDate', fromDate);
        }
        if (toDate) {
            params = params.set('toDate', toDate);
        }

        return lastValueFrom(
            this._http.get<FogSystemReport>(url, { params })
        );
    }

    public async getEarliestTripTicketDate(): Promise<string | null> {
        const url = this._urlResolver.resolveUrl('/api/fog/reports/trip-tickets/earliest-date');

        const result = await lastValueFrom(
            this._http.get<{ earliestDate: string | null }>(url)
        );

        return result.earliestDate;
    }

    public getInspectionReport(fromDate: string, toDate: string): Promise<FogSystemReport> {
        const url = this._urlResolver.resolveUrl('/api/fog/reports/inspections');

        let params = new HttpParams();

        if (fromDate) {
            params = params.set('fromDate', fromDate);
        }
        if (toDate) {
            params = params.set('toDate', toDate);
        }

        return lastValueFrom(
            this._http.get<FogSystemReport>(url, { params })
        );
    }

    public async getEarliestInspectionDate(): Promise<string | null> {
        const url = this._urlResolver.resolveUrl('/api/fog/reports/inspections/earliest-date');

        const result = await lastValueFrom(
            this._http.get<{ earliestDate: string | null }>(url)
        );

        return result.earliestDate;
    }
}
