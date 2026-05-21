import { Component, OnInit } from "@angular/core";
import { ModalReference } from "@developer-partners/ngx-modal-dialog";
import { GisArea, GisAreaCoordinate } from "../../../models/gis-areas/gis-area";
import { GisAreaService } from "../../../services/gis-areas/gis-area.service";
import { GisAreaCoordinateService } from "../../../services/gis-areas/gis-area-coordinate.service";
import { MapPolygon } from "../../map/map.component";

export interface GisAreaSelectionModel {
    currentAreaId: string;
}

@Component({
    standalone: false,
    templateUrl: './gis-area-selection-modal.component.html'
})
export class GisAreaSelectionModalComponent implements OnInit {
    public isLoading: boolean = false;
    public latitude: number = 30.9;
    public longitude: number = -97.2829;
    public zoom: number = 7;
    public polygons: MapPolygon<GisArea>[] = [];
    public selectedAreaId: string | null = null;

    private _areas: GisArea[] = [];
    private _coordinates: GisAreaCoordinate[] = [];

    constructor(
        private readonly _modalReference: ModalReference<GisAreaSelectionModel, string>,
        private readonly _gisAreaService: GisAreaService,
        private readonly _coordinateService: GisAreaCoordinateService
    ) { }

    public async ngOnInit(): Promise<void> {
        try {
            this.isLoading = true;
            await this.loadDataAsync();
        } finally {
            this.isLoading = false;
        }
    }

    private async loadDataAsync(): Promise<void> {
        const [areas, coordinates, defaultView] = await Promise.all([
            this._gisAreaService.getAllAreas(),
            this._coordinateService.getAll(),
            this._gisAreaService.getDefaultView()
        ]);

        this._areas = areas;
        this._coordinates = coordinates;

        if (defaultView.gisCenterLatitude != null) {
            this.latitude = defaultView.gisCenterLatitude;
        }

        if (defaultView.gisCenterLongitude != null) {
            this.longitude = defaultView.gisCenterLongitude;
        }

        if (defaultView.gisCenterZoom != null) {
            this.zoom = defaultView.gisCenterZoom;
        }

        const currentAreaId = this._modalReference.config.model?.currentAreaId;
        if (currentAreaId && currentAreaId !== '' && currentAreaId !== '0') {
            this.selectedAreaId = currentAreaId;
        }

        this.polygons = this.buildPolygons();
    }

    private buildPolygons(): MapPolygon<GisArea>[] {
        return this._areas
            .map((area): MapPolygon<GisArea> | null => {
                const coords = this._coordinates
                    .filter(c => c.area?.id === area.id)
                    .map(c => ({ lat: c.latitude!, lng: c.longitude! }));

                if (coords.length === 0) {
                    return null;
                }

                const isSelected = String(area.id) === this.selectedAreaId;
                return {
                    name: area.name,
                    color: isSelected ? '#0d0772' : (area.color ?? '#000000'), // Highlight selected area
                    coordinates: coords,
                    onClick: (polygon) => this.onPolygonClick(polygon),
                    data: area
                };
            })
            .filter((p): p is MapPolygon<GisArea> => p !== null);
    }

    public onPolygonClick(polygon: MapPolygon<GisArea>): void {
        this.selectedAreaId = String(polygon.data?.id ?? '');
        this.polygons = this.buildPolygons();
    }

    public ok(): void {
        if (this.selectedAreaId) {
            this._modalReference.closeSuccess(this.selectedAreaId);
        }
    }

    public cancel(): void {
        this._modalReference.cancel();
    }
}
