import { Component, OnInit, TemplateRef, ViewChild } from "@angular/core";
import { TableViewModel } from "../../shared/models/table-view-model";
import { Site } from "../../shared/models/sites/site";
import { SiteService } from "../../shared/services/sites/site.service";
import { ActivatedRoute, Router } from "@angular/router";
import { CellTemplateData, TableColumn } from "../../shared/components/data-components/table/table.component";
import { ColumnType } from "../../shared/components/data-components/sorting-filtering/query-view-model";
import { QueryProperty } from "../../shared/models/query";
import { NgForm } from "@angular/forms";
import { ModalHelperService } from "../../shared/services/helpers/modal-helper.service";
import { CreateSiteComponent } from "../create/create-site-component";
import { InputOption } from "../../shared/components/input/input.component";
import { PropertyType } from "../../shared/enums/property-type.enum";
import { GisAreaService } from "../../shared/services/gis-areas/gis-area.service";
import { GisAreaCoordinateService } from "../../shared/services/gis-areas/gis-area-coordinate.service";
import { GisMapService } from "../../shared/services/gis-areas/gis-map.service";
import { GisArea } from "../../shared/models/gis-areas/gis-area";
import { MapMarker, MapPolygon } from "../../shared/components/map/map.component";
import { DownloadConfig } from "../../shared/models/download-config";
import { DownloadManagerComponent } from "../../shared/components/data-components/download-manager/download-manager.component";

@Component({
    standalone: false,
    templateUrl: './site-list.component.html'
})
export class SiteListComponent implements OnInit {
    public showResults: boolean = false;
    public showMapResults: boolean = false;
    public isMapLoading: boolean = false;
    public mapResultCount: number = 0;
    public mapMarkers: MapMarker<Site>[] = [];
    public mapPolygons: MapPolygon<GisArea>[] = [];
    public mapLatitude: number = 30.9;
    public mapLongitude: number = -97.2829;
    public mapZoom: number = 10;

    public table: TableViewModel<Site> = {
        query: {
            sort: {},
            filter: []
        },
        freeTextSearch: {
            searchQuery: [
                { field: 'accountNumber', operator: 'Ct' },
                { field: 'businessName', operator: 'Ct', multiWordSearch: true },
                { field: 'streetName', operator: 'Ct', multiWordSearch: true },
                { field: 'city', operator: 'Ct', multiWordSearch: true }
            ]
        }
    };

    public facilityTypes: InputOption[] = [
        { id: "", text: "Any Value" },
        { id: "0", text: "Other" },
        { id: "1", text: "Restaurant" },
        { id: "2", text: "Fast Food Establishment" },
        { id: "3", text: "Hotel/Motel" },
        { id: "4", text: "Car Wash" },
        { id: "5", text: "School/University" },
        { id: "6", text: "Grocery Store" },
        { id: "7", text: "Convenience Store" },
        { id: "8", text: "Assisted Living Facility" },
        { id: "9", text: "Medical Facility" },
        { id: "10", text: "Industrial" },
        { id: "11", text: "City Owned Facility" }
    ];

    public yesNoOptions: InputOption[] = [
        { id: "", text: "Any Value" },
        { id: "true", text: "Yes" },
        { id: "false", text: "No" }
    ];

    public greaseTrapoptions: InputOption[] = [
        { id: "", text: "Any Value" },
        { id: "0", text: "Trap Not Required" },
        { id: "1", text: "Has Grease Trap" },
        { id: "2", text: "Should Have Grease Trap" },
        { id: "3", text: "Might Have Grease Trap" }
    ];

    public propertyTypes: InputOption[] = [
        { id: "", text: "Any Value" },
        { id: "0", text: "Residential" },
        { id: "1", text: "Commercial" }
    ];

    public propertyType = PropertyType;

    public downloadConfig: DownloadConfig;

    @ViewChild('propertyInformation', { static: true })
    public propertyInformation?: TemplateRef<CellTemplateData<Site>>

    @ViewChild('mailingInformation', { static: true })
    public mailingInformation?: TemplateRef<CellTemplateData<Site>>;

