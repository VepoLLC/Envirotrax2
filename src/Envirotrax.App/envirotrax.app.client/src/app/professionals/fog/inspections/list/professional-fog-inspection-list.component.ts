import { Component, ElementRef, OnInit, TemplateRef, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ProfessionalFogInspectionService } from '../../../../shared/services/fog/professional-fog-inspection.service';
import { ProfessionalSupplierService } from '../../../../shared/services/professionals/professional-supplier.service';
import { FogInspectionOptionsService } from '../../../../shared/services/fog/fog-inspection-options.service';
import { QueryProperty } from '../../../../shared/models/query';
import { TableViewModel } from '../../../../shared/models/table-view-model';
import { FogInspection } from '../../../../shared/models/fog/fog-inspection';

import { FogInspectionResult } from '../../../../shared/models/fog/fog-inspection-enums';
import { InterceptorType } from '../../../../shared/enums/interceptor-type.enum';
import { PropertyType } from '../../../../shared/enums/property-type.enum';
import { CellTemplateData, ColumnType, InputOption, TableColumn } from '@envirotrax/common-ui';
import { AppContainerHelperService } from '../../../../shared/services/helpers/app-contaner-helper.service';
import { PrintableTableService } from '../../../../shared/services/printable-table.service';
import { DownloadConfig } from '../../../../shared/models/download-config';
import { DownloadService } from '../../../../shared/services/download.service';

type DownloadCategory = 'property' | 'interceptor' | 'inspector' | 'mailing';

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

    @ViewChild('printableSection')
    private _printableSection!: ElementRef;

    public waterSupplierScopeOptions: InputOption[] = [
        { id: '', text: 'My inspection history only' }
    ];

    public readonly inspectionHistoryOptions: InputOption[] = [
        { id: 'true', text: 'Latest inspection only' },
        { id: 'false', text: 'Complete inspection history' }
    ];

    public readonly inspectionResultOptions: InputOption[];
    public readonly interceptorTypeOptions: InputOption[];
    public readonly facilityTypeOptions: InputOption[];
    public readonly propertyTypeOptions: InputOption[];

    public downloadConfig: DownloadConfig<DownloadCategory>;

    constructor(
        private readonly _fogInspectionService: ProfessionalFogInspectionService,
        private readonly _supplierService: ProfessionalSupplierService,
        private readonly _fogOptions: FogInspectionOptionsService,
        private readonly _router: Router,
        private readonly _activatedRoute: ActivatedRoute,
        private readonly _containerHelper: AppContainerHelperService,
        private readonly _printService: PrintableTableService,
        private readonly _downloadService: DownloadService
    ) {
        this.inspectionResultOptions = this._fogOptions.inspectionResultFilterOptions;
        this.interceptorTypeOptions = this._fogOptions.interceptorTypeFilterOptions;
        this.facilityTypeOptions = this._fogOptions.facilityTypeFilterOptions;
        this.propertyTypeOptions = this._fogOptions.propertyTypeFilterOptions;

        this.downloadConfig = {
            fileName: 'FOG Inspections',
            endpoint: this._fogInspectionService.getAllEndpoint(this.latestOnly),
            pdfEndpoint: this._fogInspectionService.getAllPdfEndpoint(this.latestOnly),
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
                // Property
                { field: 'propertyBusinessName', caption: 'PropertyBusinessName', category: 'property' },
                { field: 'propertyStreetNumber', caption: 'PropertyStreetNumber', category: 'property' },
                { field: 'propertyStreetName', caption: 'PropertyStreetName', category: 'property' },
                { field: 'propertyNumber', caption: 'PropertyNumber', category: 'property' },
                { field: 'propertyCity', caption: 'PropertyCity', category: 'property' },
                { field: 'propertyState.code', caption: 'PropertyState', category: 'property' },
                { field: 'propertyZip', caption: 'PropertyZip', category: 'property' },
                // Interceptor
                { field: 'interceptorType', caption: 'InterceptorType', category: 'interceptor' },
                { field: 'interceptorCapacity', caption: 'InterceptorCapacity', category: 'interceptor' },
                { field: 'interceptorLocationDescription', caption: 'InterceptorLocationDescription', category: 'interceptor' },
                { field: 'totalCapacityPercent', caption: 'TotalCapacityPercent', category: 'interceptor' },
                // Inspector
                { field: 'inspectorCompanyName', caption: 'InspectorCompanyName', category: 'inspector' },
                { field: 'inspectorContactName', caption: 'InspectorContactName', category: 'inspector' },
                { field: 'inspectorAddress', caption: 'InspectorAddress', category: 'inspector' },
                { field: 'inspectorCity', caption: 'InspectorCity', category: 'inspector' },
                { field: 'inspectorState', caption: 'InspectorState', category: 'inspector' },
                { field: 'inspectorZip', caption: 'InspectorZip', category: 'inspector' },
                // Mailing
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

    public showDownloadManager(): void {
        this.downloadConfig.endpoint = this._fogInspectionService.getAllEndpoint(this.latestOnly);
        this.downloadConfig.pdfEndpoint = this._fogInspectionService.getAllPdfEndpoint(this.latestOnly);
        this._downloadService.showDownloadManager(this.downloadConfig, this.table.query);
    }

    public async ngOnInit(): Promise<void> {
        this.table.columns = this.getColumns();
        await this.loadWaterSupplierScopeOptions();
    }

    private async loadWaterSupplierScopeOptions(): Promise<void> {
        const suppliers = await this._supplierService.getAllMy({
            hasFogInspection: true
        });

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

    public setShowResults(visible: boolean): void {
        this.showResults = visible;
        this._containerHelper.setContainerVisibility(!visible);
    }

    public async search(): Promise<void> {
        this.searchAttempted = true;
        await this.getInspections();

        if (this.table.items?.data.length! > 0) {
            this.setShowResults(true);
        }
    }

    public viewInspection(inspection: FogInspection): void {
        const url = this._router.serializeUrl(
            this._router.createUrlTree([inspection.id], { relativeTo: this._activatedRoute })
        );
        window.open(url, '_blank');
    }

    public viewPrintableTable(): void {
        this._printService.open(this._printableSection.nativeElement);
    }
}
