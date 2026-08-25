import { Component, NgZone, OnDestroy, OnInit, ViewChild } from "@angular/core";
import { BaseChartDirective } from "ng2-charts";
import { BackflowTestReport, BackflowReportPeriod } from "../../../../shared/models/backflow/backflow-test-report";
import { BackflowReportService } from "../../../../shared/services/backflow/backflow-report.service";
import { DownloadService } from "../../../../shared/services/download.service";
import { chartGridColor, chartTickColor, onThemeChange, themeLegendLabels } from "../../../../shared/utils/chart-theme.util";
import { ChartConfiguration, ChartData } from "chart.js";

@Component({
    standalone: false,
    selector: 'backflow-test-reports-tab',
    templateUrl: './backflow-test-reports-tab.component.html',
    styleUrls: ['./backflow-test-reports-tab.component.scss']
})
export class BackflowTestReportsTabComponent implements OnInit, OnDestroy {
    @ViewChild(BaseChartDirective) private _chart?: BaseChartDirective;

    private _disposeThemeObserver?: () => void;

    public report: BackflowTestReport | null = null;
    public isLoading = false;
    public fromDate: string = '';
    public toDate: string = '';
    public dateRangeError: string = '';
    public isDateValid: boolean = false;

    // Totals bar chart (horizontal bars, 0-100% axis, tooltips, click-to-drill).
    public readonly barChartType = 'bar' as const;
    public barChartData: ChartData<'bar'> = { labels: [], datasets: [] };
    public barChartOptions: ChartConfiguration<'bar'>['options'] = {};
    public chartHeight = 0;

    // Chart height scales with the number of bars.
    private readonly minimumChartHeight = 160;   // px floor so a 1-bar chart isn't tiny
    private readonly chartHeightPerBar = 44;      // px of vertical space each bar needs
    private readonly chartHeightPadding = 48;     // px for the axis labels / legend

    // Per-section bar color, matching V1's backflow report. Uses the shared global .reportbar
    // variants (green / steelblue / blue); any unmapped section falls back to green.
    private readonly sectionBarColors: Record<string, string> = {
        'Added By': 'steelblue',
        'Property Type': 'green',
        'Test Result': 'blue',
        'Reason for Test': 'steelblue',
        'Hazard Type': 'blue',
        'Assembly Type': 'steelblue',
        'Rain / Freeze Sensor': 'green',
        'On-site Sewage Facility': 'green'
    };

    constructor(
        private readonly _reportService: BackflowReportService,
        private readonly _downloadService: DownloadService,
        private readonly _zone: NgZone
    ) {}

    public async ngOnInit(): Promise<void> {
        try {
            this.isLoading = true;
            // Axis/legend/grid colors are drawn from theme CSS variables, so repaint on theme toggle.
            this._disposeThemeObserver = onThemeChange(() => this._chart?.update());

            await this.setDefaultDateRange();
            await this.search();
        } finally {
            this.isLoading = false;
        }
    }

    public ngOnDestroy(): void {
        this._disposeThemeObserver?.();
    }

    // The shared .reportbar color variant for a statistics section (matches V1).
    public barColor(title: string): string {
        return this.sectionBarColors[title] ?? 'green';
    }

    // Default range follows V1: from the first day of the earliest test record's month
    // to the last day of the current month (falls back to the current month when no records).
    private async setDefaultDateRange(): Promise<void> {
        const today = new Date();
        const pad = (value: number) => String(value).padStart(2, '0');

        const earliest = await this._reportService.getEarliestTestDate();
        const startYearMonth = earliest ? earliest.split('T')[0] : `${today.getFullYear()}-${pad(today.getMonth() + 1)}`;
        const [startYear, startMonth] = startYearMonth.split('-');
        this.fromDate = `${startYear}-${startMonth}-01`;

        const lastDayOfMonth = new Date(today.getFullYear(), today.getMonth() + 1, 0).getDate();
        this.toDate = `${today.getFullYear()}-${pad(today.getMonth() + 1)}-${pad(lastDayOfMonth)}`;
    }

    // Restores the default date range (earliest test → current month) and reloads, undoing any drill-down.
    public async reset(): Promise<void> {
        await this.setDefaultDateRange();
        await this.search();
    }

    public async search(): Promise<void> {
        this.validateDates();

        if (this.isDateValid) {
            try {
                this.isLoading = true;
                this.report = await this._reportService.getTestReport(this.fromDate, this.toDate);
                this.buildChart();
            } finally {
                this.isLoading = false;
            }
        }
    }

