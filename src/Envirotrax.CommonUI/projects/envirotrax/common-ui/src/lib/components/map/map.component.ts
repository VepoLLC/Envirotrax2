import { AfterViewInit, Component, ElementRef, EventEmitter, HostListener, Input, NgZone, OnChanges, OnInit, Output, SimpleChanges, ViewChild } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { lastValueFrom, Observable, shareReplay } from "rxjs";
import { UrlResolverService } from "../../services/helpers/url-resolver.service";
import { importLibrary, setOptions } from "@googlemaps/js-api-loader";

const MIN_POLYGON_VERTICES = 3;
const COORDINATE_EPSILON = 1e-9;
const VERTEX_ICON_PATH = 'M -5,0 a 5,5 0 1,0 10,0 a 5,5 0 1,0 -10,0';

@Component({
    standalone: false,
    selector: 'vp-map',
    templateUrl: './map.component.html'
})
export class MapComponent implements OnInit, AfterViewInit, OnChanges {
    private _map!: any;
    private _container!: HTMLElement;
    private _polygonInstances: any[] = [];
    private _markerInstances: any[] = [];
    private _infoWindow: any;

    private _drawingPolygon?: MapPolygon<any>;
    private _drawingPath: { lat: number, lng: number }[] = [];
    private _drawingLine: any;
    private _drawingVertexMarkers: any[] = [];
    private _drawingListeners: any[] = [];

    private static _apiKey$?: Observable<ApiKey>;
    private static _mapsLibrary?: any;
    private static _markerLibrary?: any;

    public autoSetHeight?: string;

    @ViewChild('mapElement', { static: true })
    public mapElement!: ElementRef<HTMLElement>;

    @Input()
    public latitude?: number;

    @Input()
    public longitude?: number;

    @Input()
    public zoom?: number;

    @Input()
    public height?: string;

    @Input()
    public polygons?: MapPolygon<any>[];

    @Input()
    public showMarker?: boolean;

    @Input()
    public markers?: MapMarker<any>[];

    @Output()
    public mouseMoved = new EventEmitter<{ lat: number, lng: number }>();

    @Output()
    public centerChanged = new EventEmitter<{ lat: number, lng: number }>();

    @Output()
    public zoomChanged = new EventEmitter<number>();

    constructor(
        private readonly _http: HttpClient,
        private readonly _urlResolver: UrlResolverService,
        private readonly _ngZone: NgZone
    ) {

    }

    private getApiKey(): Promise<ApiKey> {
        if (!MapComponent._apiKey$) {
            const url = this._urlResolver.resolveUrl('/api/google-maps/api-key');
            MapComponent._apiKey$ = this._http.get<ApiKey>(url).pipe(shareReplay(1));
        }

        return lastValueFrom(MapComponent._apiKey$);
    }

    public async ngOnInit(): Promise<void> {
        this._container = document.getElementById('main-content')!;
    }

    public async ngAfterViewInit(): Promise<void> {
        const apiKey = await this.getApiKey();
        setOptions({ key: apiKey.apiKey });

        // Load the Maps and Marker libraries.
        MapComponent._mapsLibrary ??= await importLibrary('maps');
        MapComponent._markerLibrary ??= await importLibrary('marker');
        const { Map } = MapComponent._mapsLibrary;

        // Set map options.
        const mapOptions = {
            center: { lat: this.latitude ?? 0, lng: this.longitude ?? 0 },
            zoom: this.zoom ?? 1,
            clickableIcons: false,
            gestureHandling: "greedy",
            streetViewControl: false,
            zoomControl: false,
        };

        this.autoSetHeight = this._container.getBoundingClientRect().height.toString() + 'px';

        // Declare the map.
        this._map = new Map(
            this.mapElement.nativeElement,
            mapOptions
        );

        await this.renderPolygons();
        this.renderMarkers();

        this._map.addListener('mousemove', (event: any) => {
            this._ngZone.run(() => this.mouseMoved.emit({ lat: event.latLng.lat(), lng: event.latLng.lng() }));
        });

        this._map.addListener('center_changed', () => {
            const center = this._map.getCenter();
            this._ngZone.run(() => this.centerChanged.emit({ lat: center.lat(), lng: center.lng() }));
        });

        this._map.addListener('zoom_changed', () => {
            this._ngZone.run(() => this.zoomChanged.emit(this._map.getZoom()));
        });
    }

