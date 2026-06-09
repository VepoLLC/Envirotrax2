import { Component, Input, OnInit } from "@angular/core";
import { MapMarker, MapPolygon } from "./map.component";

@Component({
    standalone: false,
    selector: 'app-map-results',
    templateUrl: './map-results.component.html'
})
export class MapResultsComponent implements OnInit {
    @Input() public markers: MapMarker<any>[] = [];
    @Input() public polygons: MapPolygon<any>[] = [];
    @Input() public latitude: number = 0;
    @Input() public longitude: number = 0;
    @Input() public zoom: number = 10;

    public mouseLat: number | null = null;
    public mouseLng: number | null = null;
    public centerLat: number | null = null;
    public centerLng: number | null = null;
    public currentZoom: number | null = null;

    public ngOnInit(): void {
        this.centerLat = this.latitude;
        this.centerLng = this.longitude;
        this.currentZoom = this.zoom;
    }

    public get mousePosition(): string {
        if (this.mouseLat == null || this.mouseLng == null) return '';
        return `${this.mouseLat.toFixed(6)}, ${this.mouseLng.toFixed(6)}`;
    }

    public get centerPosition(): string {
        if (this.centerLat == null || this.centerLng == null) return '';
        return `${this.centerLat.toFixed(6)}, ${this.centerLng.toFixed(6)}`;
    }

    public onMouseMoved(e: { lat: number; lng: number }): void {
        this.mouseLat = e.lat;
        this.mouseLng = e.lng;
    }

    public onCenterChanged(e: { lat: number; lng: number }): void {
        this.centerLat = e.lat;
        this.centerLng = e.lng;
    }

    public onZoomChanged(zoom: number): void {
        this.currentZoom = zoom;
    }
}
