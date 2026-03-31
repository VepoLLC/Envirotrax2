import { Component, OnInit } from '@angular/core';
import { NgForm } from '@angular/forms';
import { ActivatedRoute, Router } from "@angular/router";
import { CsiInspectoreManagementService } from "../../../shared/services/csi/csi-inspector-management.service";
import { QueryProperty } from "../../../shared/models/query";
import { TableViewModel } from "../../../shared/models/table-view-model";
import { Professional } from "../../../shared/models/professionals/professional";
import { TableColumn } from "../../../shared/components/data-components/table/table.component";
import { ColumnType } from "../../../shared/components/data-components/sorting-filtering/query-view-model";

@Component({
    selector: 'app-csi-inspector-list',
  standalone: false,
    templateUrl: './csi-inspector-list.component.html'
})
export class CsiInspectorListComponent implements OnInit {
    public showResults: boolean = false;

    public table: TableViewModel<Professional> = {
        columns: this.getColumns(),
        query: {
            sort: {},
            filter: []
        },
        freeTextSearch: {
            searchQuery: [
                //todo
                //{ field: 'accountNumber', operator: 'Ct' },
                //{ field: 'businessName', operator: 'Ct', multiWordSearch: true },
                //{ field: 'streetName', operator: 'Ct', multiWordSearch: true },
                //{ field: 'city', operator: 'Ct', multiWordSearch: true }
            ]
        }
    };
    constructor(
        private readonly _csiInspectoreManagementService: CsiInspectoreManagementService,
        private readonly _router: Router,
        private readonly _activatedRoute: ActivatedRoute
    ) {
    }

    public async ngOnInit(): Promise<void> {
    }

    private getColumns(): TableColumn<Professional>[] {
        return [
            {
                field: 'name',
                caption: 'Company Name',
                type: ColumnType.text
            },
            {
                field: 'address',
                caption: 'Address',
                type: ColumnType.text
            },
            {
                field: 'street',
                caption: 'Street',
                type: ColumnType.text
            },
            {
                field: 'city',
                caption: 'City',
                type: ColumnType.text
            },
            {
                field: 'zipCode',
                caption: 'Zip code',
                type: ColumnType.text
            },
            {
                field: 'phoneNumber',
                caption: 'Phone Number',
                type: ColumnType.text
            }
        ];
    }

    public async  getInspectors(): Promise<void> {
        try {
            this.table.isLoading = true;
            this.table.items = await this._csiInspectoreManagementService.getAll(this.table.items?.pageInfo || {}, this.table.query);
            console.log(this.table.items);
        } finally {
            this.table.isLoading = false;
        }
    }

    public onFilterChange(queryProperties: QueryProperty[]): void {
        this.table.query.filter = queryProperties
    }

    public async search(searchForm: NgForm): Promise<void> {
        if (searchForm.valid) {
            await this.getInspectors();
            this.showResults = true;
        }
    }
}
