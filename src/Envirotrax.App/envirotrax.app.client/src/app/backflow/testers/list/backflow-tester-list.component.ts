import { Component, OnInit } from '@angular/core';
import { NgForm } from '@angular/forms';
import { ActivatedRoute, Router } from "@angular/router";
import { BackflowTesterManagementService } from "../../../shared/services/backflow/backflow-tester-management.service";
import { QueryProperty } from "../../../shared/models/query";
import { TableViewModel } from "../../../shared/models/table-view-model";
import { Professional } from "../../../shared/models/professionals/professional";
import { TableColumn } from "../../../shared/components/data-components/table/table.component";
import { ColumnType } from "../../../shared/components/data-components/sorting-filtering/query-view-model";

@Component({
    selector: 'app-backflow-tester-list',
    standalone: false,
    templateUrl: './backflow-tester-list.component.html'
})
export class BackflowTesterListComponent implements OnInit {
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
                //{ field: 'name', operator: 'Ct' },
                //{ field: 'city', operator: 'Ct', multiWordSearch: true }
            ]
        }
    };

    constructor(
        private readonly _backflowTesterManagementService: BackflowTesterManagementService,
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
                caption: 'Zip Code',
                type: ColumnType.text
            },
            {
                field: 'phoneNumber',
                caption: 'Phone Number',
                type: ColumnType.text
            }
        ];
    }

    public async getTesters(): Promise<void> {
        try {
            this.table.isLoading = true;
            this.table.items = await this._backflowTesterManagementService.getAll(this.table.items?.pageInfo || {}, this.table.query);
        } finally {
            this.table.isLoading = false;
        }
    }

    public onFilterChange(queryProperties: QueryProperty[]): void {
        this.table.query.filter = queryProperties;
    }

    public async search(searchForm: NgForm): Promise<void> {
        if (searchForm.valid) {
            await this.getTesters();
            this.showResults = true;
        }
    }
}
