import { Component, OnDestroy, OnInit, QueryList, ViewChildren } from "@angular/core";
import { BaseChartDirective } from "ng2-charts";
import { BackflowComplianceHistory, BackflowComplianceHistoryPoint } from "../../../../shared/models/backflow/backflow-compliance-history";
import { BackflowReportService } from "../../../../shared/services/backflow/backflow-report.service";
import { chartGridColor, chartTickColor, onThemeChange, themeLegendLabels } from "../../../../shared/utils/chart-theme.util";
import { ChartConfiguration, ChartData, Plugin } from "chart.js";
import ChartDataLabels from "chartjs-plugin-datalabels";

@Component({
    standalone: false,
    selector: 'backflow-compliance-history-tab',
    templateUrl: './backflow-compliance-history-tab.component.html',
    styleUrls: ['./backflow-compliance-history-tab.component.scss']
})
export class BackflowComplianceHistoryTabComponent implements OnInit, OnDestroy {
    @ViewChildren(BaseChartDirective) private _charts?: QueryList<BaseChartDirective>;

    private _disposeThemeObserver?: () => void;

    public report: BackflowComplianceHistory | null = null;
    public isLoading = false;

    public points: BackflowComplianceHistoryPoint[] = [];
    public reversedPoints: BackflowComplianceHistoryPoint[] = [];

    // Table bar color per year — alternates green/blue as the year changes (matches V1), grouping the
    // rows visually by year. Uses the shared global .reportbar variants.
    private _yearBarColors = new Map<number, string>();

    // Charts scale with the number of months so labels stay readable; they scroll horizontally when wide.
    private readonly minimumChartWidth = 640;   // px floor so a few months aren't cramped
    private readonly chartWidthPerMonth = 46;   // px of width each month column needs

    // Semibold weight for the % value labels drawn on the percent chart's points.
    private readonly percentLabelFontWeight = 600;

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
            x: { ticks: { color: () => chartTickColor(), maxRotation: 45, minRotation: 45 }, grid: { display: false } },
            y: { beginAtZero: true, ticks: { color: () => chartTickColor(), precision: 0 }, grid: { color: () => chartGridColor() } }
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
                // Theme-aware labels; toggled-off series read as disabled (see themeLegendLabels).
                labels: {
                    generateLabels: themeLegendLabels
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
            x: { ticks: { color: () => chartTickColor(), maxRotation: 45, minRotation: 45 }, grid: { display: false } },
            y: { min: 0, max: 100, ticks: { color: () => chartTickColor(), callback: value => `${value}%` }, grid: { color: () => chartGridColor() } }
        },
        plugins: {
            legend: { display: false },
            tooltip: {
                callbacks: {
                    label: ctx => `${ctx.parsed.y}%`
                }
            },
            // Show each month's % value above its point (matches V1, and keeps values visible when the
            // report is printed, where tooltips don't exist). Color follows the theme; clamped so labels
            // near 100% stay inside the plot area.
            datalabels: {
                anchor: 'end',
                align: 'top',
                clamp: true,
                color: () => chartTickColor(),
                font: { weight: this.percentLabelFontWeight },
                formatter: (value: number) => `${value}%`
            }
        }
    };

    // Registered only on this chart (not globally) so labels appear on the percent line, not the other charts.
    public readonly percentChartPlugins: Plugin<'line'>[] = [ChartDataLabels];

    constructor(private readonly _reportService: BackflowReportService) {}

    public async ngOnInit(): Promise<void> {
        // Axis/legend/grid colors are drawn from theme CSS variables, so repaint both charts on toggle.
        this._disposeThemeObserver = onThemeChange(() => this._charts?.forEach(chart => chart.update()));
        await this.load();
    }

    public ngOnDestroy(): void {
        this._disposeThemeObserver?.();
    }

    // Assign each year an alternating bar color (oldest year green, then blue, …) so the table groups
    // rows by year visually, matching V1.
    private buildYearBarColors(): void {
        const colors = ['green', 'blue'];
        const years = [...new Set(this.points.map(p => p.year))].sort((a, b) => a - b);

        this._yearBarColors = new Map(years.map((year, index) => [year, colors[index % colors.length]]));
    }

    // The shared .reportbar color variant for a table row, by its year.
    public barColor(point: BackflowComplianceHistoryPoint): string {
        return this._yearBarColors.get(point.year) ?? 'green';
    }

    public async load(): Promise<void> {
        try {
            this.isLoading = true;
            this.report = await this._reportService.getComplianceHistory();
            this.points = this.report?.points ?? [];
            this.reversedPoints = [...this.points].reverse();
            this.buildYearBarColors();
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
