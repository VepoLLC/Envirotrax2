import { Component, ElementRef, OnInit, TemplateRef, ViewChild } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";
import { TableViewModel } from "../../shared/models/table-view-model";
import { Site } from "../../shared/models/sites/site";
import { SiteService } from "../../shared/services/sites/site.service";
import { QueryProperty } from "../../shared/models/query";
import { NgForm } from "@angular/forms";
import { ProfessionalSupplierService } from "../../shared/services/professionals/professional-supplier.service";
import { PropertyType } from "../../shared/enums/property-type.enum";
import { CellTemplateData, ColumnType, InputOption, MapMarker, MapPolygon, TableColumn } from "@envirotrax/common-ui";
import { AppContainerHelperService } from "../../shared/services/helpers/app-contaner-helper.service";
import { GisAreaService } from "../../shared/services/gis-areas/gis-area.service";
import { GisAreaCoordinateService } from "../../shared/services/gis-areas/gis-area-coordinate.service";
import { GisMapService } from "../../shared/services/gis-areas/gis-map.service";
import { GisArea } from "../../shared/models/gis-areas/gis-area";
import { DownloadConfig } from "../../shared/models/download-config";
import { DownloadService } from "../../shared/services/download.service";
import { PrintableTableService } from "../../shared/services/printable-table.service";

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

    public waterSupplierOptions: InputOption[] = [];

    public downloadConfig?: DownloadConfig<'Property Information' | 'Mailing Information'>;

    @ViewChild('printableSection')
    private _printableSection!: ElementRef;

    @ViewChild('keyIndicators', { static: true })
    public keyIndicators?: TemplateRef<CellTemplateData<Site>>;

    @ViewChild('propertyInformation', { static: true })
    public propertyInformation?: TemplateRef<CellTemplateData<Site>>;

    @ViewChild('mailingInformation', { static: true })
    public mailingInformation?: TemplateRef<CellTemplateData<Site>>;

    public propertyType = PropertyType;

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

    public yesNoOptions: InputOption[] = [
        { id: "", text: "Any Value" },
        { id: "true", text: "Yes" },
        { id: "false", text: "No" }
    ];

    public propertyTypes: InputOption[] = [
        { id: "", text: "Any Value" },
        { id: PropertyType.Residential.toString(), text: "Residential" },
        { id: PropertyType.Commercial.toString(), text: "Commercial" }
    ];

    constructor(
        private readonly _siteService: SiteService,
        private readonly _proSupplierService: ProfessionalSupplierService,
        private readonly _router: Router,
        private readonly _activatedRoute: ActivatedRoute,
        private readonly _containerHelper: AppContainerHelperService,
        private readonly _gisAreaService: GisAreaService,
        private readonly _coordinateService: GisAreaCoordinateService,
        private readonly _gisMapService: GisMapService,
        private readonly _downloadService: DownloadService,
        private readonly _printService: PrintableTableService
    ) {
    }

    public async ngOnInit(): Promise<void> {
        this.table.columns = this.getColumns();
        this.waterSupplierOptions = await this._proSupplierService.getMyAsOptions();
        this.setDownloadConfig();
    }

    private setDownloadConfig(): void {
        this.downloadConfig = {
            fileName: 'Sites',
            endpoint: this._siteService.getAllForProfessionalEndpoint(),
            suppoertedFormats: ['CSV', 'Excel', 'XML'],
            categories: [
                { name: 'Property Information', isSelected: true },
                { name: 'Mailing Information', isSelected: true }
            ],
            columns: [
                { field: 'accountNumber', caption: 'AccountNumber' },
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
                { field: 'mailingZipCode', caption: 'MailingZIP', category: 'Mailing Information' }
            ]
        };
    }

    private getColumns(): TableColumn<Site>[] {
        return [
            {
                field: '_rowNumber',
                caption: '#',
                type: ColumnType.number,
                queryColumnExcluded: true
            },
            {
                field: 'Key Indicators',
                caption: 'Key Indicators',
                type: ColumnType.other,
                cellTemplate: this.keyIndicators,
                queryColumnExcluded: true
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
                caption: 'Mailing/Contact Information',
                type: ColumnType.other,
                cellTemplate: this.mailingInformation,
                queryColumnExcluded: true
            }
        ];
    }

    public async getSites(): Promise<void> {
        try {
            this.table.isLoading = true;

            const result = await this._siteService.getAllForProfessional(
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
            this.table.columns = this.getColumns();
            this.showMapResults = false;
            await this.getSites();
            this.setShowResults(true);
        }
    }

    public async searchMap(searchForm: NgForm): Promise<void> {
        searchForm.form.markAllAsTouched();

        if (!searchForm.valid) {
            return;
        }

        const waterSupplierId = this.getSelectedWaterSupplierId();
        if (waterSupplierId == null) {
            return;
        }

        try {
            this.isMapLoading = true;
            this.setShowResults(false);
            this.showMapResults = false;

            const [sitesPage, areas, coordinates, defaultView] = await Promise.all([
                this._siteService.getAllForProfessional({ pageSize: 10000, pageNumber: 1 }, this.table.query),
                this._gisAreaService.getAllAreasForProfessional(waterSupplierId),
                this._coordinateService.getAllForProfessional(waterSupplierId),
                this._gisAreaService.getDefaultViewForProfessional(waterSupplierId)
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

    public showDownloadManager(): void {
        this._downloadService.showDownloadManager(this.downloadConfig!, this.table.query);
    }

    public viewPrintableTable(): void {
        this._printService.open(this._printableSection.nativeElement);
    }

    public viewSite(site: Site): void {
        this._router.navigate([site.id], { relativeTo: this._activatedRoute });
    }

    private getSelectedWaterSupplierId(): number | null {
        const property = this.table.query.filter?.find(p => p.columnName === 'waterSupplier.id');
        const value = property?.value;

        return value != null && value !== '' ? Number(value) : null;
    }

    private buildMapMarkers(sites: Site[]): MapMarker<Site>[] {
        return sites
            .filter(s => s.gisLatitude != null && s.gisLongitude != null)
            .map(s => {
                const siteUrl = this._router.serializeUrl(
                    this._router.createUrlTree([s.id], { relativeTo: this._activatedRoute })
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
