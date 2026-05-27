import { Component, OnInit } from "@angular/core";
import { CsiSystemReport, CsiReportPeriod } from "../../../shared/models/csi/csi-system-report";
import { CsiSystemReportService } from "../../../shared/services/csi/csi-system-report.service";

@Component({
    standalone: false,
    templateUrl: './csi-system-report.component.html',
    styleUrls: ['./csi-system-report.component.scss']
})
export class CsiSystemReportComponent implements OnInit {
    public report: CsiSystemReport | null = null;
    public isLoading = false;
    public fromDate: string = '';
    public toDate: string = '';
    public dateRangeError: string = '';
    public isDateValid: boolean = false;

    constructor(private readonly _reportService: CsiSystemReportService) {}

    public ngOnInit(): void {
        this.fromDate = '2014-01-01';
        this.toDate = new Date().toISOString().split('T')[0];
        this.loadReport();
    }

    public async loadReport(): Promise<void> {
        this.validateDates();

        if (this.isDateValid) {
            try {
                this.isLoading = true;
                this.report = await this._reportService.getSystemReport(this.fromDate, this.toDate);
            } finally {
                this.isLoading = false;
            }
        }
    }

    public showByMonth(period: CsiReportPeriod): void {
        if (!period.year || period.month) return;
        this.fromDate = `${period.year}-01-01`;
        this.toDate = `${period.year}-12-31`;
        this.loadReport();
    }

    public filterToMonth(period: CsiReportPeriod): void {
        if (!period.year || !period.month) return;
        const lastDay = new Date(period.year, period.month, 0).getDate();
        const mm = String(period.month).padStart(2, '0');
        const dd = String(lastDay).padStart(2, '0');
        this.fromDate = `${period.year}-${mm}-01`;
        this.toDate = `${period.year}-${mm}-${dd}`;
        this.loadReport();
    }

    private validateDates(): void {
        if (this.fromDate && this.toDate && this.fromDate > this.toDate) {
            this.isDateValid = false;
            this.dateRangeError = 'Invalid Date Range: The From Date cannot be after the To Date.';
            return;
        }
        this.isDateValid = true;
        this.dateRangeError = '';
    }

    public isYearView(): boolean {
        return this.report?.periods.some(p => !p.month) ?? false;
    }

    public getBarPercent(value: number): number {
        if (this.report != null && this.report.totalCount > 0) {
            return Math.round((value / this.report.totalCount) * 100);
        } else {
            return 0;
        }
    }
}
