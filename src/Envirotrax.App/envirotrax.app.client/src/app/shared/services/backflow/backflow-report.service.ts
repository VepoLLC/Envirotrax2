import { Injectable } from "@angular/core";
import { HttpClient, HttpParams } from "@angular/common/http";
import { lastValueFrom } from "rxjs";
import { UrlResolverService } from "../helpers/url-resolver.service";
import { BackflowTestReport } from "../../models/backflow/backflow-test-report";
import { BackflowComplianceReport } from "../../models/backflow/backflow-compliance-report";
import { BackflowComplianceHistory } from "../../models/backflow/backflow-compliance-history";
import { BackflowNewRemovedReport } from "../../models/backflow/backflow-new-removed-report";

@Injectable({
    providedIn: 'root'
})
export class BackflowReportService {
    constructor(
        private readonly _urlResolver: UrlResolverService,
        private readonly _http: HttpClient
    ) {
    }

    public getTestReport(fromDate: string, toDate: string): Promise<BackflowTestReport> {
        const url = this._urlResolver.resolveUrl('/api/backflow/reports/tests');

        let params = new HttpParams();
        if (fromDate) {
            params = params.set('fromDate', fromDate);
        }
        if (toDate) {
            params = params.set('toDate', toDate);
        }

        return lastValueFrom(
            this._http.get<BackflowTestReport>(url, { params })
        );
    }

    public getTestReportPdf(fromDate: string, toDate: string): Promise<Blob> {
        const url = this._urlResolver.resolveUrl('/api/backflow/reports/tests/pdf');

        let params = new HttpParams();
        if (fromDate) {
            params = params.set('fromDate', fromDate);
        }
        if (toDate) {
            params = params.set('toDate', toDate);
        }

        return lastValueFrom(
            this._http.get(url, { params, responseType: 'blob' })
        );
    }

    public getTestReportExcel(fromDate: string, toDate: string): Promise<Blob> {
        const url = this._urlResolver.resolveUrl('/api/backflow/reports/tests/excel');

        let params = new HttpParams();
        if (fromDate) {
            params = params.set('fromDate', fromDate);
        }
        if (toDate) {
            params = params.set('toDate', toDate);
        }

        return lastValueFrom(
            this._http.get(url, { params, responseType: 'blob' })
        );
    }

    public getTestReportWord(fromDate: string, toDate: string): Promise<Blob> {
        const url = this._urlResolver.resolveUrl('/api/backflow/reports/tests/word');

        let params = new HttpParams();
        if (fromDate) {
            params = params.set('fromDate', fromDate);
        }
        if (toDate) {
            params = params.set('toDate', toDate);
        }

        return lastValueFrom(
            this._http.get(url, { params, responseType: 'blob' })
        );
    }

    public async getEarliestTestDate(): Promise<string | null> {
        const url = this._urlResolver.resolveUrl('/api/backflow/reports/tests/earliest-date');

        const result = await lastValueFrom(
            this._http.get<{ earliestDate: string | null }>(url)
        );

        return result.earliestDate;
    }

    public getComplianceReport(ignoreLast30Days: boolean): Promise<BackflowComplianceReport> {
        const url = this._urlResolver.resolveUrl('/api/backflow/reports/compliance/current');

        const params = new HttpParams().set('ignoreLast30Days', ignoreLast30Days);

        return lastValueFrom(
            this._http.get<BackflowComplianceReport>(url, { params })
        );
    }

    public getComplianceReportPdf(ignoreLast30Days: boolean): Promise<Blob> {
        const url = this._urlResolver.resolveUrl('/api/backflow/reports/compliance/current/pdf');
        const params = new HttpParams().set('ignoreLast30Days', ignoreLast30Days);

        return lastValueFrom(
            this._http.get(url, { params, responseType: 'blob' })
        );
    }

    public getComplianceReportExcel(ignoreLast30Days: boolean): Promise<Blob> {
        const url = this._urlResolver.resolveUrl('/api/backflow/reports/compliance/current/excel');
        const params = new HttpParams().set('ignoreLast30Days', ignoreLast30Days);

        return lastValueFrom(
            this._http.get(url, { params, responseType: 'blob' })
        );
    }

    public getComplianceReportWord(ignoreLast30Days: boolean): Promise<Blob> {
        const url = this._urlResolver.resolveUrl('/api/backflow/reports/compliance/current/word');
        const params = new HttpParams().set('ignoreLast30Days', ignoreLast30Days);

        return lastValueFrom(
            this._http.get(url, { params, responseType: 'blob' })
        );
    }

    public getComplianceHistory(): Promise<BackflowComplianceHistory> {
        const url = this._urlResolver.resolveUrl('/api/backflow/reports/compliance/history');

        return lastValueFrom(
            this._http.get<BackflowComplianceHistory>(url)
        );
    }

    public getComplianceHistoryPdf(): Promise<Blob> {
        const url = this._urlResolver.resolveUrl('/api/backflow/reports/compliance/history/pdf');

        return lastValueFrom(
            this._http.get(url, { responseType: 'blob' })
        );
    }

    public getComplianceHistoryExcel(): Promise<Blob> {
        const url = this._urlResolver.resolveUrl('/api/backflow/reports/compliance/history/excel');

        return lastValueFrom(
            this._http.get(url, { responseType: 'blob' })
        );
    }

    public getComplianceHistoryWord(): Promise<Blob> {
        const url = this._urlResolver.resolveUrl('/api/backflow/reports/compliance/history/word');

        return lastValueFrom(
            this._http.get(url, { responseType: 'blob' })
        );
    }

    public getNewRemoved(): Promise<BackflowNewRemovedReport> {
        const url = this._urlResolver.resolveUrl('/api/backflow/reports/assemblies/new-removed');

        return lastValueFrom(
            this._http.get<BackflowNewRemovedReport>(url)
        );
    }

    public getNewRemovedPdf(): Promise<Blob> {
        const url = this._urlResolver.resolveUrl('/api/backflow/reports/assemblies/new-removed/pdf');

        return lastValueFrom(
            this._http.get(url, { responseType: 'blob' })
        );
    }

    public getNewRemovedExcel(): Promise<Blob> {
        const url = this._urlResolver.resolveUrl('/api/backflow/reports/assemblies/new-removed/excel');

        return lastValueFrom(
            this._http.get(url, { responseType: 'blob' })
        );
    }

    public getNewRemovedWord(): Promise<Blob> {
        const url = this._urlResolver.resolveUrl('/api/backflow/reports/assemblies/new-removed/word');

        return lastValueFrom(
            this._http.get(url, { responseType: 'blob' })
        );
    }
}
