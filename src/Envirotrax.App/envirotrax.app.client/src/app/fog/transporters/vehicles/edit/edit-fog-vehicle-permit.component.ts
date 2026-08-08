import { Component } from "@angular/core";
import { NgForm } from "@angular/forms";
import { ModalReference } from "@developer-partners/ngx-modal-dialog";
import { FogVehiclePermit, FogVehiclePermitSearch } from "../../../../shared/models/fog/fog-vehicle-permit";
import { FOG_VEHICLE_CAPACITY_TYPE_LABELS } from "../../../../shared/models/fog/fog-vehicle-enums";
import { FogVehiclePermitService } from "../../../../shared/services/fog/fog-vehicle-permit.service";
import { HelperService } from "../../../../shared/services/helpers/helper.service";
import { ToastService } from "../../../../shared/services/toast.service";

export interface FogVehiclePermitModalData {
    vehicle: FogVehiclePermitSearch;
}

@Component({
    standalone: false,
    templateUrl: './edit-fog-vehicle-permit.component.html'
})
export class EditFogVehiclePermitComponent {
    public permit: FogVehiclePermit;
    public isLoading: boolean = false;
    public validationErrors: string[] = [];

    public readonly transporterName: string;
    public readonly vehicleDescription: string;

    private readonly vehicleId: number;

    constructor(
        private readonly _modalReference: ModalReference<FogVehiclePermitModalData, FogVehiclePermitSearch>,
        private readonly _permitService: FogVehiclePermitService,
        private readonly _helper: HelperService,
        private readonly _toastService: ToastService
    ) {
        const vehicle = this._modalReference.config.model!.vehicle;

        this.vehicleId = vehicle.id!;
        this.permit = {
            id: vehicle.id,
            permitNumber: vehicle.permitNumber ?? '',
            inspectionDueDate: vehicle.inspectionDueDate ?? null,
            // V1 defaulted a brand new permit to active.
            isActive: vehicle.hasPermit ? !!vehicle.isActive : true
        };

        this.transporterName = vehicle.transporterCompanyName ?? '';
        this.vehicleDescription = this.buildVehicleDescription(vehicle);
    }

    private buildVehicleDescription(vehicle: FogVehiclePermitSearch): string {
        const capacityTypeLabel = vehicle.capacityType != null
            ? FOG_VEHICLE_CAPACITY_TYPE_LABELS[vehicle.capacityType]
            : '';

        return [
            [vehicle.manufacturedYear, vehicle.manufacturer].filter(part => !!part).join(' '),
            [vehicle.capacity, capacityTypeLabel].filter(part => !!part).join(' '),
            vehicle.licensePlateNumber ? `License Plate #: ${vehicle.licensePlateNumber}` : '',
            vehicle.stickerNumber ? `Sticker #: ${vehicle.stickerNumber}` : ''
        ].filter(part => !!part).join(' — ');
    }

    public async save(form: NgForm): Promise<void> {
        if (!form.valid) {
            return;
        }

        try {
            this.isLoading = true;
            this.validationErrors = [];

            const result = await this._permitService.setPermit(this.vehicleId, this.permit);

            this._toastService.successfullySaved('Permit');
            this._modalReference.closeSuccess(result);
        } catch (error) {
            if (!this._helper.parseValidationErrors(error, this.validationErrors)) {
                throw error;
            }
            this._toastService.failedToSave('Permit');
        } finally {
            this.isLoading = false;
        }
    }

    public cancel(): void {
        this._modalReference.cancel();
    }
}
