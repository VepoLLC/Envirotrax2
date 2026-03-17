import { Component } from "@angular/core";
import { ProfessionalUser } from "../../../shared/models/professionals/professional-user";
import { ProfesionalUserService } from "../../../shared/services/professionals/professional-user.service";
import { ModalReference } from "@developer-partners/ngx-modal-dialog";
import { HelperService } from "../../../shared/services/helpers/helper.service";
import { NgForm } from "@angular/forms";

@Component({
    standalone: false,
    templateUrl: './create-professional-user.component.html'
})
export class CreateProfessionalUserComponent {
    public isLoading: boolean = false;
    public validationErrors: string[] = [];
    public user: ProfessionalUser = {};

    constructor(
        private readonly _userService: ProfesionalUserService,
        private readonly _modalReference: ModalReference<ProfessionalUser>,
        private readonly _helper: HelperService
    ) {

    }

    public async save(form: NgForm): Promise<void> {
        if (form.valid) {
            try {
                this.isLoading = true;
                this.validationErrors = [];

                const result = await this._userService.add(this.user);
                this._modalReference.closeSuccess(result);
            } catch (e) {
                if (!this._helper.parseValidationErrors(e, this.validationErrors)) {
                    throw e;
                }
            } finally {
                this.isLoading = false;
            }
        }
    }

    public cancel(): void {
        this._modalReference.cancel();
    }
}