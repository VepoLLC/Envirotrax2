import { Component } from "@angular/core";
import { Router, ActivatedRoute } from "@angular/router";
import { NgForm } from "@angular/forms";
import { HelperService } from "@envirotrax/common-ui";
import { BackflowTest } from "../../../shared/models/backflow/backflow-test";
import { BackflowSubmissionSite } from "../../../shared/models/backflow/backflow-submission-site";
import { Site } from "../../../shared/models/sites/site";
import { BackflowSubmissionSearchCriteria, SiteService } from "../../../shared/services/sites/site.service";
import { PageInfo } from "../../../shared/models/page-info";
import { Query, QueryProperty } from "../../../shared/models/query";
import { PropertyType } from "../../../shared/enums/property-type.enum";

interface SubmissionAssemblyRow {
    test: BackflowTest;
    renewalMonth: string;
    description: string;
}

interface SubmissionSiteBox {
    site: BackflowSubmissionSite;
    showWsAccountNumber: boolean;
    renewalMonth: string;
    assemblies: SubmissionAssemblyRow[];
}

@Component({
    standalone: false,
    templateUrl: './backflow-test-assembly-search.component.html'
})
export class BackflowTestAssemblySearchComponent {

    public readonly PropertyType = PropertyType;

    public isLoading = false;
    public showResults = false;
    public hasResults = false;
    public accountNumber = '';
    public manufacturer = '';
    public serialNumber = '';
    public streetNumber = '';
    public errorMessage = '';
    public resultsHeader = '';

    public siteBoxes: SubmissionSiteBox[] = [];
    public pageInfo: PageInfo = {};

    private readonly _query: Query = {
        sort: { streetNumber: 'Asc', streetName: 'Desc' },
        filter: []
    };

    private _criteria: BackflowSubmissionSearchCriteria = {};

    constructor(
        private readonly _siteService: SiteService,
        private readonly _helperService: HelperService,
        private readonly _router: Router,
        private readonly _activatedRoute: ActivatedRoute
    ) { }

    public async search(searchForm: NgForm): Promise<void> {
        if (!searchForm.valid) {
            return;
        }

        if (!this.accountNumber && !(this.manufacturer && this.serialNumber) && !this.streetNumber) {
            this.errorMessage = 'Please enter an Account Number, Manufacturer and Serial Number, or a Street Number to search.';
            return;
        }

        this.errorMessage = '';
        this.pageInfo = {};
        this.buildSearch();

        await this.getSites();

        this.showResults = true;
    }

    public async getSites(): Promise<void> {
        try {
            this.isLoading = true;

            const result = await this._siteService.getAllForBackflowSubmission(this.pageInfo, this._query, this._criteria);

            this.pageInfo = result.pageInfo;
            this.siteBoxes = this.buildSiteBoxes(result.data);
            this.hasResults = this.siteBoxes.length > 0;
        } finally {
            this.isLoading = false;
        }
    }

    public submitUnlisted(): void {
        this._router.navigate(['new'], { relativeTo: this._activatedRoute });
    }

    public submitForSite(site: Site): void {
        this._router.navigate(['new'], { relativeTo: this._activatedRoute, queryParams: { siteId: site.id } });
    }

    public selectAssembly(test: BackflowTest): void {
        this._router.navigate([test.id], { relativeTo: this._activatedRoute });
    }

    private buildSearch(): void {
        const filter: QueryProperty[] = [];

        this._criteria = {};

        if (this.accountNumber) {
            filter.push({ columnName: 'accountNumber', comparisonOperator: 'Eq', value: this.accountNumber });
            this.resultsHeader = `Property Matching Account Number '${this.accountNumber}'`;
        } else if (this.manufacturer && this.serialNumber) {
            this._criteria = { manufacturer: this.manufacturer, serialNumber: this.serialNumber };
            this.resultsHeader = `Properties With Assembly Serial Numbers Similar To '${this.serialNumber}'`;
        } else {
            filter.push({ columnName: 'streetNumber', comparisonOperator: 'Eq', value: this.streetNumber });
            this.resultsHeader = `Property Locations Similar To '${this.streetNumber}'`;
        }

        this._query.filter = filter;
    }

    private buildSiteBoxes(sites: BackflowSubmissionSite[]): SubmissionSiteBox[] {
        const boxes: SubmissionSiteBox[] = [];

        for (const site of sites) {
            const assemblies: SubmissionAssemblyRow[] = [];

            if (site.assemblies) {
                for (const test of site.assemblies) {
                    assemblies.push(this.buildAssemblyRow(site, test));
                }
            }

            boxes.push({
                site: site,
                showWsAccountNumber: !!site.includeWsAccountNumbers && !!site.waterSupplierAccountNumber,
                renewalMonth: this._helperService.getShortMonthName(site.backflowScheduleMonth),
                assemblies: assemblies
            });
        }

        return boxes;
    }

    private buildAssemblyRow(site: BackflowSubmissionSite, test: BackflowTest): SubmissionAssemblyRow {
        let renewalMonth = this._helperService.getShortMonthName(site.backflowScheduleMonth);

        if (test.backflowScheduleMonth) {
            renewalMonth = this._helperService.getShortMonthName(test.backflowScheduleMonth);
        }

        return {
            test: test,
            renewalMonth: renewalMonth,
            description: this.buildAssemblyDescription(test)
        };
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
