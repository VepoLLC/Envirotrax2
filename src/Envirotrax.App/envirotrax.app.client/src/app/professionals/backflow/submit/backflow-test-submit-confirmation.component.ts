import { Component, OnDestroy, OnInit } from "@angular/core";
import { ActivatedRoute, Params, Router } from "@angular/router";
import { Subscription } from "rxjs";
import { BackflowTest } from "../../../shared/models/backflow/backflow-test";
import { BackflowSubmissionSite } from "../../../shared/models/backflow/backflow-submission-site";
import { MAX_PAGE_SIZE } from "../../../shared/models/page-info";
import { BackflowTestService } from "../../../shared/services/backflow/backflow-test.service";
import { SiteService } from "../../../shared/services/sites/site.service";

interface ConfirmationAssemblyRow {
    test: BackflowTest;
    description: string;
    isAwaitingCheckout: boolean;
}

@Component({
    standalone: false,
    templateUrl: './backflow-test-submit-confirmation.component.html'
})
export class BackflowTestSubmitConfirmationComponent implements OnInit, OnDestroy {
    public isLoading = false;

    public site: BackflowSubmissionSite | null = null;
    public siteHeader = '';
    public assemblies: ConfirmationAssemblyRow[] = [];

    public accountNumber = '';
    public manufacturer = '';
    public serialNumber = '';
    public streetNumber = '';

    private _routeSub?: Subscription;

    constructor(
        private readonly _siteService: SiteService,
        private readonly _backflowTestService: BackflowTestService,
        private readonly _router: Router,
        private readonly _activatedRoute: ActivatedRoute
    ) { }

    public ngOnInit(): void {
        this._routeSub = this._activatedRoute.queryParamMap.subscribe(async params => {
            const siteIdParam = params.get('siteId');

            this.site = null;
            this.assemblies = [];

            if (siteIdParam) {
                await this.loadSite(Number(siteIdParam));
            }
        });
    }

    public ngOnDestroy(): void {
        this._routeSub?.unsubscribe();
    }

    public returnToAccountOverview(): void {
        this._router.navigate(['/']);
    }

    public checkout(): void {
        this._router.navigate(['/professionals/checkout'], { queryParams: { tab: 'backflow' } });
    }

    public submitForAssembly(test: BackflowTest): void {
        this._router.navigate(['..', test.id], { relativeTo: this._activatedRoute });
    }

    public submitUnlisted(site: BackflowSubmissionSite): void {
        this._router.navigate(['..', 'new'], { relativeTo: this._activatedRoute, queryParams: { siteId: site.id } });
    }

    public search(): void {
        const queryParams: Params = {};

        if (this.accountNumber) {
            queryParams['accountNumber'] = this.accountNumber;
        }

        if (this.manufacturer) {
            queryParams['manufacturer'] = this.manufacturer;
        }

        if (this.serialNumber) {
            queryParams['serialNumber'] = this.serialNumber;
        }

        if (this.streetNumber) {
            queryParams['streetNumber'] = this.streetNumber;
        }

        this._router.navigate(['..'], { relativeTo: this._activatedRoute, queryParams: queryParams });
    }

    private async loadSite(siteId: number): Promise<void> {
        try {
            this.isLoading = true;

            const [sitesPage, unpaidPage] = await Promise.all([
                this._siteService.getAllForBackflowSubmission({ pageSize: 1 }, {
                    sort: {},
                    filter: [{ columnName: 'id', comparisonOperator: 'Eq', value: String(siteId) }]
                }),
                this._backflowTestService.getAllForProfessional({ pageSize: MAX_PAGE_SIZE }, {
                    sort: {},
                    filter: [
                        { columnName: 'transactionId', isValueNull: true },
                        { columnName: 'site.id', comparisonOperator: 'Eq', value: String(siteId) }
                    ]
                })
            ]);

            if (sitesPage.data.length === 0) {
                return;
            }

            this.site = sitesPage.data[0];
            this.siteHeader = this.buildSiteHeader(this.site);
            this.assemblies = this.buildAssemblyRows(this.site, unpaidPage.data);
        } finally {
            this.isLoading = false;
        }
    }

    private buildSiteHeader(site: BackflowSubmissionSite): string {
        const parts: string[] = [];

        if (site.businessName) {
            parts.push(site.businessName);
        }

        if (site.streetNumber) {
            parts.push(site.streetNumber);
        }

        if (site.streetName) {
            parts.push(site.streetName);
        }

        if (site.propertyNumber) {
            parts.push(`#${site.propertyNumber}`);
        }

        return parts.join(' ');
    }

    private buildAssemblyRows(site: BackflowSubmissionSite, unpaidTests: BackflowTest[]): ConfirmationAssemblyRow[] {
        const rows: ConfirmationAssemblyRow[] = [];

        if (!site.assemblies) {
            return rows;
        }

        for (const test of site.assemblies) {
            rows.push({
                test: test,
                description: this.buildAssemblyDescription(test),
                isAwaitingCheckout: this.isAwaitingCheckout(test, unpaidTests)
            });
        }

        return rows;
    }

    private isAwaitingCheckout(test: BackflowTest, unpaidTests: BackflowTest[]): boolean {
        const serialNumber = this.normalizeSerialNumber(test.serialNumber);

        if (!serialNumber) {
            return false;
        }

        for (const unpaidTest of unpaidTests) {
            if (this.normalizeSerialNumber(unpaidTest.serialNumber) === serialNumber) {
                return true;
            }
        }

        return false;
    }

    private normalizeSerialNumber(serialNumber: string | undefined): string {
        if (!serialNumber) {
            return '';
        }

        return serialNumber.replace(/[^a-zA-Z0-9]/g, '').toUpperCase();
    }

    private buildAssemblyDescription(test: BackflowTest): string {
        const parts: string[] = [];

        if (test.manufacturer) {
            parts.push(test.manufacturer);
        }

        if (test.model) {
            parts.push(test.model);
        }

        if (test.size) {
            parts.push(test.size);
        }

        let description = parts.join(' ');

        if (test.deviceType) {
            description = `${description} - ${test.deviceType}`;
        }

        return description;
    }
}
