import { Component } from '@angular/core';
import { NgForm } from '@angular/forms';
import { ModalReference } from '@developer-partners/ngx-modal-dialog';
import { WaterSupplierLicense, UpdateWaterSupplierLicense } from '../../shared/models/professionals/licenses/water-supplier-license';
import { WaterSupplierLicenseService } from '../../shared/services/licenses/water-supplier-license.service';
import { HelperService } from '../../shared/services/helpers/helper.service';
import { ToastService } from '../../shared/services/toast.service';

export interface WaterSupplierLicenseModalData {
    license: WaterSupplierLicense;
}

@Component({
    standalone: false,
    templateUrl: './edit-water-supplier-license.component.html'
})
export class EditWaterSupplierLicenseComponent {
    public model: UpdateWaterSupplierLicense;
    public isLoading = false;
    public validationErrors: string[] = [];

    constructor(
        private readonly _modalReference: ModalReference<WaterSupplierLicenseModalData, WaterSupplierLicense>,
        private readonly _licenseService: WaterSupplierLicenseService,
        private readonly _helper: HelperService,
        private readonly _toastService: ToastService
    ) {
        const { license } = this._modalReference.config.model!;
        this.model = {
            licenseNumber: license.licenseNumber ?? '',
            contactName: license.contactName,
            expirationDate: license.expirationDate
        };
    }

    public async save(ngForm: NgForm): Promise<void> {
        this.validationErrors = [];
        if (!ngForm.valid) return;
        try {
            this.isLoading = true;
            const { license } = this._modalReference.config.model!;
            const result = await this._licenseService.update(license.id!, this.model);
            this._toastService.successfullySaved('License');
            this._modalReference.closeSuccess(result);
        } catch (error) {
            if (!this._helper.parseValidationErrors(error, this.validationErrors)) {
                throw error;
            }
            this._toastService.failedToSave('License');
        } finally {
            this.isLoading = false;
        }
    }

    public cancel(): void {
        this._modalReference.cancel();
    }
}
