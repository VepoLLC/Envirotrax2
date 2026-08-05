import { Component } from "@angular/core";
import { NgForm } from "@angular/forms";
import { ModalReference } from "@developer-partners/ngx-modal-dialog";
import { ProfessionalWaterSupplier } from "../../../../../shared/models/professionals/professional-water-supplier";
import { FogTransporterWaterSuppliersService } from "../../../../../shared/services/fog/fog-transporter-water-suppliers.service";
import { HelperService } from "../../../../../shared/services/helpers/helper.service";
import { ToastService } from "../../../../../shared/services/toast.service";

export interface EditWaterSupplierModalData {
    transporterId: number;
    supplier: ProfessionalWaterSupplier;
}

@Component({
    standalone: false,
    templateUrl: './edit-fog-transporter-water-supplier.component.html'
})
export class EditFogTransporterWaterSupplierComponent {
    public supplier: ProfessionalWaterSupplier;
    public isLoading: boolean = false;
    public validationErrors: string[] = [];

    constructor(
        private readonly _modalReference: ModalReference<EditWaterSupplierModalData, ProfessionalWaterSupplier>,
        private readonly _service: FogTransporterWaterSuppliersService,
        private readonly _helper: HelperService,
        private readonly _toastService: ToastService
    ) {
        this.supplier = { ...this._modalReference.config.model!.supplier };
    }

    public async save(form: NgForm): Promise<void> {
        this.validationErrors = [];

        if (form.valid) {
            try {
                this.isLoading = true;
                const { transporterId } = this._modalReference.config.model!;

                const result = await this._service.updateWaterSupplier(transporterId, this.supplier);
                this._toastService.successfullySaved('Water Supplier Registration');
                this._modalReference.closeSuccess(result);
            } catch (error) {
                if (!this._helper.parseValidationErrors(error, this.validationErrors)) {
                    throw error;
                }
                this._toastService.failedToSave('Water Supplier Registration');
            } finally {
                this.isLoading = false;
            }
        }
    }

    public cancel(): void {
        this._modalReference.cancel();
    }
}
