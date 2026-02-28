import { Component } from "@angular/core";
import { ModalReference } from "@developer-partners/ngx-modal-dialog";
import { HelperService } from "../../../../shared/services/helpers/helper.service";
import { NgForm } from "@angular/forms";
import { Role } from "../../../../shared/models/users/role";
import { RoleService } from "../../../../shared/services/users/role.service";

@Component({
    templateUrl: './create-role.component.html',
    standalone: false
})
export class CreateRoleComponent {
    public isLoading: boolean = false;
    public validationErrors: string[] = [];
    public role: Role = {};

    constructor(
        private readonly _modalReference: ModalReference<Role>,
        private readonly _helper: HelperService,
        private readonly _roleService: RoleService
    ) {

    }

    public async save(form: NgForm): Promise<void> {
        if (form.valid) {
            try {
                this.isLoading = true;
                this.validationErrors = [];

                const result = await this._roleService.add(this.role);
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