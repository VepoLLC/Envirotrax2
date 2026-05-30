import { Component } from "@angular/core";
import { NgForm } from "@angular/forms";
import { ModalReference } from "@developer-partners/ngx-modal-dialog";
import { BackflowGauge } from "../../../../../shared/models/backflow/backflow-gauge";
import { BackflowTesterGaugeService } from "../../../../../shared/services/backflow/backflow-tester-gauge.service";
import { HelperService } from "../../../../../shared/services/helpers/helper.service";
import { ToastService } from "../../../../../shared/services/toast.service";

export interface BackflowGaugeModalData {
    testerId: number;
    gauge: BackflowGauge;
}

@Component({
    standalone: false,
    templateUrl: './add-edit-backflow-tester-gauge.component.html'
})
export class AddEditBackflowTesterGaugeComponent {
    public gauge: BackflowGauge;
    public isLoading: boolean = false;
    public validationErrors: string[] = [];
    public calibrationFile: File | null = null;

    public get isEditMode(): boolean {
        return !!this._modalReference.config.model?.gauge?.id;
    }

    constructor(
        private readonly _modalReference: ModalReference<BackflowGaugeModalData, BackflowGauge>,
        private readonly _gaugeService: BackflowTesterGaugeService,
        private readonly _helper: HelperService,
        private readonly _toastService: ToastService
    ) {
        this.gauge = { ...this._modalReference.config.model!.gauge };
    }

    public async save(form: NgForm): Promise<void> {
        if (!form.valid) {
            return;
        }

        try {
            this.isLoading = true;
            this.validationErrors = [];

            const { testerId } = this._modalReference.config.model!;

            const result = this.isEditMode
                ? await this._gaugeService.update(testerId, this.gauge)
                : await this._gaugeService.add(testerId, this.gauge, this.calibrationFile!);

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

    public cancel(): void {
        this._modalReference.cancel();
    }
}
