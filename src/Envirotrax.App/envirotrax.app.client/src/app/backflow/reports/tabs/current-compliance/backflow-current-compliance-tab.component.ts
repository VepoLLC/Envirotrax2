import { Component, OnInit } from "@angular/core";
import { Router } from "@angular/router";
import { BackflowComplianceReport, BackflowComplianceRequirement } from "../../../../shared/models/backflow/backflow-compliance-report";
import { BackflowReportService } from "../../../../shared/services/backflow/backflow-report.service";
import { PropertyType } from "../../../../shared/enums/property-type.enum";
import { BackflowComplianceParams } from "../../../../shared/models/backflow/backflow-compliance-params";
import { ChartConfiguration, ChartData, Plugin } from "chart.js";

@Component({
    standalone: false,
    selector: 'backflow-current-compliance-tab',
    templateUrl: './backflow-current-compliance-tab.component.html',
    styleUrls: ['./backflow-current-compliance-tab.component.scss']
})
export class BackflowCurrentComplianceTabComponent implements OnInit {
    public report: BackflowComplianceReport | null = null;
    public isLoading = false;
    public ignoreLast30Days = false;

    // Doughnut center-text styling (drawn on the canvas, so it can't live in SCSS).
    private readonly centerPercentColor = '#212529';              // near-black for the big % number
    private readonly centerLabelColor = '#6c757d';                // muted grey for the "Compliant" caption
    private readonly centerPercentFont = '700 1.75rem sans-serif';
    private readonly centerLabelFont = '400 0.8rem sans-serif';
    private readonly centerPercentOffsetY = -8;                   // nudge the % slightly above the middle
    private readonly centerLabelOffsetY = 16;                     // place the caption below the %

    // Doughnut slice colors. These must stay in sync with the legend swatch colors in the SCSS
    // (.bf-swatch.compliant / .noncompliant) — the canvas can't read CSS, so they're duplicated.
    private readonly compliantColor = '#20a845';
    private readonly nonCompliantColor = '#dc3545';
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

            ctx.save();
            ctx.textAlign = 'center';
            ctx.textBaseline = 'middle';

            ctx.fillStyle = this.centerPercentColor;
            ctx.font = this.centerPercentFont;
            ctx.fillText(`${pct}%`, arc.x, arc.y + this.centerPercentOffsetY);

            ctx.fillStyle = this.centerLabelColor;
            ctx.font = this.centerLabelFont;
            ctx.fillText('Compliant', arc.x, arc.y + this.centerLabelOffsetY);

            ctx.restore();
        }
    }];

    constructor(
        private readonly _reportService: BackflowReportService,
        private readonly _router: Router
    ) {}

    public async ngOnInit(): Promise<void> {
        await this.refresh();
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

    public viewPrintableReport(): void {
        // Printable / export report is not implemented yet (parity placeholder).
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
