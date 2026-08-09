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
import { CsiInspection, CsiPaymentStatus } from '../../../shared/models/csi/csi-inspection';
import { PropertyType } from '../../../shared/models/sites/site';
import { CsiInspectionService } from '../../../shared/services/csi/csi-inspection.service';
import { WaterSupplierService } from '../../../shared/services/water-suppliers/water-supplier.service';
import { WindowService } from '../../../shared/services/window.service';
import { CsiInspectionDetailsComponent } from '../details/csi-inspection-details.component';

@Component({
    templateUrl: './csi-inspection-list.component.html',
    standalone: false,
})
export class CsiInspectionListComponent implements OnInit {
    @ViewChild('statusCell', { static: true })
    public statusCell?: TemplateRef<CellTemplateData<CsiInspection>>;

    @ViewChild('propertyCell', { static: true })
    public propertyCell?: TemplateRef<CellTemplateData<CsiInspection>>;

    @ViewChild('inspectorCell', { static: true })
    public inspectorCell?: TemplateRef<CellTemplateData<CsiInspection>>;

    public readonly propertyType = PropertyType;

    public showResults: boolean = false;

    public dateSearchField: string = '';

    public dateRange?: DateRange;

    private panelFilter: QueryProperty[] = [];

    private paymentStatus: CsiPaymentStatus | null = null;

    public table: TableViewModel<CsiInspection> = {
        query: {
            sort: {},
            filter: []
        }
    };

    public waterSupplierOptions: InputOption[] = [{ id: '', text: 'Any Value' }];

    public readonly dateSearchOptions: InputOption[] = [
        { id: '', text: 'None' },
        { id: 'inspectionDate', text: 'Inspection Date' },
        { id: 'transactionDate', text: 'Submission Date' }
    ];

    public readonly passFailOptions: InputOption[] = [
        { id: '', text: 'Any Value' },
        { id: 'true', text: 'Pass' },
        { id: 'false', text: 'Fail' }
    ];

    public readonly paymentStatusOptions: InputOption[] = [
        { id: '', text: 'Any Value' },
        { id: String(CsiPaymentStatus.Paid), text: 'Paid' },
        { id: String(CsiPaymentStatus.Unpaid), text: 'Unpaid' }
    ];

    public readonly propertyTypes: InputOption[] = [
        { id: '', text: 'Any Value' },
        { id: '0', text: 'Residential' },
        { id: '1', text: 'Commercial' }
    ];

    constructor(
        private readonly _csiInspectionService: CsiInspectionService,
        private readonly _waterSupplierService: WaterSupplierService,
        private readonly _windowService: WindowService
    ) {

    }

    public openDetails(inspection: CsiInspection): void {
        this._windowService.addWindow(CsiInspectionDetailsComponent, {
            title: `${inspection.id} - ${this.buildPropertyAddress(inspection)}`,
            model: inspection
        });
    }

    private buildPropertyAddress(inspection: CsiInspection): string {
        let address = `${inspection.propertyStreetNumber ?? ''} ${inspection.propertyStreetName ?? ''}`.trim();

        if (inspection.propertyNumber) {
            address = `${address} #${inspection.propertyNumber}`;
        }

        return address;
    }

    public async ngOnInit(): Promise<void> {
        this.table.columns = this.getColumns();

        await this.loadWaterSuppliers();
    }

    public onFilterChange(queryProperties: QueryProperty[]): void {
        const payment = queryProperties.find(p => p.columnName === 'paymentStatus');

        this.paymentStatus = payment?.value ? Number(payment.value) as CsiPaymentStatus : null;

        this.panelFilter = queryProperties.filter(p => p.columnName !== 'paymentStatus');

        this.table.query.filter = this.buildFilter();
    }

    public async search(searchForm: NgForm): Promise<void> {
        if (!searchForm.valid) {
            return;
        }

        this.table.query.filter = this.buildFilter();
        this.table.query.sort = this.buildSort();

        await this.getInspections();

        this.showResults = true;
    }

    public async getInspections(): Promise<void> {
        try {
            this.table.isLoading = true;
            this.table.items = await this._csiInspectionService.getAll(
                this.table.items?.pageInfo || {},
                this.table.query,
                this.paymentStatus
            );
        } finally {
            this.table.isLoading = false;
        }
    }

    private buildFilter(): QueryProperty[] {
        const filter = [...this.panelFilter];

        if (!this.dateSearchField) {
            return filter;
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

        return filter;
    }

    private buildSort(): { [key: string]: SortOperator } {
        if (this.dateSearchField === 'inspectionDate') {
            return { inspectionDate: 'Asc' };
        }

        if (this.dateSearchField === 'transactionDate') {
            return { transactionDate: 'Desc' };
        }

        return { createdTime: 'Desc' };
    }

    private async loadWaterSuppliers(): Promise<void> {
        const options = await this._waterSupplierService.getAllAsOptions();

        this.waterSupplierOptions = [{ id: '', text: 'Any Value' }, ...options];
    }

    private getColumns(): TableColumn<CsiInspection>[] {
        return [
            { field: 'id', caption: 'ID', type: ColumnType.number },
            { field: 'inspectionResult', caption: '', type: ColumnType.other, cellTemplate: this.statusCell, queryColumnExcluded: true },
            { field: 'inspectionDate', caption: 'Inspection Date', type: ColumnType.date },
            { field: 'propertyBusinessName', caption: 'Property Information', type: ColumnType.other, cellTemplate: this.propertyCell, queryColumnExcluded: true },
            { field: 'inspectorCompanyName', caption: 'Inspector Information', type: ColumnType.other, cellTemplate: this.inspectorCell, queryColumnExcluded: true }
        ];
    }
}
