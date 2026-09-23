import { Component } from "@angular/core";
import { NgForm } from "@angular/forms";
import { ModalReference } from "@developer-partners/ngx-modal-dialog";
import { ProfessionalUser } from "../../../../../shared/models/professionals/professional-user";
import { BackflowTesterUserService } from "../../../../../shared/services/backflow/backflow-tester-user.service";
import { HelperService } from "../../../../../shared/services/helpers/helper.service";
import { ToastService } from '@envirotrax/common-ui';

export interface BackflowUserModalData {
    testerId: number;
    user: ProfessionalUser;
}

@Component({
    standalone: false,
    templateUrl: './add-edit-backflow-tester-user.component.html'
})
export class AddEditBackflowTesterUserComponent {
    public user: ProfessionalUser;
    public isLoading: boolean = false;
    public validationErrors: string[] = [];

    public get isEditMode(): boolean {
        return !!this._modalReference.config.model?.user?.id;
    }

    constructor(
        private readonly _modalReference: ModalReference<BackflowUserModalData, ProfessionalUser>,
        private readonly _service: BackflowTesterUserService,
        private readonly _helper: HelperService,
        private readonly _toastService: ToastService
    ) {
        this.user = { ...this._modalReference.config.model!.user };
    }

    public async save(form: NgForm): Promise<void> {
        this.validationErrors = [];

        if (form.valid) {
            try {
                this.isLoading = true;
                const { testerId } = this._modalReference.config.model!;

                const result = this.isEditMode
                    ? await this._service.update(testerId, this.user)
                    : await this._service.add(testerId, this.user);

                this._toastService.successfullySaved('User');
                this._modalReference.closeSuccess(result);
            } catch (error) {
                if (!this._helper.parseValidationErrors(error, this.validationErrors)) {
                    throw error;
                }
                this._toastService.failedToSave('User');
            } finally {
                this.isLoading = false;
            }
        }
    }

    public cancel(): void {
        this._modalReference.cancel();
    }
}
