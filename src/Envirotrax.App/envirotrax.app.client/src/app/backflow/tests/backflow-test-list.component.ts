import { Component, ElementRef, OnInit, OnDestroy, ViewChild, TemplateRef } from '@angular/core';
import { NgForm } from '@angular/forms';
import { ActivatedRoute, ParamMap, Router } from '@angular/router';
import { Subscription } from 'rxjs';
import { BackflowTestService } from '../../shared/services/backflow/backflow-test.service';
import { BackflowTestOptionsService } from '../../shared/services/backflow/backflow-test-options.service';
import { GisAreaService } from '../../shared/services/gis-areas/gis-area.service';
import { GisAreaCoordinateService } from '../../shared/services/gis-areas/gis-area-coordinate.service';
import { GisMapService } from '../../shared/services/gis-areas/gis-map.service';
import { QueryProperty } from '../../shared/models/query';
import { TableViewModel } from '../../shared/models/table-view-model';
import { BackflowTest } from '../../shared/models/backflow/backflow-test';
import { GisArea } from '../../shared/models/gis-areas/gis-area';
import { FacilityType } from '../../shared/enums/facility-type.enum';
import { CellTemplateData, ColumnType, InputOption, MapMarker, MapPolygon } from '@envirotrax/common-ui';
import { BackflowPaymentStatus, BackflowTestResult } from '../../shared/models/backflow/backflow-test-enums';
import { DownloadConfig } from '../../shared/models/download-config';
import { DownloadService } from '../../shared/services/download.service';
import { PrintableTableService } from '../../shared/services/printable-table.service';
import { PropertyType } from '../../shared/enums/property-type.enum';
import { BackflowComplianceParams } from '../../shared/models/backflow/backflow-compliance-params';
import { AppContainerHelperService } from '../../shared/services/helpers/app-contaner-helper.service';

@Component({
    standalone: false,
    templateUrl: './backflow-test-list.component.html'
})
export class BackflowTestListComponent implements OnInit, OnDestroy {
    private _queryParamSub?: Subscription;

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

    @ViewChild('bpatTemplate', { static: true })
    public bpatTemplate!: TemplateRef<CellTemplateData<BackflowTest>>;

    @ViewChild('viewTemplate', { static: true })
    public viewTemplate!: TemplateRef<CellTemplateData<BackflowTest>>;

    @ViewChild('printableSection')
    private _printableSection!: ElementRef;

    public readonly BackflowTestResult = BackflowTestResult;

    public showResults: boolean = false;
    public showMapResults: boolean = false;
    public isMapLoading: boolean = false;
    public mapResultCount: number = 0;
    public mapMarkers: MapMarker<BackflowTest>[] = [];
    public mapPolygons: MapPolygon<GisArea>[] = [];
    public mapLatitude: number = 30.9;
    public mapLongitude: number = -97.2829;
    public mapZoom: number = 10;

    public table: TableViewModel<BackflowTest> = {
        columns: [],
        query: {
            sort: {},
            filter: []
        },
        freeTextSearch: {
            searchQuery: [
                { field: 'accountNumber', operator: 'Ct' },
                { field: 'serialNumber', operator: 'Ct' },
                { field: 'bpatLicenseNumber', operator: 'Ct' }
            ]
        }
    };

    public testHistoryOptions: InputOption[] = [
        { id: "", text: "All Tests" },
        { id: "true", text: "Latest Test Only" }
    ];

    public serviceStatusOptions: InputOption[] = [
        { id: "", text: "All Status Types" },
        { id: "false", text: "Active Only" },
        { id: "true", text: "Out of Service Only" }
    ];

    public rejectedStatusOptions: InputOption[] = [
        { id: "", text: "Any Status" },
        { id: "false", text: "Not Rejected" },
        { id: "true", text: "Rejected" }
    ];

    public testResultOptions: InputOption[];
    public paymentStatusOptions: InputOption[];

    public paymentStatus: string = '';
    public approvalStatusOptions: InputOption[];
    public reasonForTestOptions: InputOption[];

    public yesNoOptions: InputOption[] = [
        { id: "", text: "Any Value" },
        { id: "true", text: "Yes" },
        { id: "false", text: "No" }
    ];

