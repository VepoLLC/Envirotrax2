import { Injectable } from "@angular/core";
import { HttpClient, HttpParams } from "@angular/common/http";
import { lastValueFrom } from "rxjs";
import { UrlResolverService } from "../helpers/url-resolver.service";
import { CsiSystemReport } from "../../models/csi/csi-system-report";

@Injectable({
    providedIn: 'root'
})
export class CsiSystemReportService {
    constructor(
        private readonly _urlResolver: UrlResolverService,
        private readonly _http: HttpClient
    ) {
    }

    public getSystemReport(fromDate: string, toDate: string): Promise<CsiSystemReport> {
        const url = this._urlResolver.resolveUrl('/api/csi/reports/system');

        let params = new HttpParams();
        if (fromDate) params = params.set('fromDate', fromDate);
        if (toDate) params = params.set('toDate', toDate);

        return lastValueFrom(
            this._http.get<CsiSystemReport>(url, { params })
        );
    }
}
