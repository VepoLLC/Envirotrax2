import { Component, OnInit } from "@angular/core";
import { TableViewModel } from "../../shared/models/table-view-model";
import { CsiInspection } from "../../shared/models/csi/csi-inspection";
import { CsiInspectionService } from "../../shared/services/csi/csi-inspection.service";
import { TableColumn } from "../../shared/components/data-components/table/table.component";
import { ColumnType } from "../../shared/components/data-components/sorting-filtering/query-view-model";
import { QueryProperty } from "../../shared/models/query";
import { NgForm } from "@angular/forms";
import { ActivatedRoute, Router } from "@angular/router";
import { InputOption } from "../../shared/components/input/input.component";

@Component({
    standalone: false,
    templateUrl: './csi-search.component.html'
})
export class CsiSearchComponent implements OnInit {
    public showResults: boolean = false;

    public table: TableViewModel<CsiInspection> = {
        columns: this.getColumns(),
        query: {
            sort: {},
            filter: []
        },
        freeTextSearch: {
            searchQuery: [
                { field: 'inspectorLicenseNumber', operator: 'Ct' },
                { field: 'inspectorCompanyName', operator: 'Ct', multiWordSearch: true },
                { field: 'inspectorContactName', operator: 'Ct', multiWordSearch: true }
            ]
        }
    };

    public passFailOptions: InputOption[] = [
        { id: '', text: 'Any result' },
        { id: 'true', text: 'Pass' },
        { id: 'false', text: 'Fail' }
    ];

    public yesNoOptions: InputOption[] = [
        { id: '', text: 'Any result' },
        { id: 'true', text: 'Yes' },
        { id: 'false', text: 'No' }
    ];

    public approvalStatusOptions: InputOption[] = [
        { id: '', text: 'Any Status' },
        { id: 'false', text: 'Approved' },
        { id: 'true', text: 'Disapproved' }
    ];

    public propertyTypes: InputOption[] = [
        { id: '', text: 'Any Value' },
        { id: '0', text: 'Residential' },
        { id: '1', text: 'Commercial' }
    ];

    constructor(
        private readonly _csiInspectionService: CsiInspectionService,
        private readonly _router: Router,
        private readonly _activatedRoute: ActivatedRoute
    ) {
    }

    public async ngOnInit(): Promise<void> {
    }

    private getColumns(): TableColumn<CsiInspection>[] {
        return [
            {
                field: 'inspectionDate',
                caption: 'Inspection Date',
                type: ColumnType.date
            },
            {
                field: 'propertyBusinessName',
                caption: 'Business Name',
                type: ColumnType.text
            },
            {
                field: 'propertyStreetNumber',
                caption: 'Street #',
                type: ColumnType.text
            },
            {
                field: 'propertyStreetName',
                caption: 'Street Name',
                type: ColumnType.text
            },
            {
                field: 'propertyCity',
                caption: 'City',
                type: ColumnType.text
            },
            {
                field: 'inspectorCompanyName',
                caption: 'Inspector Company',
                type: ColumnType.text
            },
            {
                field: 'inspectorContactName',
                caption: 'Inspector Contact',
                type: ColumnType.text
            }
        ];
    }

    public onFilterChange(queryProperties: QueryProperty[]): void {
        this.table.query.filter = queryProperties;
    }

    public async search(searchForm: NgForm): Promise<void> {
        if (searchForm.valid) {
            await this.getInspections();
            this.showResults = true;
        }
    }

    public async getInspections(): Promise<void> {
        try {
            this.table.isLoading = true;
            this.table.items = await this._csiInspectionService.getAll(
                this.table.items?.pageInfo || {},
                this.table.query
            );
        } finally {
            this.table.isLoading = false;
        }
    }

    public edit(inspection: CsiInspection): void {
        this._router.navigate([inspection.id, 'edit'], {
            relativeTo: this._activatedRoute
        });
    }
}
