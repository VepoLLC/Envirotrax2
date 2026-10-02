import { Component } from '@angular/core';
import { NgForm } from '@angular/forms';
import { ModalReference } from '@developer-partners/ngx-modal-dialog';
import { InputOption } from '@envirotrax/common-ui';
import { BackflowDeviceType, BYPASS_DEVICE_TYPES } from '../../../../shared/models/backflow/backflow-test-enums';
import { CsiInspectionAssemblyRequest } from '../../../../shared/models/csi/csi-inspection-assembly';
import { BackflowTestOptionsService } from '../../../../shared/services/backflow/backflow-test-options.service';

// Nothing is saved here: the assembly joins the submission's list and is written with it.
@Component({
    standalone: false,
    templateUrl: './add-csi-inspection-assembly.component.html'
})
export class AddCsiInspectionAssemblyComponent {
    public readonly deviceTypeOptions: InputOption[];
    public readonly hazardTypeOptions: InputOption[];

    public model: CsiInspectionAssemblyRequest = { visuallyIdentified: false };

    public showMainAssembly = true;
    public showBypassAssembly = false;
    public showHazardTypeOtherDescription = false;

    constructor(
        private readonly _modalReference: ModalReference<CsiInspectionAssemblyRequest>,
        options: BackflowTestOptionsService
    ) {
        this.deviceTypeOptions = options.deviceTypeOptions;
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
        if (!form.valid) {
            return;
        }

        this._modalReference.closeSuccess(this.model);
    }

    public cancel(): void {
        this._modalReference.cancel();
    }
}
