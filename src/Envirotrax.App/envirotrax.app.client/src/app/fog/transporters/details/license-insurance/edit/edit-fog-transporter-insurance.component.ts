import { Component } from "@angular/core";
import { NgForm } from "@angular/forms";
import { ModalReference } from "@developer-partners/ngx-modal-dialog";
import { ProfessionalInsurance } from "../../../../../shared/models/professionals/professional-insurance";
import { FogTransporterInsurancesService } from "../../../../../shared/services/fog/fog-transporter-insurances.service";
import { HelperService } from "../../../../../shared/services/helpers/helper.service";
import { ToastService } from "@envirotrax/common-ui";

export interface FogInsuranceModalData {
    transporterId: number;
    insurance: ProfessionalInsurance;
}

@Component({
    standalone: false,
    templateUrl: './edit-fog-transporter-insurance.component.html'
})
export class EditFogTransporterInsuranceComponent {
    public insurance: ProfessionalInsurance;
    public isLoading: boolean = false;
    public validationErrors: string[] = [];
    public certificateFile: File | null = null;
    public isEditMode: boolean = false;

    constructor(
        private readonly _modalReference: ModalReference<FogInsuranceModalData, ProfessionalInsurance>,
        private readonly _insurancesService: FogTransporterInsurancesService,
        private readonly _helper: HelperService,
        private readonly _toastService: ToastService
    ) {
        this.insurance = { ...this._modalReference.config.model!.insurance };
        this.isEditMode = !!this.insurance.id;
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
                ? await this._insurancesService.update(transporterId, this.insurance)
                : await this._insurancesService.add(transporterId, this.insurance, this.certificateFile);

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

    public cancel(): void {
        this._modalReference.cancel();
    }
}
