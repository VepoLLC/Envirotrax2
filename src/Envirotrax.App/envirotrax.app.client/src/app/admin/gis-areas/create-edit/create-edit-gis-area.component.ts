import { Component, EventEmitter, Input, OnChanges, OnInit, Output, SimpleChanges } from "@angular/core";
import { GisArea, GisAreaCoordinate } from "../../../shared/models/gis-areas/gis-area";
import { GisAreaService } from "../../../shared/services/gis-areas/gis-area.service";
import { GisAreaCoordinateService } from "../../../shared/services/gis-areas/gis-area-coordinate.service";
import { MapPolygon } from "../../../shared/components/map/map.component";
import { HelperService } from "../../../shared/services/helpers/helper.service";
import { NgForm } from "@angular/forms";
import { ModalHelperService } from "../../../shared/services/helpers/modal-helper.service";


@Component({
    standalone: false,
    selector: 'vp-create-edit-gis-area',
    templateUrl: './create-edit-gis-area.component.html',
    host: { class: 'd-flex w-100' },
    styleUrl: './create-edit-gis-area.component.css'
})
export class CreateEditGisAreaComponent implements OnInit, OnChanges {
    public validationErrors: string[] = [];
    public coordinatesText: string = '';
    public isLoading: boolean = false;

    @Input()
    public polygon!: MapPolygon<GisAreaVm>;

    @Input()
    public polygonChanged?: EventEmitter<MapPolygon<GisAreaVm>>;

    @Output()
    public cancel: EventEmitter<void> = new EventEmitter();

    @Output()
    public dataSaved: EventEmitter<void> = new EventEmitter();

    constructor(
        private readonly _gisAreaService: GisAreaService,
        private readonly _coordianteService: GisAreaCoordinateService,
        private readonly _helper: HelperService,
        private readonly _modalHelper: ModalHelperService
    ) {

    }

    public ngOnInit(): void {
        this.polygonChanged?.subscribe(newPolygon => {
            if (this.polygon?.data) {
                this.polygon.data.coordinates = this._helper.copy(newPolygon.data?.coordinates ?? []);
                this.polygon.coordinates = [...newPolygon.coordinates];
            }
            this.setCoordinatesText();
        });
    }

    public ngOnChanges(changes: SimpleChanges): void {
        if (changes['polygon']) {
            this.polygon = this._helper.copy(this.polygon);
            this.setCoordinatesText();
        }
    }

    private setCoordinatesText(): void {
        if (this.polygon?.data?.coordinates?.length) {
            this.coordinatesText = this.polygon
                .data
                .coordinates
                .map(c => `${c.longitude}, ${c.latitude}`)
                .join('\n');
        } else {
            this.coordinatesText = '';
        }
    }

    private getCoordinatesModel(): GisAreaCoordinate[] {
        return this.coordinatesText.split('\n')
            .map(coordinate => {
                const parts = coordinate.split(',');

                return {
                    longitude: +(parts[0].trim()),
                    latitude: +(parts[1].trim())
                };
            });
    }

    public async save(form: NgForm): Promise<void> {
        if (form.valid) {
            try {
                this.isLoading = true;
                this.validationErrors = [];

                const coordiantes = this.getCoordinatesModel();

                const result = !this.polygon.data!.area.id
                    ? await this._gisAreaService.add(this.polygon.data!.area)
                    : await this._gisAreaService.update(this.polygon.data!.area);

                for (let coordinate of coordiantes) {
                    coordinate.area = { id: result.id };
                }

                await this._coordianteService.addOrUpdate(result.id!, coordiantes);

                this.dataSaved.emit();
            } catch (e) {
                if (!this._helper.parseValidationErrors(e, this.validationErrors)) {
                    throw e;
                }
            } finally {
                this.isLoading = false;
            }
        }
    }

    private async processDelete(): Promise<void> {
        try {
            this.isLoading = true;

            await this._gisAreaService.delete(this.polygon.data?.area.id!);
            this.dataSaved.emit();
        } finally {
            this.isLoading = false;
        }
    }

    public delete(): void {
        this._modalHelper.showDeleteConfirmation()
            .result()
            .subscribe(() => this.processDelete());
    }
}

interface GisAreaVm {
    area: GisArea,
    coordinates: GisAreaCoordinate[]
}