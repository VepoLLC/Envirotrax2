import { Component, OnInit, TemplateRef, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { ActivatedRoute, Router } from "@angular/router";
import { CsiInspectoreManagementService } from "../../../shared/services/csi/csi-inspector-management.service";
import { QueryProperty } from "../../../shared/models/query";
import { TableViewModel } from "../../../shared/models/table-view-model";
import { Professional } from "../../../shared/models/professionals/professional";
import { CellTemplateData, ColumnType, TableColumn } from '@envirotrax/common-ui';
import { AppContainerHelperService } from "../../../shared/services/helpers/app-contaner-helper.service";

interface CsiInspectorLicenseSearchVm {
    inspectorLicenseNumber?: string;
    insurancePolicyNumber?: string;
}

@Component({
    selector: 'app-csi-inspector-list',
    standalone: false,
    templateUrl: './csi-inspector-list.component.html'
})
export class CsiInspectorListComponent implements OnInit {
    public showResults: boolean = false;

    public table: TableViewModel<Professional> = {
        query: {
            sort: {},
            filter: []
        },
        freeTextSearch: {
            searchQuery: [
                { field: 'companyName' },
                { field: 'companyEmail' },
                { field: 'phoneNumber' }
            ]
        }
    };

    @ViewChild('addressCell', { static: true })
    public addressCell?: TemplateRef<CellTemplateData<Professional>>;

    private _licenseSearch: CsiInspectorLicenseSearchVm = {};

    constructor(
        private readonly _csiInspectoreManagementService: CsiInspectoreManagementService,
        private readonly _router: Router,
        private readonly _activatedRoute: ActivatedRoute,
        private readonly _containerHelper: AppContainerHelperService
    ) {
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

    public async getInspectors(): Promise<void> {
        try {
            this.table.isLoading = true;
            const pageInfo = this.table.items?.pageInfo || {};
            const { inspectorLicenseNumber, insurancePolicyNumber } = this._licenseSearch;

            this.table.items = (inspectorLicenseNumber || insurancePolicyNumber)
                ? await this._csiInspectoreManagementService.search(inspectorLicenseNumber, insurancePolicyNumber, pageInfo)
                : await this._csiInspectoreManagementService.getAll(pageInfo, this.table.query);
        } finally {
            this.table.isLoading = false;
        }
    }

    private extractLicenseSearchVm(): CsiInspectorLicenseSearchVm {
        const getValue = (columnName: string) =>
            this.table.query.filter?.find(f => f.columnName === columnName)?.value as string | undefined;

        return {
            inspectorLicenseNumber: getValue('inspectorLicenseNumber'),
            insurancePolicyNumber: getValue('insurancePolicyNumber')
        };
    }

    public setShowResults(visible: boolean): void {
        this.showResults = visible;
        this._containerHelper.setContainerVisibility(!visible);
    }

    public onFilterChange(queryProperties: QueryProperty[]): void {
        this.table.query.filter = queryProperties
    }

    public openDetails(row: any): void {
        this._router.navigate(['details', row.id], {
            relativeTo: this._activatedRoute
        });
    }

    public async search(searchForm: NgForm): Promise<void> {
        if (searchForm.valid) {
            this._licenseSearch = this.extractLicenseSearchVm();

            await this.getInspectors();
            this.setShowResults(true);
        }
    }
}
