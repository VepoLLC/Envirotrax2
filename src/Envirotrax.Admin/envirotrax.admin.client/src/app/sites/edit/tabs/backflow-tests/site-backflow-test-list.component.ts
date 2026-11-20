import { CommonModule } from '@angular/common';
import { Component, Input, OnInit, TemplateRef, ViewChild } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CellTemplateData, ColumnType, InputOption, QueryProperty, SortOperator, TableColumn, TableViewModel } from '@envirotrax/common-ui';
import { BackflowTestDetailsComponent } from '../../../../backflow/tests/details/backflow-test-details.component';
import { SharedComponentsModule } from '../../../../shared/components/shared.components.module';
import { BackflowDeviceType, BackflowPaymentStatus, BackflowTest, BackflowTestResult, BackflowTestRow } from '../../../../shared/models/backflow/backflow-test';
import { MODULE_COLORS } from '../../../../shared/models/module-colors';
import { BackflowTestOptionsService } from '../../../../shared/services/backflow/backflow-test-options.service';
import { BackflowTestService } from '../../../../shared/services/backflow/backflow-test.service';
import { WindowService } from '../../../../shared/services/window.service';

const MILLISECONDS_PER_DAY = 24 * 60 * 60 * 1000;

@Component({
    selector: 'app-site-backflow-test-list',
    templateUrl: './site-backflow-test-list.component.html',
    imports: [CommonModule, FormsModule, SharedComponentsModule],
})
export class SiteBackflowTestListComponent implements OnInit {
    @Input()
    public siteId: number = 0;

    @Input()
    public outOfService: boolean = false;

    @ViewChild('statusCell', { static: true })
    public statusCell?: TemplateRef<CellTemplateData<BackflowTestRow>>;

    @ViewChild('datesCell', { static: true })
    public datesCell?: TemplateRef<CellTemplateData<BackflowTestRow>>;

    @ViewChild('serialNumberCell', { static: true })
    public serialNumberCell?: TemplateRef<CellTemplateData<BackflowTestRow>>;

    @ViewChild('assemblyCell', { static: true })
    public assemblyCell?: TemplateRef<CellTemplateData<BackflowTestRow>>;

    public readonly testResult = BackflowTestResult;

    public viewType: string = '';

    public testResultFilter: string = '';

    public serviceStatus: string = '';

    public paymentStatus: string = '';

    public approvalStatus: string = '';

    public rejectedStatus: string = '';

    public serialNumber: string = '';

    public hazardType: string = '';

    public sortBy: string = 'testDateDesc';

    public readonly viewTypeOptions: InputOption[] = [
        { id: 'true', text: 'View current tests only' },
        { id: '', text: 'View complete test history' }
    ];

    public readonly testResultOptions: InputOption[] = [
        { id: '', text: '' },
        { id: 'passing', text: 'All passing tests' },
        { id: 'passedNoRepairs', text: 'Passing with no repairs' },
        { id: 'passedWithRepairs', text: 'Passing with repairs' },
        { id: 'failed', text: 'Failed tests' }
    ];

    public readonly serviceStatusOptions: InputOption[] = [
        { id: '', text: '' },
        { id: 'false', text: 'In service' },
        { id: 'true', text: 'Out of service' }
    ];

    public readonly paymentStatusOptions: InputOption[] = [
        { id: '', text: '' },
        { id: String(BackflowPaymentStatus.Paid), text: 'Paid' },
        { id: String(BackflowPaymentStatus.Unpaid), text: 'Unpaid' }
    ];

    public readonly approvalStatusOptions: InputOption[] = [
        { id: '', text: '' },
        { id: 'false', text: 'Approved' },
        { id: 'true', text: 'Disapproved' }
    ];

    public readonly rejectedStatusOptions: InputOption[] = [
        { id: '', text: '' },
        { id: 'false', text: 'Accepted' },
        { id: 'true', text: 'Rejected' }
    ];

    public readonly sortByOptions: InputOption[] = [
        { id: 'testDateAsc', text: 'TestDate ASC' },
        { id: 'testDateDesc', text: 'TestDate DESC' },
        { id: 'serialNumberAsc', text: 'SerialNumber ASC' },
        { id: 'serialNumberDesc', text: 'SerialNumber DESC' }
    ];

    public readonly hazardTypeOptions: InputOption[];

    public table: TableViewModel<BackflowTestRow> = {
        query: {
            sort: {},
            filter: []
        }
    };

    private appliedFilter: QueryProperty[] = [];

    private appliedPaymentStatus: BackflowPaymentStatus | null = null;

    constructor(
        private readonly _backflowTestService: BackflowTestService,
        private readonly _windowService: WindowService,
        private readonly _options: BackflowTestOptionsService
    ) {
        this.hazardTypeOptions = this._options.hazardTypeOptions;
    }

    public async ngOnInit(): Promise<void> {
        this.table.columns = this.getColumns();

        await this.load();
    }

    public async load(): Promise<void> {
        this.appliedFilter = this.buildFilter();
        this.appliedPaymentStatus = this.buildPaymentStatus();
        this.table.query.sort = this.buildSort();

        if (this.table.items) {
            this.table.items.pageInfo.pageNumber = 1;
        }

        await this.getTests();
    }

    public async getTests(): Promise<void> {
        try {
            this.table.isLoading = true;
            this.table.query.filter = this.appliedFilter;

            const tests = await this._backflowTestService.getAll(this.table.items?.pageInfo || {}, this.table.query, this.appliedPaymentStatus);

            const rows: BackflowTestRow[] = [];

            for (const test of tests.data) {
                rows.push(this.toRow(test));
            }

            this.table.items = {
                pageInfo: tests.pageInfo,
                data: rows
            };
        } finally {
            this.table.isLoading = false;
        }
    }

