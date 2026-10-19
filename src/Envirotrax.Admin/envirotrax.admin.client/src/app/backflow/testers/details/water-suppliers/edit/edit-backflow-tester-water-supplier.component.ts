import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { ModalReference } from '@developer-partners/ngx-modal-dialog';
import { HelperService, ToastService } from '@envirotrax/common-ui';
import { SharedComponentsModule } from '../../../../../shared/components/shared.components.module';
import { ProfessionalWaterSupplier } from '../../../../../shared/models/professionals/professional-water-supplier';
import { BackflowTesterWaterSupplierService } from '../../../../../shared/services/backflow/backflow-tester-water-supplier.service';

export interface BackflowTesterWaterSupplierModalData {
    professionalId: number;
    registration: ProfessionalWaterSupplier;
}

@Component({
    templateUrl: './edit-backflow-tester-water-supplier.component.html',
    imports: [
        CommonModule,
        FormsModule,
        SharedComponentsModule
    ],
})
export class EditBackflowTesterWaterSupplierComponent {
    public registration: ProfessionalWaterSupplier;
    public isLoading: boolean = false;
    public validationErrors: string[] = [];

    // Element ids must be unique per window, or a label's `for` would toggle a checkbox in another window.
    public readonly idPrefix: string;

    constructor(
        private readonly _modalReference: ModalReference<BackflowTesterWaterSupplierModalData, ProfessionalWaterSupplier>,
        private readonly _supplierService: BackflowTesterWaterSupplierService,
        private readonly _helper: HelperService,
        private readonly _toastService: ToastService
    ) {
        const { professionalId, registration } = this._modalReference.config.model!;

        // The App overwrites the whole registration row, so every other program's flags and fees travel back as loaded.
        this.registration = { ...registration };
        this.idPrefix = `backflow-tester-${professionalId}-registration-${registration.waterSupplier?.id ?? 0}`;
    }

    public async save(form: NgForm): Promise<void> {
        this.validationErrors = [];

        if (!form.valid) {
            return;
        }

        try {
            this.isLoading = true;

            const { professionalId } = this._modalReference.config.model!;

            const saved = await this._supplierService.update(professionalId, {
                ...this.registration,
                backflowCommercialTestFee: this.toFee(this.registration.backflowCommercialTestFee),
                backflowResidentialTestFee: this.toFee(this.registration.backflowResidentialTestFee)
            });

            this._toastService.successfullySaved('Water Supplier Registration');
            this._modalReference.closeSuccess(saved);
        } catch (error) {
            if (!this._helper.parseValidationErrors(error, this.validationErrors)) {
                throw error;
            }

            this._toastService.failedToSave('Water Supplier Registration');
        } finally {
            this.isLoading = false;
        }
    }

    public cancel(): void {
        this._modalReference.cancel();
    }

    /**
     * vp-input hands back the raw <input> string, so a cleared fee arrives as '' and an edited one as '25.5'.
     * An empty string is not a valid JSON number for the decimal? column, so coerce both back to number | null.
     */
    private toFee(value: number | string | null | undefined): number | null {
        if (value == null || value === '') {
            return null;
        }

        const fee = Number(value);

        return isNaN(fee) ? null : fee;
    }
}
