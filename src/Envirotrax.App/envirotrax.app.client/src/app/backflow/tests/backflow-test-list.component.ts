import { Component, OnInit } from '@angular/core';
import { NgForm } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { BackflowTestService } from '../../shared/services/backflow/backflow-test.service';
import { BackflowTestOptionsService } from '../../shared/services/backflow/backflow-test-options.service';
import { GisAreaService } from '../../shared/services/gis-areas/gis-area.service';
import { GisAreaCoordinateService } from '../../shared/services/gis-areas/gis-area-coordinate.service';
import { GisMapService } from '../../shared/services/gis-areas/gis-map.service';
import { QueryProperty } from '../../shared/models/query';
import { TableViewModel } from '../../shared/models/table-view-model';
import { BackflowTest } from '../../shared/models/backflow/backflow-test';
import { GisArea } from '../../shared/models/gis-areas/gis-area';
import { TableColumn } from '../../shared/components/data-components/table/table.component';
import { ColumnType } from '../../shared/components/data-components/sorting-filtering/query-view-model';
import { InputOption } from '../../shared/components/input/input.component';
import { FacilityType } from '../../shared/enums/facility-type.enum';
import { MapMarker, MapPolygon } from '../../shared/components/map/map.component';

@Component({
    standalone: false,
    templateUrl: './backflow-test-list.component.html'
})
export class BackflowTestListComponent implements OnInit {
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
        columns: this.getColumns(),
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
        { id: "0", text: "Residential" },
        { id: "1", text: "Commercial" }
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

    constructor(
        private readonly _backflowTestService: BackflowTestService,
        private readonly _router: Router,
        private readonly _activatedRoute: ActivatedRoute,
        private readonly _gisAreaService: GisAreaService,
        private readonly _coordinateService: GisAreaCoordinateService,
        private readonly _gisMapService: GisMapService,
        private readonly _options: BackflowTestOptionsService
    ) {
        this.testResultOptions = this._options.testResultOptions;
        this.paymentStatusOptions = this._options.paymentStatusOptions;
        this.approvalStatusOptions = this._options.approvalStatusOptions;
        this.reasonForTestOptions = this._options.reasonFilterOptions;
        this.hazardTypeOptions = this._options.hazardTypeFilterOptions;
        this.deviceTypeOptions = this._options.deviceTypeFilterOptions;
    }

    public async ngOnInit(): Promise<void> {}

    public viewDetails(test: BackflowTest): void {
        this._router.navigate([test.id, 'view'], { relativeTo: this._activatedRoute });
    }

    private getColumns(): TableColumn<BackflowTest>[] {
        return [
            {
                field: 'accountNumber',
                caption: 'Account Number',
                type: ColumnType.text
            },
            {
                field: 'serialNumber',
                caption: 'Serial Number',
                type: ColumnType.text
            },
            {
                field: 'propertyBusinessName',
                caption: 'Business Name',
                type: ColumnType.text
            },
            {
                field: 'propertyStreetNumber',
                caption: 'Street Number',
                type: ColumnType.text
            },
            {
                field: 'propertyStreetName',
                caption: 'Street Name',
                type: ColumnType.text
            },
            {
                field: 'propertyCity',
                caption: 'City',
                type: ColumnType.text
            },
            {
                field: 'testDate',
                caption: 'Test Date',
                type: ColumnType.date
            },
            {
                field: 'testResult',
                caption: 'Test Result',
                type: ColumnType.text
            },
            {
                field: 'bpatCompanyName',
                caption: 'BPAT Company',
                type: ColumnType.text
            },
            {
                field: 'expirationDate',
                caption: 'Expiration Date',
                type: ColumnType.date
            }
        ];
    }

    public async getTests(): Promise<void> {
        try {
            this.table.isLoading = true;
            this.table.items = await this._backflowTestService.getAll(
                this.table.items?.pageInfo || {},
                this.table.query
            );
        } finally {
            this.table.isLoading = false;
        }
    }

    public onFilterChange(queryProperties: QueryProperty[]): void {
        this.table.query.filter = queryProperties;
    }

    public async search(searchForm: NgForm): Promise<void> {
        if (searchForm.valid) {
            this.showMapResults = false;
            await this.getTests();
            this.showResults = true;
        }
    }

    public async searchMap(searchForm: NgForm): Promise<void> {
        if (!searchForm.valid) {
            return;
        }
        try {
            this.isMapLoading = true;
            this.showResults = false;
            this.showMapResults = false;

            const [testsPage, areas, coordinates, defaultView] = await Promise.all([
                this._backflowTestService.getAll({ pageSize: 10000, pageNumber: 1 }, this.table.query),
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