    public viewDetails(test: BackflowTestRow): void {
        this._windowService.addWindow(BackflowTestDetailsComponent, {
            title: this.buildWindowTitle(test),
            color: MODULE_COLORS.BACKFLOW,
            model: { id: test.id }
        });
    }

    private buildWindowTitle(test: BackflowTest): string {
        let address = `${test.propertyStreetNumber ?? ''} ${test.propertyStreetName ?? ''}`.trim();

        if (test.propertyNumber) {
            address = `${address} #${test.propertyNumber}`.trim();
        }

        return address ? `${test.id} - ${address}` : String(test.id);
    }

    private buildFilter(): QueryProperty[] {
        const filter: QueryProperty[] = [
            { columnName: 'site.id', value: String(this.siteId), comparisonOperator: 'Eq' }
        ];

        if (this.outOfService) {
            filter.push({ columnName: 'outOfService', value: 'true', comparisonOperator: 'Eq' });

            return filter;
        }

        if (this.viewType) {
            filter.push({ columnName: 'isCurrent', value: this.viewType, comparisonOperator: 'Eq' });
        }

        this.addTestResultFilter(filter);

        if (this.serviceStatus) {
            filter.push({ columnName: 'outOfService', value: this.serviceStatus, comparisonOperator: 'Eq' });
        }

        if (this.approvalStatus) {
            filter.push({ columnName: 'disapproved', value: this.approvalStatus, comparisonOperator: 'Eq' });
        }

        if (this.rejectedStatus) {
            filter.push({ columnName: 'rejected', value: this.rejectedStatus, comparisonOperator: 'Eq' });
        }

        this.addSerialNumberFilter(filter);

        if (this.hazardType) {
            filter.push({ columnName: 'hazardType', value: this.hazardType, comparisonOperator: 'Eq' });
        }

        return filter;
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

        let result = BackflowTestResult.Fail;

        if (this.testResultFilter === 'passedNoRepairs') {
            result = BackflowTestResult.Pass;
        }

        if (this.testResultFilter === 'passedWithRepairs') {
            result = BackflowTestResult.PassAfterRepairs;
        }

        filter.push({ columnName: 'testResult', value: String(result), comparisonOperator: 'Eq' });
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

    private buildPaymentStatus(): BackflowPaymentStatus | null {
        if (this.outOfService) {
            return BackflowPaymentStatus.Paid;
        }

        if (!this.paymentStatus) {
            return null;
        }

        return Number(this.paymentStatus) as BackflowPaymentStatus;
    }

    private buildSort(): { [key: string]: SortOperator } {
        if (this.outOfService) {
            return { testDate: 'Desc' };
        }

        switch (this.sortBy) {
            case 'testDateAsc':
                return { testDate: 'Asc' };
            case 'serialNumberAsc':
                return { serialNumber: 'Asc' };
            case 'serialNumberDesc':
                return { serialNumber: 'Desc' };
            default:
                return { testDate: 'Desc' };
        }
    }

    private toRow(test: BackflowTest): BackflowTestRow {
        const row: BackflowTestRow = {
            ...test,
            assemblyDescription: `${test.manufacturer ?? ''} ${test.model ?? ''} ${test.size ?? ''}`.trim(),
            bypassAssemblyDescription: '',
            expirationColor: '#c0c0c0',
            expirationTextClass: 'text-white'
        };

        if (this.hasBypassAssembly(test.deviceType)) {
            row.bypassAssemblyDescription = `${test.manufacturer2 ?? ''} ${test.model2 ?? ''} ${test.size2 ?? ''}`.trim();
        }

        if (test.renewalRequired && test.expirationDate) {
            this.setExpiration(row, test.expirationDate);
        }

        return row;
    }

    private hasBypassAssembly(deviceType?: string): boolean {
        return deviceType === BackflowDeviceType.DCD
            || deviceType === BackflowDeviceType.DCD2
            || deviceType === BackflowDeviceType.RPPD
            || deviceType === BackflowDeviceType.RPPD2;
    }

    private setExpiration(row: BackflowTestRow, expirationDate: string): void {
        const expiration = new Date(expirationDate);
        const daysExpired = (Date.now() - expiration.getTime()) / MILLISECONDS_PER_DAY;

        if (daysExpired <= 0) {
            row.expirationColor = '#50e550';
            row.expirationTextClass = 'text-black';
        } else if (daysExpired <= 30) {
            row.expirationColor = '#ffff00';
            row.expirationTextClass = 'text-black';
        } else if (daysExpired <= 60) {
            row.expirationColor = '#ff8000';
            row.expirationTextClass = 'text-white';
        } else {
            row.expirationColor = '#ff0000';
            row.expirationTextClass = 'text-white';
        }
    }

    private getColumns(): TableColumn<BackflowTestRow>[] {
        return [
            { field: 'id', caption: 'ID', type: ColumnType.number },
            { field: 'isCurrent', caption: 'Status', type: ColumnType.other, cellTemplate: this.statusCell, queryColumnExcluded: true },
            { field: 'testDate', caption: 'Dates', type: ColumnType.other, cellTemplate: this.datesCell, queryColumnExcluded: true },
            { field: 'serialNumber', caption: 'Serial #', type: ColumnType.other, cellTemplate: this.serialNumberCell, queryColumnExcluded: true },
            { field: 'deviceType', caption: 'Type', type: ColumnType.text },
            { field: 'manufacturer', caption: 'Assembly Information', type: ColumnType.other, cellTemplate: this.assemblyCell, queryColumnExcluded: true }
        ];
    }
}
