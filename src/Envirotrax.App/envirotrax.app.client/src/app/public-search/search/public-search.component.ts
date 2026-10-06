import { Component, OnInit, TemplateRef, ViewChild } from "@angular/core";
import { formatDate } from "@angular/common";
import { ActivatedRoute } from "@angular/router";
import { NgForm } from "@angular/forms";
import { CellTemplateData, ColumnType, InputOption, TableColumn } from "@envirotrax/common-ui";
import { TableViewModel } from "../../shared/models/table-view-model";
import { PagedData } from "../../shared/models/paged-data";
import { AppContainerHelperService } from "../../shared/services/helpers/app-contaner-helper.service";
import { PublicSearchService } from "../../shared/services/public-search/public-search.service";
import { PublicSearchCriteria } from "../../shared/models/public-search/public-search-criteria";
import { PublicBackflowTestResult } from "../../shared/models/public-search/public-backflow-test-result";
import { PublicCsiInspectionResult } from "../../shared/models/public-search/public-csi-inspection-result";

type PublicSearchTab = 'backflow' | 'csi';

interface PublicSearchRowVm {
    id: number;
    rowNumber: number;
    propertyBusinessName: string;
    propertyAddress: string;
    propertyCityStateZip: string;
}

interface PublicBackflowTestRowVm extends PublicSearchRowVm {
    testedOn: string;
    expiresOn: string;
    expirationBadgeClass: string;
    deviceDescription: string;
    serialNumber: string;
    hazardType: string;
}

interface PublicCsiInspectionRowVm extends PublicSearchRowVm {
    inspectionDate: string;
    inspectorCompanyName: string;
    inspectorContactName: string;
    inspectorAddress: string;
    inspectorCityStateZip: string;
}

@Component({
    standalone: false,
    templateUrl: './public-search.component.html'
})
export class PublicSearchComponent implements OnInit {
    public waterSupplierOptions: InputOption[] = [];
    public selectedWaterSupplierId: string = '';
    public propertyBusinessName: string = '';
    public propertyStreetNumber: string = '';
    public propertyStreetName: string = '';
    public propertyNumber: string = '';

    public errorMessage: string = '';
    public hasWaterSuppliers: boolean = false;
    public isSuppliersLoading: boolean = false;
    public isSuppliersLoaded: boolean = false;
    public showResults: boolean = false;
    public activeTab: PublicSearchTab = 'backflow';

    public backflowTable: TableViewModel<PublicBackflowTestRowVm> = {
        query: {
            sort: {},
            filter: []
        }
    };

    public csiTable: TableViewModel<PublicCsiInspectionRowVm> = {
        query: {
            sort: {},
            filter: []
        }
    };

    @ViewChild('rowNumberCell', { static: true })
    public rowNumberCell?: TemplateRef<CellTemplateData<any>>;

    @ViewChild('propertyCell', { static: true })
    public propertyCell?: TemplateRef<CellTemplateData<any>>;

    @ViewChild('testDatesCell', { static: true })
    public testDatesCell?: TemplateRef<CellTemplateData<PublicBackflowTestRowVm>>;

    @ViewChild('deviceCell', { static: true })
    public deviceCell?: TemplateRef<CellTemplateData<PublicBackflowTestRowVm>>;

    @ViewChild('inspectionDateCell', { static: true })
    public inspectionDateCell?: TemplateRef<CellTemplateData<PublicCsiInspectionRowVm>>;

    @ViewChild('inspectorCell', { static: true })
    public inspectorCell?: TemplateRef<CellTemplateData<PublicCsiInspectionRowVm>>;

    constructor(
        private readonly _publicSearchService: PublicSearchService,
        private readonly _containerHelper: AppContainerHelperService,
        private readonly _activatedRoute: ActivatedRoute
    ) {

    }

    public async ngOnInit(): Promise<void> {
        this.backflowTable.columns = this.getBackflowColumns();
        this.csiTable.columns = this.getCsiColumns();

        await this.loadWaterSuppliers(this._activatedRoute.snapshot.queryParamMap.get('domain'));
    }

    public async search(searchForm: NgForm): Promise<void> {
        searchForm.form.markAllAsTouched();

        if (!searchForm.valid) {
            return;
        }

        this.errorMessage = this.validateCriteria();

        if (this.errorMessage) {
            return;
        }

        this.resetPageNumber(this.backflowTable.items);
        this.resetPageNumber(this.csiTable.items);

        await Promise.all([this.getBackflowTests(), this.getCsiInspections()]);

        this.activeTab = 'backflow';
        this.setShowResults(true);
    }

