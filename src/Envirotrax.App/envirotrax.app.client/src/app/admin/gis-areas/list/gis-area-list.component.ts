import { Component } from "@angular/core";


@Component({
    standalone: false,
    templateUrl: 'gis-area-list.component.html'
})
export class GisAreaListComponent {
    public latitude: number = 30.9;
    public longitude: number = -97.2829;
    public zoom: number = 7;
}