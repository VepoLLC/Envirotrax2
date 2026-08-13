import { Component, OnInit, TemplateRef, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import {
    CellTemplateData,
    ColumnType,
    DateRange,
    InputOption,
    QueryProperty,
    SortOperator,
    TableColumn,
    TableViewModel
} from '@envirotrax/common-ui';
import { BackflowPaymentStatus, BackflowTest, BackflowTestResult } from '../../../shared/models/backflow/backflow-test';
import { PropertyType } from '../../../shared/models/sites/site';
import { BackflowTestService } from '../../../shared/services/backflow/backflow-test.service';
import { BackflowTestOptionsService } from '../../../shared/services/backflow/backflow-test-options.service';
import { WaterSupplierService } from '../../../shared/services/water-suppliers/water-supplier.service';

@Component({
    templateUrl: './backflow-test-list.component.html',
    standalone: false,
})
export class BackflowTestListComponent implements OnInit {
    @ViewChild('statusCell', { static: true })
    public statusCell?: TemplateRef<CellTemplateData<BackflowTest>>;

    @ViewChild('datesCell', { static: true })
    public datesCell?: TemplateRef<CellTemplateData<BackflowTest>>;

    @ViewChild('assemblyCell', { static: true })
    public assemblyCell?: TemplateRef<CellTemplateData<BackflowTest>>;

    @ViewChild('propertyTypeCell', { static: true })
    public propertyTypeCell?: TemplateRef<CellTemplateData<BackflowTest>>;

    @ViewChild('propertyCell', { static: true })
    public propertyCell?: TemplateRef<CellTemplateData<BackflowTest>>;

    @ViewChild('bpatCell', { static: true })
    public bpatCell?: TemplateRef<CellTemplateData<BackflowTest>>;

    public readonly propertyType = PropertyType;

    public readonly testResult = BackflowTestResult;

    public readonly todayIso: string = new Date().toISOString();

    public showResults: boolean = false;

    public testHistory: string = 'true';

    public testResultFilter: string = '';

    public serialNumber: string = '';

    public bpatId: string = '';

    public dateSearchField: string = '';

    public dateRange?: DateRange;

    public paymentStatus: string = '';

    private panelFilter: QueryProperty[] = [];

    public table: TableViewModel<BackflowTest> = {
        query: {
            sort: {},
            filter: []
        }
    };

    public waterSupplierOptions: InputOption[] = [{ id: '', text: 'Any Value' }];

    public deviceTypeOptions: InputOption[];

    public hazardTypeOptions: InputOption[];

    public reasonForTestOptions: InputOption[];

    public readonly testHistoryOptions: InputOption[] = [
        { id: 'true', text: 'Latest test only' },
        { id: '', text: 'Complete test history' }
    ];

    public readonly testResultOptions: InputOption[] = [
        { id: '', text: 'Any Value' },
        { id: 'passing', text: 'All Passing Tests' },
        { id: 'passedNoRepairs', text: 'Passed With No Repairs' },
        { id: 'passedWithRepairs', text: 'Passed With Repairs' },
        { id: 'failed', text: 'Failed Tests' }
    ];

    public readonly serviceStatusOptions: InputOption[] = [
        { id: '', text: 'Any Value' },
        { id: 'false', text: 'In Service' },
        { id: 'true', text: 'Out of Service' }
    ];

    public readonly paymentStatusOptions: InputOption[] = [
        { id: '', text: 'Any Value' },
        { id: String(BackflowPaymentStatus.Paid), text: 'Paid' },
        { id: String(BackflowPaymentStatus.Unpaid), text: 'Unpaid' }
    ];

    public readonly approvedStatusOptions: InputOption[] = [
        { id: '', text: 'Any Value' },
        { id: 'false', text: 'Approved' },
        { id: 'true', text: 'Disapproved' }
    ];

    public readonly rejectedStatusOptions: InputOption[] = [
        { id: '', text: 'Any Value' },
        { id: 'false', text: 'Accepted' },
        { id: 'true', text: 'Rejected' }
    ];

    public readonly forceRenewalOptions: InputOption[] = [
        { id: '', text: 'Any Value' },
        { id: 'true', text: 'On' },
        { id: 'false', text: 'Off' }
    ];

    public readonly yesNoOptions: InputOption[] = [
        { id: '', text: 'Any Value' },
        { id: 'true', text: 'Yes' },
        { id: 'false', text: 'No' }
    ];

    public readonly propertyTypes: InputOption[] = [
        { id: '', text: 'Any Value' },
        { id: String(PropertyType.Residential), text: 'Residential' },
        { id: String(PropertyType.Commercial), text: 'Commercial' }
    ];

    public readonly dateSearchOptions: InputOption[] = [
        { id: '', text: 'None' },
        { id: 'testDate', text: 'Test Date' },
        { id: 'createdTime', text: 'Submission Date' },
        { id: 'expirationDate', text: 'Expiration Date' }
    ];

    constructor(
        private readonly _backflowTestService: BackflowTestService,
        private readonly _waterSupplierService: WaterSupplierService,
        private readonly _options: BackflowTestOptionsService
    ) {
        this.deviceTypeOptions = this._options.deviceTypeOptions;
        this.hazardTypeOptions = this._options.hazardTypeOptions;
        this.reasonForTestOptions = this._options.reasonForTestOptions;
    }

    public async ngOnInit(): Promise<void> {
        this.table.columns = this.getColumns();

        await this.loadWaterSuppliers();
    }

    public onFilterChange(queryProperties: QueryProperty[]): void {
        this.panelFilter = queryProperties;

        this.table.query.filter = this.buildFilter();
    }

    public async search(searchForm: NgForm): Promise<void> {
        if (!searchForm.valid) {
            return;
        }

        this.table.query.filter = this.buildFilter();
        this.table.query.sort = this.buildSort();

        await this.getTests();

        this.showResults = true;
    }

    public async getTests(): Promise<void> {
        try {
            this.table.isLoading = true;
            this.table.items = await this._backflowTestService.getAll(
                this.table.items?.pageInfo || {},
                this.table.query,
                this.paymentStatus ? Number(this.paymentStatus) as BackflowPaymentStatus : null
            );
        } finally {
            this.table.isLoading = false;
        }
    }

    private buildFilter(): QueryProperty[] {
        const filter = [...this.panelFilter];

        this.addTestHistoryFilter(filter);
        this.addTestResultFilter(filter);
        this.addSerialNumberFilter(filter);
        this.addBpatIdFilter(filter);
        this.addDateRangeFilter(filter);

        return filter;
    }

    private addTestHistoryFilter(filter: QueryProperty[]): void {
        if (!this.testHistory) {
            return;
        }

        filter.push({ columnName: 'isCurrent', value: this.testHistory, comparisonOperator: 'Eq' });
    }

    private addTestResultFilter(filter: QueryProperty[]): void {
        if (!this.testResultFilter) {
            return;
        }

        if (this.testResultFilter === 'passing') {
            filter.push({
                columnName: 'testResult',
                children: [
                    { columnName: 'testResult', value: String(BackflowTestResult.Pass), comparisonOperator: 'Eq', logicalOperator: 'Or' },
                    { columnName: 'testResult', value: String(BackflowTestResult.PassAfterRepairs), comparisonOperator: 'Eq', logicalOperator: 'Or' }
                ]
            });

            return;
        }

        const resultsByOption: { [key: string]: BackflowTestResult } = {
            passedNoRepairs: BackflowTestResult.Pass,
            passedWithRepairs: BackflowTestResult.PassAfterRepairs,
            failed: BackflowTestResult.Fail
        };

        filter.push({
            columnName: 'testResult',
            value: String(resultsByOption[this.testResultFilter]),
            comparisonOperator: 'Eq'
        });
    }

    private addSerialNumberFilter(filter: QueryProperty[]): void {
        const serialNumber = this.serialNumber.trim();

        if (!serialNumber) {
            return;
        }

        filter.push({
            columnName: 'serialNumber',
            children: [
                { columnName: 'serialNumber', value: serialNumber, comparisonOperator: 'Ct', logicalOperator: 'Or' },
                { columnName: 'serialNumber2', value: serialNumber, comparisonOperator: 'Ct', logicalOperator: 'Or' }
            ]
        });
    }

    private addBpatIdFilter(filter: QueryProperty[]): void {
        const bpatId = this.bpatId.trim();

        if (!bpatId || !/^\d+$/.test(bpatId)) {
            return;
        }

        filter.push({
            columnName: 'bpat.id',
            children: [
                { columnName: 'professional.id', value: bpatId, comparisonOperator: 'Eq', logicalOperator: 'Or' },
                { columnName: 'bpat.id', value: bpatId, comparisonOperator: 'Eq', logicalOperator: 'Or' }
            ]
        });
    }

    private addDateRangeFilter(filter: QueryProperty[]): void {
        if (!this.dateSearchField) {
            return;
        }

        const children: QueryProperty[] = [];

        if (this.dateRange?.startDate) {
            children.push({
                columnName: this.dateSearchField,
                value: this.dateRange.startDate,
                comparisonOperator: 'Gte',
                logicalOperator: 'And'
            });
        }

        if (this.dateRange?.endDate) {
            children.push({
                columnName: this.dateSearchField,
                value: this.dateRange.endDate,
                comparisonOperator: 'Lte',
                logicalOperator: 'And'
            });
        }

        if (children.length > 0) {
            filter.push({ columnName: this.dateSearchField, children });
        }
    }

    private buildSort(): { [key: string]: SortOperator } {
        if (this.dateSearchField) {
            return { [this.dateSearchField]: 'Desc' };
        }

        return { testDate: 'Desc' };
    }

    private async loadWaterSuppliers(): Promise<void> {
        const options = await this._waterSupplierService.getAllAsOptions();

        this.waterSupplierOptions = [{ id: '', text: 'Any Value' }, ...options];
    }

    private getColumns(): TableColumn<BackflowTest>[] {
        return [
            { field: 'id', caption: 'ID', type: ColumnType.number },
            { field: 'isCurrent', caption: 'Status', type: ColumnType.other, cellTemplate: this.statusCell, queryColumnExcluded: true },
            { field: 'testDate', caption: 'Dates', type: ColumnType.other, cellTemplate: this.datesCell, queryColumnExcluded: true },
            { field: 'serialNumber', caption: 'Serial #', type: ColumnType.text },
            { field: 'deviceType', caption: 'Assembly Information', type: ColumnType.other, cellTemplate: this.assemblyCell, queryColumnExcluded: true },
            { field: 'propertyType', caption: '', type: ColumnType.other, cellTemplate: this.propertyTypeCell, queryColumnExcluded: true },
            { field: 'accountNumber', caption: 'Account #', type: ColumnType.text },
            { field: 'propertyBusinessName', caption: 'Property Information', type: ColumnType.other, cellTemplate: this.propertyCell, queryColumnExcluded: true },
            { field: 'bpatCompanyName', caption: 'BPAT Information', type: ColumnType.other, cellTemplate: this.bpatCell, queryColumnExcluded: true }
        ];
    }
}
