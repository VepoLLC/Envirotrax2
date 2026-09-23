import { Component, OnDestroy, OnInit, ViewChild } from "@angular/core";
import { Router } from "@angular/router";
import { BaseChartDirective } from "ng2-charts";
import { BackflowComplianceReport, BackflowComplianceRequirement } from "../../../../shared/models/backflow/backflow-compliance-report";
import { BackflowReportService } from "../../../../shared/services/backflow/backflow-report.service";
import { DownloadService } from "../../../../shared/services/download.service";
import { PropertyType } from "../../../../shared/enums/property-type.enum";
import { BackflowComplianceParams } from "../../../../shared/models/backflow/backflow-compliance-params";
import { readCssVar, onThemeChange } from "../../../../shared/utils/chart-theme.util";
import { ChartConfiguration, ChartData, Plugin } from "chart.js";

@Component({
    standalone: false,
    selector: 'backflow-current-compliance-tab',
    templateUrl: './backflow-current-compliance-tab.component.html',
    styleUrls: ['./backflow-current-compliance-tab.component.scss']
})
export class BackflowCurrentComplianceTabComponent implements OnInit, OnDestroy {
    @ViewChild(BaseChartDirective) private _chart?: BaseChartDirective;

    private _disposeThemeObserver?: () => void;

    public report: BackflowComplianceReport | null = null;
    public isLoading = false;
    public ignoreLast30Days = false;

    // Doughnut center-text layout. The text colors are theme-driven CSS custom properties
    // (--bf-doughnut-percent-color / --bf-doughnut-label-color, defined in styles.css and overridden
    // in dark-theme.css) read from the canvas in the plugin below, since canvas can't consume CSS.
    private readonly centerPercentFont = '700 1.75rem sans-serif';
    private readonly centerLabelFont = '400 0.8rem sans-serif';
    private readonly centerPercentOffsetY = -8;                   // nudge the % slightly above the middle
    private readonly centerLabelOffsetY = 16;                     // place the caption below the %

    // Compliance colors — the single source of truth for both the doughnut canvas (which can't read
    // CSS) and the legend swatches, which bind to these same values in the template. Keeping them here
    // guarantees the legend always matches the chart, in any theme.
    public readonly totalColor = '#adb5bd';          // legend "Total" swatch only (not a doughnut slice)
    public readonly compliantColor = '#20a845';
    public readonly nonCompliantColor = '#dc3545';
    private readonly compliantHoverColor = '#1c9540';
    private readonly nonCompliantHoverColor = '#c82333';

    // Compliance doughnut (Compliant vs Non-Compliant with the % in the center).
    public readonly doughnutType = 'doughnut' as const;
    public doughnutData: ChartData<'doughnut'> = { labels: [], datasets: [] };
    public doughnutOptions: ChartConfiguration<'doughnut'>['options'] = {
        responsive: true,
        maintainAspectRatio: false,
        cutout: '62%',
        plugins: {
            legend: { display: false },
            tooltip: {
                callbacks: {
                    label: ctx => {
                        const pct = ctx.dataIndex === 0
                            ? this.report?.compliantPercentage ?? 0
                            : this.report?.nonCompliantPercentage ?? 0;
                        return `${ctx.label}: ${ctx.parsed} (${pct}%)`;
                    }
                }
            }
        }
    };

    // Draws the compliant % in the middle of the doughnut.
    public readonly doughnutPlugins: Plugin<'doughnut'>[] = [{
        id: 'complianceCenterText',
        afterDraw: chart => {
            // Hide the center text while a tooltip is showing so the two don't overlap.
            const activeElements = chart.tooltip?.getActiveElements() ?? [];
            if (activeElements.length > 0) {
                return;
            }

            const arc = chart.getDatasetMeta(0).data[0] as unknown as { x: number; y: number } | undefined;
            if (!arc) {
                return;
            }

            const pct = this.report?.compliantPercentage ?? 0;
            const ctx = chart.ctx;

            // Canvas can't consume CSS, so read the theme-driven center-text colors from the
            // CSS custom properties (they resolve to the light or dark value via the cascade).
            const percentColor = readCssVar('--bf-doughnut-percent-color', '#212529');
            const labelColor = readCssVar('--bf-doughnut-label-color', '#6c757d');

            ctx.save();
            ctx.textAlign = 'center';
            ctx.textBaseline = 'middle';

            ctx.fillStyle = percentColor;
            ctx.font = this.centerPercentFont;
            ctx.fillText(`${pct}%`, arc.x, arc.y + this.centerPercentOffsetY);

            ctx.fillStyle = labelColor;
            ctx.font = this.centerLabelFont;
            ctx.fillText('Compliant', arc.x, arc.y + this.centerLabelOffsetY);

            ctx.restore();
        }
    }];

