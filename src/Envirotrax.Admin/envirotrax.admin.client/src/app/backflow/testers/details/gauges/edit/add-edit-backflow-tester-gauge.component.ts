import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { ModalReference } from '@developer-partners/ngx-modal-dialog';
import { HelperService, InputOption, ToastService } from '@envirotrax/common-ui';
import { SharedComponentsModule } from '../../../../../shared/components/shared.components.module';
import { BackflowGauge } from '../../../../../shared/models/backflow/backflow-gauge';
import { BackflowTesterGaugeService } from '../../../../../shared/services/backflow/backflow-tester-gauge.service';
import { BackflowTesterUserService } from '../../../../../shared/services/backflow/backflow-tester-user.service';

const COMPANY_OPTION_ID = 0;

export interface BackflowTesterGaugeModalData {
    professionalId: number;
    companyName: string;
    gauge: BackflowGauge;
}

@Component({
    templateUrl: './add-edit-backflow-tester-gauge.component.html',
    imports: [
        CommonModule,
        FormsModule,
        SharedComponentsModule
    ],
})
export class AddEditBackflowTesterGaugeComponent {
    public gauge: BackflowGauge;
    public calibrationFile: File | null = null;
    public isLoading: boolean = false;
    public validationErrors: string[] = [];
    public assignedToOptions: InputOption[] = [];
    public assignedUserId = COMPANY_OPTION_ID;

    public readonly isEditMode: boolean;

    // Element ids must be unique per window, or a label's `for` would select a radio button in another window.
    public readonly idPrefix: string;

    constructor(
        private readonly _modalReference: ModalReference<BackflowTesterGaugeModalData, BackflowGauge>,
        private readonly _gaugeService: BackflowTesterGaugeService,
        private readonly _userService: BackflowTesterUserService,
        private readonly _helper: HelperService,
        private readonly _toastService: ToastService
    ) {
        const { professionalId, gauge } = this._modalReference.config.model!;

        // The App's gauge update copies FilePath from the request, so it has to travel back as loaded or the
        // calibration certificate would be detached.
        this.gauge = { ...gauge };
        this.isEditMode = !!gauge.id;
        this.idPrefix = `backflow-tester-${professionalId}-gauge-${gauge.id ?? 'new'}`;

        if (!this.isEditMode) {
            this.loadAssignedToOptions();
        }
    }

    // V1 parity: a gauge is registered for the company or for one of its testers, and stays with it.
    private async loadAssignedToOptions(): Promise<void> {
        const { professionalId, companyName } = this._modalReference.config.model!;

        try {
            this.isLoading = true;

            const testerOptions = await this._userService.getAllAsOptions(professionalId);

            this.assignedToOptions = [{ id: COMPANY_OPTION_ID, text: companyName }, ...testerOptions];
        } finally {
            this.isLoading = false;
        }
    }

    public async save(form: NgForm): Promise<void> {
        this.validationErrors = [];

        if (!form.valid) {
            return;
        }

        try {
            this.isLoading = true;

            const { professionalId } = this._modalReference.config.model!;

            if (!this.isEditMode) {
                this.gauge.professionalUser = this.assignedUserId !== COMPANY_OPTION_ID ? { id: this.assignedUserId } : null;
            }

            // The certificate is stored once at creation; editing only revises the gauge details.
            const saved = this.isEditMode
                ? await this._gaugeService.update(professionalId, this.gauge)
                : await this._gaugeService.add(professionalId, this.gauge, this.calibrationFile!);

            this._toastService.successfullySaved('Gauge');
            this._modalReference.closeSuccess(saved);
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
