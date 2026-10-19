import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { ModalReference } from '@developer-partners/ngx-modal-dialog';
import { HelperService, ToastService } from '@envirotrax/common-ui';
import { SharedComponentsModule } from '../../../../../shared/components/shared.components.module';
import { ProfessionalInsurance } from '../../../../../shared/models/professionals/professional-insurance';
import { BackflowTesterInsuranceService } from '../../../../../shared/services/backflow/backflow-tester-insurance.service';

export interface BackflowTesterInsuranceModalData {
    professionalId: number;
    insurance: ProfessionalInsurance;
}

@Component({
    templateUrl: './add-edit-backflow-tester-insurance.component.html',
    imports: [
        CommonModule,
        FormsModule,
        SharedComponentsModule
    ],
})
export class AddEditBackflowTesterInsuranceComponent {
    public insurance: ProfessionalInsurance;
    public certificateFile: File | null = null;
    public isLoading: boolean = false;
    public validationErrors: string[] = [];

    public readonly isEditMode: boolean;

    constructor(
        private readonly _modalReference: ModalReference<BackflowTesterInsuranceModalData, ProfessionalInsurance>,
        private readonly _insuranceService: BackflowTesterInsuranceService,
        private readonly _helper: HelperService,
        private readonly _toastService: ToastService
    ) {
        this.insurance = { ...this._modalReference.config.model!.insurance };
        this.isEditMode = !!this.insurance.id;
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