    constructor(
        private readonly _reportService: BackflowReportService,
        private readonly _downloadService: DownloadService,
        private readonly _router: Router
    ) {}

    public async ngOnInit(): Promise<void> {
        // The doughnut center text is drawn from theme CSS variables, so repaint it when the theme toggles.
        this._disposeThemeObserver = onThemeChange(() => this._chart?.update());
        await this.refresh();
    }

    public ngOnDestroy(): void {
        this._disposeThemeObserver?.();
    }

    public async refresh(): Promise<void> {
        try {
            this.isLoading = true;
            this.report = await this._reportService.getComplianceReport(this.ignoreLast30Days);
            this.buildChart();
        } finally {
            this.isLoading = false;
        }
    }

    public async downloadPDF(): Promise<void> {
        try {
            this.isLoading = true;
            const blob = await this._reportService.getComplianceReportPdf(this.ignoreLast30Days);
            this._downloadService.downloadFileFromBlob(blob, 'backflow-compliance-report.pdf');
        } finally {
            this.isLoading = false;
        }
    }

    public async downloadWord(): Promise<void> {
        try {
            this.isLoading = true;
            const blob = await this._reportService.getComplianceReportWord(this.ignoreLast30Days);
            this._downloadService.downloadFileFromBlob(blob, 'backflow-compliance-report.docx');
        } finally {
            this.isLoading = false;
        }
    }

    public async downloadExcel(): Promise<void> {
        try {
            this.isLoading = true;
            const blob = await this._reportService.getComplianceReportExcel(this.ignoreLast30Days);
            this._downloadService.downloadFileFromBlob(blob, 'backflow-compliance-report.xlsx');
        } finally {
            this.isLoading = false;
        }
    }

    public viewRequirement(requirement: BackflowComplianceRequirement): void {
        // Opens the Backflow Test Search in a new window showing this row's non-compliant assemblies
        // The search reads these params in ngOnInit and runs the matching filter; results
        // will populate once ExpirationDate is derived at submit — no further report change needed.
        const queryParams: Record<string, string> = {
            [BackflowComplianceParams.mode]: 'true',
            [BackflowComplianceParams.propertyType]: requirement.propertyType === 'Commercial'
                ? PropertyType.Commercial.toString()
                : PropertyType.Residential.toString(),
            [BackflowComplianceParams.hazardType]: requirement.hazardType
        };

        if (requirement.assemblyType && requirement.assemblyType !== 'All') {
            queryParams[BackflowComplianceParams.deviceType] = requirement.assemblyType;
        }

        if (requirement.hasSiteOssf) {
            queryParams[BackflowComplianceParams.ossf] = 'true';
        }

        if (requirement.auxWaterSupply) {
            queryParams[BackflowComplianceParams.auxWater] = 'true';
        }

        if (this.ignoreLast30Days) {
            queryParams[BackflowComplianceParams.ignoreLast30Days] = 'true';
        }

        const url = this._router.serializeUrl(
            this._router.createUrlTree(['/backflow/tests'], { queryParams })
        );

        window.open(url, '_blank');
    }

    private buildChart(): void {
        this.doughnutData = {
            labels: ['Compliant', 'Non-Compliant'],
            datasets: [{
                data: [this.report?.compliant ?? 0, this.report?.nonCompliant ?? 0],
                backgroundColor: [this.compliantColor, this.nonCompliantColor],
                hoverBackgroundColor: [this.compliantHoverColor, this.nonCompliantHoverColor],
                borderWidth: 0
            }]
        };
    }
}