    private buildChart(): void {
        const periods = this.report?.periods ?? [];

        this.barChartData = this.buildChartData(periods);
        this.barChartOptions = this.buildChartOptions(periods);

        this.chartHeight = Math.max(
            this.minimumChartHeight,
            periods.length * this.chartHeightPerBar + this.chartHeightPadding
        );
    }

    private buildChartData(periods: BackflowReportPeriod[]): ChartData<'bar'> {
        const seriesName = this.isYearView() ? 'year' : 'month';

        return {
            labels: periods.map(p => p.label),
            datasets: [{
                label: seriesName,
                data: periods.map(p => p.percentage),
                backgroundColor: '#fd7e14',
                hoverBackgroundColor: '#e8590c',
                borderWidth: 0
            }]
        };
    }

    private buildChartOptions(periods: BackflowReportPeriod[]): ChartConfiguration<'bar'>['options'] {
        return {
            indexAxis: 'y',
            responsive: true,
            maintainAspectRatio: false,
            scales: {
                x: {
                    min: 0,
                    max: 100,
                    ticks: { color: () => chartTickColor(), callback: value => `${value}%` },
                    grid: { color: () => chartGridColor() }
                },
                y: {
                    ticks: { color: () => chartTickColor() },
                    grid: { display: false }
                }
            },
            plugins: {
                legend: {
                    display: true,
                    position: 'right',
                    // Pointer cursor on hover so users see the legend is clickable.
                    onHover: event => {
                        const target = event.native?.target as HTMLElement | null;
                        if (target) {
                            target.style.cursor = 'pointer';
                        }
                    },
                    onLeave: event => {
                        const target = event.native?.target as HTMLElement | null;
                        if (target) {
                            target.style.cursor = 'default';
                        }
                    },
                    // Grey out the series' label when toggled off so it reads as disabled.
                    labels: {
                        generateLabels: themeLegendLabels
                    }
                },
                tooltip: {
                    callbacks: {
                        label: ctx => {
                            const period = periods[ctx.dataIndex];
                            return `${period.count} Tests (${period.percentage}%)`;
                        }
                    }
                }
            },
            onClick: (_event, elements) => {
                if (elements.length === 0) {
                    return;
                }

                const period = periods[elements[0].index];

                // Chart.js fires this outside Angular's zone; run the reload inside it so the loading
                // state and results update through change detection consistently on every click.
                this._zone.run(() => {
                    if (this.isYearView()) {
                        this.showByMonth(period);
                    } else {
                        this.filterToMonth(period);
                    }
                });
            }
        };
    }

    public showByMonth(period: BackflowReportPeriod): void {
        if (!period.year || period.month) {
            return;
        }
        // Whole year: January 1st through December 31st.
        this.fromDate = this.toDateString(period.year, 1, 1);
        this.toDate = this.toDateString(period.year, 12, 31);
        this.search();
    }

    public filterToMonth(period: BackflowReportPeriod): void {
        if (!period.year || !period.month) {
            return;
        }
        const lastDay = new Date(period.year, period.month, 0).getDate();
        this.fromDate = this.toDateString(period.year, period.month, 1);
        this.toDate = this.toDateString(period.year, period.month, lastDay);
        this.search();
    }

    // Formats year/month/day as the zero-padded YYYY-MM-DD string the date inputs and report API expect.
    private toDateString(year: number, month: number, day: number): string {
        const pad = (value: number) => String(value).padStart(2, '0');
        return `${year}-${pad(month)}-${pad(day)}`;
    }

    public async downloadPDF(): Promise<void> {
        try {
            this.isLoading = true;
            const blob = await this._reportService.getTestReportPdf(this.fromDate, this.toDate);
            this._downloadService.downloadFileFromBlob(blob, `backflow-test-report_${this.fromDate}_${this.toDate}.pdf`);
        } finally {
            this.isLoading = false;
        }
    }

    public async downloadWord(): Promise<void> {
        try {
            this.isLoading = true;
            const blob = await this._reportService.getTestReportWord(this.fromDate, this.toDate);
            this._downloadService.downloadFileFromBlob(blob, `backflow-test-report_${this.fromDate}_${this.toDate}.docx`);
        } finally {
            this.isLoading = false;
        }
    }

    public async downloadExcel(): Promise<void> {
        try {
            this.isLoading = true;
            const blob = await this._reportService.getTestReportExcel(this.fromDate, this.toDate);
            this._downloadService.downloadFileFromBlob(blob, `backflow-test-report_${this.fromDate}_${this.toDate}.xlsx`);
        } finally {
            this.isLoading = false;
        }
    }

    public isYearView(): boolean {
        return this.report?.periods.some(p => !p.month) ?? false;
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
}
