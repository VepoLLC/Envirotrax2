import { AfterViewInit, Component, ElementRef, HostListener, Input, input, OnInit, ViewChild } from "@angular/core";
import { HelperService } from "../../services/helpers/helper.service";
import { HttpClient } from "@angular/common/http";
import { lastValueFrom, Observable, shareReplay } from "rxjs";
import { UrlResolverService } from "../../services/helpers/url-resolver.service";
import { importLibrary, setOptions } from "@googlemaps/js-api-loader";

@Component({
    standalone: false,
    selector: 'vp-map',
    templateUrl: './map.component.html'
})
export class MapComponent implements OnInit, AfterViewInit {
    private _map!: any;
    private _container!: HTMLElement;

    private static _apiKey$?: Observable<ApiKey>;

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

    constructor(
        private readonly _helper: HelperService,
        private readonly _http: HttpClient,
        private readonly _urlResolver: UrlResolverService
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
        const { Map } = (await importLibrary('maps'));

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
    }

    @HostListener('window:resize', ['$event'])
    public windowResized(_: any): void {
        this.autoSetHeight = this._container.getBoundingClientRect().height.toString() + 'px';
    }
}

interface ApiKey {
    apiKey: string;
}