import { Component, OnInit } from '@angular/core';
import { NgForm } from '@angular/forms';
import { BackflowTestService } from '../../../shared/services/backflow/backflow-test.service';
import { ProfessionalSupplierService } from '../../../shared/services/professionals/professional-supplier.service';
import { QueryProperty } from '../../../shared/models/query';
import { TableViewModel } from '../../../shared/models/table-view-model';
import { BackflowTest } from '../../../shared/models/backflow/backflow-test';
import { TableColumn } from '../../../shared/components/data-components/table/table.component';
import { ColumnType } from '../../../shared/components/data-components/sorting-filtering/query-view-model';
import { InputOption } from '../../../shared/components/input/input.component';

@Component({
    standalone: false,
    templateUrl: './professional-backflow-test-list.component.html'
})
export class ProfessionalBackflowTestListComponent implements OnInit {
    public showResults: boolean = false;

    public table: TableViewModel<BackflowTest> = {
        columns: this.getColumns(),
        query: {
            sort: {},
            filter: []
        },
        freeTextSearch: {
            searchQuery: [
                { field: 'accountNumber', operator: 'Ct' },
                { field: 'serialNumber', operator: 'Ct' }
            ]
        }
    };

    public waterSupplierScopeOptions: InputOption[] = [
        { id: '', text: 'My test history only' }
    ];

    public testHistoryOptions: InputOption[] = [
        { id: 'true', text: 'Latest test only' },
        { id: 'false', text: 'Complete test history' }
    ];

    public propertyTypeOptions: InputOption[] = [
        { id: '', text: 'Any value' },
        { id: '0', text: 'Residential' },
        { id: '1', text: 'Commercial' }
    ];

    constructor(
        private readonly _backflowTestService: BackflowTestService,
        private readonly _supplierService: ProfessionalSupplierService
    ) { }

    public async ngOnInit(): Promise<void> {
        await this.loadWaterSupplierScopeOptions();
    }

    private async loadWaterSupplierScopeOptions(): Promise<void> {
        const suppliers = await this._supplierService.getAllMy(false, true);
        const supplierOptions: InputOption[] = suppliers.data
            .filter(s => s.waterSupplier?.id)
            .map(s => ({ id: String(s.waterSupplier!.id!), text: s.waterSupplier!.name ?? '' }));
        this.waterSupplierScopeOptions = [
            { id: '', text: 'My test history only' },
            ...supplierOptions
        ];
    }

    private getColumns(): TableColumn<BackflowTest>[] {
        return [
            {
                field: 'accountNumber',
                caption: 'Account Number',
                type: ColumnType.text
            },
            {
                field: 'serialNumber',
                caption: 'Serial Number',
                type: ColumnType.text
            },
            {
                field: 'propertyBusinessName',
                caption: 'Business Name',
                type: ColumnType.text
            },
            {
                field: 'propertyStreetNumber',
                caption: 'Street Number',
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
                field: 'testDate',
                caption: 'Test Date',
                type: ColumnType.date
            },
            {
                field: 'testResult',
                caption: 'Test Result',
                type: ColumnType.text
            },
            {
                field: 'bpatCompanyName',
                caption: 'BPAT Company',
                type: ColumnType.text
            },
            {
                field: 'expirationDate',
                caption: 'Expiration Date',
                type: ColumnType.date
            }
        ];
    }

    public async getTests(): Promise<void> {
        try {
            this.table.isLoading = true;
            this.table.items = await this._backflowTestService.getAllForProfessional(
                this.table.items?.pageInfo || {},
                this.table.query
            );
        } finally {
            this.table.isLoading = false;
        }
    }

    public onFilterChange(queryProperties: QueryProperty[]): void {
        this.table.query.filter = queryProperties;
    }

    public async search(searchForm: NgForm): Promise<void> {
        if (searchForm.valid) {
            await this.getTests();
            this.showResults = true;
        }
    }
}
