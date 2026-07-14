import { Component, OnInit } from "@angular/core";
import { BackflowComplianceHistory, BackflowComplianceHistoryPoint } from "../../../../shared/models/backflow/backflow-compliance-history";
import { BackflowReportService } from "../../../../shared/services/backflow/backflow-report.service";
import { Chart, ChartConfiguration, ChartData, Plugin } from "chart.js";

@Component({
    standalone: false,
    selector: 'backflow-compliance-history-tab',
    templateUrl: './backflow-compliance-history-tab.component.html',
    styleUrls: ['./backflow-compliance-history-tab.component.scss']
})
export class BackflowComplianceHistoryTabComponent implements OnInit {
    public report: BackflowComplianceHistory | null = null;
    public isLoading = false;

    public points: BackflowComplianceHistoryPoint[] = [];
    public reversedPoints: BackflowComplianceHistoryPoint[] = [];

    // Charts scale with the number of months so labels stay readable; they scroll horizontally when wide.
    private readonly minimumChartWidth = 640;   // px floor so a few months aren't cramped
    private readonly chartWidthPerMonth = 46;   // px of width each month column needs

    public chartPixelWidth = this.minimumChartWidth;

    // Count-chart series colors.
    private readonly totalColor = '#50a0ff';
    private readonly compliantColor = '#3ec46e';
    private readonly nonCompliantColor = '#ff5a5a';

    // Percent-compliant area/line colors.
    private readonly percentFillColor = 'rgba(80, 240, 80, 0.35)';
    private readonly percentLineColor = '#2bbd4f';

    // "By Number of Assemblies" grouped column chart (Total / Compliant / Non-Compliant per month).
    public readonly countChartType = 'bar' as const;
    public countChartData: ChartData<'bar'> = { labels: [], datasets: [] };
    public countChartOptions: ChartConfiguration<'bar'>['options'] = {
        responsive: true,
        maintainAspectRatio: false,
        scales: {
            x: { ticks: { maxRotation: 45, minRotation: 45 }, grid: { display: false } },
            y: { beginAtZero: true, ticks: { precision: 0 } }
        },
        plugins: {
            legend: {
                display: true,
                position: 'top',
                align: 'center',
                // Show the pointer cursor on hover so users can tell the legend items are clickable.
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
                // Grey out a toggled-off series' label so it reads as disabled.
                labels: {
                    generateLabels: chart => {
                        const items = Chart.defaults.plugins.legend.labels.generateLabels(chart);
                        items.forEach(item => {
                            if (item.hidden) {
                                // Grey the label to show it's disabled, but clear `hidden` so Chart.js
                                // doesn't draw the strike-through line over it.
                                item.fontColor = '#adb5bd';
                                item.hidden = false;
                            }
                        });
                        return items;
                    }
                }
            }
        }
    };

    // Chart.js has no "legend margin" option, so this local plugin grows the legend box height,
    // adding clear vertical space between the legend row and the bars (no whitespace above the legend).
    public readonly countChartPlugins: Plugin<'bar'>[] = [{
        id: 'legendSpacing',
        beforeInit(chart): void {
            const legend = chart.legend as unknown as { fit: () => void; height: number } | undefined;

            if (!legend) {
                return;
            }

            const originalFit = legend.fit;
            legend.fit = function (): void {
                originalFit.call(this);
                this.height += 24;
            };
        }
    }];

    // "Percent of Compliant Assemblies" area/line chart (% compliant per month).
    public readonly percentChartType = 'line' as const;
    public percentChartData: ChartData<'line'> = { labels: [], datasets: [] };
    public percentChartOptions: ChartConfiguration<'line'>['options'] = {
        responsive: true,
        maintainAspectRatio: false,
        scales: {
            x: { ticks: { maxRotation: 45, minRotation: 45 }, grid: { display: false } },
            y: { min: 0, max: 100, ticks: { callback: value => `${value}%` } }
        },
        plugins: {
            legend: { display: false },
            tooltip: {
                callbacks: {
                    label: ctx => `${ctx.parsed.y}%`
                }
            }
        }
    };

    constructor(private readonly _reportService: BackflowReportService) {}

    public async ngOnInit(): Promise<void> {
        await this.load();
    }

    public async load(): Promise<void> {
        try {
            this.isLoading = true;
            this.report = await this._reportService.getComplianceHistory();
            this.points = this.report?.points ?? [];
            this.reversedPoints = [...this.points].reverse();
            this.chartPixelWidth = Math.max(this.minimumChartWidth, this.points.length * this.chartWidthPerMonth);
            this.buildCharts();
        } finally {
            this.isLoading = false;
        }
    }

    public viewPrintableReport(): void {
        // Printable / export report is not implemented yet (parity placeholder).
    }

    private buildCharts(): void {
        const labels = this.points.map(p => p.label);

        this.countChartData = {
            labels,
            datasets: [
                { label: 'Total', data: this.points.map(p => p.total), backgroundColor: this.totalColor },
                { label: 'Compliant', data: this.points.map(p => p.compliant), backgroundColor: this.compliantColor },
                { label: 'Non-Compliant', data: this.points.map(p => p.nonCompliant), backgroundColor: this.nonCompliantColor }
            ]
        };

        this.percentChartData = {
            labels,
            datasets: [{
                label: 'Percent Compliant',
                data: this.points.map(p => p.percentage),
                fill: true,
                backgroundColor: this.percentFillColor,
                borderColor: this.percentLineColor,
                pointBackgroundColor: this.percentLineColor,
                tension: 0.2
            }]
        };
    }
}
