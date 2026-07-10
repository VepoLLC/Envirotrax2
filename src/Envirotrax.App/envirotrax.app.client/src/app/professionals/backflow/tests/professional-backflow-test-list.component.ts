import { Component, ElementRef, OnDestroy, OnInit, ViewChild, TemplateRef } from '@angular/core';
import { NgForm } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { Subscription } from 'rxjs';
import { BackflowTestService, getBackflowExpiryRange, BackflowExpiryRangeKey } from '../../../shared/services/backflow/backflow-test.service';
import { ProfessionalSupplierService } from '../../../shared/services/professionals/professional-supplier.service';
import { QueryProperty } from '../../../shared/models/query';
import { TableViewModel } from '../../../shared/models/table-view-model';
import { BackflowTest } from '../../../shared/models/backflow/backflow-test';
import { CellTemplateData, ColumnType, InputOption } from '@envirotrax/common-ui';
import { BackflowTestResult } from '../../../shared/models/backflow/backflow-test-enums';
import { DownloadConfig } from '../../../shared/models/download-config';
import { DownloadService } from '../../../shared/services/download.service';
import { PrintableTableService } from '../../../shared/services/printable-table.service';
import { PropertyType } from '../../../shared/enums/property-type.enum';
import { AppContainerHelperService } from '../../../shared/services/helpers/app-contaner-helper.service';

@Component({
    standalone: false,
    templateUrl: './professional-backflow-test-list.component.html'
})
export class ProfessionalBackflowTestListComponent implements OnInit, OnDestroy {
    private _routeSub?: Subscription;

    @ViewChild('statusTemplate', { static: true })
    public statusTemplate!: TemplateRef<CellTemplateData<BackflowTest>>;

    @ViewChild('datesTemplate', { static: true })
    public datesTemplate!: TemplateRef<CellTemplateData<BackflowTest>>;

    @ViewChild('assemblyTemplate', { static: true })
    public assemblyTemplate!: TemplateRef<CellTemplateData<BackflowTest>>;

    @ViewChild('propertyTemplate', { static: true })
    public propertyTemplate!: TemplateRef<CellTemplateData<BackflowTest>>;

    @ViewChild('mailingTemplate', { static: true })
    public mailingTemplate!: TemplateRef<CellTemplateData<BackflowTest>>;

    @ViewChild('printableSection')
    private _printableSection!: ElementRef;

    public readonly BackflowTestResult = BackflowTestResult;

    public showResults: boolean = false;

    public downloadConfig: DownloadConfig<'property' | 'mailing' | 'assembly' | 'testResults'>;

    public table: TableViewModel<BackflowTest> = {
        columns: [],
        query: {
            sort: {},
            filter: []
        },
        freeTextSearch: {
            searchQuery: [
                { field: 'accountNumber', operator: 'Ct' },
                { field: 'serialNumber', operator: 'Ct' }
            ]
        }
    };

    public waterSupplierScopeOptions: InputOption[] = [
        { id: '', text: 'My test history only' }
    ];

    public testHistoryOptions: InputOption[] = [
        { id: 'true', text: 'Latest test only' },
        { id: 'false', text: 'Complete test history' }
    ];

    public propertyTypeOptions: InputOption[] = [
        { id: '', text: 'Any value' },
        { id: PropertyType.Residential.toString(), text: 'Residential' },
        { id: PropertyType.Commercial.toString(), text: 'Commercial' }
    ];

    constructor(
        private readonly _backflowTestService: BackflowTestService,
        private readonly _supplierService: ProfessionalSupplierService,
        private readonly _router: Router,
        private readonly _activatedRoute: ActivatedRoute,
        private readonly _downloadService: DownloadService,
        private readonly _printService: PrintableTableService,
        private readonly _containerHelper: AppContainerHelperService
    ) {
        this.downloadConfig = {
            fileName: 'Backflow Tests',
            endpoint: this._backflowTestService.getAllForProfessionalEndpoint(),
            pdfEndpoint: this._backflowTestService.getAllForProfessionalPdfEndpoint(),
            suppoertedFormats: ['CSV', 'Excel', 'PDF'],
            categories: [
                { name: 'property', caption: 'Property Information', isSelected: true },
                { name: 'mailing', caption: 'Mailing Information', isSelected: true },
                { name: 'assembly', caption: 'Assembly Information', isSelected: true },
                { name: 'testResults', caption: 'Test Results', isSelected: true }
            ],
            columns: [
                { field: 'testDate', caption: 'TestDate' },
                { field: 'expirationDate', caption: 'ExpirationDate' },
                { field: 'accountNumber', caption: 'AccountNumber' },
                { field: 'serialNumber', caption: 'SerialNumber' },
                // Assembly
                { field: 'manufacturer', caption: 'Manufacturer', category: 'assembly' },
                { field: 'model', caption: 'Model', category: 'assembly' },
                { field: 'size', caption: 'Size', category: 'assembly' },
                { field: 'deviceType', caption: 'DeviceType', category: 'assembly' },
                { field: 'hazardType', caption: 'HazardType', category: 'assembly' },
                { field: 'locationDescription', caption: 'LocationDescription', category: 'assembly' },
                // Property
                { field: 'propertyBusinessName', caption: 'PropertyBusinessName', category: 'property' },
                { field: 'propertyStreetNumber', caption: 'PropertyStreetNumber', category: 'property' },
                { field: 'propertyStreetName', caption: 'PropertyStreetName', category: 'property' },
                { field: 'propertyNumber', caption: 'PropertyNumber', category: 'property' },
                { field: 'propertyCity', caption: 'PropertyCity', category: 'property' },
                { field: 'propertyState.code', caption: 'PropertyState', category: 'property' },
                { field: 'propertyZip', caption: 'PropertyZip', category: 'property' },
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
                { field: 'mailingEmailAddress', caption: 'MailingEmailAddress', category: 'mailing' },
                // Test Results
                { field: 'testResult', caption: 'TestResult', category: 'testResults' },
                { field: 'reasonForTest', caption: 'ReasonForTest', category: 'testResults' },
                { field: 'properlyInstalled', caption: 'ProperlyInstalled', category: 'testResults' }
            ]
        };
    }

