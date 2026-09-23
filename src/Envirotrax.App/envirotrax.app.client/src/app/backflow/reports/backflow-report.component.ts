import { Component } from "@angular/core";

export type BackflowReportTab = 'test-reports' | 'current-compliance' | 'compliance-history' | 'new-removed';

interface BackflowReportTabDefinition {
    key: BackflowReportTab;
    title: string;
    iconCss: string;
}

@Component({
    standalone: false,
    templateUrl: './backflow-report.component.html',
    styleUrls: ['./backflow-report.component.scss']
})
export class BackflowReportComponent {
    // Each report is its own routed child page under /backflow/reports (rendered in the <router-outlet>).
    // This list only drives the pill navigation; the router decides which report is shown and which pill
    // is active (routerLinkActive).
    public readonly tabs: BackflowReportTabDefinition[] = [
        { key: 'test-reports', title: 'Backflow Test Reports', iconCss: 'fa-regular fa-chart-simple-horizontal' },
        { key: 'current-compliance', title: 'Current Compliance Status', iconCss: 'fa-regular fa-chart-pie-simple' },
        { key: 'compliance-history', title: 'Compliance History', iconCss: 'fa-solid fa-chart-line-up' },
        { key: 'new-removed', title: 'New/Removed', iconCss: 'fa-solid fa-chart-column' }
    ];
}
