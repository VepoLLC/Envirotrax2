import { Component, OnInit } from "@angular/core";
import { TableViewModel } from "../../shared/models/table-view-model";
import { Site } from "../../shared/models/sites/site";
import { SiteService } from "../../shared/services/sites/site.service";
import { ActivatedRoute, Router } from "@angular/router";
import { TableColumn } from "../../shared/components/data-components/table/table.component";
import { ColumnType } from "../../shared/components/data-components/sorting-filtering/query-view-model";
import { QueryProperty } from "../../shared/models/query";
import { NgForm } from "@angular/forms";
import { InputOption } from "../../shared/components/input/input.component";

@Component({
    standalone: false,
    templateUrl: './site-list.component.html'
})
export class SiteListComponent implements OnInit {
    public showResults: boolean = false;

    public table: TableViewModel<Site> = {
        columns: this.getColumns(),
        query: {
            sort: {},
            filter: []
        },
        freeTextSearch: {
            searchQuery: [
                { field: 'accountNumber', operator: 'Ct' },
                { field: 'businessName', operator: 'Ct', multiWordSearch: true },
                { field: 'streetName', operator: 'Ct', multiWordSearch: true },
                { field: 'city', operator: 'Ct', multiWordSearch: true }
            ]
        }
    };

    public facilityTypes: InputOption[] = [
        { id: "", text: "Any Value" },
        { id: "Restaurant", text: "Restaurant" },
        { id: "Fast Food Establishment", text: "Fast Food Establishment" },
        { id: "Hotel/Motel", text: "Hotel/Motel" },
        { id: "Car Wash", text: "Car Wash" },
        { id: "School/University", text: "School/University" },
        { id: "Grocery Store", text: "Grocery Store" },
        { id: "Convenience Store", text: "Convenience Store" },
        { id: "Assisted Living Facility", text: "Assisted Living Facility" },
        { id: "Medical Facility", text: "Medical Facility" },
        { id: "Industrial", text: "Industrial" },
        { id: "City Owned Facility", text: "City Owned Facility" },
        { id: "Other", text: "Other" }
    ];

    public yesNoOptions: InputOption[] = [
        { id: "", text: "Any Value" },
        { id: "true", text: "Yes" },
        { id: "false", text: "No" }
    ];

    public greaseTrapoptions: InputOption[] = [
        { id: "", text: "Any Value" },
        { id: "0", text: "Trap Not Required" },
        { id: "1", text: "Has Grease Trap" },
        { id: "2", text: "Should Have Grease Trap" },
        { id: "3", text: "Might Have Grease Trap" }
    ];

    public propertyTypes: InputOption[] = [
        { id: "", text: "Any Value" },
        { id: "0", text: "Residential" },
        { id: "1", text: "Commercial" }
    ];

    constructor(
        private readonly _siteService: SiteService,
        private readonly _router: Router,
        private readonly _activatedRoute: ActivatedRoute
    ) {
    }

    public async ngOnInit(): Promise<void> {
        await this.getSites();
    }

    private getColumns(): TableColumn<Site>[] {
        return [
            {
                field: 'accountNumber',
                caption: 'Account Number',
                type: ColumnType.text
            },
            {
                field: 'businessName',
                caption: 'Business Name',
                type: ColumnType.text
            },
            {
                field: 'streetNumber',
                caption: 'Street Number',
                type: ColumnType.text
            },
            {
                field: 'streetName',
                caption: 'Street Name',
                type: ColumnType.text
            },
            {
                field: 'city',
                caption: 'City',
                type: ColumnType.text
            },
            {
                field: 'propertyNumber',
                caption: 'Property Number',
                type: ColumnType.text
            }
        ];
    }

    public async getSites(): Promise<void> {
        try {
            this.table.isLoading = true;
            this.table.items = await this._siteService.getAll(this.table.items?.pageInfo || {}, this.table.query);
        } finally {
            this.table.isLoading = false;
        }
    }

    public onFilterChange(queryProperties: QueryProperty[]): void {
        this.table.query.filter = queryProperties
    }

    public async search(searchForm: NgForm): Promise<void> {
        if (searchForm.valid) {
            await this.getSites();
            this.showResults = true;
        }
    }
}