    public searchAgain(): void {
        this.setShowResults(false);
    }

    public setActiveTab(tab: PublicSearchTab): void {
        this.activeTab = tab;
    }

    public async getBackflowTests(): Promise<void> {
        try {
            this.backflowTable.isLoading = true;

            const results = await this._publicSearchService.searchBackflowTests(
                this.buildCriteria(),
                this.backflowTable.items?.pageInfo || {}
            );

            const firstRowNumber = PublicSearchComponent.getFirstRowNumber(results);

            this.backflowTable.items = {
                pageInfo: results.pageInfo,
                data: results.data.map((test, index) => this.toBackflowRowVm(test, firstRowNumber + index))
            };
        } finally {
            this.backflowTable.isLoading = false;
        }
    }

    public async getCsiInspections(): Promise<void> {
        try {
            this.csiTable.isLoading = true;

            const results = await this._publicSearchService.searchCsiInspections(
                this.buildCriteria(),
                this.csiTable.items?.pageInfo || {}
            );

            const firstRowNumber = PublicSearchComponent.getFirstRowNumber(results);

            this.csiTable.items = {
                pageInfo: results.pageInfo,
                data: results.data.map((inspection, index) => this.toCsiRowVm(inspection, firstRowNumber + index))
            };
        } finally {
            this.csiTable.isLoading = false;
        }
    }

    private async loadWaterSuppliers(domain: string | null): Promise<void> {
        try {
            this.isSuppliersLoading = true;

            const result = await this._publicSearchService.getWaterSuppliers(domain ?? undefined);

            const placeholders: InputOption[] = result.selectedWaterSupplierId
                ? []
                : [{ id: '', text: 'Please select a water supplier' }];

            this.waterSupplierOptions = [
                ...placeholders,
                ...result.suppliers.map(supplier => ({ id: String(supplier.id), text: supplier.name }))
            ];

            this.hasWaterSuppliers = result.suppliers.length > 0;
            this.selectedWaterSupplierId = result.selectedWaterSupplierId
                ? String(result.selectedWaterSupplierId)
                : '';
        } finally {
            this.isSuppliersLoading = false;
            this.isSuppliersLoaded = true;
        }
    }

    private validateCriteria(): string {
        const hasCriteria = [
            this.propertyBusinessName,
            this.propertyStreetNumber,
            this.propertyStreetName,
            this.propertyNumber
        ].some(value => !!value?.trim());

        if (!hasCriteria) {
            return 'Please enter at least one search criterion.';
        }

        const streetNumber = this.propertyStreetNumber?.trim();

        if (streetNumber && !/^\d+$/.test(streetNumber)) {
            return 'Invalid numeric value for Property Street Number.';
        }

        return '';
    }

    private buildCriteria(): PublicSearchCriteria {
        return {
            waterSupplierId: Number(this.selectedWaterSupplierId),
            propertyBusinessName: this.propertyBusinessName?.trim(),
            propertyStreetNumber: this.propertyStreetNumber?.trim(),
            propertyStreetName: this.propertyStreetName?.trim(),
            propertyNumber: this.propertyNumber?.trim()
        };
    }

    private resetPageNumber(items?: PagedData<unknown>): void {
        if (items) {
            items.pageInfo = { ...items.pageInfo, pageNumber: 1 };
        }
    }

    private setShowResults(visible: boolean): void {
        this.showResults = visible;
        this._containerHelper.setContainerVisibility(!visible);
    }

    private getBackflowColumns(): TableColumn<PublicBackflowTestRowVm>[] {
        return [
            {
                field: '',
                caption: '#',
                type: ColumnType.other,
                cellTemplate: this.rowNumberCell,
                queryColumnExcluded: true
            },
            {
                field: '',
                caption: 'Test/Expiration Dates',
                type: ColumnType.other,
                cellTemplate: this.testDatesCell,
                queryColumnExcluded: true
            },
            {
                field: '',
                caption: 'Device Description',
                type: ColumnType.other,
                cellTemplate: this.deviceCell,
                queryColumnExcluded: true
            },
            {
                field: '',
                caption: 'Property Address',
                type: ColumnType.other,
                cellTemplate: this.propertyCell,
                queryColumnExcluded: true
            }
        ];
    }

