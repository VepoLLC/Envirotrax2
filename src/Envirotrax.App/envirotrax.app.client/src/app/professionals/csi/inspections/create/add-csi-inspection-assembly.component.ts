import { Component } from '@angular/core';
import { NgForm } from '@angular/forms';
import { ModalReference } from '@developer-partners/ngx-modal-dialog';
import { InputOption } from '@envirotrax/common-ui';
import { BackflowDeviceType, BYPASS_DEVICE_TYPES } from '../../../../shared/models/backflow/backflow-test-enums';
import { CsiInspectionNewAssembly } from '../../../../shared/models/csi/csi-inspection-assembly';
import { BackflowTestOptionsService } from '../../../../shared/services/backflow/backflow-test-options.service';

// Nothing is saved here: the assembly joins the form's list and is saved with the inspection.
@Component({
    standalone: false,
    templateUrl: './add-csi-inspection-assembly.component.html'
})
export class AddCsiInspectionAssemblyComponent {
    public readonly deviceTypeOptions: InputOption[];
    public readonly manufacturerOptions: InputOption[];
    public readonly sizeOptions: InputOption[];
    public readonly hazardTypeOptions: InputOption[];

    public model: CsiInspectionNewAssembly = {};

    public showMainAssembly = true;
    public showBypassAssembly = false;
    public showHazardTypeOtherDescription = false;
    public validationErrors: string[] = [];

    constructor(
        private readonly _modalReference: ModalReference<CsiInspectionNewAssembly>,
        options: BackflowTestOptionsService
    ) {
        this.deviceTypeOptions = options.deviceTypeOptions;
        this.manufacturerOptions = options.manufacturerOptions;
        this.sizeOptions = options.sizeOptions;
        this.hazardTypeOptions = options.hazardTypeOptions;
    }

    public onDeviceTypeChange(deviceType: string): void {
        this.model.deviceType = deviceType;
        this.showMainAssembly = deviceType !== BackflowDeviceType.AG;
        this.showBypassAssembly = BYPASS_DEVICE_TYPES.includes(deviceType);
    }

    public onHazardTypeChange(hazardType: string): void {
        this.model.hazardType = hazardType;
        this.showHazardTypeOtherDescription = hazardType === 'Other';
    }

    public add(form: NgForm): void {
        this.validationErrors = [];

        if (!form.valid) {
            return;
        }

        // The required check accepts text made only of spaces, which the server rejects on submit.
        const requiredText = [
            ...(this.showMainAssembly ? [this.model.model, this.model.serialNumber] : []),
            ...(this.showBypassAssembly ? [this.model.model2, this.model.serialNumber2] : []),
            ...(this.showHazardTypeOtherDescription ? [this.model.hazardTypeOtherDescription] : [])
        ];

        if (requiredText.some(value => !value?.trim())) {
            this.validationErrors.push('Required fields cannot contain only spaces.');
            return;
        }

        this._modalReference.closeSuccess(this.model);
    }

    public cancel(): void {
        this._modalReference.cancel();
    }
}
