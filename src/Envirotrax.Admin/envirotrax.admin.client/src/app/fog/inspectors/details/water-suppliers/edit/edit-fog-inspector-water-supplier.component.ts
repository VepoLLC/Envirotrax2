import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { ModalReference } from '@developer-partners/ngx-modal-dialog';
import { HelperService, ToastService } from '@envirotrax/common-ui';
import { SharedComponentsModule } from '../../../../../shared/components/shared.components.module';
import { ProfessionalWaterSupplier } from '../../../../../shared/models/professionals/professional-water-supplier';
import { FogInspectorWaterSupplierService } from '../../../../../shared/services/fog/fog-inspector-water-supplier.service';

export interface FogInspectorWaterSupplierModalData {
    professionalId: number;
    registration: ProfessionalWaterSupplier;
}

@Component({
    templateUrl: './edit-fog-inspector-water-supplier.component.html',
    imports: [
        CommonModule,
        FormsModule,
        SharedComponentsModule
    ],
})
export class EditFogInspectorWaterSupplierComponent {
    public registration: ProfessionalWaterSupplier;
    public isLoading: boolean = false;
    public validationErrors: string[] = [];

    public readonly idPrefix: string;

    constructor(
        private readonly _modalReference: ModalReference<FogInspectorWaterSupplierModalData, ProfessionalWaterSupplier>,
        private readonly _supplierService: FogInspectorWaterSupplierService,
        private readonly _helper: HelperService,
        private readonly _toastService: ToastService
    ) {
        const { professionalId, registration } = this._modalReference.config.model!;

        this.registration = { ...registration };
        this.idPrefix = `fog-inspector-${professionalId}-registration-${registration.waterSupplier?.id ?? 0}`;
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
                fogInspectorFee: this.toFee(this.registration.fogInspectorFee)
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

    private toFee(value: number | string | null | undefined): number | null {
        if (value == null || value === '') {
            return null;
        }

        const fee = Number(value);

        return isNaN(fee) ? null : fee;
    }
}
