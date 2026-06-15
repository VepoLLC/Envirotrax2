import { Component, OnInit, TemplateRef, ViewChild } from '@angular/core';
import { ProfessionalFogInspectionService } from '../../../../shared/services/fog/professional-fog-inspection.service';
import { ProfessionalSupplierService } from '../../../../shared/services/professionals/professional-supplier.service';
import { QueryProperty } from '../../../../shared/models/query';
import { TableViewModel } from '../../../../shared/models/table-view-model';
import { FogInspection } from '../../../../shared/models/fog/fog-inspection';

import { FogInspectionResult } from '../../../../shared/models/fog/fog-inspection-enums';
import { FacilityType } from '../../../../shared/enums/facility-type.enum';
import { InterceptorType } from '../../../../shared/enums/interceptor-type.enum';
import { PropertyType } from '../../../../shared/enums/property-type.enum';
import { CellTemplateData, ColumnType, InputOption, TableColumn } from '@envirotrax/common-ui';

@Component({
    standalone: false,
    templateUrl: './professional-fog-inspection-list.component.html'
})
export class ProfessionalFogInspectionListComponent implements OnInit {
    public showResults: boolean = false;
    public latestOnly: boolean = true;
    public searchAttempted: boolean = false;

    public readonly FogInspectionResult = FogInspectionResult;
    public readonly InterceptorType = InterceptorType;
    public readonly PropertyType = PropertyType;

    public table: TableViewModel<FogInspection> = {
        columns: [],
        query: {
            sort: {},
            filter: []
        },
        freeTextSearch: {
            searchQuery: [
                { field: 'propertyBusinessName', operator: 'Ct' },
                { field: 'propertyStreetName', operator: 'Ct' }
            ]
        }
    };

    @ViewChild('iconsCell', { static: true })
    public iconsCell?: TemplateRef<CellTemplateData<FogInspection>>;

    @ViewChild('generatorCell', { static: true })
    public generatorCell?: TemplateRef<CellTemplateData<FogInspection>>;

    @ViewChild('interceptorCell', { static: true })
    public interceptorCell?: TemplateRef<CellTemplateData<FogInspection>>;

    @ViewChild('resultCell', { static: true })
    public resultCell?: TemplateRef<CellTemplateData<FogInspection>>;

    public waterSupplierScopeOptions: InputOption[] = [
        { id: '', text: 'My inspection history only' }
    ];

    public readonly inspectionHistoryOptions: InputOption[] = [
        { id: 'true', text: 'Latest inspection only' },
        { id: 'false', text: 'Complete inspection history' }
    ];

    public readonly inspectionResultOptions: InputOption[] = [
        { id: '', text: 'All results' },
        { id: FogInspectionResult.Passed.toString(), text: 'Passed' },
        { id: FogInspectionResult.Failed.toString(), text: 'Failed' }
    ];

    public readonly interceptorTypeOptions: InputOption[] = [
        { id: '', text: 'Any type' },
        { id: InterceptorType.GreaseTrap, text: 'Grease Trap' },
        { id: InterceptorType.GritTrap, text: 'Grit Trap' },
        { id: InterceptorType.SepticTank, text: 'Septic Tank' },
        { id: InterceptorType.ChemicalToilet, text: 'Chemical Toilet' },
        { id: InterceptorType.Other, text: 'Other' }
    ];

    public readonly facilityTypeOptions: InputOption[] = [
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

    public readonly propertyTypeOptions: InputOption[] = [
        { id: '', text: 'Any value' },
        { id: PropertyType.Residential.toString(), text: 'Residential' },
        { id: PropertyType.Commercial.toString(), text: 'Commercial' }
    ];

    constructor(
        private readonly _fogInspectionService: ProfessionalFogInspectionService,
        private readonly _supplierService: ProfessionalSupplierService
    ) { }

    public async ngOnInit(): Promise<void> {
        this.table.columns = this.getColumns();
        await this.loadWaterSupplierScopeOptions();
    }

    private async loadWaterSupplierScopeOptions(): Promise<void> {
        const suppliers = await this._supplierService.getAllMy(false, false, true);
        const supplierOptions: InputOption[] = suppliers.data
            .filter(s => s.waterSupplier?.id)
            .map(s => ({ id: String(s.waterSupplier!.id!), text: s.waterSupplier!.name ?? '' }));
        this.waterSupplierScopeOptions = [
            { id: '', text: 'My inspection history only' },
            ...supplierOptions
        ];
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
                field: 'site.accountNumber',
                caption: 'Account number',
                type: ColumnType.text
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
                this.table.query,
                this.latestOnly
            );
        } finally {
            this.table.isLoading = false;
        }
    }

    public onLatestOnlyChange(value: string): void {
        this.latestOnly = value === 'true';
    }

    public onFilterChange(queryProperties: QueryProperty[]): void {
        this.table.query.filter = queryProperties;
    }

    public async search(): Promise<void> {
        this.searchAttempted = true;
        await this.getInspections();

        if (this.table.items?.data.length! > 0) {
            this.showResults = true;
        }
    }

    public searchAgain(): void {
        this.showResults = false;
        this.searchAttempted = false;
    }
}