    @ViewChild('keyIndicators', { static: true })
    public keyIndicators?: TemplateRef<CellTemplateData<Site>>;

    constructor(
        private readonly _siteService: SiteService,
        private readonly _router: Router,
        private readonly _activatedRoute: ActivatedRoute,
        private readonly _modalHelper: ModalHelperService,
        private readonly _gisAreaService: GisAreaService,
        private readonly _coordinateService: GisAreaCoordinateService,
        private readonly _gisMapService: GisMapService
    ) {
        this.downloadConfig = {
            fileName: 'Sites',
            endpoint: this._siteService.getAllEndpoint(),
            suppoertedFormats: ['CSV', 'Excel', 'XML'],
            columns: [
                { field: 'id', caption: 'SiteID' },
                { field: 'accountNumber', caption: 'AccountNumber' },
                { field: 'active', caption: 'Active' },
                { field: 'outOfArea', caption: 'OutOfArea' },
                { field: 'invalidMailingAddress', caption: 'InvalidMailingAddress' },
                { field: 'isFeeExempt', caption: 'IsFeeExempt' },
                { field: 'propertyType', caption: 'PropertyType' },
                { field: 'businessName', caption: 'PropertyBusinessName' },
                { field: 'streetNumber', caption: 'PropertyStreetNumber' },
                { field: 'streetName', caption: 'PropertyStreetName' },
                { field: 'propertyNumber', caption: 'PropertyNumber' },
                { field: 'city', caption: 'PropertyCity' },
                { field: 'state.code', caption: 'PropertyState' },
                { field: 'zipCode', caption: 'PropertyZIP' },
                { field: 'mailingCompanyName', caption: 'MailingCompanyName' },
                { field: 'mailingContactName', caption: 'MailingContactName' },
                { field: 'mailingStreetNumber', caption: 'MailingStreetNumber' },
                { field: 'mailingStreetName', caption: 'MailingStreetName' },
                { field: 'mailingNumber', caption: 'MailingNumber' },
                { field: 'mailingCity', caption: 'MailingCity' },
                { field: 'mailingState.code', caption: 'MailingState' },
                { field: 'mailingZipCode', caption: 'MailingZIP' },
                { field: 'mailingPhoneNumber', caption: 'MailingPhoneNumber' },
                { field: 'mailingEmailAddress', caption: 'MailingEmailAddress' },
                { field: 'gisLatitude', caption: 'GisLatitude' },
                { field: 'gisLongitude', caption: 'GisLongitude' },
                { field: 'gisStatus', caption: 'GisStatus' },
                { field: 'gisDate', caption: 'GisDate' },
                { field: 'gisAreaId', caption: 'GisAreaID' },
                { field: 'backflowScheduleMonth', caption: 'BackflowScheduleMonth' },
                { field: 'needsCsiInspection', caption: 'NeedsCSIInspection' },
                { field: 'csiRenewalDate', caption: 'CSIRenewalDate' },
                { field: 'needsFogInspection', caption: 'NeedsFogInspection' },
                { field: 'fogInspectionExpirationDate', caption: 'FogInspectionExpirationDate' },
                { field: 'needsFogPermit', caption: 'NeedsFogPermit' },
                { field: 'fogPermitExpirationDate', caption: 'FogPermitExpirationDate' },
                { field: 'lastTripTicketDate', caption: 'LastTripTicketDate' },
                { field: 'tripTicketInterval', caption: 'TripTicketInterval' },
                { field: 'fogDaysOverdue', caption: 'FogDaysOverdue' },
                { field: 'facilityType', caption: 'FacilityType' },
                { field: 'hasOnSiteSewageFacility', caption: 'HasOnSiteSewageFacility' },
                { field: 'hasAuxWaterSupply', caption: 'HasAuxWaterSupply' },
                { field: 'hasFireSystem', caption: 'HasFireSystem' },
                { field: 'fireSeparateWater', caption: 'FireSeparateWater' },
                { field: 'greaseTrapType', caption: 'HasGreaseTrap' },
                { field: 'hasGritTrap', caption: 'HasGritTrap' },
                { field: 'hasReclaimed', caption: 'HasReclaimed' },
                { field: 'hasIrrigation', caption: 'HasIrrigation' },
                { field: 'irrigationSeparateWater', caption: 'IrrigationSeparateWater' },
                { field: 'hasDomesticPremisesIsolation', caption: 'HasDomesticPremisesIsolation' },
                { field: 'requiresDomesticPremisesIsolation', caption: 'RequiresDomesticPremisesIsolation' },
                { field: 'lastModifiedBy', caption: 'LastModifiedBy' },
                { field: 'lastModifiedDate', caption: 'LastModifiedDate' }
            ]
        };
    }

