import { Component } from "@angular/core";
import { WaterSupplier } from "../../../shared/models/water-suppliers/water-supplier";
import { WaterSupplierService } from "../../../shared/services/water-suppliers/water-supplier.service";
import { ModalReference } from "@developer-partners/ngx-modal-dialog";
import { HelperService } from "../../../shared/services/helpers/helper.service";
import { NgForm } from "@angular/forms";
import { createEmptyWaterSupplier } from "../../../shared/factories/water-supplier/water-supplier.factory";

@Component({
    templateUrl: './create-supplier.component.html',
    standalone: false
})
export class CreateSupplierComponent {
    public isLoading: boolean = false;
    public validationErrors: string[] = [];
    public supplier: WaterSupplier = createEmptyWaterSupplier();

    constructor(
        private readonly _supplierService: WaterSupplierService,
        private readonly _modalReference: ModalReference<WaterSupplier>,
        private readonly _helper: HelperService
    ) {

    }

    public async save(form: NgForm): Promise<void> {
        if (form.valid) {
            try {
                this.isLoading = true;
                this.validationErrors = [];

                const result = await this._supplierService.add(this.supplier);
                this._modalReference.closeSuccess(result);
            } catch (e) {
                if (!this._helper.parseValidationErrors(e, this.validationErrors)) {
                    throw e;
                }
            } finally {
                this.isLoading = false;
            }
        }
    }

    public cancel(): void {
        this._modalReference.cancel();
    }
}
