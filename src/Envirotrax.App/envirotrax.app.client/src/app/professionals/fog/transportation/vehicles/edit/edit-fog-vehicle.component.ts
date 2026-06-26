import { Component } from "@angular/core";
import { NgForm } from "@angular/forms";
import { ModalReference } from "@developer-partners/ngx-modal-dialog";
import { FogVehicle } from "../../../../../shared/models/fog/fog-vehicle";
import { FogVehicleCapacityType, FOG_VEHICLE_CAPACITY_TYPE_LABELS } from "../../../../../shared/models/fog/fog-vehicle-enums";
import { ProfessionalFogVehicleService } from "../../../../../shared/services/fog/professional-fog-vehicle.service";
import { HelperService } from "../../../../../shared/services/helpers/helper.service";
import { ToastService } from "../../../../../shared/services/toast.service";
import { InputOption } from "@envirotrax/common-ui";

@Component({
    standalone: false,
    templateUrl: './edit-fog-vehicle.component.html'
})
export class EditFogVehicleComponent {
    public vehicle: FogVehicle;
    public isLoading: boolean = false;
    public validationErrors: string[] = [];

    public readonly capacityTypeOptions: InputOption[] = [
        { id: FogVehicleCapacityType.Gallons, text: FOG_VEHICLE_CAPACITY_TYPE_LABELS[FogVehicleCapacityType.Gallons] },
        { id: FogVehicleCapacityType.CubicYards, text: FOG_VEHICLE_CAPACITY_TYPE_LABELS[FogVehicleCapacityType.CubicYards] }
    ];

    constructor(
        private readonly _vehicleService: ProfessionalFogVehicleService,
        private readonly _modalReference: ModalReference<FogVehicle>,
        private readonly _helperService: HelperService,
        private readonly _toastService: ToastService
    ) {
        this.vehicle = { ...this._modalReference.config.model! };
    }

    public async save(form: NgForm): Promise<void> {
        if (form.valid) {
            try {
                this.isLoading = true;
                this.validationErrors = [];
                const result = await this._vehicleService.update(this.vehicle.id!, this.vehicle);
                this._toastService.successfullySaved('Vehicle');
                this._modalReference.closeSuccess(result);
            } catch (error) {
                if (!this._helperService.parseValidationErrors(error, this.validationErrors)) {
                    throw error;
                }
                this._toastService.failedToSave('Vehicle');
            } finally {
                this.isLoading = false;
            }
        }
    }

    public cancel(): void {
        this._modalReference.cancel();
    }
}
