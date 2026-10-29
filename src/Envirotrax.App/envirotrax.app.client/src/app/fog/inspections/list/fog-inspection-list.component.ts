import { Component, ElementRef, OnInit, OnDestroy, TemplateRef, ViewChild } from '@angular/core';
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
import { PrintableTableService } from '../../../shared/services/printable-table.service';
import { DownloadConfig } from '../../../shared/models/download-config';
import { DownloadService } from '../../../shared/services/download.service';

type DownloadCategory = 'property' | 'interceptor' | 'inspector' | 'mailing';

@Component({
    standalone: false,
    templateUrl: './fog-inspection-list.component.html'
})
export class FogInspectionListComponent implements OnInit, OnDestroy {
    private _queryParamSub?: Subscription;
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

    @ViewChild('printableSection')
    private _printableSection!: ElementRef;

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

    public downloadConfig: DownloadConfig<DownloadCategory>;

    constructor(
        private readonly _fogInspectionService: FogInspectionService,
        private readonly _router: Router,
        private readonly _activatedRoute: ActivatedRoute,
        private readonly _containerHelper: AppContainerHelperService,
        private readonly _printService: PrintableTableService,
        private readonly _downloadService: DownloadService
    ) {
        this.downloadConfig = {
            fileName: 'FOG Inspections',
            endpoint: this._fogInspectionService.getAllEndpoint(),
            pdfEndpoint: this._fogInspectionService.getAllPdfEndpoint(),
            openPdfInNewWindow: true,
            suppoertedFormats: ['CSV', 'Excel', 'PDF'],
            categories: [
                { name: 'property', caption: 'Property Information', isSelected: true },
                { name: 'interceptor', caption: 'Interceptor Information', isSelected: true },
                { name: 'inspector', caption: 'Inspector Information', isSelected: true },
                { name: 'mailing', caption: 'Mailing Information', isSelected: true }
            ],
            columns: [
                { field: 'inspectionDate', caption: 'InspectionDate' },
                { field: 'site.accountNumber', caption: 'AccountNumber' },
                { field: 'inspectionResult', caption: 'InspectionResult' },
                { field: 'propertyBusinessName', caption: 'PropertyBusinessName', category: 'property' },
                { field: 'propertyStreetNumber', caption: 'PropertyStreetNumber', category: 'property' },
                { field: 'propertyStreetName', caption: 'PropertyStreetName', category: 'property' },
                { field: 'propertyNumber', caption: 'PropertyNumber', category: 'property' },
                { field: 'propertyCity', caption: 'PropertyCity', category: 'property' },
                { field: 'propertyState.code', caption: 'PropertyState', category: 'property' },
                { field: 'propertyZip', caption: 'PropertyZip', category: 'property' },
                { field: 'interceptorType', caption: 'InterceptorType', category: 'interceptor' },
                { field: 'interceptorCapacity', caption: 'InterceptorCapacity', category: 'interceptor' },
                { field: 'interceptorLocationDescription', caption: 'InterceptorLocationDescription', category: 'interceptor' },
                { field: 'totalCapacityPercent', caption: 'TotalCapacityPercent', category: 'interceptor' },
                { field: 'inspectorCompanyName', caption: 'InspectorCompanyName', category: 'inspector' },
                { field: 'inspectorContactName', caption: 'InspectorContactName', category: 'inspector' },
                { field: 'inspectorAddress', caption: 'InspectorAddress', category: 'inspector' },
                { field: 'inspectorCity', caption: 'InspectorCity', category: 'inspector' },
                { field: 'inspectorState', caption: 'InspectorState', category: 'inspector' },
                { field: 'inspectorZip', caption: 'InspectorZip', category: 'inspector' },
                { field: 'mailingCompanyName', caption: 'MailingCompanyName', category: 'mailing' },
                { field: 'mailingContactName', caption: 'MailingContactName', category: 'mailing' },
                { field: 'mailingStreetNumber', caption: 'MailingStreetNumber', category: 'mailing' },
                { field: 'mailingStreetName', caption: 'MailingStreetName', category: 'mailing' },
                { field: 'mailingNumber', caption: 'MailingNumber', category: 'mailing' },
                { field: 'mailingCity', caption: 'MailingCity', category: 'mailing' },
                { field: 'mailingState.code', caption: 'MailingState', category: 'mailing' },
                { field: 'mailingZip', caption: 'MailingZip', category: 'mailing' },
                { field: 'mailingPhoneNumber', caption: 'MailingPhoneNumber', category: 'mailing' },
                { field: 'mailingEmailAddress', caption: 'MailingEmailAddress', category: 'mailing' }
            ]
        };
    }

    public ngOnInit(): void {
        this.table.columns = this.getColumns();

        this._queryParamSub = this._activatedRoute.queryParamMap.subscribe(async params => {
            const dateParam = params.get('date');
            if (dateParam) {
                this.applyDateFilter(dateParam);
                await this.getInspections();
                this.setShowResults(true);
                return;
            }

            // Dashboard "View" on a sub account lands here already authenticated as that water
            // supplier (via /auth/login-redirect); this just carries over the same last-10-days
            // window shown on the dashboard so the results match what was clicked.
            const startDateParam = params.get('startDate');
            const endDateParam = params.get('endDate');
            if (startDateParam && endDateParam) {
                this.applyDateFilter(startDateParam, endDateParam);
                await this.getInspections();
                this.setShowResults(true);
            }
        });
    }

    private applyDateFilter(startDate: string, endDate: string = startDate): void {
        this.table.query.filter = [{
            columnName: 'createdTime',
            children: [
                { columnName: 'createdTime', value: startDate, comparisonOperator: 'Gte', logicalOperator: 'And' },
                { columnName: 'createdTime', value: endDate, comparisonOperator: 'Lte', logicalOperator: 'And' }
            ]
        }];
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
                caption: 'Inspection Date',
                type: ColumnType.date
            },
            {
                field: 'propertyBusinessName',
                caption: 'Generator Information',
                type: ColumnType.text,
                cellTemplate: this.generatorCell
            },
            {
                field: 'interceptorType',
                caption: 'Interceptor Information',
                type: ColumnType.text,
                cellTemplate: this.interceptorCell
            },
            {
                field: 'inspectorCompanyName',
                caption: 'Inspector Information',
                type: ColumnType.text,
                cellTemplate: this.inspectorCell
            },
            {
                field: 'totalCapacityPercent',
                caption: 'Total Capacity %',
                type: ColumnType.text
            },
            {
                field: 'inspectionResult',
                caption: 'Inspection Result',
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

    public setShowResults(visible: boolean): void {
        this.showResults = visible;
        this._containerHelper.setContainerVisibility(!visible);
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

    public showDownloadManager(): void {
        this._downloadService.showDownloadManager(this.downloadConfig, this.table.query);
    }

    public async viewPrintableTable(): Promise<void> {
        await this._printService.open(this.table, () => this.getInspections(), () => this._printableSection.nativeElement);
    }
}
