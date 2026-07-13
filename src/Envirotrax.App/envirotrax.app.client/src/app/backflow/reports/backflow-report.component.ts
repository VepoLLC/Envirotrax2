import { Component, OnDestroy, OnInit } from "@angular/core";
import { ActivatedRoute } from "@angular/router";
import { Subscription } from "rxjs";

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
export class BackflowReportComponent implements OnInit, OnDestroy {
    private readonly defaultTab: BackflowReportTab = 'test-reports';

    public selectedTab: BackflowReportTab = this.defaultTab;

    public readonly tabs: BackflowReportTabDefinition[] = [
        { key: 'test-reports', title: 'Backflow Test Reports', iconCss: 'fa-regular fa-chart-simple-horizontal' },
        { key: 'current-compliance', title: 'Current Compliance Status', iconCss: 'fa-regular fa-chart-pie-simple' },
        { key: 'compliance-history', title: 'Compliance History', iconCss: 'fa-solid fa-chart-line-up' },
        { key: 'new-removed', title: 'New/Removed', iconCss: 'fa-solid fa-chart-column' }
    ];

    private _queryParamsSubscription?: Subscription;

    constructor(private readonly _route: ActivatedRoute) {}

    public ngOnInit(): void {
        this._queryParamsSubscription = this._route.queryParamMap.subscribe(params => {
            const tab = params.get('tab');
            this.selectedTab = this.isValidTab(tab) ? tab : this.defaultTab;
        });
    }

    public ngOnDestroy(): void {
        this._queryParamsSubscription?.unsubscribe();
    }

    private isValidTab(tab: string | null): tab is BackflowReportTab {
        return tab != null && this.tabs.some(t => t.key === tab);
    }
}
