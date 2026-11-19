import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { ModalReference } from '@developer-partners/ngx-modal-dialog';
import { HelperService, RecordLog, ToastService } from '@envirotrax/common-ui';
import { SharedComponentsModule } from '../../../../../shared/components/shared.components.module';
import { ProfessionalInsurance } from '../../../../../shared/models/professionals/professional-insurance';
import { BackflowTesterInsuranceService } from '../../../../../shared/services/backflow/backflow-tester-insurance.service';
import { InsuranceService } from '../../../../../shared/services/licenses/insurance.service';

export interface BackflowTesterInsuranceModalData {
    professionalId: number;
    insurance: ProfessionalInsurance;
}

@Component({
    templateUrl: './add-edit-backflow-tester-insurance.component.html',
    imports: [
        CommonModule,
        FormsModule,
        SharedComponentsModule
    ],
})
export class AddEditBackflowTesterInsuranceComponent implements OnInit {
    public insurance: ProfessionalInsurance;
    public certificateFile: File | null = null;
    public isLoading: boolean = false;
    public isLoadingRecordLogs: boolean = false;
    public validationErrors: string[] = [];
    public recordLogs: RecordLog[] = [];
    public recordLogTitle: string = 'Record Log';

    public readonly isEditMode: boolean;

    constructor(
        private readonly _modalReference: ModalReference<BackflowTesterInsuranceModalData, ProfessionalInsurance>,
        private readonly _insuranceService: BackflowTesterInsuranceService,
        private readonly _helper: HelperService,
        private readonly _toastService: ToastService,
        private readonly _generalInsuranceService: InsuranceService
    ) {
        this.insurance = { ...this._modalReference.config.model!.insurance };
        this.isEditMode = !!this.insurance.id;
    }

    public async ngOnInit(): Promise<void> {
        await this.loadRecordLogs();
    }

    public async save(form: NgForm): Promise<void> {
        this.validationErrors = [];

        if (!form.valid) {
            return;
        }

        try {
            this.isLoading = true;

            const { professionalId } = this._modalReference.config.model!;

            // The certificate is stored once at creation; editing only revises the policy details.
            const saved = this.isEditMode
                ? await this._insuranceService.update(professionalId, this.insurance)
                : await this._insuranceService.add(professionalId, this.insurance, this.certificateFile!);

            this._toastService.successfullySaved('Insurance');
            this._modalReference.closeSuccess(saved);
        } catch (error) {
            if (!this._helper.parseValidationErrors(error, this.validationErrors)) {
                throw error;
            }

            this._toastService.failedToSave('Insurance');
        } finally {
            this.isLoading = false;
        }
    }

    public cancel(): void {
        this._modalReference.cancel();
    }

    private async loadRecordLogs(): Promise<void> {
        const id = this.insurance.id;

        if (!id) {
            return;
        }

        try {
            this.isLoadingRecordLogs = true;
            this.recordLogs = await this._generalInsuranceService.getLogs(id);
            this.recordLogTitle = `Record Log (${this.recordLogs.length})`;
        } finally {
            this.isLoadingRecordLogs = false;
        }
    }
}
