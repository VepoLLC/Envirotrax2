import { Component, OnInit, TemplateRef, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { FogInspectionService } from '../../../shared/services/fog/fog-inspection.service';
import { QueryProperty } from '../../../shared/models/query';
import { TableViewModel } from '../../../shared/models/table-view-model';
import { FogInspection } from '../../../shared/models/fog/fog-inspection';
import { CellTemplateData, TableColumn } from '../../../shared/components/data-components/table/table.component';
import { ColumnType } from '../../../shared/components/data-components/sorting-filtering/query-view-model';
import { InputOption } from '../../../shared/components/input/input.component';
import { FogInspectionResult } from '../../../shared/models/fog/fog-inspection-enums';
import { FacilityType } from '../../../shared/enums/facility-type.enum';

@Component({
    standalone: false,
    templateUrl: './fog-inspection-list.component.html'
})
export class FogInspectionListComponent implements OnInit {
    public showResults: boolean = false;

    public readonly FogInspectionResult = FogInspectionResult;

    public table: TableViewModel<FogInspection> = {
        columns: this.getColumns(),
        query: {
            sort: {},
            filter: []
        },
        freeTextSearch: {
            searchQuery: [
                { field: 'propertyBusinessName', operator: 'Ct' },
                { field: 'inspectorCompanyName', operator: 'Ct' },
                { field: 'inspectorContactName', operator: 'Ct' }
            ]
        }
    };

    @ViewChild('iconsCell', { static: true })
    public iconsCell?: TemplateRef<CellTemplateData<FogInspection>>;

    @ViewChild('generatorCell', { static: true })
    public generatorCell?: TemplateRef<CellTemplateData<FogInspection>>;

    @ViewChild('interceptorCell', { static: true })
    public interceptorCell?: TemplateRef<CellTemplateData<FogInspection>>;

    @ViewChild('inspectorCell', { static: true })
    public inspectorCell?: TemplateRef<CellTemplateData<FogInspection>>;

    @ViewChild('resultCell', { static: true })
    public resultCell?: TemplateRef<CellTemplateData<FogInspection>>;

    public inspectionResultOptions: InputOption[] = [
        { id: '', text: 'All Results' },
        { id: FogInspectionResult.Passed.toString(), text: 'Passed' },
        { id: FogInspectionResult.Failed.toString(), text: 'Failed' }
    ];

    public paymentStatusOptions: InputOption[] = [
        { id: '', text: 'Any value' },
        { id: 'paid', text: 'Paid' },
        { id: 'unpaid', text: 'Unpaid' }
    ];

    public interceptorTypeOptions: InputOption[] = [
        { id: '', text: 'Any Type' },
        { id: 'Grease Trap', text: 'Grease Trap' },
        { id: 'Grit Trap', text: 'Grit Trap' },
        { id: 'Septic Tank', text: 'Septic Tank' },
        { id: 'Chemical Toilet', text: 'Chemical Toilet' },
        { id: 'Other', text: 'Other' }
    ];

    public totalCapacityPercentOptions: InputOption[] = [
        { id: '', text: 'Any value' },
        { id: 'lte25', text: '25% or less' },
        { id: 'gt25', text: 'Greater than 25%' }
    ];

    public facilityTypeOptions: InputOption[] = [
        { id: FacilityType.Other.toString(), text: 'Other' },
        { id: FacilityType.Restaurant.toString(), text: 'Restaurant' },
        { id: FacilityType.FastFoodEstablishment.toString(), text: 'Fast food establishment' },
        { id: FacilityType.HotelMotel.toString(), text: 'Hotel/motel' },
        { id: FacilityType.CarWash.toString(), text: 'Car wash' },
        { id: FacilityType.SchoolUniversity.toString(), text: 'School/university' },
        { id: FacilityType.GroceryStore.toString(), text: 'Grocery store' },
        { id: FacilityType.ConvenienceStore.toString(), text: 'Convenience store' },
        { id: FacilityType.AssistedLivingFacility.toString(), text: 'Assisted living facility' },
        { id: FacilityType.MedicalFacility.toString(), text: 'Medical facility' },
        { id: FacilityType.Industrial.toString(), text: 'Industrial' },
        { id: FacilityType.CityOwnedFacility.toString(), text: 'City-owned facility' }
    ];

    public propertyTypeOptions: InputOption[] = [
        { id: '', text: 'Any Value' },
        { id: '0', text: 'Residential' },
        { id: '1', text: 'Commercial' }
    ];

    constructor(
        private readonly _fogInspectionService: FogInspectionService
    ) {}

    public async ngOnInit(): Promise<void> {
        this.table.columns = this.getColumns();
    }

    private getColumns(): TableColumn<FogInspection>[] {
        return [
            {
                field: 'inspectionResult',
                caption: '',
                type: ColumnType.text,
                cellTemplate: this.iconsCell,
                queryColumnExcluded: true
            },
            {
                field: 'inspectionDate',
                caption: 'Inspection date',
                type: ColumnType.date
            },
            {
                field: 'propertyBusinessName',
                caption: 'Generator information',
                type: ColumnType.text,
                cellTemplate: this.generatorCell
            },
            {
                field: 'interceptorType',
                caption: 'Interceptor information',
                type: ColumnType.text,
                cellTemplate: this.interceptorCell
            },
            {
                field: 'inspectorCompanyName',
                caption: 'Inspector information',
                type: ColumnType.text,
                cellTemplate: this.inspectorCell
            },
            {
                field: 'totalCapacityPercent',
                caption: 'Total capacity %',
                type: ColumnType.text
            },
            {
                field: 'inspectionResult',
                caption: 'Inspection result',
                type: ColumnType.text,
                cellTemplate: this.resultCell
            }
        ];
    }

    public async getInspections(): Promise<void> {
        try {
            this.table.isLoading = true;
            this.table.items = await this._fogInspectionService.getAll(
                this.table.items?.pageInfo || {},
                this.table.query
            );
        } finally {
            this.table.isLoading = false;
        }
    }

    public onFilterChange(queryProperties: QueryProperty[]): void {
        this.table.query.filter = queryProperties.map(qp => {
            if (qp.columnName === 'totalCapacityPercent') {
                if (qp.value === 'lte25') return { ...qp, value: '25', comparisonOperator: 'Lte' as const };
                if (qp.value === 'gt25') return { ...qp, value: '25', comparisonOperator: 'Gt' as const };
            }
            if (qp.columnName === 'paymentStatus') {
                return { columnName: 'transactionId', isValueNull: qp.value === 'unpaid' };
            }
            return qp;
        });
    }

    public async search(searchForm: NgForm): Promise<void> {
        if (searchForm.valid) {
            await this.getInspections();
            this.showResults = true;
        }
    }
}
