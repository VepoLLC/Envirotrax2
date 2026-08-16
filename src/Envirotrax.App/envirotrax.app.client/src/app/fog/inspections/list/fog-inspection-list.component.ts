import { Component, OnInit, OnDestroy, TemplateRef, ViewChild } from '@angular/core';
import { Subscription } from 'rxjs';
import { ActivatedRoute, Router } from '@angular/router';
import { NgForm } from '@angular/forms';
import { FogInspectionService } from '../../../shared/services/fog/fog-inspection.service';
import { QueryProperty } from '../../../shared/models/query';
import { TableViewModel } from '../../../shared/models/table-view-model';
import { FogInspection } from '../../../shared/models/fog/fog-inspection';
import { FogInspectionResult } from '../../../shared/models/fog/fog-inspection-enums';
import { FacilityType } from '../../../shared/enums/facility-type.enum';
import { InterceptorType } from '../../../shared/enums/interceptor-type.enum';
import { PropertyType } from '../../../shared/enums/property-type.enum';
import { CellTemplateData, ColumnType, InputOption, TableColumn } from '@envirotrax/common-ui';
import { AppContainerHelperService } from "../../../shared/services/helpers/app-contaner-helper.service";

@Component({
    standalone: false,
    templateUrl: './fog-inspection-list.component.html'
})
export class FogInspectionListComponent implements OnInit, OnDestroy {
    private _queryParamSub?: Subscription;
    private _subAccountWaterSupplierId?: number;
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
        { id: InterceptorType.GreaseTrap, text: 'Grease Trap' },
        { id: InterceptorType.GritTrap, text: 'Grit Trap' },
        { id: InterceptorType.SepticTank, text: 'Septic Tank' },
        { id: InterceptorType.ChemicalToilet, text: 'Chemical Toilet' },
        { id: InterceptorType.Other, text: 'Other' }
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
        { id: PropertyType.Residential.toString(), text: 'Residential' },
        { id: PropertyType.Commercial.toString(), text: 'Commercial' }
    ];

    constructor(
        private readonly _fogInspectionService: FogInspectionService,
        private readonly _router: Router,
        private readonly _activatedRoute: ActivatedRoute,
        private readonly _containerHelper: AppContainerHelperService
    ) { }

    public ngOnInit(): void {
        this.table.columns = this.getColumns();

        this._queryParamSub = this._activatedRoute.queryParamMap.subscribe(async params => {
            const dateParam = params.get('date');
            if (dateParam) {
                this.table.query.filter = [{
                    columnName: 'inspectionDate',
                    children: [
                        { columnName: 'inspectionDate', value: dateParam, comparisonOperator: 'Gte', logicalOperator: 'And' },
                        { columnName: 'inspectionDate', value: dateParam, comparisonOperator: 'Lte', logicalOperator: 'And' }
                    ]
                }];
                await this.getInspections();
                this.setShowResults(true);
                return;
            }

            const subAccountWaterSupplierIdParam = params.get('subAccountWaterSupplierId');
            if (subAccountWaterSupplierIdParam) {
                this._subAccountWaterSupplierId = Number(subAccountWaterSupplierIdParam);
                await this.getInspections();
                this.setShowResults(true);
            }
        });
    }

    public ngOnDestroy(): void {
        this._queryParamSub?.unsubscribe();
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
                this.table.query,
                this._subAccountWaterSupplierId
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

    public setShowResults(visible: boolean): void {
        this.showResults = visible;
        this._containerHelper.setContainerVisibility(!visible);

        // "Search Again" returns to the form - a subsequent manual search should search the water
        // supplier's own inspections again, not stay silently scoped to a dashboard sub account.
        if (!visible) {
            this._subAccountWaterSupplierId = undefined;
        }
    }

    public async search(searchForm: NgForm): Promise<void> {
        if (searchForm.valid) {
            await this.getInspections();
            this.setShowResults(true);
        }
    }

    public viewDetails(inspection: FogInspection): void {
        this._router.navigate([inspection.id], { relativeTo: this._activatedRoute });
    }
}