    public async ngOnChanges(changes: SimpleChanges): Promise<void> {
        if (!this._map) {
            return;
        }

        if (changes['latitude'] || changes['longitude']) {
            this._map.setCenter({ lat: this.latitude ?? 0, lng: this.longitude ?? 0 });
            this.renderMarkers();
        }

        if (changes['zoom']) {
            this._map.setZoom(this.zoom ?? 1);
        }

        if (changes['polygons']) {
            await this.renderPolygons();
        }

        if (changes['showMarker'] || changes['markers']) {
            this.renderMarkers();
        }
    }

    private renderMarkers(): void {
        this._markerInstances.forEach(m => m.setMap(null));
        this._markerInstances = [];

        if (!this._map) {
            return;
        }

        const { Marker } = MapComponent._markerLibrary as any;

        if (this.showMarker && this.latitude != null && this.longitude != null) {
            this._markerInstances.push(new Marker({
                position: { lat: this.latitude, lng: this.longitude },
                map: this._map
            }));
        }

        if (this.markers?.length) {
            const { InfoWindow } = MapComponent._mapsLibrary as any;

            if (!this._infoWindow) {
                this._infoWindow = new InfoWindow();
            }

            for (const marker of this.markers) {
                const instance = new Marker({
                    position: { lat: marker.lat, lng: marker.lng },
                    map: this._map,
                    ...(marker.icon ? { icon: marker.icon } : {})
                });

                if (marker.popupHtml) {
                    instance.addListener('click', () => {
                        this._infoWindow.setContent(marker.popupHtml);
                        this._infoWindow.open({ anchor: instance, map: this._map });
                    });
                }

                this._markerInstances.push(instance);
            }
        }
    }

    private async renderPolygons(): Promise<void> {
        this._polygonInstances.forEach(p => p.setMap(null));
        this._polygonInstances = [];

        this.stopPolygonDrawing();

        if (!this.polygons?.length) {
            return;
        }

        const { Polygon } = MapComponent._mapsLibrary as any;
        const drawingPolygon = this.polygons.find(p => p.onDrawComplete);

        for (const polygon of this.polygons) {
            if (polygon === drawingPolygon) {
                continue;
            }

            const instance = new Polygon({
                paths: polygon.coordinates,
                strokeColor: polygon.color,
                strokeOpacity: 0.8,
                strokeWeight: 1,
                fillColor: polygon.color,
                fillOpacity: 0.2,
                editable: !!polygon.onEdit,
                clickable: !!polygon.onClick && !drawingPolygon
            });
            instance.setMap(this._map);
            this._polygonInstances.push(instance);

            if (polygon.onClick && !drawingPolygon) {
                instance.addListener('click', () => {
                    this._ngZone.run(() => {
                        if (polygon.onClick) {
                            polygon.onClick(polygon);
                        }
                    })
                })
            }

            if (polygon.onEdit) {
                const path = instance.getPath();
                if (path) {
                    path.addListener("set_at", () => this._ngZone.run(() => this.onPolygonEdit(polygon, instance)));
                    path.addListener("insert_at", () => this._ngZone.run(() => this.onPolygonEdit(polygon, instance)));
                }
            }
        }

        if (drawingPolygon) {
            this.startPolygonDrawing(drawingPolygon);
        }
    }

    private startPolygonDrawing(polygon: MapPolygon<any>): void {
        const { Polyline } = MapComponent._mapsLibrary as any;

        this._drawingPolygon = polygon;
        this._drawingPath = [];

        this._drawingLine = new Polyline({
            map: this._map,
            path: [],
            strokeColor: polygon.color,
            strokeOpacity: 0.8,
            strokeWeight: 2,
            clickable: false
        });

        this._map.setOptions({ draggableCursor: 'crosshair', disableDoubleClickZoom: true });

        this._ngZone.runOutsideAngular(() => {
            this._drawingListeners.push(this._map.addListener('click', (event: any) => {
                this.addDrawingVertex(event.latLng.lat(), event.latLng.lng());
            }));

            this._drawingListeners.push(this._map.addListener('mousemove', (event: any) => {
                this.updateDrawingLine({ lat: event.latLng.lat(), lng: event.latLng.lng() });
            }));

            this._drawingListeners.push(this._map.addListener('dblclick', () => {
                this.completePolygonDrawing();
            }));
        });
    }

