import { Component, EventEmitter, OnInit } from "@angular/core";
import { GisAreaService } from "../../../shared/services/gis-areas/gis-area.service";
import { GisAreaCoordinateService } from "../../../shared/services/gis-areas/gis-area-coordinate.service";
import { MAX_PAGE_SIZE } from "../../../shared/models/page-info";
import { HelperService } from "../../../shared/services/helpers/helper.service";
import { GisArea, GisAreaCoordinate } from "../../../shared/models/gis-areas/gis-area";
import { MapPolygon } from "../../../shared/components/map/map.component";
import { AuthService } from "../../../shared/services/auth/auth.service";
import { FeatureType } from "../../../shared/models/feature-tyype";
import { PermissionAction, PermissionType } from "../../../shared/models/permission-type";

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

    public editRecord?: MapPolygon<GisAreaVm>;
    public polygonChanged?: EventEmitter<MapPolygon<GisAreaVm>> = new EventEmitter();
    public canModifyData: boolean = false;

    constructor(
        private readonly _helper: HelperService,
        private readonly _gisAreaService: GisAreaService,
        private readonly _gisAreaCoordinateService: GisAreaCoordinateService,
        private readonly _authService: AuthService
    ) {

    }

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
            this.getDefaultView(),
            this.setPermissions()
        ]);
    }

    private async setPermissions(): Promise<void> {
        this.canModifyData = await this._authService.hasAnyFeatures(FeatureType.ManageGisAreas) &&
            (await this._authService.hasAnyPermisison(PermissionAction.CanCreate, PermissionType.Settings) ||
                await this._authService.hasAnyPermisison(PermissionAction.CanEdit, PermissionType.Settings));
    }

    public async getAreas(): Promise<void> {
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
                onClick: this.edit.bind(this),
                coordinates: allCoordinates
                    .filter(c => c.area?.id === area.id)
                    .map(c => ({ lat: c.latitude!, lng: c.longitude! })),
                data: {
                    area: area,
                    coordinates: allCoordinates.filter(c => c.area?.id === area.id),
                },

            } satisfies MapPolygon<GisAreaVm>));
        } finally {
            this.isLoading = false;
            this.loadingMessage = '';
        }
    }

    public add(): void {
        this.editRecord = {
            color: '#FF00ff',
            name: 'New Area',
            coordinates: [],
            data: {
                area: {
                    name: 'New Area',
                    color: '#FF00ff'
                },
                coordinates: []
            },
            onDrawComplete: (newPalygon: MapPolygon<GisAreaVm>) => {
                newPalygon.onDrawComplete = undefined;

                newPalygon.data!.coordinates = newPalygon.coordinates.map(c => ({
                    latitude: c.lat,
                    longitude: c.lng
                }));

                newPalygon.onEdit = this.createOnEditHandler();

                this.editRecord = newPalygon;
                this.polygons = [...this.polygons]; // re-render: now has onEdit, no onDrawComplete
                this.polygonChanged?.emit(newPalygon);
            }
        };

        this.polygons = [...this.polygons, this.editRecord];
    }

    public edit(polygon: MapPolygon<GisAreaVm>): void {
        polygon.onEdit = this.createOnEditHandler();
        this.editRecord = polygon;
        this.polygons = [...this.polygons]; // re-render to enable drag editing
    }

    public cancelEdit(): void {
        if (this.editRecord) {
            if (!this.editRecord.data?.area.id) {
                this.polygons = this.polygons.filter(p => p !== this.editRecord);
            } else {
                this.editRecord.onEdit = undefined;
                this.polygons = [...this.polygons]; // re-render to remove drag handles
            }
        }
        this.editRecord = undefined;
    }

    private createOnEditHandler(): (polygon: MapPolygon<GisAreaVm>) => void {
        return (polygon) => {
            polygon.data!.coordinates = polygon.coordinates.map(c => ({
                latitude: c.lat,
                longitude: c.lng
            }));
            this.polygonChanged?.emit(polygon);
        };
    }
}

interface GisAreaVm {
    area: GisArea;
    coordinates: GisAreaCoordinate[];
}