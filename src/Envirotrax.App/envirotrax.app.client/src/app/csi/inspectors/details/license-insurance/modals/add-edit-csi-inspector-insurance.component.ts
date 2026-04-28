import { Component } from "@angular/core";
import { NgForm } from "@angular/forms";
import { ModalReference } from "@developer-partners/ngx-modal-dialog";
import { ProfessionalInsurance } from "../../../../../shared/models/professionals/professional-insurance";
import { CsiInspectorInsurancesService } from "../../../../../shared/services/csi/csi-inspector-insurances.service";
import { HelperService } from "../../../../../shared/services/helpers/helper.service";
import { ToastService } from "../../../../../shared/services/toast.service";

export interface CsiInsuranceModalData {
    inspectorId: number;
    insurance: ProfessionalInsurance;
}

@Component({
    standalone: false,
    templateUrl: './add-edit-csi-inspector-insurance.component.html'
})
export class CsiInspectorAddEditInsuranceComponent {
    public insurance: ProfessionalInsurance;
    public isLoading: boolean = false;
    public validationErrors: string[] = [];
    public hasReadHelp: boolean = false;
    public certificateFile: File | null = null;

    public get isEditMode(): boolean {
        return !!this._modalReference.config.model?.insurance?.id;
    }

    constructor(
        private readonly _modalReference: ModalReference<CsiInsuranceModalData, ProfessionalInsurance>,
        private readonly _insurancesService: CsiInspectorInsurancesService,
        private readonly _helper: HelperService,
        private readonly _toastService: ToastService
    ) {
        this.insurance = { ...this._modalReference.config.model!.insurance };
    }

    public async save(form: NgForm): Promise<void> {
        if (!form.valid) {
            return;
        }

        try {
            this.isLoading = true;
            this.validationErrors = [];

            const { inspectorId } = this._modalReference.config.model!;

            const result = this.isEditMode
                ? await this._insurancesService.update(inspectorId, this.insurance, this.certificateFile)
                : await this._insurancesService.add(inspectorId, this.insurance, this.certificateFile);

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
