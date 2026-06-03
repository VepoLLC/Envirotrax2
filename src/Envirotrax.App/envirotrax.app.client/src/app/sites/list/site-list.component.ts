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
import { AuthService } from "../../shared/services/auth/auth.service";
import { FeatureType } from "../../shared/models/feature-type";
import { DownloadService } from "../../shared/services/download.service";

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

    public downloadConfig?: DownloadConfig<'Property Information' | 'Mailing Information' | 'GIS Data' | 'Additional Information'>;

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
        private readonly _gisMapService: GisMapService,
        private readonly _authService: AuthService,
        private readonly _downloadService: DownloadService
    ) {

    }

    private async setDownloadConfig(): Promise<void> {
        this.downloadConfig = {
            fileName: 'Sites',
            endpoint: this._siteService.getAllEndpoint(),
            suppoertedFormats: ['CSV', 'Excel', 'XML'],
            categories: [
                { name: 'Property Information', isSelected: true },
                { name: 'Mailing Information', isSelected: true },
                { name: 'GIS Data', isSelected: true },
                { name: 'Additional Information', isSelected: true, caption: 'Additional Information - Facility Type, Has On-Site Sewage Facility, Has Grease Trap, Has Fire System, Has Irrigation System, etc.' }
            ],
            columns: [
                { field: 'id', caption: 'SiteID' },
                { field: 'accountNumber', caption: 'AccountNumber' },
                { field: 'active', caption: 'Active' },
                { field: 'outOfArea', caption: 'OutOfArea' },
                { field: 'invalidMailingAddress', caption: 'InvalidMailingAddress' },
                { field: 'isFeeExempt', caption: 'IsFeeExempt' },
                { field: 'propertyType', caption: 'PropertyType', category: 'Property Information' },
                { field: 'businessName', caption: 'PropertyBusinessName', category: 'Property Information' },
                { field: 'streetNumber', caption: 'PropertyStreetNumber', category: 'Property Information' },
                { field: 'streetName', caption: 'PropertyStreetName', category: 'Property Information' },
                { field: 'propertyNumber', caption: 'PropertyNumber', category: 'Property Information' },
                { field: 'city', caption: 'PropertyCity', category: 'Property Information' },
                { field: 'state.code', caption: 'PropertyState', category: 'Property Information' },
                { field: 'zipCode', caption: 'PropertyZIP', category: 'Property Information' },
                { field: 'mailingCompanyName', caption: 'MailingCompanyName', category: 'Mailing Information' },
                { field: 'mailingContactName', caption: 'MailingContactName', category: 'Mailing Information' },
                { field: 'mailingStreetNumber', caption: 'MailingStreetNumber', category: 'Mailing Information' },
                { field: 'mailingStreetName', caption: 'MailingStreetName', category: 'Mailing Information' },
                { field: 'mailingNumber', caption: 'MailingNumber', category: 'Mailing Information' },
                { field: 'mailingCity', caption: 'MailingCity', category: 'Mailing Information' },
                { field: 'mailingState.code', caption: 'MailingState', category: 'Mailing Information' },
                { field: 'mailingZipCode', caption: 'MailingZIP', category: 'Mailing Information' },
                { field: 'mailingPhoneNumber', caption: 'MailingPhoneNumber', category: 'Mailing Information' },
                { field: 'mailingEmailAddress', caption: 'MailingEmailAddress', category: 'Mailing Information' },
                { field: 'gisLatitude', caption: 'GisLatitude', category: 'GIS Data' },
                { field: 'gisLongitude', caption: 'GisLongitude', category: 'GIS Data' },
                { field: 'gisStatus', caption: 'GisStatus', category: 'GIS Data' },
                { field: 'gisDate', caption: 'GisDate', category: 'GIS Data' },
                { field: 'gisAreaId', caption: 'GisAreaID', category: 'GIS Data' }
            ]
        };

        if (await this._authService.hasAnyFeatures(FeatureType.BackflowTesting)) {
            this.downloadConfig.columns.push({ field: 'backflowScheduleMonth', caption: 'BackflowScheduleMonth' });
        }

        if (await this._authService.hasAnyFeatures(FeatureType.CsiInspection)) {
            this.downloadConfig.columns.push(
                { field: 'needsCsiInspection', caption: 'NeedsCSIInspection' },
                { field: 'csiRenewalDate', caption: 'CSIRenewalDate' }
            );
        }

        if (await this._authService.hasAnyFeatures(FeatureType.FogInspection, FeatureType.FogTransportation)) {
            this.downloadConfig.columns.push(
                { field: 'needsFogInspection', caption: 'NeedsFogInspection' },
                { field: 'fogInspectionExpirationDate', caption: 'FogInspectionExpirationDate' },
                { field: 'needsFogPermit', caption: 'NeedsFogPermit' },
                { field: 'fogPermitExpirationDate', caption: 'FogPermitExpirationDate' },
                { field: 'lastTripTicketDate', caption: 'LastTripTicketDate' },
                { field: 'tripTicketInterval', caption: 'TripTicketInterval' },
                { field: 'fogDaysOverdue', caption: 'FogDaysOverdue' }
            );
        }

        this.downloadConfig.columns.push(
            { field: 'facilityType', caption: 'FacilityType', category: 'Additional Information' },
            { field: 'hasOnSiteSewageFacility', caption: 'HasOnSiteSewageFacility', category: 'Additional Information' },
            { field: 'hasAuxWaterSupply', caption: 'HasAuxWaterSupply', category: 'Additional Information' },
            { field: 'hasFireSystem', caption: 'HasFireSystem', category: 'Additional Information' },
            { field: 'fireSeparateWater', caption: 'FireSeparateWater', category: 'Additional Information' },
            { field: 'greaseTrapType', caption: 'HasGreaseTrap', category: 'Additional Information' },
            { field: 'hasGritTrap', caption: 'HasGritTrap', category: 'Additional Information' },
            { field: 'hasReclaimed', caption: 'HasReclaimed', category: 'Additional Information' },
            { field: 'hasIrrigation', caption: 'HasIrrigation', category: 'Additional Information' },
            { field: 'irrigationSeparateWater', caption: 'IrrigationSeparateWater', category: 'Additional Information' },
            { field: 'hasDomesticPremisesIsolation', caption: 'HasDomesticPremisesIsolation', category: 'Additional Information' },
            { field: 'requiresDomesticPremisesIsolation', caption: 'RequiresDomesticPremisesIsolation', category: 'Additional Information' },
            { field: 'updatedBy.email', caption: 'LastModifiedBy' },
            { field: 'updatedTime', caption: 'LastModifiedDate' }
        );
    }

    public async ngOnInit(): Promise<void> {
        this.table.columns = this.getColumns();
        this.setDownloadConfig();
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
        this._downloadService.showDownloadManager(this.downloadConfig!, this.table.query);
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