    public gaugeOptions: InputOption[] = [
        { id: "", text: "Any Value" },
        { id: "false", text: "Potable" },
        { id: "true", text: "Non-Potable" }
    ];

    public propertyTypeOptions: InputOption[] = [
        { id: "", text: "Any Value" },
        { id: PropertyType.Residential.toString(), text: "Residential" },
        { id: PropertyType.Commercial.toString(), text: "Commercial" }
    ];

    public hazardTypeOptions: InputOption[];

    public facilityTypeOptions: InputOption[] = [
        { id: FacilityType.Other.toString(), text: "Other" },
        { id: FacilityType.Restaurant.toString(), text: "Restaurant" },
        { id: FacilityType.FastFoodEstablishment.toString(), text: "Fast food establishment" },
        { id: FacilityType.HotelMotel.toString(), text: "Hotel/motel" },
        { id: FacilityType.CarWash.toString(), text: "Car wash" },
        { id: FacilityType.SchoolUniversity.toString(), text: "School/university" },
        { id: FacilityType.GroceryStore.toString(), text: "Grocery store" },
        { id: FacilityType.ConvenienceStore.toString(), text: "Convenience store" },
        { id: FacilityType.AssistedLivingFacility.toString(), text: "Assisted living facility" },
        { id: FacilityType.MedicalFacility.toString(), text: "Medical facility" },
        { id: FacilityType.Industrial.toString(), text: "Industrial" },
        { id: FacilityType.CityOwnedFacility.toString(), text: "City-owned facility" }
    ];

    public deviceTypeOptions: InputOption[];

    public downloadConfig: DownloadConfig<'property' | 'mailing' | 'bpat' | 'assembly' | 'testResults' | 'internal'>;

    constructor(
        private readonly _backflowTestService: BackflowTestService,
        private readonly _router: Router,
        private readonly _activatedRoute: ActivatedRoute,
        private readonly _gisAreaService: GisAreaService,
        private readonly _coordinateService: GisAreaCoordinateService,
        private readonly _gisMapService: GisMapService,
        private readonly _options: BackflowTestOptionsService,
        private readonly _downloadService: DownloadService,
        private readonly _printService: PrintableTableService,
        private readonly _containerHelper: AppContainerHelperService
    ) {
        this.testResultOptions = this._options.testResultOptions;
        this.paymentStatusOptions = this._options.paymentStatusOptions;
        this.approvalStatusOptions = this._options.approvalStatusOptions;
        this.reasonForTestOptions = this._options.reasonFilterOptions;
        this.hazardTypeOptions = this._options.hazardTypeFilterOptions;
        this.deviceTypeOptions = this._options.deviceTypeFilterOptions;

        this.downloadConfig = {
            fileName: 'Backflow Tests',
            endpoint: this._backflowTestService.getAllEndpoint(),
            pdfEndpoint: this._backflowTestService.getAllPdfEndpoint(),
            suppoertedFormats: ['CSV', 'Excel', 'PDF'],
            categories: [
                { name: 'property', caption: 'Property Information', isSelected: true },
                { name: 'mailing', caption: 'Mailing Information', isSelected: true },
                { name: 'bpat', caption: 'BPAT Extended Information', isSelected: true },
                { name: 'assembly', caption: 'Assembly Information', isSelected: true },
                { name: 'testResults', caption: 'Test Results', isSelected: true },
                { name: 'internal', caption: 'Internal Data - Schedule Month, Approval, Rejection Status, Transaction ID, Amount, etc.', isSelected: false }
            ],
            columns: [
                { field: 'testDate', caption: 'TestDate' },
                { field: 'expirationDate', caption: 'ExpirationDate' },
                { field: 'accountNumber', caption: 'AccountNumber' },
                { field: 'waterSupplier.name', caption: 'WaterSupplier' },
                { field: 'meterNumber', caption: 'MeterNumber' },
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
                // BPAT
                { field: 'bpatLicenseNumber', caption: 'BPATLicenseNumber', category: 'bpat' },
                { field: 'bpatLicenseExpiration', caption: 'BPATLicenseExpiration', category: 'bpat' },
                { field: 'bpatCompanyName', caption: 'BPATCompanyName', category: 'bpat' },
                { field: 'bpatContactName', caption: 'BPATContactName', category: 'bpat' },
                // Test Results
                { field: 'testResult', caption: 'TestResult', category: 'testResults' },
                { field: 'reasonForTest', caption: 'ReasonForTest', category: 'testResults' },
                { field: 'properlyInstalled', caption: 'ProperlyInstalled', category: 'testResults' },
                // Internal
                { field: 'scheduleMonth', caption: 'ScheduleMonth', category: 'internal' },
                { field: 'disapproved', caption: 'Disapproved', category: 'internal' },
                { field: 'rejected', caption: 'Rejected', category: 'internal' },
                { field: 'transactionId', caption: 'TransactionID', category: 'internal' },
                { field: 'transactionAmount', caption: 'TransactionAmount', category: 'internal' }
            ]
        };
    }

