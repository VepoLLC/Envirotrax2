import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { ModalReference } from '@developer-partners/ngx-modal-dialog';
import { HelperService, InputOption, ToastService } from '@envirotrax/common-ui';
import { SharedComponentsModule } from '../../../../../shared/components/shared.components.module';
import {
    FogVehicle,
    FogVehicleCapacityType,
    fogVehicleCapacityTypeLabels
} from '../../../../../shared/models/fog/fog-vehicle';
import { FogTransporterVehicleService } from '../../../../../shared/services/fog/fog-transporter-vehicle.service';

export interface FogTransporterVehicleModalData {
    professionalId: number;
    vehicle: FogVehicle;
}

@Component({
    templateUrl: './add-edit-fog-transporter-vehicle.component.html',
    imports: [
        CommonModule,
        FormsModule,
        SharedComponentsModule
    ],
})
export class AddEditFogTransporterVehicleComponent {
    public vehicle: FogVehicle;
    public isLoading: boolean = false;
    public validationErrors: string[] = [];

    public readonly isEditMode: boolean;

    public readonly capacityTypeOptions: InputOption[] = [
        { id: FogVehicleCapacityType.Gallons, text: fogVehicleCapacityTypeLabels[FogVehicleCapacityType.Gallons] },
        { id: FogVehicleCapacityType.CubicYards, text: fogVehicleCapacityTypeLabels[FogVehicleCapacityType.CubicYards] }
    ];

    constructor(
        private readonly _modalReference: ModalReference<FogTransporterVehicleModalData, FogVehicle>,
        private readonly _vehicleService: FogTransporterVehicleService,
        private readonly _helper: HelperService,
        private readonly _toastService: ToastService
    ) {
        this.vehicle = { ...this._modalReference.config.model!.vehicle };
        this.isEditMode = !!this.vehicle.id;
    }

    public async save(form: NgForm): Promise<void> {
        this.validationErrors = [];

        if (!form.valid) {
            return;
        }

        try {
            this.isLoading = true;

            const { professionalId } = this._modalReference.config.model!;

            // vp-input type="number" hands back the raw <input> string, so convert before sending.
            const vehicle: FogVehicle = {
                ...this.vehicle,
                manufacturedYear: Number(this.vehicle.manufacturedYear),
                capacity: Number(this.vehicle.capacity)
            };

            const saved = this.isEditMode
                ? await this._vehicleService.update(professionalId, vehicle)
                : await this._vehicleService.add(professionalId, vehicle);

            this._toastService.successfullySaved('Vehicle');
            this._modalReference.closeSuccess(saved);
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