    public async ngOnInit(): Promise<void> {
        this.table.columns = this.getColumns();
    }

    private getColumns(): TableColumn<Site>[] {
        return [
            {
                field: 'Key Indicators',
                caption: 'Key Indicators',
                type: ColumnType.other,
                cellTemplate: this.keyIndicators
            },
            {
                field: 'accountNumber',
                caption: 'Account Number',
                type: ColumnType.text
            },
            {
                field: 'Property Information',
                caption: 'Property Information',
                type: ColumnType.other,
                cellTemplate: this.propertyInformation,
                queryColumnExcluded: true
            },
            {
                field: 'Mailing Information',
                caption: 'Mailing Information',
                type: ColumnType.other,
                cellTemplate: this.mailingInformation,
                queryColumnExcluded: true
            }
        ];
    }

    public async getSites(): Promise<void> {
        try {
            this.table.isLoading = true;
            this.table.items = await this._siteService.getAll(this.table.items?.pageInfo || {}, this.table.query);
        } finally {
            this.table.isLoading = false;
        }
    }

    public onFilterChange(queryProperties: QueryProperty[]): void {
        this.table.query.filter = queryProperties
    }

    public async search(searchForm: NgForm): Promise<void> {
        if (searchForm.valid) {
            this.showMapResults = false;
            await this.getSites();
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

            const [sitesPage, areas, coordinates, defaultView] = await Promise.all([
                this._siteService.getAll({ pageSize: 10000, pageNumber: 1 }, this.table.query),
                this._gisAreaService.getAllAreas(),
                this._coordinateService.getAll(),
                this._gisAreaService.getDefaultView()
            ]);

            this.mapResultCount = sitesPage.pageInfo?.totalItems ?? sitesPage.data.length;
            this.mapMarkers = this.buildMapMarkers(sitesPage.data);
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

    public add(): void {
        this._modalHelper.show<Site>(CreateSiteComponent, {
            title: 'Location / Property information',
        }).result()
            .subscribe(_ => this.getSites());
    }

    public edit(site: Site): void {
        this._router.navigate([site.id, 'edit'], {
            relativeTo: this._activatedRoute
        });
    }

    public showDownloadManager(): void {
        this._modalHelper.show(DownloadManagerComponent, {
            title: 'Export Results',
            model: {
                ...this.downloadConfig,
                endpoint: {
                    ...this.downloadConfig.endpoint,
                    query: this.table.query
                }
            }
        });
    }

    private buildMapMarkers(sites: Site[]): MapMarker<Site>[] {
        return sites
            .filter(s => s.gisLatitude != null && s.gisLongitude != null)
            .map(s => {
                const siteUrl = this._router.serializeUrl(
                    this._router.createUrlTree([s.id, 'edit'], { relativeTo: this._activatedRoute })
                );
                const label = [s.businessName, s.streetNumber, s.streetName, s.city]
                    .filter(Boolean)
                    .join(', ');
                const popupHtml = this._gisMapService.buildSitePopupHtml(label, siteUrl);
                const icon = { path: 0, fillColor: '#e8342e', fillOpacity: 0.85, strokeWeight: 0, scale: 7 };
                return { lat: s.gisLatitude!, lng: s.gisLongitude!, popupHtml, icon, data: s };
            });
    }

}
