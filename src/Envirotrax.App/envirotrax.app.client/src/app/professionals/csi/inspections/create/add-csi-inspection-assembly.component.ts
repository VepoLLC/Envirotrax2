import { Component } from '@angular/core';
import { NgForm } from '@angular/forms';
import { ModalReference } from '@developer-partners/ngx-modal-dialog';
import { InputOption } from '@envirotrax/common-ui';
import { BackflowDeviceType, BYPASS_DEVICE_TYPES } from '../../../../shared/models/backflow/backflow-test-enums';
import { CsiInspectionAssembly, CsiInspectionAssemblyRequest } from '../../../../shared/models/csi/csi-inspection-assembly';
import { BackflowTestOptionsService } from '../../../../shared/services/backflow/backflow-test-options.service';
import { CsiInspectionService } from '../../../../shared/services/csi/csi-inspection.service';
import { HelperService } from '../../../../shared/services/helpers/helper.service';

// Opened with the form's submissionId and siteId; saves the assembly under that submission.
@Component({
    standalone: false,
    templateUrl: './add-csi-inspection-assembly.component.html'
})
export class AddCsiInspectionAssemblyComponent {
    public readonly deviceTypeOptions: InputOption[];
    public readonly manufacturerOptions: InputOption[];
    public readonly sizeOptions: InputOption[];
    public readonly hazardTypeOptions: InputOption[];

    public model: CsiInspectionAssemblyRequest;

    public showMainAssembly = true;
    public showBypassAssembly = false;
    public showHazardTypeOtherDescription = false;
    public isLoading = false;
    public validationErrors: string[] = [];

    constructor(
        private readonly _modalReference: ModalReference<CsiInspectionAssemblyRequest, CsiInspectionAssembly>,
        private readonly _inspectionService: CsiInspectionService,
        private readonly _helper: HelperService,
        options: BackflowTestOptionsService
    ) {
        this.model = { ..._modalReference.config.model! };

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

    public async add(form: NgForm): Promise<void> {
        if (!form.valid) {
            return;
        }

        try {
            this.isLoading = true;
            this.validationErrors = [];

            const assembly = await this._inspectionService.addAssembly(this.model);

            this._modalReference.closeSuccess(assembly);
        } catch (error) {
            if (!this._helper.parseValidationErrors(error, this.validationErrors)) {
                throw error;
            }
        } finally {
            this.isLoading = false;
        }
    }

    public cancel(): void {
        this._modalReference.cancel();
    }
}
