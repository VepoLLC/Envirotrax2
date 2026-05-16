import { Component } from "@angular/core";
import { NgForm } from "@angular/forms";
import { ModalReference } from "@developer-partners/ngx-modal-dialog";
import { ProfessionalUser } from "../../../../../shared/models/professionals/professional-user";
import { CsiInspectorSubAccountsService } from "../../../../../shared/services/csi/csi-inspector-user.service";
import { HelperService } from "../../../../../shared/services/helpers/helper.service";
import { ToastService } from "../../../../../shared/services/toast.service";

export interface CsiUserModalData {
    inspectorId: number;
    user: ProfessionalUser;
}

@Component({
    standalone: false,
    templateUrl: './add-edit-csi-inspector-user.component.html'
})
export class AddEditCsiInspectorUserComponent {
    public user: ProfessionalUser;
    public isLoading: boolean = false;
    public validationErrors: string[] = [];

    public get isEditMode(): boolean {
        return !!this._modalReference.config.model?.user?.id;
    }

    constructor(
        private readonly _modalReference: ModalReference<CsiUserModalData, ProfessionalUser>,
        private readonly _csiInspectorSubAccountService: CsiInspectorSubAccountsService,
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
                const { inspectorId } = this._modalReference.config.model!;

                const result = this.isEditMode
                    ? await this._csiInspectorSubAccountService.update(inspectorId, this.user)
                    : await this._csiInspectorSubAccountService.add(inspectorId, this.user);

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
