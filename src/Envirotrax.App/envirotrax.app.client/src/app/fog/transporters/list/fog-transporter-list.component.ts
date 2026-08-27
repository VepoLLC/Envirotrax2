import { Component, OnInit, TemplateRef, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { ActivatedRoute, Router } from "@angular/router";
import { FogTransporterService } from "../../../shared/services/fog/fog-transporter.service";
import { QueryProperty } from "../../../shared/models/query";
import { TableViewModel } from "../../../shared/models/table-view-model";
import { Professional } from "../../../shared/models/professionals/professional";
import { ExpirationType } from "../../../shared/models/professionals/professional-user";
import { DownloadConfig } from "../../../shared/models/download-config";
import { DownloadService } from "../../../shared/services/download.service";
import { CellTemplateData, ColumnType, TableColumn } from '@envirotrax/common-ui';
import { AppContainerHelperService } from "../../../shared/services/helpers/app-contaner-helper.service";

interface FogTransporterLicenseSearchVm {
    registrationNumber?: string;
    insurancePolicyNumber?: string;
}

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

    @ViewChild('licensesCell', { static: true })
    public licensesCell?: TemplateRef<CellTemplateData<Professional>>;

    public readonly expirationType = ExpirationType;

    public downloadConfig: DownloadConfig;

    private _licenseSearch: FogTransporterLicenseSearchVm = {};

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
            },
            {
                field: 'licensesAndInsurances',
                caption: 'Licenses & Insurance Policies',
                type: ColumnType.text,
                cellTemplate: this.licensesCell,
                queryColumnExcluded: true
            }
        ];
    }

    public async getTransporters(): Promise<void> {
        try {
            this.table.isLoading = true;
            const pageInfo = this.table.items?.pageInfo || {};
            const { registrationNumber, insurancePolicyNumber } = this._licenseSearch;

            this.table.items = (registrationNumber || insurancePolicyNumber)
                ? await this._fogTransporterService.search(registrationNumber, insurancePolicyNumber, pageInfo)
                : await this._fogTransporterService.getAll(pageInfo, this.table.query);
        } finally {
            this.table.isLoading = false;
        }
    }

    private extractLicenseSearchVm(): FogTransporterLicenseSearchVm {
        const getValue = (columnName: string) =>
            this.table.query.filter?.find(f => f.columnName === columnName)?.value as string | undefined;

        return {
            registrationNumber: getValue('registrationNumber'),
            insurancePolicyNumber: getValue('insurancePolicyNumber')
        };
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
            this._licenseSearch = this.extractLicenseSearchVm();

            await this.getTransporters();
            this.setShowResults(true);
        }
    }

    public showDownloadManager(): void {
        this._downloadService.showDownloadManager(this.downloadConfig, this.table.query);
    }
}
