import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { ModalReference } from '@developer-partners/ngx-modal-dialog';
import { HelperService, ToastService } from '@envirotrax/common-ui';
import { SharedComponentsModule } from '../../../../../shared/components/shared.components.module';
import { ProfessionalInsurance } from '../../../../../shared/models/professionals/professional-insurance';
import { CsiInspectorInsuranceService } from '../../../../../shared/services/csi/csi-inspector-insurance.service';

export interface CsiInspectorInsuranceModalData {
    professionalId: number;
    insurance: ProfessionalInsurance;
}

@Component({
    templateUrl: './add-edit-csi-inspector-insurance.component.html',
    imports: [
        CommonModule,
        FormsModule,
        SharedComponentsModule
    ],
})
export class AddEditCsiInspectorInsuranceComponent {
    public insurance: ProfessionalInsurance;
    public certificateFile: File | null = null;
    public isLoading: boolean = false;
    public validationErrors: string[] = [];

    public get isEditMode(): boolean {
        return !!this._modalReference.config.model?.insurance?.id;
    }

    constructor(
        private readonly _modalReference: ModalReference<CsiInspectorInsuranceModalData, ProfessionalInsurance>,
        private readonly _insuranceService: CsiInspectorInsuranceService,
        private readonly _helper: HelperService,
        private readonly _toastService: ToastService
    ) {
        this.insurance = { ...this._modalReference.config.model!.insurance };
    }

    public async save(form: NgForm): Promise<void> {
        this.validationErrors = [];

        if (!form.valid) {
            return;
        }

        try {
            this.isLoading = true;

            const { professionalId } = this._modalReference.config.model!;

            // The certificate is stored once at creation; editing only revises the policy details.
            const saved = this.isEditMode
                ? await this._insuranceService.update(professionalId, this.insurance)
                : await this._insuranceService.add(professionalId, this.insurance, this.certificateFile!);

            this._toastService.successfullySaved('Insurance');
            this._modalReference.closeSuccess(saved);
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
