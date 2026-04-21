import { Component, EventEmitter, Input, Output } from "@angular/core";
import { GisArea, GisAreaCoordinate } from "../../../shared/models/gis-areas/gis-area";
import { GisAreaService } from "../../../shared/services/gis-areas/gis-area.service";
import { GisAreaCoordinateService } from "../../../shared/services/gis-areas/gis-area-coordinate.service";


@Component({
    standalone: false,
    selector: 'vp-create-edit-gis-area',
    templateUrl: './create-edit-gis-area.component.html',
    host: { class: 'd-flex w-100' },
    styles: `
        .vp-color-option {
            border: 1px solid black; 
            width: 20px; 
            height: 20px; 
            border-radius: 3px;
        }

        .vp-color-option.vp-red {
            background-color: #ff0000; 
        }

        .vp-color-option.vp-orange {
            background-color: #ff8000;
        }

        .vp-color-option.vp-yellow {
            background-color: #ffff00;
        }

        .vp-color-option.vp-green {
            background-color: #00ff00;
        }

        .vp-color-option.vp-teal {
            background-color: #00ffff;
        }

        .vp-color-option.vp-blue {
            background-color: #0000ff;
        }

        .vp-color-option.vp-purple {
            background-color: #8000ff;
        }

        .vp-color-option.vp-pink {
            background-color: #ff00ff;
        }
    `
})
export class CreateEditGisAreaComponent {
    public validationErrors: string[] = [];

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
}