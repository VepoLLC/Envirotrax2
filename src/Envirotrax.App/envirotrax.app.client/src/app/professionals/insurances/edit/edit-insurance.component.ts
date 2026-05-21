import { Component } from "@angular/core";
import { ProfessionalInsurance } from "../../../shared/models/professionals/professional-insurance";
import { ProfessionalInsuranceService } from "../../../shared/services/professionals/professional-insurance.service";
import { ModalReference } from "@developer-partners/ngx-modal-dialog";
import { HelperService } from "../../../shared/services/helpers/helper.service";
import { NgForm } from "@angular/forms";
import { ToastService } from "../../../shared/services/toast.service";

@Component({
    standalone: false,
    templateUrl: './edit-insurance.component.html'
})
export class EditInsuranceComponent {
    public insurance: ProfessionalInsurance;
    public isLoading: boolean = false;
    public validationErrors: string[] = [];

    constructor(
        private readonly _insuranceService: ProfessionalInsuranceService,
        private readonly _modalReference: ModalReference<ProfessionalInsurance>,
        private readonly _helper: HelperService,
        private readonly _toastService: ToastService
    ) {
        this.insurance = { ...this._modalReference.config.model! };
    }

    public async save(form: NgForm): Promise<void> {
        if (form.valid) {
            try {
                this.isLoading = true;
                this.validationErrors = [];

                const result = await this._insuranceService.update(this.insurance);
                this._toastService.successfullySaved('Insurance');
                this._modalReference.closeSuccess(result);
            } catch (error) {
                if (!this._helper.parseValidationErrors(error, this.validationErrors)) {
                    throw error;
                }

                this._toastService.failedToSave('Insurance');
            } finally {
                this.isLoading = false;
            }
        }
    }

    public cancel(): void {
        this._modalReference.cancel();
    }
}
