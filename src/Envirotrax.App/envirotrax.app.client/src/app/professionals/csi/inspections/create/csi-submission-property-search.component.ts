import { AfterViewInit, ChangeDetectorRef, Component, OnInit, TemplateRef, ViewChild } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";
import { TableViewModel } from "../../../../shared/models/table-view-model";
import { SiteService } from "../../../../shared/services/sites/site.service";
import { CellTemplateData, TableColumn, TableCustomAction } from "../../../../shared/components/data-components/table/table.component";
import { ColumnType } from "../../../../shared/components/data-components/sorting-filtering/query-view-model";
import { Site } from "../../../../shared/models/sites/site";
import { PropertyType } from "../../../../shared/enums/property-type.enum";
import { QueryProperty } from "../../../../shared/models/query";
import { NgForm } from "@angular/forms";

@Component({
    standalone: false,
    templateUrl: './csi-submission-property-search.component.html'
})
export class CsiSubmissionPropertySearchComponent implements OnInit, AfterViewInit {

    @ViewChild('propertyTypeTemplate') private propertyTypeTemplate!: TemplateRef<CellTemplateData<Site>>;
    @ViewChild('propertyInfoTemplate') private propertyInfoTemplate!: TemplateRef<CellTemplateData<Site>>;
    @ViewChild('contactInfoTemplate') private contactInfoTemplate!: TemplateRef<CellTemplateData<Site>>;

    public readonly PropertyType = PropertyType;

    public showResults: boolean = false;
    public accountNumber: string = '';
    public streetNumber: string = '';

    public errorMessage: string = '';

    public table: TableViewModel<Site> = {
        columns: [],
        query: {
            sort: {},
            filter: []
        }
    };

    public readonly customActions: TableCustomAction<Site>[] = [
        {
            text: 'Submit CSI',
            action: (site: Site) => this.viewSite(site)
        }
    ];

    constructor(
        private readonly _siteService: SiteService,
        private readonly _router: Router,
        private readonly _activatedRoute: ActivatedRoute,
        private readonly _cdr: ChangeDetectorRef
    ) { }

    public async ngOnInit(): Promise<void> {
    }

    public ngAfterViewInit(): void {
        this.table.columns = this.getColumns();
        this._cdr.detectChanges();
    }

    public async search(searchForm: NgForm): Promise<void> {
        if (!searchForm.valid) {
            return;
        }

        if (!this.accountNumber && !this.streetNumber) {
            this.errorMessage = 'Please enter an Account Number or a Street Number to search.';
            return;
        }

        this.errorMessage = '';
        await this.getSites();
        this.showResults = true;
    }

    public async getSites(): Promise<void> {
        try {
            this.table.isLoading = true;

            const filter: QueryProperty[] = [];

            if (this.accountNumber) {
                filter.push({
                    columnName: 'accountNumber',
                    comparisonOperator: 'Eq',
                    value: this.accountNumber
                });
            };
            if (this.streetNumber) {
                filter.push({
                    columnName: 'streetNumber',
                    comparisonOperator: 'StW',
                    value: this.streetNumber
                });
            }

            this.table.query.filter = filter;

            this.table.items = await this._siteService.getAllForProfessional(
                this.table.items?.pageInfo || {},
                this.table.query
            );
        } finally {
            this.table.isLoading = false;
        }
    }

    public viewSite(site: Site): void {
        this._router.navigate([site.id], { relativeTo: this._activatedRoute });
    }

    private getColumns(): TableColumn<Site>[] {
        return [
            {
                field: 'accountNumber',
                caption: 'Account #',
                type: ColumnType.text
            },
            {
                field: 'propertyType',
                caption: 'Property Type',
                type: ColumnType.other,
                queryColumnExcluded: true,
                cellTemplate: this.propertyTypeTemplate
            },
            {
                field: '',
                caption: 'Property Information',
                type: ColumnType.other,
                queryColumnExcluded: true,
                cellTemplate: this.propertyInfoTemplate
            },
            {
                field: '',
                caption: 'Contact Information',
                type: ColumnType.other,
                queryColumnExcluded: true,
                cellTemplate: this.contactInfoTemplate
            }
        ];
    }
}
