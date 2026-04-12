import { Component, OnInit } from '@angular/core';
import { NgForm } from '@angular/forms';
import { FogInspectorManagementService } from "../../../shared/services/fog/fog-inspector-management.service";
import { QueryProperty } from "../../../shared/models/query";
import { TableViewModel } from "../../../shared/models/table-view-model";
import { Professional } from "../../../shared/models/professionals/professional";
import { TableColumn } from "../../../shared/components/data-components/table/table.component";
import { ColumnType } from "../../../shared/components/data-components/sorting-filtering/query-view-model";

@Component({
    selector: 'app-fog-inspector-list',
    standalone: false,
    templateUrl: './fog-inspector-list.component.html'
})
export class FogInspectorListComponent implements OnInit {
    public showResults: boolean = false;

    public table: TableViewModel<Professional> = {
        columns: this.getColumns(),
        query: {
            sort: {},
            filter: []
        },
        freeTextSearch: {
            searchQuery: [
                {field: 'name', operator: 'Ct', multiWordSearch: true},
                {field: 'address', operator: 'Ct', multiWordSearch: true},
                {field: 'street', operator: 'Ct', multiWordSearch: true},
                {field: 'city', operator: 'Ct', multiWordSearch: true},
                {field: 'zipCode', operator: 'Ct', multiWordSearch: true},
                {field: 'phoneNumber', operator: 'Ct', multiWordSearch: true}
            ]
        }
    };

    constructor(
        private readonly _fogInspectorManagementService: FogInspectorManagementService
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

    public async getInspectors(): Promise<void> {
        try {
            this.table.isLoading = true;
            this.table.items = await this._fogInspectorManagementService.getAll(this.table.items?.pageInfo || {}, this.table.query);
        } finally {
            this.table.isLoading = false;
        }
    }

    public onFilterChange(queryProperties: QueryProperty[]): void {
        this.table.query.filter = queryProperties;
    }

    public async search(searchForm: NgForm): Promise<void> {
        if (searchForm.valid) {
            await this.getInspectors();
            this.showResults = true;
        }
    }
}
