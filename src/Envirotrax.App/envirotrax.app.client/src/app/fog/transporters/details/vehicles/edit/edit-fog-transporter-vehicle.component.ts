import { Component } from "@angular/core";
import { NgForm } from "@angular/forms";
import { ModalReference } from "@developer-partners/ngx-modal-dialog";
import { FogVehicle } from "../../../../../shared/models/fog/fog-vehicle";
import { FogVehicleCapacityType, FOG_VEHICLE_CAPACITY_TYPE_LABELS } from "../../../../../shared/models/fog/fog-vehicle-enums";
import { FogTransporterVehiclesService } from "../../../../../shared/services/fog/fog-transporter-vehicles.service";
import { HelperService } from "../../../../../shared/services/helpers/helper.service";
import { ToastService } from "../../../../../shared/services/toast.service";
import { InputOption } from "@envirotrax/common-ui";

export interface FogVehicleModalData {
    transporterId: number;
    vehicle: FogVehicle;
}

@Component({
    standalone: false,
    templateUrl: './edit-fog-transporter-vehicle.component.html'
})
export class EditFogTransporterVehicleComponent {
    public vehicle: FogVehicle;
    public isLoading: boolean = false;
    public validationErrors: string[] = [];

    public readonly capacityTypeOptions: InputOption[] = [
        { id: FogVehicleCapacityType.Gallons, text: FOG_VEHICLE_CAPACITY_TYPE_LABELS[FogVehicleCapacityType.Gallons] },
        { id: FogVehicleCapacityType.CubicYards, text: FOG_VEHICLE_CAPACITY_TYPE_LABELS[FogVehicleCapacityType.CubicYards] }
    ];

    public get isEditMode(): boolean {
        return !!this._modalReference.config.model?.vehicle?.id;
    }

    constructor(
        private readonly _modalReference: ModalReference<FogVehicleModalData, FogVehicle>,
        private readonly _vehicleService: FogTransporterVehiclesService,
        private readonly _helper: HelperService,
        private readonly _toastService: ToastService
    ) {
        this.vehicle = { ...this._modalReference.config.model!.vehicle };
    }

    public async save(form: NgForm): Promise<void> {
        if (!form.valid) {
            return;
        }

        try {
            this.isLoading = true;
            this.validationErrors = [];

            const { transporterId } = this._modalReference.config.model!;

            const result = this.isEditMode
                ? await this._vehicleService.update(transporterId, this.vehicle)
                : await this._vehicleService.add(transporterId, this.vehicle);

            this._toastService.successfullySaved('Vehicle');
            this._modalReference.closeSuccess(result);
        } catch (error) {
            if (!this._helper.parseValidationErrors(error, this.validationErrors)) {
                throw error;
            }
            this._toastService.failedToSave('Vehicle');
        } finally {
            this.isLoading = false;
        }
    }

    public cancel(): void {
        this._modalReference.cancel();
    }
}
