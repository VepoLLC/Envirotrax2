import { Component } from "@angular/core";
import { NgForm } from "@angular/forms";
import { ModalReference } from "@developer-partners/ngx-modal-dialog";
import { ProfessionalUser } from "../../../../../shared/models/professionals/professional-user";
import { FogTransporterSubAccountsService } from "../../../../../shared/services/fog/fog-transporter-user.service";
import { HelperService } from "../../../../../shared/services/helpers/helper.service";
import { ToastService } from "@envirotrax/common-ui";

export interface FogUserModalData {
    transporterId: number;
    user: ProfessionalUser;
}

@Component({
    standalone: false,
    templateUrl: './edit-fog-transporter-user.component.html'
})
export class EditFogTransporterUserComponent {
    public user: ProfessionalUser;
    public isLoading: boolean = false;
    public validationErrors: string[] = [];
    public isEditMode: boolean = false;

    constructor(
        private readonly _modalReference: ModalReference<FogUserModalData, ProfessionalUser>,
        private readonly _service: FogTransporterSubAccountsService,
        private readonly _helper: HelperService,
        private readonly _toastService: ToastService
    ) {
        this.user = { ...this._modalReference.config.model!.user };
        this.isEditMode = !!this.user.id;
    }

    public async save(form: NgForm): Promise<void> {
        this.validationErrors = [];

        if (form.valid) {
            try {
                this.isLoading = true;
                const { transporterId } = this._modalReference.config.model!;

                const result = this.isEditMode
                    ? await this._service.update(transporterId, this.user)
                    : await this._service.add(transporterId, this.user);

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
