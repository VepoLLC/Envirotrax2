import { Component, OnInit } from "@angular/core";
import { InputOption } from "@envirotrax/common-ui";
import { FogReportPeriod, FogReportStatItem, FogSystemReport, FogTripTicketReportDateType } from "../../../../shared/models/fog/fog-system-report";
import { FogSystemReportService } from "../../../../shared/services/fog/fog-system-report.service";

interface FogReportStatCategoryView {
    title: string;
    barColor: string;
    items: FogReportStatItem[];
}

@Component({
    standalone: false,
    selector: 'fog-trip-ticket-report-tab',
    templateUrl: './fog-trip-ticket-report-tab.component.html'
})
export class FogTripTicketReportTabComponent implements OnInit {
    private readonly _barColors: Record<string, string> = {
        'Property Type': 'green',
        'Interceptor Type': 'blue',
        'Disposal Sites': 'blue'
    };

    public report: FogSystemReport | null = null;
    public statCategories: FogReportStatCategoryView[] = [];
    public isLoading = true;
    public dateType: string = String(FogTripTicketReportDateType.GeneratorRemovalDate);
    public fromDate: string = '';
    public toDate: string = '';
    public dateRangeError: string = '';

    public readonly dateTypeOptions: InputOption[] = [
        { id: String(FogTripTicketReportDateType.GeneratorRemovalDate), text: 'Generator Removal Date' },
        { id: String(FogTripTicketReportDateType.ReceiverDeliveryDate), text: 'Receiver Delivery Date' }
    ];

    constructor(private readonly _reportService: FogSystemReportService) {
    }

    public async ngOnInit(): Promise<void> {
        try {
            await this.setDefaultDateRange();
            await this.search();
        } finally {
            this.isLoading = false;
        }
    }

    public async search(): Promise<void> {
        if (!this.validateDates()) {
            return;
        }

        try {
            this.isLoading = true;
            this.report = await this._reportService.getTripTicketReport(Number(this.dateType), this.fromDate, this.toDate);
            this.statCategories = this.report.stats.map(category => ({
                title: category.title,
                barColor: this._barColors[category.title] ?? 'green',
                items: category.items
            }));
        } finally {
            this.isLoading = false;
        }
    }

    public selectPeriod(period: FogReportPeriod): void {
        if (!period.year) {
            return;
        }

        if (period.month) {
            const lastDay = new Date(period.year, period.month, 0).getDate();
            this.fromDate = this.toDateString(period.year, period.month, 1);
            this.toDate = this.toDateString(period.year, period.month, lastDay);
        } else {
            this.fromDate = this.toDateString(period.year, 1, 1);
            this.toDate = this.toDateString(period.year, 12, 31);
        }

        this.search();
    }

    private async setDefaultDateRange(): Promise<void> {
        const today = new Date();
        const earliest = await this._reportService.getEarliestTripTicketDate();

        if (earliest) {
            const [year, month] = earliest.split('T')[0].split('-');
            this.fromDate = `${year}-${month}-01`;
        } else {
            this.fromDate = this.toDateString(today.getFullYear(), today.getMonth() + 1, 1);
        }

        const lastDayOfMonth = new Date(today.getFullYear(), today.getMonth() + 1, 0).getDate();
        this.toDate = this.toDateString(today.getFullYear(), today.getMonth() + 1, lastDayOfMonth);
    }

    private toDateString(year: number, month: number, day: number): string {
        const pad = (value: number) => String(value).padStart(2, '0');

        return `${year}-${pad(month)}-${pad(day)}`;
    }

    private validateDates(): boolean {
        if (this.fromDate && this.toDate && this.fromDate > this.toDate) {
            this.dateRangeError = 'Invalid Date Range: The From date cannot be after the To Date.';
            return false;
        }

        this.dateRangeError = '';
        return true;
    }
}
