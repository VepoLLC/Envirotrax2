import { Component, OnDestroy, OnInit } from "@angular/core";
import { NavigationEnd, Router } from "@angular/router";
import { Subscription, filter } from "rxjs";

export type FogReportTab = 'trip-tickets' | 'inspections';

interface FogReportTabDefinition {
    key: FogReportTab;
    title: string;
    pageTitle: string;
}

@Component({
    standalone: false,
    templateUrl: './fog-report.component.html'
})
export class FogReportComponent implements OnInit, OnDestroy {
    private _routerSubscription?: Subscription;

    public readonly tabs: FogReportTabDefinition[] = [
        { key: 'trip-tickets', title: 'Trip Ticket Reports', pageTitle: 'FOG Trip Ticket Reports' },
        { key: 'inspections', title: 'Inspection Status', pageTitle: 'FOG Inspection Reports' }
    ];

    public pageTitle: string = this.tabs[0].pageTitle;

    constructor(private readonly _router: Router) {
    }

    public ngOnInit(): void {
        this.setPageTitle();

        this._routerSubscription = this._router.events
            .pipe(filter(event => event instanceof NavigationEnd))
            .subscribe(() => this.setPageTitle());
    }

    public ngOnDestroy(): void {
        this._routerSubscription?.unsubscribe();
    }

    private setPageTitle(): void {
        const activeTab = this.tabs.find(tab => this._router.url.includes(`/fog/reports/${tab.key}`));

        this.pageTitle = activeTab?.pageTitle ?? this.tabs[0].pageTitle;
    }
}