    public ngOnInit(): void {
        this.setupColumns();
        this.subscribeToQueryParams();
    }

    public ngOnDestroy(): void {
        this._queryParamSub?.unsubscribe();
    }

    // Both drill-down entry points arrive as query params on this list: the dashboard "date" click
    // (a single-day test-date range) and the Tab 2 (Current Compliance Status) "View" link (the
    // non-compliant assemblies behind a requirement row). Apply the matching preset filter and run
    // the search automatically.
    private subscribeToQueryParams(): void {
        this._queryParamSub = this._activatedRoute.queryParamMap.subscribe(async params => {
            const dateParam = params.get('date');
            if (dateParam) {
                this.applyDateFilter(dateParam);
                await this.getTests();
                this.setShowResults(true);
                return;
            }

            if (params.get(BackflowComplianceParams.mode)) {
                this.applyComplianceFilter(params);
                await this.getTests();
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
                await this.getTests();
                this.setShowResults(true);
            }
        });
    }

    private applyDateFilter(startDate: string, endDate: string = startDate): void {
        this.table.query.filter = [{
            columnName: 'testDate',
            children: [
                { columnName: 'testDate', value: startDate, comparisonOperator: 'Gte', logicalOperator: 'And' },
                { columnName: 'testDate', value: endDate, comparisonOperator: 'Lte', logicalOperator: 'And' }
            ]
        }];
    }

    public viewDetails(test: BackflowTest): void {
        this._router.navigate([test.id, 'view'], { relativeTo: this._activatedRoute });
    }

    public showDownloadManager(): void {
        const additionalParams = this.paymentStatus
            ? { paymentStatus: this.paymentStatus }
            : undefined;

        this.downloadConfig.endpoint.additionalParams = additionalParams;
        this.downloadConfig.pdfEndpoint!.additionalParams = additionalParams;

        this._downloadService.showDownloadManager(this.downloadConfig, this.table.query);
    }

    private getPaymentStatus(): BackflowPaymentStatus | null {
        return this.paymentStatus ? Number(this.paymentStatus) as BackflowPaymentStatus : null;
    }

    public viewPrintableTable(): void {
        this._printService.open(this._printableSection.nativeElement);
    }