    private getCsiColumns(): TableColumn<PublicCsiInspectionRowVm>[] {
        return [
            {
                field: '',
                caption: '#',
                type: ColumnType.other,
                cellTemplate: this.rowNumberCell,
                queryColumnExcluded: true
            },
            {
                field: '',
                caption: 'Inspection Date',
                type: ColumnType.other,
                cellTemplate: this.inspectionDateCell,
                queryColumnExcluded: true
            },
            {
                field: '',
                caption: 'Property Address',
                type: ColumnType.other,
                cellTemplate: this.propertyCell,
                queryColumnExcluded: true
            },
            {
                field: '',
                caption: 'Inspector',
                type: ColumnType.other,
                cellTemplate: this.inspectorCell,
                queryColumnExcluded: true
            }
        ];
    }

    private toBackflowRowVm(test: PublicBackflowTestResult, rowNumber: number): PublicBackflowTestRowVm {
        const expiration = PublicSearchComponent.getExpiration(test);

        return {
            id: test.id,
            rowNumber: rowNumber,
            testedOn: PublicSearchComponent.toDateText(test.testDate),
            expiresOn: expiration.text,
            expirationBadgeClass: expiration.badgeClass,
            deviceDescription: [test.manufacturer, test.model, test.size, test.deviceType]
                .filter(part => !!part)
                .join(' '),
            serialNumber: test.serialNumber ?? '',
            hazardType: PublicSearchComponent.getHazardType(test),
            propertyBusinessName: test.propertyBusinessName ?? '',
            propertyAddress: PublicSearchComponent.buildStreetAddress(
                test.propertyStreetNumber,
                test.propertyStreetName,
                test.propertyNumber
            ),
            propertyCityStateZip: PublicSearchComponent.buildCityStateZip(
                test.propertyCity,
                test.propertyState,
                test.propertyZip
            )
        };
    }

    private toCsiRowVm(inspection: PublicCsiInspectionResult, rowNumber: number): PublicCsiInspectionRowVm {
        return {
            id: inspection.id,
            rowNumber: rowNumber,
            inspectionDate: PublicSearchComponent.toDateText(inspection.inspectionDate),
            propertyBusinessName: inspection.propertyBusinessName ?? '',
            propertyAddress: PublicSearchComponent.buildStreetAddress(
                inspection.propertyStreetNumber,
                inspection.propertyStreetName,
                inspection.propertyNumber
            ),
            propertyCityStateZip: PublicSearchComponent.buildCityStateZip(
                inspection.propertyCity,
                inspection.propertyState,
                inspection.propertyZip
            ),
            inspectorCompanyName: inspection.inspectorCompanyName ?? '',
            inspectorContactName: inspection.inspectorContactName ?? '',
            inspectorAddress: inspection.inspectorAddress ?? '',
            inspectorCityStateZip: PublicSearchComponent.buildCityStateZip(
                inspection.inspectorCity,
                inspection.inspectorState,
                inspection.inspectorZip
            )
        };
    }

    private static getFirstRowNumber(results: PagedData<unknown>): number {
        const pageNumber = results.pageInfo.pageNumber ?? 1;
        const pageSize = results.pageInfo.pageSize ?? 0;

        return (pageNumber - 1) * pageSize + 1;
    }

    private static getExpiration(test: PublicBackflowTestResult): { text: string; badgeClass: string } {
        if (!test.renewalRequired || !test.expirationDate) {
            return { text: 'N/A', badgeClass: 'text-bg-secondary' };
        }

        return {
            text: PublicSearchComponent.toDateText(test.expirationDate),
            badgeClass: new Date(test.expirationDate) >= new Date() ? 'text-bg-success' : 'text-bg-danger'
        };
    }

    private static getHazardType(test: PublicBackflowTestResult): string {
        switch (test.hazardType ?? '') {
            case '':
                return 'Unknown';
            case 'Other':
                return `Other - ${test.hazardTypeOtherDescription ?? ''}`;
            default:
                return test.hazardType!;
        }
    }

    private static toDateText(value?: string): string {
        return value ? formatDate(value, 'M/d/yyyy', 'en-US') : '';
    }

    private static buildStreetAddress(streetNumber?: string, streetName?: string, number?: string): string {
        const street = [streetNumber, streetName]
            .filter(part => !!part)
            .join(' ');

        return number ? `${street} #${number}` : street;
    }

    private static buildCityStateZip(city?: string, state?: string, zip?: string): string {
        return [[city, state].filter(part => !!part).join(', '), zip]
            .filter(part => !!part)
            .join(' ');
    }
}
