import { Component, EventEmitter, Input, OnChanges, OnInit, Output, SimpleChanges } from "@angular/core";
import { GisArea, GisAreaCoordinate } from "../../../shared/models/gis-areas/gis-area";
import { GisAreaService } from "../../../shared/services/gis-areas/gis-area.service";
import { GisAreaCoordinateService } from "../../../shared/services/gis-areas/gis-area-coordinate.service";
import { HelperService } from "../../../shared/services/helpers/helper.service";
import { NgForm } from "@angular/forms";
import { ToastService, MapPolygon, ModalHelperService } from '@envirotrax/common-ui';


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
        private readonly _modalHelper: ModalHelperService,
        private readonly _toastService: ToastService
    ) {

    }

    public ngOnInit(): void {
        this.polygonChanged?.subscribe(newPolygon => {
            if (this.polygon?.data) {
                this.polygon.data.coordinates = this._helper.copy(newPolygon.data?.coordinates ?? []);
                this.polygon.coordinates = [...newPolygon.coordinates];

                const holes: { lat: number, lng: number }[][] = [];

                for (const hole of newPolygon.holes ?? []) {
                    holes.push([...hole]);
                }

                this.polygon.holes = holes;
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
        const coordinates = this.polygon?.data?.coordinates;

        if (!coordinates?.length) {
            this.coordinatesText = '';
            return;
        }

        const lines: string[] = [];
        let polygonIndex = 0;

        for (const coordinate of coordinates) {
            const index = coordinate.polygonIndex ?? 0;

            while (polygonIndex < index) {
                lines.push('0, 0');
                polygonIndex++;
            }

            lines.push(`${coordinate.longitude}, ${coordinate.latitude}`);
        }

        this.coordinatesText = lines.join('\n');
    }

    private getCoordinatesModel(): GisAreaCoordinate[] {
        const coordinates: GisAreaCoordinate[] = [];
        let polygonIndex = 0;

        for (const line of this.coordinatesText.split('\n')) {
            const parts = line.split(',');

            if (parts.length < 2) {
                continue;
            }

            const longitude = +(parts[0].trim());
            const latitude = +(parts[1].trim());

            if (isNaN(longitude) || isNaN(latitude)) {
                continue;
            }

            if (longitude === 0 && latitude === 0) {
                polygonIndex++;
                continue;
            }

            coordinates.push({
                polygonIndex: polygonIndex,
                longitude: longitude,
                latitude: latitude
            });
        }

        return coordinates;
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
                this._toastService.successfullySaved('GIS Area');

                this.dataSaved.emit();
            } catch (e) {
                this._toastService.failedToSave('GIS Area');

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
            await this._coordianteService.deleteByArea(this.polygon.data?.area.id!);

            this._toastService.successFullyDeleted('GIS Area');

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