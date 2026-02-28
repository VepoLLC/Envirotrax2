import { Component, OnInit } from "@angular/core";
import { HelperService } from "../../../../shared/services/helpers/helper.service";
import { ActivatedRoute } from "@angular/router";
import { NgForm } from "@angular/forms";
import { ModalHelperService } from "../../../../shared/services/helpers/modal-helper.service";
import { Role } from "../../../../shared/models/users/role";
import { RoleService } from "../../../../shared/services/users/role.service";

@Component({
    templateUrl: './edit-role.component.html',
    standalone: false
})
export class EditRoleComponent implements OnInit {
    public isLoading: boolean = false;
    public role: Role = {}
    public validationErrors: string[] = [];

    constructor(
        private readonly _roleService: RoleService,
        private readonly _helper: HelperService,
        private readonly _activeRoute: ActivatedRoute,
        private readonly _modalHelper: ModalHelperService
    ) {

    }

    public ngOnInit(): void {
        this._activeRoute.paramMap.subscribe(async params => {
            const roleId = params.get('id');

            if (roleId) {
                await this.getRole(+roleId);
            }
        });
    }

    private async getRole(id: number): Promise<void> {
        try {
            this.isLoading = true;
            this.role = await this._roleService.get(id);
        } finally {
            this.isLoading = false;
        }
    }

    public async save(form: NgForm): Promise<void> {
        if (form.valid) {
            try {
                this.isLoading = true;

                await this._roleService.update(this.role);

            } catch (error) {
                if (!this._helper.parseValidationErrors(error, this.validationErrors)) {
                    throw error;
                }
            }
        }
    }

    private async processReactivate(): Promise<void> {
        try {
            this.isLoading = true;

            this.role = await this._roleService.reactivate(this.role.id!);
        } finally {
            this.isLoading = false;
        }
    }

    public reactivate(): void {
        this._modalHelper.showReactivateConfirmation()
            .result()
            .subscribe(() => this.processReactivate());
    }
}