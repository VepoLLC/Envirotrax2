import { AfterViewInit, ChangeDetectorRef, Component, TemplateRef, ViewChild } from "@angular/core";
import { Router, ActivatedRoute } from "@angular/router";
import { NgForm } from "@angular/forms";
import { TableViewModel } from "../../../shared/models/table-view-model";
import { BackflowTest } from "../../../shared/models/backflow/backflow-test";
import { Site } from "../../../shared/models/sites/site";
import { BackflowTestService } from "../../../shared/services/backflow/backflow-test.service";
import { SiteService } from "../../../shared/services/sites/site.service";
import { QueryProperty } from "../../../shared/models/query";
import { PropertyType } from "../../../shared/enums/property-type.enum";
import { CellTemplateData, ColumnType, TableColumn, TableCustomAction } from "@envirotrax/common-ui";

@Component({
    standalone: false,
    templateUrl: './backflow-test-assembly-search.component.html'
})
export class BackflowTestAssemblySearchComponent implements AfterViewInit {

    @ViewChild('assemblyInfoTemplate') private assemblyInfoTemplate!: TemplateRef<CellTemplateData<BackflowTest>>;
    @ViewChild('propertyInfoTemplate') private propertyInfoTemplate!: TemplateRef<CellTemplateData<BackflowTest>>;
    @ViewChild('testInfoTemplate') private testInfoTemplate!: TemplateRef<CellTemplateData<BackflowTest>>;
    @ViewChild('siteTypeTemplate') private siteTypeTemplate!: TemplateRef<CellTemplateData<Site>>;
    @ViewChild('sitePropertyInfoTemplate') private sitePropertyInfoTemplate!: TemplateRef<CellTemplateData<Site>>;
    @ViewChild('siteContactInfoTemplate') private siteContactInfoTemplate!: TemplateRef<CellTemplateData<Site>>;

    public readonly PropertyType = PropertyType;

    public showResults = false;
    public searchType: 'sites' | 'tests' = 'sites';
    public accountNumber = '';
    public manufacturer = '';
    public serialNumber = '';
    public streetNumber = '';
    public errorMessage = '';

    public testTable: TableViewModel<BackflowTest> = {
        columns: [],
        query: { sort: {}, filter: [] }
    };

    public siteTable: TableViewModel<Site> = {
        columns: [],
        query: { sort: {}, filter: [] }
    };

    public readonly testCustomActions: TableCustomAction<BackflowTest>[] = [
        { text: 'Submit Test', action: (test: BackflowTest) => this.selectAssembly(test) }
    ];

    public readonly siteCustomActions: TableCustomAction<Site>[] = [
        { text: 'Submit Test for Unlisted Assembly', action: (site: Site) => this.submitForSite(site) }
    ];

    constructor(
        private readonly _backflowTestService: BackflowTestService,
        private readonly _siteService: SiteService,
        private readonly _router: Router,
        private readonly _activatedRoute: ActivatedRoute,
        private readonly _cdr: ChangeDetectorRef
    ) { }

    public ngAfterViewInit(): void {
        this.testTable.columns = this.getTestColumns();
        this.siteTable.columns = this.getSiteColumns();
        this._cdr.detectChanges();
    }

    public async search(searchForm: NgForm): Promise<void> {
        if (!searchForm.valid) return;

        if (!this.accountNumber && !(this.manufacturer && this.serialNumber) && !this.streetNumber) {
            this.errorMessage = 'Please enter an Account Number, Manufacturer and Serial Number, or a Street Number to search.';
            return;
        }

        this.errorMessage = '';

        if (this.accountNumber) {
            this.searchType = 'sites';
            await this.getSites();
        } else if (this.manufacturer && this.serialNumber) {
            this.searchType = 'tests';
            await this.getAssemblies();
        } else {
            this.searchType = 'sites';
            await this.getSites();
        }

        this.showResults = true;
    }

    public async getSites(): Promise<void> {
        try {
            this.siteTable.isLoading = true;
            const filter: QueryProperty[] = [];

            if (this.accountNumber) {
                filter.push({ columnName: 'accountNumber', comparisonOperator: 'Eq', value: this.accountNumber });
            } else if (this.streetNumber) {
                filter.push({ columnName: 'streetNumber', comparisonOperator: 'Eq', value: this.streetNumber });
            }

            this.siteTable.query.filter = filter;
            this.siteTable.items = await this._siteService.getAllForProfessional(
                this.siteTable.items?.pageInfo || {},
                this.siteTable.query
            );
        } finally {
            this.siteTable.isLoading = false;
        }
    }

    public async getAssemblies(): Promise<void> {
        try {
            this.testTable.isLoading = true;
            const filter: QueryProperty[] = [];

            if (this.manufacturer) {
                filter.push({ columnName: 'manufacturer', comparisonOperator: 'StW', value: this.manufacturer });
            }
            if (this.serialNumber) {
                filter.push({ columnName: 'serialNumber', comparisonOperator: 'Eq', value: this.serialNumber });
            }
            filter.push({ columnName: 'isCurrent', comparisonOperator: 'Eq', value: 'true' });

            this.testTable.query.filter = filter;
            this.testTable.items = await this._backflowTestService.getAllForProfessional(
                this.testTable.items?.pageInfo || {},
                this.testTable.query
            );
        } finally {
            this.testTable.isLoading = false;
        }
    }

    public submitUnlisted(): void {
        this._router.navigate(['new'], { relativeTo: this._activatedRoute });
    }

    public submitForSite(site: Site): void {
        this._router.navigate(['new'], { relativeTo: this._activatedRoute, queryParams: { siteId: site.id } });
    }

    private selectAssembly(test: BackflowTest): void {
        this._router.navigate([test.id], { relativeTo: this._activatedRoute });
    }

    private getTestColumns(): TableColumn<BackflowTest>[] {
        return [
            { field: 'accountNumber', caption: 'Account #', type: ColumnType.text },
            { field: '', caption: 'Assembly Information', type: ColumnType.other, queryColumnExcluded: true, cellTemplate: this.assemblyInfoTemplate },
            { field: '', caption: 'Property Information', type: ColumnType.other, queryColumnExcluded: true, cellTemplate: this.propertyInfoTemplate },
            { field: '', caption: 'Last Test', type: ColumnType.other, queryColumnExcluded: true, cellTemplate: this.testInfoTemplate }
        ];
    }

    private getSiteColumns(): TableColumn<Site>[] {
        return [
            { field: 'accountNumber', caption: 'Account #', type: ColumnType.text },
            { field: 'propertyType', caption: 'Property Type', type: ColumnType.other, queryColumnExcluded: true, cellTemplate: this.siteTypeTemplate },
            { field: '', caption: 'Property Information', type: ColumnType.other, queryColumnExcluded: true, cellTemplate: this.sitePropertyInfoTemplate },
            { field: '', caption: 'Contact Information', type: ColumnType.other, queryColumnExcluded: true, cellTemplate: this.siteContactInfoTemplate }
        ];
    }
}
