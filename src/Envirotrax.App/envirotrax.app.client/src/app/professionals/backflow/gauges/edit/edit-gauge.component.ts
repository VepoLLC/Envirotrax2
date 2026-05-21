import { Component } from "@angular/core";
import { BackflowGauge } from "../../../../shared/models/backflow/backflow-gauge";
import { BackflowGaugeService } from "../../../../shared/services/backflow/backflow-gauge.service";
import { ModalReference } from "@developer-partners/ngx-modal-dialog";
import { HelperService } from "../../../../shared/services/helpers/helper.service";
import { NgForm } from "@angular/forms";
import { ToastService } from "../../../../shared/services/toast.service";

@Component({
    standalone: false,
    templateUrl: './edit-gauge.component.html'
})
export class EditGaugeComponent {
    public gauge: BackflowGauge;
    public isLoading: boolean = false;
    public validationErrors: string[] = [];

    constructor(
        private readonly _gaugeService: BackflowGaugeService,
        private readonly _modalReference: ModalReference<BackflowGauge>,
        private readonly _helper: HelperService,
        private readonly _toastService: ToastService
    ) {
        this.gauge = { ...this._modalReference.config.model! };
    }

    public async save(form: NgForm): Promise<void> {
        if (form.valid) {
            try {
                this.isLoading = true;
                this.validationErrors = [];

                const result = await this._gaugeService.update(this.gauge);
                this._toastService.successfullySaved('Gauge');
                this._modalReference.closeSuccess(result);
            } catch (error) {
                if (!this._helper.parseValidationErrors(error, this.validationErrors)) {
                    throw error;
                }

                this._toastService.failedToSave('Gauge');
            } finally {
                this.isLoading = false;
            }
        }
    }

    public cancel(): void {
        this._modalReference.cancel();
    }
}
