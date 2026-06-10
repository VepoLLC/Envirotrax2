import { Component, ElementRef, OnInit, ViewChild, TemplateRef } from '@angular/core';
import { NgForm } from '@angular/forms';
import { Router } from '@angular/router';
import { BackflowTestService } from '../../../shared/services/backflow/backflow-test.service';
import { ProfessionalSupplierService } from '../../../shared/services/professionals/professional-supplier.service';
import { QueryProperty } from '../../../shared/models/query';
import { TableViewModel } from '../../../shared/models/table-view-model';
import { BackflowTest } from '../../../shared/models/backflow/backflow-test';
import { CellTemplateData } from '../../../shared/components/data-components/table/table.component';
import { ColumnType } from '../../../shared/components/data-components/sorting-filtering/query-view-model';
import { InputOption } from '../../../shared/components/input/input.component';
import { BackflowTestResult } from '../../../shared/models/backflow/backflow-test-enums';
import { DownloadConfig } from '../../../shared/models/download-config';
import { DownloadService } from '../../../shared/services/download.service';
import { PrintableTableService } from '../../../shared/services/printable-table.service';

@Component({
    standalone: false,
    templateUrl: './professional-backflow-test-list.component.html'
})
export class ProfessionalBackflowTestListComponent implements OnInit {
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

    @ViewChild('viewTemplate', { static: true })
    public viewTemplate!: TemplateRef<CellTemplateData<BackflowTest>>;

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
        { id: '0', text: 'Residential' },
        { id: '1', text: 'Commercial' }
    ];

    constructor(
        private readonly _backflowTestService: BackflowTestService,
        private readonly _supplierService: ProfessionalSupplierService,
        private readonly _router: Router,
        private readonly _downloadService: DownloadService,
        private readonly _printService: PrintableTableService
    ) {
        this.downloadConfig = {
            fileName: 'Backflow Tests',
            endpoint: this._backflowTestService.getAllForProfessionalEndpoint(),
            suppoertedFormats: ['CSV', 'Excel'],
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

    public viewTest(test: BackflowTest): void {
        const url = this._router.serializeUrl(
            this._router.createUrlTree(['/professionals/backflow/submit', test.id])
        );
        window.open(url, '_blank');
    }

    public navigateToSite(test: BackflowTest): void {
        if (!test.site?.id) 
            { 
                return; 
            }

        const url = this._router.serializeUrl(
            this._router.createUrlTree(['/professionals/sites', test.site.id])
        );
        window.open(url, '_blank');
    }

    public async ngOnInit(): Promise<void> {
        this.setupColumns();
        await this.loadWaterSupplierScopeOptions();
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
            },
            {
                field: '',
                caption: '',
                type: ColumnType.other,
                queryColumnExcluded: true,
                isDownloadExcluded: true,
                cellTemplate: this.viewTemplate
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

    public onFilterChange(queryProperties: QueryProperty[]): void {
        this.table.query.filter = queryProperties;
    }

    public async search(searchForm: NgForm): Promise<void> {
        if (searchForm.valid) {
            await this.getTests();
            this.showResults = true;
        }
    }
}
