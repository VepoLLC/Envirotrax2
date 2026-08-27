import { Component, OnInit } from '@angular/core';
import { NgForm } from '@angular/forms';
import { ActivatedRoute, Router } from "@angular/router";
import { BackflowTesterManagementService } from "../../../shared/services/backflow/backflow-tester-management.service";
import { QueryProperty } from "../../../shared/models/query";
import { TableViewModel } from "../../../shared/models/table-view-model";
import { Professional } from "../../../shared/models/professionals/professional";
import { ColumnType, TableColumn } from '@envirotrax/common-ui';
import { AppContainerHelperService } from "../../../shared/services/helpers/app-contaner-helper.service";

interface BackflowLicenseSearchVm {
    bpatLicenseNumber?: string;
    fireLicenseNumber?: string;
    insurancePolicyNumber?: string;
}

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

    private _licenseSearch: BackflowLicenseSearchVm = {};

    constructor(
        private readonly _backflowTesterManagementService: BackflowTesterManagementService,
        private readonly _router: Router,
        private readonly _activatedRoute: ActivatedRoute,
        private readonly _containerHelper: AppContainerHelperService
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
            const pageInfo = this.table.items?.pageInfo || {};
            const { bpatLicenseNumber, fireLicenseNumber, insurancePolicyNumber } = this._licenseSearch;

            this.table.items = (bpatLicenseNumber || fireLicenseNumber || insurancePolicyNumber)
                ? await this._backflowTesterManagementService.search(bpatLicenseNumber, fireLicenseNumber, insurancePolicyNumber, pageInfo)
                : await this._backflowTesterManagementService.getAll(pageInfo, this.table.query);
        } finally {
            this.table.isLoading = false;
        }
    }

    private extractLicenseSearchVm(): BackflowLicenseSearchVm {
        const getValue = (columnName: string) =>
            this.table.query.filter?.find(f => f.columnName === columnName)?.value as string | undefined;

        return {
            bpatLicenseNumber: getValue('bpatLicenseNumber'),
            fireLicenseNumber: getValue('fireLicenseNumber'),
            insurancePolicyNumber: getValue('insurancePolicyNumber')
        };
    }

    public setShowResults(visible: boolean): void {
        this.showResults = visible;
        this._containerHelper.setContainerVisibility(!visible);
    }

    public onFilterChange(queryProperties: QueryProperty[]): void {
        this.table.query.filter = queryProperties;
    }

    public openDetails(row: any): void {
        this._router.navigate(['details', row.id], {
            relativeTo: this._activatedRoute
        });
    }

    public async search(searchForm: NgForm): Promise<void> {
        if (searchForm.valid) {
            this._licenseSearch = this.extractLicenseSearchVm();

            await this.getTesters();
            this.setShowResults(true);
        }
    }
}
