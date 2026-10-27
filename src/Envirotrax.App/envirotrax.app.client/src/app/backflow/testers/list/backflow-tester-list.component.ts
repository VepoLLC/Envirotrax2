import { Component, OnInit } from '@angular/core';
import { NgForm } from '@angular/forms';
import { ActivatedRoute, Router } from "@angular/router";
import { BackflowTesterManagementService, BackflowTesterSearchCriteria } from "../../../shared/services/backflow/backflow-tester-management.service";
import { QueryProperty } from "../../../shared/models/query";
import { TableViewModel } from "../../../shared/models/table-view-model";
import { Professional } from "../../../shared/models/professionals/professional";
import { ColumnType, TableColumn } from '@envirotrax/common-ui';
import { AppContainerHelperService } from "../../../shared/services/helpers/app-contaner-helper.service";
import { DownloadConfig } from "../../../shared/models/download-config";
import { DownloadService } from "../../../shared/services/download.service";

const CRITERIA_FIELDS: (keyof BackflowTesterSearchCriteria)[] = [
    "bpatLicenseNumber",
    "fireLicenseNumber",
    "insurancePolicyNumber",
    "userEmail",
    "contactName",
    "cellNumber"
];

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

    public downloadConfig: DownloadConfig;

    private _criteria: BackflowTesterSearchCriteria = {};

    constructor(
        private readonly _backflowTesterManagementService: BackflowTesterManagementService,
        private readonly _router: Router,
        private readonly _activatedRoute: ActivatedRoute,
        private readonly _containerHelper: AppContainerHelperService,
        private readonly _downloadService: DownloadService
    ) {
        this.downloadConfig = {
            fileName: 'Backflow Testers',
            endpoint: this._backflowTesterManagementService.getSearchEndpoint(this._criteria),
            suppoertedFormats: ['CSV', 'Excel'],
            columns: [
                { field: 'name', caption: 'CompanyName' },
                { field: 'companyEmail', caption: 'CompanyEmail' },
                { field: 'address', caption: 'Address' },
                { field: 'city', caption: 'City' },
                { field: 'state.code', caption: 'State' },
                { field: 'zipCode', caption: 'ZIP' },
                { field: 'phoneNumber', caption: 'WorkNumber' },
                { field: 'faxNumber', caption: 'FaxNumber' },
                { field: 'webSiteUrl', caption: 'WebsiteUrl' }
            ]
        };
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

            // One call carrying both halves: the company-column filters ride query, the licence/insurance and
            // person-level criteria ride their own parameters. Previously a filled licence box switched to a
            // licence-only call and silently dropped every company filter.
            this.table.items = await this._backflowTesterManagementService.search(
                this._criteria,
                this.table.items?.pageInfo || {},
                this.table.query
            );
        } finally {
            this.table.isLoading = false;
        }
    }

    public setShowResults(visible: boolean): void {
        this.showResults = visible;
        this._containerHelper.setContainerVisibility(!visible);
    }

    public onFilterChange(queryProperties: QueryProperty[]): void {
        this._criteria = {};

        for (const field of CRITERIA_FIELDS) {
            const property = queryProperties.find(p => p.columnName === field);

            this._criteria[field] = property?.value ? property.value : null;
        }

        this.table.query.filter = queryProperties.filter(p => !CRITERIA_FIELDS.includes(p.columnName as keyof BackflowTesterSearchCriteria));
    }

    public showDownloadManager(): void {
        this.downloadConfig.endpoint = this._backflowTesterManagementService.getSearchEndpoint(this._criteria);

        this._downloadService.showDownloadManager(this.downloadConfig, this.table.query);
    }

    public openDetails(row: any): void {
        this._router.navigate(['details', row.id], {
            relativeTo: this._activatedRoute
        });
    }

    public async search(searchForm: NgForm): Promise<void> {
        if (searchForm.valid) {

            await this.getTesters();
            this.setShowResults(true);
        }
    }
}
