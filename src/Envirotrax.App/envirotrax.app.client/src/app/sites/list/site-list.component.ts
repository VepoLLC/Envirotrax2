import { Component, OnInit } from "@angular/core";
import { TableViewModel } from "../../shared/models/table-view-model";
import { Site } from "../../shared/models/sites/site";
import { SiteService } from "../../shared/services/sites/site.service";
import { ActivatedRoute, Router } from "@angular/router";
import { TableColumn } from "../../shared/components/data-components/table/table.component";
import { ColumnType } from "../../shared/components/data-components/sorting-filtering/query-view-model";

@Component({
    standalone: false,
    templateUrl: './site-list.component.html'
})
export class SiteListComponent implements OnInit {
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
}
