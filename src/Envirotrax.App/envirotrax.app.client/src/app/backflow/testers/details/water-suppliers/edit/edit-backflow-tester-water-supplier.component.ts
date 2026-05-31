import { Component } from "@angular/core";
import { NgForm } from "@angular/forms";
import { ModalReference } from "@developer-partners/ngx-modal-dialog";
import { ProfessionalWaterSupplier } from "../../../../../shared/models/professionals/professional-water-supplier";
import { BackflowTesterWaterSuppliersService } from "../../../../../shared/services/backflow/backflow-tester-water-suppliers.service";
import { HelperService } from "../../../../../shared/services/helpers/helper.service";
import { ToastService } from "../../../../../shared/services/toast.service";

export interface EditBackflowWaterSupplierModalData {
    testerId: number;
    supplier: ProfessionalWaterSupplier;
}

@Component({
    standalone: false,
    templateUrl: './edit-backflow-tester-water-supplier.component.html'
})
export class EditBackflowTesterWaterSupplierComponent {
    public supplier: ProfessionalWaterSupplier;
    public isLoading: boolean = false;
    public validationErrors: string[] = [];

    constructor(
        private readonly _modalReference: ModalReference<EditBackflowWaterSupplierModalData, ProfessionalWaterSupplier>,
        private readonly _service: BackflowTesterWaterSuppliersService,
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
                const { testerId } = this._modalReference.config.model!;

                const result = await this._service.updateWaterSupplier(testerId, this.supplier);
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
