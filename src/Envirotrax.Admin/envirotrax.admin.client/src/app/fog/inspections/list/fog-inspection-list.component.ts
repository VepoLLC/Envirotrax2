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
import {
    FogInspection,
    FogInspectionResult,
    FogInspectionRow,
    FogPaymentStatus,
    FogTotalCapacityRange,
    InterceptorCapacityType,
    interceptorCapacityTypeLabels,
    InterceptorType
} from '../../../shared/models/fog/fog-inspection';
import { PropertyType } from '../../../shared/models/sites/site';
import { FogInspectionOptionsService } from '../../../shared/services/fog/fog-inspection-options.service';
import { FogInspectionService } from '../../../shared/services/fog/fog-inspection.service';
import { WaterSupplierService } from '../../../shared/services/water-suppliers/water-supplier.service';

@Component({
    templateUrl: './fog-inspection-list.component.html',
    standalone: false,
})
export class FogInspectionListComponent implements OnInit {
    @ViewChild('statusCell', { static: true })
    public statusCell?: TemplateRef<CellTemplateData<FogInspectionRow>>;

    @ViewChild('generatorCell', { static: true })
    public generatorCell?: TemplateRef<CellTemplateData<FogInspectionRow>>;

    @ViewChild('interceptorCell', { static: true })
    public interceptorCell?: TemplateRef<CellTemplateData<FogInspectionRow>>;

    @ViewChild('inspectorCell', { static: true })
    public inspectorCell?: TemplateRef<CellTemplateData<FogInspectionRow>>;

    @ViewChild('resultCell', { static: true })
    public resultCell?: TemplateRef<CellTemplateData<FogInspectionRow>>;

    public readonly propertyType = PropertyType;

    public readonly inspectionResult = FogInspectionResult;

    public showResults: boolean = false;

    public dateSearchField: string = '';

    public dateRange?: DateRange;

    private panelFilter: QueryProperty[] = [];

    private paymentStatus: FogPaymentStatus | null = null;

    private totalCapacityRange: FogTotalCapacityRange | null = null;

    public table: TableViewModel<FogInspectionRow> = {
        query: {
            sort: {},
            filter: []
        }
    };

    public waterSupplierOptions: InputOption[] = [{ id: '', text: 'Any Value' }];

    public readonly facilityTypeOptions: InputOption[];
    public readonly interceptorTypeOptions: InputOption[];
    public readonly totalCapacityOptions: InputOption[];
    public readonly inspectionResultOptions: InputOption[];
    public readonly paymentStatusOptions: InputOption[];
    public readonly propertyTypeOptions: InputOption[];
    public readonly dateSearchOptions: InputOption[];

    constructor(
        private readonly _fogInspectionService: FogInspectionService,
        private readonly _fogOptions: FogInspectionOptionsService,
        private readonly _waterSupplierService: WaterSupplierService
    ) {
        this.facilityTypeOptions = this._fogOptions.facilityTypeOptions;
        this.interceptorTypeOptions = this._fogOptions.interceptorTypeOptions;
        this.totalCapacityOptions = this._fogOptions.totalCapacityOptions;
        this.inspectionResultOptions = this._fogOptions.inspectionResultOptions;
        this.paymentStatusOptions = this._fogOptions.paymentStatusOptions;
        this.propertyTypeOptions = this._fogOptions.propertyTypeOptions;
        this.dateSearchOptions = this._fogOptions.dateSearchOptions;
    }

    public async ngOnInit(): Promise<void> {
        this.table.columns = this.getColumns();

        await this.loadWaterSuppliers();
    }

    // Total capacity and payment status are their own API parameters rather than column
    // filters, so they are split out of the panel filter before it reaches the query.
    public onFilterChange(queryProperties: QueryProperty[]): void {
        const payment = queryProperties.find(p => p.columnName === 'paymentStatus');
        const capacity = queryProperties.find(p => p.columnName === 'totalCapacityRange');

        this.paymentStatus = payment?.value ? Number(payment.value) as FogPaymentStatus : null;
        this.totalCapacityRange = capacity?.value ? Number(capacity.value) as FogTotalCapacityRange : null;

        this.panelFilter = queryProperties.filter(p =>
            p.columnName !== 'paymentStatus' && p.columnName !== 'totalCapacityRange');

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

            const inspections = await this._fogInspectionService.getAll(
                this.table.items?.pageInfo || {},
                this.table.query,
                this.paymentStatus,
                this.totalCapacityRange
            );

            this.table.items = {
                ...inspections,
                data: inspections.data.map(inspection => this.toRow(inspection))
            };
        } finally {
            this.table.isLoading = false;
        }
    }

    private toRow(inspection: FogInspection): FogInspectionRow {
        return {
            ...inspection,
            propertyAddress: this.buildPropertyAddress(inspection),
            propertyCityStateZip: this.buildPropertyCityStateZip(inspection),
            interceptorDescription: this.buildInterceptorDescription(inspection)
        };
    }

    private buildPropertyAddress(inspection: FogInspection): string {
        const address = `${inspection.propertyStreetNumber ?? ''} ${inspection.propertyStreetName ?? ''}`.trim();

        return inspection.propertyNumber ? `${address} #${inspection.propertyNumber}`.trim() : address;
    }

    private buildPropertyCityStateZip(inspection: FogInspection): string {
        const city = inspection.propertyCity ? `${inspection.propertyCity},` : '';

        return `${city} ${inspection.propertyState?.code ?? ''} ${inspection.propertyZip ?? ''}`.trim();
    }

    private buildInterceptorDescription(inspection: FogInspection): string {
        let description = inspection.interceptorType ?? '';

        if (inspection.interceptorType === InterceptorType.Other && inspection.interceptorOtherDescription) {
            description = `${description} ${inspection.interceptorOtherDescription}`;
        }

        if (inspection.interceptorCapacity) {
            const units = interceptorCapacityTypeLabels[
                inspection.interceptorCapacityType ?? InterceptorCapacityType.Gallons];

            description = `${description} ${inspection.interceptorCapacity} ${units}`;
        }

        return description.trim();
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

    // V1 orders by the date column being searched, ascending, and by inspection date otherwise.
    private buildSort(): { [key: string]: SortOperator } {
        if (this.dateSearchField === 'createdTime') {
            return { createdTime: 'Asc' };
        }

        return { inspectionDate: 'Asc' };
    }

    private async loadWaterSuppliers(): Promise<void> {
        const options = await this._waterSupplierService.getAllAsOptions();

        this.waterSupplierOptions = [{ id: '', text: 'Any Value' }, ...options];
    }

    private getColumns(): TableColumn<FogInspectionRow>[] {
        return [
            { field: 'id', caption: 'ID', type: ColumnType.number },
            { field: '', caption: '', type: ColumnType.other, cellTemplate: this.statusCell },
            { field: 'inspectionDate', caption: 'Inspection Date', type: ColumnType.date },
            { field: '', caption: 'Generator Information', type: ColumnType.other, cellTemplate: this.generatorCell },
            { field: '', caption: 'Interceptor Information', type: ColumnType.other, cellTemplate: this.interceptorCell },
            { field: '', caption: 'Inspector Information', type: ColumnType.other, cellTemplate: this.inspectorCell },
            { field: 'totalCapacityPercent', caption: 'Total Capacity %', type: ColumnType.text },
            { field: 'inspectionResult', caption: 'Inspection Result', type: ColumnType.other, cellTemplate: this.resultCell }
        ];
    }
}
