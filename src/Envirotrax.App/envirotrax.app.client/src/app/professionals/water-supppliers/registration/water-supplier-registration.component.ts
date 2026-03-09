import { Component } from "@angular/core";
import { ModalReference } from "@developer-partners/ngx-modal-dialog";
import { AvailableWaterSupplier, ProfessionalWaterSupplier } from "../../../shared/models/professionals/professional-water-supplier";
import { ProfessionalSupplierService } from "../../../shared/services/professionals/professional-supplier.service";
import { HelperService } from "../../../shared/services/helpers/helper.service";
import { ToastService } from "../../../shared/services/toast.service";

export interface WaterSupplierRegistrationVm {
    availableSupplier: AvailableWaterSupplier;
    selected?: ProfessionalWaterSupplier;
}

@Component({
    templateUrl: './water-supplier-registration.component.html',
    standalone: false
})
export class WaterSupplierRegistrationComponent {
    public isLoading: boolean = false;
    public validationErrors: string[] = [];
    public readonly availableSupplier: AvailableWaterSupplier;
    public registration: ProfessionalWaterSupplier;

    constructor(
        private readonly _modalReference: ModalReference<WaterSupplierRegistrationVm, ProfessionalWaterSupplier>,
        private readonly _supplierService: ProfessionalSupplierService,
        private readonly _helper: HelperService,
        private readonly _toastService: ToastService
    ) {
        const model = _modalReference.config.model!;
        this.availableSupplier = model.availableSupplier;

        this.registration = {
            waterSupplier: { id: model.availableSupplier.id },
            hasBackflowTesting: model.selected?.hasBackflowTesting ?? false,
            hasCsiInspection: model.selected?.hasCsiInspection ?? false,
            hasFogInspection: model.selected?.hasFogInspection ?? false,
            hasFogTransportation: model.selected?.hasFogTransportation ?? false,
        };
    }

    public async save(): Promise<void> {
        try {
            this.isLoading = true;
            this.validationErrors = [];

            let result: ProfessionalWaterSupplier;

            if (this._modalReference.config.model!.selected) {
                result = await this._supplierService.update(this.registration);
            } else {
                result = await this._supplierService.add(this.registration);
            }

            this._modalReference.closeSuccess(result);
            this._toastService.successfullySaved('Registration');
        } catch (e) {
            if (!this._helper.parseValidationErrors(e, this.validationErrors)) {
                throw e;
            }

            this._toastService.failedToSave('Registration');
        } finally {
            this.isLoading = false;
        }
    }

    public cancel(): void {
        this._modalReference.cancel();
    }
}