    public showDownloadManager(): void {
        this._downloadService.showDownloadManager(this.downloadConfig, this.table.query);
    }

    public viewPrintableTable(): void {
        this._printService.open(this._printableSection.nativeElement);
    }

    public viewDetails(test: BackflowTest): void {
        if (test?.id == null) {
            return;
        }
        const url = this._router.serializeUrl(
            this._router.createUrlTree([test.id, 'view'], { relativeTo: this._activatedRoute })
        );
        window.open(url, '_blank');
    }

    public async ngOnInit(): Promise<void> {
        this.setupColumns();
        await this.loadWaterSupplierScopeOptions();

        this._routeSub = this._activatedRoute.queryParamMap.subscribe(async params => {
            const expiring = params.get('expiring') as BackflowExpiryRangeKey | null;
            if (expiring === 'expired' || expiring === 'thismonth' || expiring === 'nextmonth' || expiring === 'twomonths') {
                this.applyExpiringFilter(expiring);
                await this.getTests();
                this.setShowResults(true);
            }
        });
    }

    public ngOnDestroy(): void {
        this._routeSub?.unsubscribe();
    }

    private applyExpiringFilter(key: BackflowExpiryRangeKey): void {
        const { start, end } = getBackflowExpiryRange(key);
        this.table.query.filter = [
            { columnName: 'isCurrent', value: 'true', comparisonOperator: 'Eq' },
            { columnName: 'expirationDate', value: start.toISOString(), comparisonOperator: 'Gte' },
            { columnName: 'expirationDate', value: end.toISOString(), comparisonOperator: 'Lte' }
        ];
    }

    private async loadWaterSupplierScopeOptions(): Promise<void> {
        const suppliers = await this._supplierService.getAllMy(false, true);
        const supplierOptions: InputOption[] = suppliers.data
            .filter(s => s.waterSupplier?.id)
            .map(s => ({ id: String(s.waterSupplier!.id!), text: s.waterSupplier!.name ?? '' }));
        this.waterSupplierScopeOptions = [
            { id: '', text: 'My test history only' },
            ...supplierOptions
        ];
    }

    public isExpired(date?: string): boolean {
        if (!date) {
            return false;
        }

        return new Date(date) < new Date();
    }

    private setupColumns(): void {
        this.table.columns = [
            {
                field: '_rowNumber',
                caption: '#',
                type: ColumnType.number,
                queryColumnExcluded: true
            },
            {
                field: '',
                caption: 'Status',
                type: ColumnType.other,
                queryColumnExcluded: true,
                cellTemplate: this.statusTemplate
            },
            {
                field: 'testDate',
                caption: 'Dates',
                type: ColumnType.date,
                queryColumnExcluded: true,
                cellTemplate: this.datesTemplate
            },
            {
                field: 'accountNumber',
                caption: 'Account Number',
                type: ColumnType.text
            },
            {
                field: 'serialNumber',
                caption: 'Serial #',
                type: ColumnType.text
            },
            {
                field: 'manufacturer',
                caption: 'Assembly Information',
                type: ColumnType.other,
                queryColumnExcluded: true,
                cellTemplate: this.assemblyTemplate
            },
            {
                field: '',
                caption: 'Property Information',
                type: ColumnType.other,
                queryColumnExcluded: true,
                cellTemplate: this.propertyTemplate
            },
            {
                field: '',
                caption: 'Mailing Information',
                type: ColumnType.other,
                queryColumnExcluded: true,
                cellTemplate: this.mailingTemplate
            }
        ];
    }

    public async getTests(): Promise<void> {
        try {
            this.table.isLoading = true;
            const result = await this._backflowTestService.getAllForProfessional(
                this.table.items?.pageInfo || {},
                this.table.query
            );
            const startIndex = ((result.pageInfo.pageNumber ?? 1) - 1) * (result.pageInfo.pageSize ?? 10);
            result.data.forEach((item, i) => (item as any)['_rowNumber'] = startIndex + i + 1);
            this.table.items = result;
        } finally {
            this.table.isLoading = false;
        }
    }

    public setShowResults(visible: boolean): void {
        this.showResults = visible;
        this._containerHelper.setContainerVisibility(!visible);
    }

    public onFilterChange(queryProperties: QueryProperty[]): void {
        this.table.query.filter = queryProperties;
    }

    public async search(searchForm: NgForm): Promise<void> {
        if (searchForm.valid) {
            // Rebuild columns fresh: vp-table appends an Actions column bound to its own instance,
            // so reusing the array after "Search Again" leaves a stale View handler (dead until refresh).
            this.setupColumns();
            await this.getTests();
            this.setShowResults(true);
        }
    }
}
