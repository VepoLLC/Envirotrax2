import { Component, OnDestroy, OnInit, ViewChild } from "@angular/core";
import { BaseChartDirective } from "ng2-charts";
import { BackflowNewRemovedReport, BackflowNewRemovedPoint } from "../../../../shared/models/backflow/backflow-new-removed-report";
import { BackflowReportService } from "../../../../shared/services/backflow/backflow-report.service";
import { DownloadService } from "../../../../shared/services/download.service";
import { chartGridColor, chartTickColor, onThemeChange, themeLegendLabels } from "../../../../shared/utils/chart-theme.util";
import { ChartConfiguration, ChartData, Plugin } from "chart.js";

@Component({
    standalone: false,
    selector: 'backflow-new-removed-tab',
    templateUrl: './backflow-new-removed-tab.component.html',
    styleUrls: ['./backflow-new-removed-tab.component.scss']
})
export class BackflowNewRemovedTabComponent implements OnInit, OnDestroy {
    @ViewChild(BaseChartDirective) private _chart?: BaseChartDirective;

    private _disposeThemeObserver?: () => void;

    public report: BackflowNewRemovedReport | null = null;
    public isLoading = false;

    public points: BackflowNewRemovedPoint[] = [];

    // Chart width scales with the number of months so labels stay readable; it scrolls horizontally when wide.
    private readonly minimumChartWidth = 640;   // px floor so a few months aren't cramped
    private readonly chartWidthPerMonth = 60;   // px of width each month column needs
    private readonly maxBarThickness = 70;      // px cap so a few months don't render as very wide bars

    public chartPixelWidth = this.minimumChartWidth;

    // Grouped column chart: Assemblies Created vs Removed per month (matches V1's ColumnChartNewRemoved).
    public readonly barChartType = 'bar' as const;
    public barChartData: ChartData<'bar'> = { labels: [], datasets: [] };
    public barChartOptions: ChartConfiguration<'bar'>['options'] = {
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
                // Pointer cursor on hover so users see the legend items are clickable.
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
    public readonly barChartPlugins: Plugin<'bar'>[] = [{
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

    constructor(
        private readonly _reportService: BackflowReportService,
        private readonly _downloadService: DownloadService
    ) {}

    public async ngOnInit(): Promise<void> {
        // Axis/legend/grid colors are drawn from theme CSS variables, so repaint on theme toggle.
        this._disposeThemeObserver = onThemeChange(() => this._chart?.update());
        await this.load();
    }

    public ngOnDestroy(): void {
        this._disposeThemeObserver?.();
    }

    public async load(): Promise<void> {
        try {
            this.isLoading = true;
            this.report = await this._reportService.getNewRemoved();
            this.points = this.report?.points ?? [];
            this.chartPixelWidth = Math.max(this.minimumChartWidth, this.points.length * this.chartWidthPerMonth);
            this.buildChart();
        } finally {
            this.isLoading = false;
        }
    }

    public async downloadPDF(): Promise<void> {
        try {
            this.isLoading = true;
            const blob = await this._reportService.getNewRemovedPdf();
            this._downloadService.downloadFileFromBlob(blob, 'backflow-new-removed.pdf');
        } finally {
            this.isLoading = false;
        }
    }

    public async downloadWord(): Promise<void> {
        try {
            this.isLoading = true;
            const blob = await this._reportService.getNewRemovedWord();
            this._downloadService.downloadFileFromBlob(blob, 'backflow-new-removed.docx');
        } finally {
            this.isLoading = false;
        }
    }

    public async downloadExcel(): Promise<void> {
        try {
            this.isLoading = true;
            const blob = await this._reportService.getNewRemovedExcel();
            this._downloadService.downloadFileFromBlob(blob, 'backflow-new-removed.xlsx');
        } finally {
            this.isLoading = false;
        }
    }

    private buildChart(): void {
        this.barChartData = {
            labels: this.points.map(p => p.label),
            datasets: [
                { label: 'Assemblies Created', data: this.points.map(p => p.created), backgroundColor: '#f0922b', maxBarThickness: this.maxBarThickness },
                { label: 'Assemblies Removed', data: this.points.map(p => p.removed), backgroundColor: '#5ac8e8', maxBarThickness: this.maxBarThickness }
            ]
        };
    }
}
