import { Component } from "@angular/core";
import { WaterSupplierUser } from "../../../shared/models/users/water-supplier-user";
import { UserService } from "../../../shared/services/water-suppliers/user.service";
import { ModalReference } from "@developer-partners/ngx-modal-dialog";
import { HelperService } from "../../../shared/services/helpers/helper.service";
import { NgForm } from "@angular/forms";

@Component({
    standalone: false,
    templateUrl: './create-user.component.html'
})
export class CreateUserComponent {
    public isLoading: boolean = false;
    public validationErrors: string[] = [];
    public user: WaterSupplierUser = {};

    constructor(
        private readonly _userService: UserService,
        private readonly _modalReference: ModalReference<WaterSupplierUser>,
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