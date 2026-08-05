import { Component, OnInit, TemplateRef, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { ActivatedRoute, Router } from "@angular/router";
import { FogTransporterService } from "../../../shared/services/fog/fog-transporter.service";
import { QueryProperty } from "../../../shared/models/query";
import { TableViewModel } from "../../../shared/models/table-view-model";
import { Professional } from "../../../shared/models/professionals/professional";
import { DownloadConfig } from "../../../shared/models/download-config";
import { DownloadService } from "../../../shared/services/download.service";
import { CellTemplateData, ColumnType, TableColumn } from '@envirotrax/common-ui';
import { AppContainerHelperService } from "../../../shared/services/helpers/app-contaner-helper.service";

@Component({
    selector: 'app-fog-transporter-list',
    standalone: false,
    templateUrl: './fog-transporter-list.component.html'
})
export class FogTransporterListComponent implements OnInit {
    public showResults: boolean = false;

    public table: TableViewModel<Professional> = {
        query: {
            sort: {},
            filter: []
        },
        freeTextSearch: {
            searchQuery: [
                { field: 'name', operator: 'Ct', multiWordSearch: true },
                { field: 'companyEmail', operator: 'Ct', multiWordSearch: true },
                { field: 'phoneNumber', operator: 'Ct', multiWordSearch: true }
            ]
        }
    };

    @ViewChild('addressCell', { static: true })
    public addressCell?: TemplateRef<CellTemplateData<Professional>>;

    public downloadConfig: DownloadConfig;

    constructor(
        private readonly _fogTransporterService: FogTransporterService,
        private readonly _downloadService: DownloadService,
        private readonly _router: Router,
        private readonly _activatedRoute: ActivatedRoute,
        private readonly _containerHelper: AppContainerHelperService
    ) {
        this.downloadConfig = {
            fileName: 'FOG Transporters',
            endpoint: this._fogTransporterService.getAllEndpoint(),
            suppoertedFormats: ['CSV', 'Excel'],
            columns: [
                { field: 'name', caption: 'Company Name' },
                { field: 'companyEmail', caption: 'Company Email' },
                { field: 'phoneNumber', caption: 'Company Phone' },
                { field: 'address', caption: 'Address' },
                { field: 'city', caption: 'City' },
                { field: 'state.code', caption: 'State' },
                { field: 'zipCode', caption: 'ZIP' }
            ]
        };
    }

    public async ngOnInit(): Promise<void> {
        this.table.columns = this.getColumns();
    }

    private getColumns(): TableColumn<Professional>[] {
        return [
            {
                field: 'name',
                caption: 'Company Name',
                type: ColumnType.text
            },
            {
                field: 'companyEmail',
                caption: 'Company Email',
                type: ColumnType.text
            },
            {
                field: 'phoneNumber',
                caption: 'Company Phone',
                type: ColumnType.text
            },
            {
                field: 'address',
                caption: 'Address',
                type: ColumnType.text,
                cellTemplate: this.addressCell
            }
        ];
    }

    public async getTransporters(): Promise<void> {
        try {
            this.table.isLoading = true;
            this.table.items = await this._fogTransporterService.getAll(this.table.items?.pageInfo || {}, this.table.query);
        } finally {
            this.table.isLoading = false;
        }
    }

    public onFilterChange(queryProperties: QueryProperty[]): void {
        this.table.query.filter = queryProperties;
    }

    public openDetails(row: any): void {
        this._router.navigate(['details', row.id], {
            relativeTo: this._activatedRoute
        });
    }

    public setShowResults(visible: boolean): void {
        this.showResults = visible;
        this._containerHelper.setContainerVisibility(!visible);
    }

    public async search(searchForm: NgForm): Promise<void> {
        if (searchForm.valid) {
            await this.getTransporters();
            this.setShowResults(true);
        }
    }

    public showDownloadManager(): void {
        this._downloadService.showDownloadManager(this.downloadConfig, this.table.query);
    }
}
