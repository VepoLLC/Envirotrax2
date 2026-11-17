import { CommonModule, formatDate } from '@angular/common';
import { Component, OnInit, TemplateRef, ViewChild } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import {
    CellTemplateData,
    ColumnType,
    DateRange,
    InputOption,
    PagedData,
    QueryProperty,
    RecordLogType,
    recordLogTypeLabels,
    TableColumn,
    TableViewModel
} from '@envirotrax/common-ui';
import { SharedComponentsModule } from '../../shared/components/shared.components.module';
import { RecordLogSearchCriteria, RecordLogSearchResult, RecordLogService } from '../../shared/services/logs/record-log.service';
import { WaterSupplierService } from '../../shared/services/water-suppliers/water-supplier.service';

interface UserLogRow extends RecordLogSearchResult {
    logTypeLabel: string;
}

const LOG_DATE_FIELD = 'logDate';
const DEFAULT_RANGE_DAYS = 30;

@Component({
    templateUrl: './user-log.component.html',
    imports: [
        CommonModule,
        FormsModule,
        SharedComponentsModule
    ],
})
export class UserLogComponent implements OnInit {
    @ViewChild('descriptionCell', { static: true })
    public descriptionCell?: TemplateRef<CellTemplateData<UserLogRow>>;

    public showResults: boolean = false;

    public criteria: RecordLogSearchCriteria = {
        logType: RecordLogType.Edit
    };

    public useDateRange: boolean = false;

    public dateRange: DateRange = this.buildDefaultDateRange();

    public table: TableViewModel<UserLogRow> = {
        query: {
            sort: {},
            filter: []
        }
    };

    public waterSupplierOptions: InputOption[] = [{ id: '', text: 'Any Value' }];

    public readonly logTypeOptions: InputOption[] = [RecordLogType.Edit, RecordLogType.Add, RecordLogType.Delete]
        .map(type => ({ id: type, text: recordLogTypeLabels[type] }));

    public readonly dateSearchOptions: InputOption[] = [
        { id: false, text: 'Any Date' },
        { id: true, text: 'Date Range' }
    ];

    constructor(
        private readonly _recordLogService: RecordLogService,
        private readonly _waterSupplierService: WaterSupplierService
    ) {

    }

    public async ngOnInit(): Promise<void> {
        this.table.columns = this.getColumns();

        await this.loadWaterSuppliers();
    }

    public async search(searchForm: NgForm): Promise<void> {
        if (!searchForm.valid) {
            return;
        }

        this.table.query.filter = this.buildDateFilter();
        this.table.items = undefined;

        await this.getLogs();

        this.showResults = true;
    }

    public async getLogs(): Promise<void> {
        try {
            this.table.isLoading = true;

            const logs = await this._recordLogService.search(
                this.table.items?.pageInfo || {},
                this.table.query,
                this.criteria
            );

            this.table.items = this.toRows(logs);
        } finally {
            this.table.isLoading = false;
        }
    }

    private toRows(logs: PagedData<RecordLogSearchResult>): PagedData<UserLogRow> {
        return {
            ...logs,
            data: logs.data.map(log => ({
                ...log,
                logTypeLabel: log.logType == null ? '' : recordLogTypeLabels[log.logType]
            }))
        };
    }

    private buildDateFilter(): QueryProperty[] {
        if (!this.useDateRange) {
            return [];
        }

        const children: QueryProperty[] = [];

        if (this.dateRange.startDate) {
            children.push({
                columnName: LOG_DATE_FIELD,
                value: this.dateRange.startDate,
                comparisonOperator: 'Gte',
                logicalOperator: 'And'
            });
        }

        if (this.dateRange.endDate) {
            children.push({
                columnName: LOG_DATE_FIELD,
                value: this.dateRange.endDate,
                comparisonOperator: 'Lte',
                logicalOperator: 'And'
            });
        }

        return children.length > 0 ? [{ columnName: LOG_DATE_FIELD, children }] : [];
    }

    private buildDefaultDateRange(): DateRange {
        const today = new Date();
        const start = new Date(today);

        start.setDate(today.getDate() - DEFAULT_RANGE_DAYS);

        return {
            startDate: formatDate(start, 'yyyy-MM-dd', 'en-US'),
            endDate: formatDate(today, 'yyyy-MM-dd', 'en-US')
        };
    }

    private async loadWaterSuppliers(): Promise<void> {
        const options = await this._waterSupplierService.getAllAsOptions();

        this.waterSupplierOptions = [{ id: '', text: 'Any Value' }, ...options];
    }

    private getColumns(): TableColumn<UserLogRow>[] {
        return [
            { field: LOG_DATE_FIELD, caption: 'Log Date', type: ColumnType.dateTime },
            { field: 'waterSupplierId', caption: 'WS ID', type: ColumnType.number },
            { field: 'tableName', caption: 'Table Name', type: ColumnType.text },
            { field: 'recordId', caption: 'Record ID', type: ColumnType.number },
            { field: 'user.email', caption: 'User ID', type: ColumnType.text, queryColumnExcluded: true },
            { field: 'logTypeLabel', caption: 'Type', type: ColumnType.text, queryColumnExcluded: true },
            { field: 'description', caption: 'Description', type: ColumnType.other, cellTemplate: this.descriptionCell, queryColumnExcluded: true }
        ];
    }
}