    public viewTest(test: BackflowTest): void {
        const url = this._router.serializeUrl(
            this._router.createUrlTree(['/backflow/tests', test.id])
        );
        window.open(url, '_blank');
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
                caption: 'status',
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
                field: 'waterSupplier.name',
                caption: 'Water Supplier',
                type: ColumnType.text,
                queryColumnExcluded: true
            },
            {
                field: 'meterNumber',
                caption: 'Water Meter Number',
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
                caption: 'BPAT',
                type: ColumnType.other,
                queryColumnExcluded: true,
                cellTemplate: this.bpatTemplate
            }
        ];
    }

    public async getTests(): Promise<void> {
        try {
            this.table.isLoading = true;
            const result = await this._backflowTestService.getAll(
                this.table.items?.pageInfo || {},
                this.table.query,
                this.getPaymentStatus()
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

    // Builds the preset filter for a Tab 2 (Current Compliance Status) View drill-down: the non-compliant
    // assemblies behind a requirement row — current, in-service assemblies whose expiration has passed the
    // cutoff, matching the site property/hazard/OSSF/aux-water filters of the compliance report row.
    private applyComplianceFilter(params: ParamMap): void {
        const filter: QueryProperty[] = [
            { columnName: 'isCurrent', value: 'true', comparisonOperator: 'Eq' },
            { columnName: 'outOfService', value: 'false', comparisonOperator: 'Eq' }
        ];

        const propertyType = params.get(BackflowComplianceParams.propertyType);
        if (propertyType) {
            filter.push({ columnName: 'propertyType', value: propertyType, comparisonOperator: 'Eq' });
        }

        const deviceType = params.get(BackflowComplianceParams.deviceType);
        if (deviceType) {
            filter.push({ columnName: 'deviceType', value: deviceType, comparisonOperator: 'Eq' });
        }

        const hazardType = params.get(BackflowComplianceParams.hazardType);
        if (hazardType) {
            filter.push({ columnName: 'hazardType', value: hazardType, comparisonOperator: 'Eq' });
        }

        if (params.get(BackflowComplianceParams.ossf) === 'true') {
            filter.push({ columnName: 'site.hasOnSiteSewageFacility', value: 'true', comparisonOperator: 'Eq' });
        }

        if (params.get(BackflowComplianceParams.auxWater) === 'true') {
            filter.push({ columnName: 'site.hasAuxWaterSupply', value: 'true', comparisonOperator: 'Eq' });
        }

        // Non-compliant = expired as of the cutoff (today, or 30 days earlier when ignoring the last 30 days).
        const cutoff = new Date();
        if (params.get(BackflowComplianceParams.ignoreLast30Days) === 'true') {
            cutoff.setDate(cutoff.getDate() - 30);
        }
        filter.push({ columnName: 'expirationDate', value: cutoff.toISOString(), comparisonOperator: 'Lte' });

        this.table.query.filter = filter;
    }

    public async search(searchForm: NgForm): Promise<void> {
        if (searchForm.valid) {
            this.showMapResults = false;
            await this.getTests();
            this.setShowResults(true);
        }
    }

    public async searchMap(searchForm: NgForm): Promise<void> {
        if (!searchForm.valid) {
            return;
        }
        try {
            this.isMapLoading = true;
            this.setShowResults(false);
            this.showMapResults = false;

            const [testsPage, areas, coordinates, defaultView] = await Promise.all([
                this._backflowTestService.getAll({ pageSize: 10000, pageNumber: 1 }, this.table.query, this.getPaymentStatus()),
                this._gisAreaService.getAllAreas(),
                this._coordinateService.getAll(),
                this._gisAreaService.getDefaultView()
            ]);

            this.mapResultCount = testsPage.pageInfo?.totalItems ?? testsPage.data.length;
            this.mapMarkers = this.buildMapMarkers(testsPage.data);
            this.mapPolygons = this._gisMapService.buildMapPolygons(areas, coordinates);

            if (defaultView.gisCenterLatitude != null) {
                this.mapLatitude = defaultView.gisCenterLatitude;
            }
            if (defaultView.gisCenterLongitude != null) {
                this.mapLongitude = defaultView.gisCenterLongitude;
            }
            if (defaultView.gisCenterZoom != null) {
                this.mapZoom = defaultView.gisCenterZoom;
            }

            this.showMapResults = true;
            window.scrollTo({ top: 0 });
        } finally {
            this.isMapLoading = false;
        }
    }

    private buildMapMarkers(tests: BackflowTest[]): MapMarker<BackflowTest>[] {
        return tests
            .filter(t => t.site?.gisLatitude != null && t.site?.gisLongitude != null)
            .map(t => {
                const siteUrl = this._router.serializeUrl(
                    this._router.createUrlTree(['/sites', t.site!.id!, 'edit'])
                );
                const label = [t.propertyBusinessName, t.propertyStreetNumber, t.propertyStreetName, t.propertyCity]
                    .filter(Boolean)
                    .join(', ');
                const popupHtml = this._gisMapService.buildSitePopupHtml(label, siteUrl);
                const icon = { path: 0, fillColor: '#e8342e', fillOpacity: 0.85, strokeWeight: 0, scale: 7 };
                return { lat: t.site!.gisLatitude!, lng: t.site!.gisLongitude!, popupHtml, icon, data: t };
            });
    }

}
