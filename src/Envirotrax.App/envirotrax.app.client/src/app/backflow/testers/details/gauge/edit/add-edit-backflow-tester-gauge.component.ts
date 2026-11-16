import { Component } from "@angular/core";
import { NgForm } from "@angular/forms";
import { ModalReference } from "@developer-partners/ngx-modal-dialog";
import { BackflowGauge } from "../../../../../shared/models/backflow/backflow-gauge";
import { BackflowTesterGaugeService } from "../../../../../shared/services/backflow/backflow-tester-gauge.service";
import { HelperService } from "../../../../../shared/services/helpers/helper.service";
import { BackflowTesterUserService } from "../../../../../shared/services/backflow/backflow-tester-user.service";
import { MAX_PAGE_SIZE } from "../../../../../shared/models/page-info";
import { InputOption, ToastService } from '@envirotrax/common-ui';

const COMPANY_OPTION_ID = 0;

export interface BackflowGaugeModalData {
    testerId: number;
    companyName: string;
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
    public assignedToOptions: InputOption[] = [];
    public assignedUserId = COMPANY_OPTION_ID;

    public get isEditMode(): boolean {
        return !!this._modalReference.config.model?.gauge?.id;
    }

    constructor(
        private readonly _modalReference: ModalReference<BackflowGaugeModalData, BackflowGauge>,
        private readonly _gaugeService: BackflowTesterGaugeService,
        private readonly _userService: BackflowTesterUserService,
        private readonly _helper: HelperService,
        private readonly _toastService: ToastService
    ) {
        this.gauge = { ...this._modalReference.config.model!.gauge };

        if (!this.gauge.id) {
            this.loadAssignedToOptions();
        }
    }

    // V1 parity: a water supplier registers a gauge for the company or for one of its testers.
    private async loadAssignedToOptions(): Promise<void> {
        const { testerId, companyName } = this._modalReference.config.model!;

        try {
            this.isLoading = true;

            const testersPage = await this._userService.getSubAccounts(testerId, { pageSize: MAX_PAGE_SIZE }, { sort: {}, filter: [] });

            this.assignedToOptions = [
                { id: COMPANY_OPTION_ID, text: companyName },
                ...(testersPage.data ?? []).map(tester => ({ id: tester.id, text: tester.contactName ?? tester.emailAddress ?? '' }))
            ];
        } finally {
            this.isLoading = false;
        }
    }

    public async save(form: NgForm): Promise<void> {
        if (!form.valid) {
            return;
        }

        try {
            this.isLoading = true;
            this.validationErrors = [];

            const { testerId } = this._modalReference.config.model!;

            if (!this.gauge.id) {
                this.gauge.professionalUser = this.assignedUserId !== COMPANY_OPTION_ID ? { id: this.assignedUserId } : null;
            }

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