    private addDrawingVertex(lat: number, lng: number): void {
        const { Marker } = MapComponent._markerLibrary as any;
        const isFirstVertex = this._drawingPath.length === 0;

        this._drawingPath.push({ lat: lat, lng: lng });
        this.updateDrawingLine();

        const marker = new Marker({
            position: { lat: lat, lng: lng },
            map: this._map,
            clickable: isFirstVertex,
            cursor: isFirstVertex ? 'pointer' : undefined,
            icon: {
                path: VERTEX_ICON_PATH,
                fillColor: '#ffffff',
                fillOpacity: 1,
                strokeColor: this._drawingPolygon!.color,
                strokeWeight: 2
            }
        });

        this._drawingVertexMarkers.push(marker);

        if (isFirstVertex) {
            this._drawingListeners.push(marker.addListener('click', () => this.completePolygonDrawing()));
        }
    }

    private updateDrawingLine(cursorPosition?: { lat: number, lng: number }): void {
        if (!this._drawingLine) {
            return;
        }

        const path = [...this._drawingPath];

        if (cursorPosition && path.length) {
            path.push(cursorPosition);
        }

        this._drawingLine.setPath(path);
    }

    private completePolygonDrawing(): void {
        const polygon = this._drawingPolygon;

        if (!polygon) {
            return;
        }

        this.removeDuplicatedLastVertex();

        if (this._drawingPath.length < MIN_POLYGON_VERTICES) {
            return;
        }

        polygon.coordinates = this._drawingPath.map(point => ({ lat: point.lat, lng: point.lng }));

        this.stopPolygonDrawing();

        this._ngZone.run(() => {
            if (polygon.onDrawComplete) {
                polygon.onDrawComplete(polygon);
            }
        });
    }

    private removeDuplicatedLastVertex(): void {
        if (this._drawingPath.length < 2) {
            return;
        }

        const last = this._drawingPath[this._drawingPath.length - 1];
        const previous = this._drawingPath[this._drawingPath.length - 2];

        const isSamePoint = Math.abs(last.lat - previous.lat) < COORDINATE_EPSILON &&
            Math.abs(last.lng - previous.lng) < COORDINATE_EPSILON;

        if (isSamePoint) {
            this._drawingPath.pop();
            this._drawingVertexMarkers.pop()?.setMap(null);
        }
    }

    private stopPolygonDrawing(): void {
        this._drawingListeners.forEach(listener => listener.remove());
        this._drawingListeners = [];

        this._drawingVertexMarkers.forEach(marker => marker.setMap(null));
        this._drawingVertexMarkers = [];

        if (this._drawingLine) {
            this._drawingLine.setMap(null);
            this._drawingLine = null;
        }

        this._drawingPath = [];
        this._drawingPolygon = undefined;

        if (this._map) {
            this._map.setOptions({ draggableCursor: null, disableDoubleClickZoom: false });
        }
    }

    private onPolygonEdit(polygonVm: MapPolygon<any>, polygonInstance: any): void {
        const coordinates: { lat: number; lng: number }[] = [];
        const vertices = polygonInstance.getPath();

        for (let i = 0; i < vertices.getLength(); i++) {
            const xy = vertices.getAt(i);

            coordinates.push({
                lat: xy.lat(),
                lng: xy.lng()
            });
        }

        polygonVm.coordinates = coordinates;

        if (polygonVm.onEdit) {
            polygonVm.onEdit(polygonVm);
        }
    }

    @HostListener('window:resize', ['$event'])
    public windowResized(_: any): void {
        this.autoSetHeight = this._container.getBoundingClientRect().height.toString() + 'px';
    }
}

interface ApiKey {
    apiKey: string;
}

export interface MapPolygon<TData extends any> {
    name?: string;
    color: string;
    coordinates: { lat: number; lng: number }[];
    onClick?: (polygon: MapPolygon<TData>) => void;
    onEdit?: (polygon: MapPolygon<TData>) => void;
    onDrawComplete?: (polygon: MapPolygon<TData>) => void;
    data?: TData;
}

export interface MapMarker<TData = any> {
    lat: number;
    lng: number;
    popupHtml?: string;
    icon?: any;
    data?: TData;
}