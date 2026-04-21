import { Component, EventEmitter, Input, OnChanges, Output, SimpleChanges } from "@angular/core";
import { GisArea, GisAreaCoordinate } from "../../../shared/models/gis-areas/gis-area";
import { GisAreaService } from "../../../shared/services/gis-areas/gis-area.service";
import { GisAreaCoordinateService } from "../../../shared/services/gis-areas/gis-area-coordinate.service";


@Component({
    standalone: false,
    selector: 'vp-create-edit-gis-area',
    templateUrl: './create-edit-gis-area.component.html',
    host: { class: 'd-flex w-100' },
    styleUrl: './create-edit-gis-area.component.css'
})
export class CreateEditGisAreaComponent implements OnChanges {
    public validationErrors: string[] = [];
    public coordinatesText: string = '';

    @Input()
    public gisArea!: GisArea;

    @Input()
    public coordinates: GisAreaCoordinate[] = [];

    @Output()
    public cancel: EventEmitter<void> = new EventEmitter();

    constructor(
        private readonly _gisAreaService: GisAreaService,
        private readonly _coordianteService: GisAreaCoordinateService
    ) {

    }

    public ngOnChanges(changes: SimpleChanges): void {
        if (changes['coordiantes']) {
            if (this.coordinates) {
                this.coordinatesText = this.coordinates
                    .map(c => `${c.longitude}, ${c.latitude}`)
                    .join('\n');
            } else {
                this.coordinatesText = '';
            }
        }
    }
}