import { AfterViewInit, Component, ElementRef, EventEmitter, HostListener, Input, NgZone, OnChanges, OnInit, Output, SimpleChanges, ViewChild } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { lastValueFrom, Observable, shareReplay } from "rxjs";
import { UrlResolverService } from "../../services/helpers/url-resolver.service";
import { importLibrary, setOptions } from "@googlemaps/js-api-loader";

@Component({
    standalone: false,
    selector: 'vp-map',
    templateUrl: './map.component.html'
})
export class MapComponent implements OnInit, AfterViewInit, OnChanges {
    private _map!: any;
    private _container!: HTMLElement;
    private _polygonInstances: any[] = [];

    private static _apiKey$?: Observable<ApiKey>;
    private static _mapsLibrary?: any;

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

        // Load the Maps library.
        MapComponent._mapsLibrary ??= await importLibrary('maps');
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
        }

        if (changes['zoom']) {
            this._map.setZoom(this.zoom ?? 1);
        }

        if (changes['polygons']) {
            await this.renderPolygons();
        }
    }

    private async renderPolygons(): Promise<void> {
        this._polygonInstances.forEach(p => p.setMap(null));
        this._polygonInstances = [];

        if (!this.polygons?.length) {
            return;
        }

        const { Polygon } = MapComponent._mapsLibrary as any;

        for (const polygon of this.polygons) {
            const instance = new Polygon({
                paths: polygon.coordinates,
                strokeColor: polygon.color,
                strokeOpacity: 0.8,
                strokeWeight: 1,
                fillColor: polygon.color,
                fillOpacity: 0.2,
            });
            instance.setMap(this._map);
            this._polygonInstances.push(instance);

            if (polygon.onClick) {
                instance.addEventListener('click', () => {
                    this._ngZone.run(() => {
                        if (polygon.onClick) {
                            polygon.onClick(instance);
                        }
                    })
                })
            }
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
    onClick?: (poligon: MapPolygon<TData>) => void;
    data?: TData;
}
