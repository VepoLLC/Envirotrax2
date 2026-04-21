import { Component, OnInit } from "@angular/core";
import { GisAreaService } from "../../../shared/services/gis-areas/gis-area.service";
import { GisAreaCoordinateService } from "../../../shared/services/gis-areas/gis-area-coordinate.service";
import { MAX_PAGE_SIZE } from "../../../shared/models/page-info";
import { HelperService } from "../../../shared/services/helpers/helper.service";
import { GisArea, GisAreaCoordinate } from "../../../shared/models/gis-areas/gis-area";
import { MapPolygon } from "../../../shared/components/map/map.component";

@Component({
    standalone: false,
    templateUrl: 'gis-area-list.component.html'
})
export class GisAreaListComponent implements OnInit {
    public latitude = 30.9;
    public longitude = -97.2829;
    public zoom = 7;
    public polygons: MapPolygon<GisAreaVm>[] = [];

    public cursorPosition: { lat: number, lng: number } | null = null;

    public isLoading: boolean = false;
    public loadingMessage?: string;

    public editRecord?: GisAreaVm;

    constructor(
        private readonly _helper: HelperService,
        private readonly _gisAreaService: GisAreaService,
        private readonly _gisAreaCoordinateService: GisAreaCoordinateService,
    ) { }

    public onMouseMoved(pos: { lat: number, lng: number }): void {
        this.cursorPosition = pos;
    }

    public onCenterChanged(center: { lat: number, lng: number }): void {
        this.longitude = center.lng;
        this.latitude = center.lat;
    }

    public onZoomChanged(zoom: number): void {
        this.zoom = zoom;
    }

    public async saveDefaultView(): Promise<void> {
        try {
            this.isLoading = true;
            this.loadingMessage = 'Saving Map View';

            await this._gisAreaService.updateDefaultView({
                gisCenterLatitude: this.latitude,
                gisCenterLongitude: this.longitude,
                gisCenterZoom: this.zoom,
            });
        } finally {
            this.isLoading = false;
            this.loadingMessage = '';
        }

        this.getDefaultView();
    }

    private async getDefaultView(): Promise<void> {
        const view = await this._gisAreaService.getDefaultView();

        if (this._helper.isDefined(view.gisCenterLatitude)) {
            this.latitude = view.gisCenterLatitude!;
        }

        if (this._helper.isDefined(view.gisCenterLongitude)) {
            this.longitude = view.gisCenterLongitude!;
        }

        if (this._helper.isDefined(view.gisCenterZoom)) {
            this.zoom = view.gisCenterZoom!;
        }
    }

    public async ngOnInit(): Promise<void> {
        await Promise.all([
            this.getAreas(),
            this.getDefaultView()
        ]);
    }

    private async getAreas(): Promise<void> {
        try {
            this.isLoading = true;
            this.loadingMessage = 'Loading GIS Areas...'

            const [areas, allCoordinates] = await Promise.all([
                this._gisAreaService.getAll({ pageNumber: 1, pageSize: MAX_PAGE_SIZE }, {}),
                this._gisAreaCoordinateService.getAll()
            ]);

            this.polygons = areas.data.map(area => ({
                name: area.name,
                color: area.color ?? '#000000',
                coordinates: allCoordinates
                    .filter(c => c.area?.id === area.id)
                    .map(c => ({ lat: c.latitude!, lng: c.longitde! })),
                data: {
                    area: area,
                    coordinates: allCoordinates.filter(c => c.area?.id === area.id),
                }
            } satisfies MapPolygon<GisAreaVm>));
        } finally {
            this.isLoading = false;
            this.loadingMessage = '';
        }
    }

    public add(): void {
        this.editRecord = {
            area: {},
            coordinates: []
        };
    }

    public cancelEdit(): void {
        this.editRecord = undefined;
    }
}

interface GisAreaVm {
    area: GisArea;
    coordinates: GisAreaCoordinate[];
}