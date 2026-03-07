import { Component, OnInit } from "@angular/core";
import { WaterSupplierUser } from "../../../shared/models/users/water-supplier-user";
import { UserService } from "../../../shared/services/water-suppliers/user.service";
import { ActivatedRoute } from "@angular/router";
import { HelperService } from "../../../shared/services/helpers/helper.service";
import { NgForm } from "@angular/forms";
import { InputOption } from "../../../shared/components/input/input.component";
import { RoleService } from "../../../shared/services/users/role.service";
import { UserRoleService } from "../../../shared/services/users/user-role.service";
import { MAX_PAGE_SIZE } from "../../../shared/models/page-info";
import { ModalHelperService } from "../../../shared/services/helpers/modal-helper.service";


@Component({
    standalone: false,
    templateUrl: './edit-user.component.html'
})
export class EditUserComponent implements OnInit {
    public user: WaterSupplierUser = {};
    public isLoading: boolean = false;
    public validationErrors: string[] = [];

    public roleOptions: InputOption[] = [];
    public selectedRoleIds: number[] = [];
    private _originalRoleIds: number[] = [];

    constructor(
        private readonly _userService: UserService,
        private readonly _roleService: RoleService,
        private readonly _userRoleService: UserRoleService,
        private readonly _acitvatedRoute: ActivatedRoute,
        private readonly _helper: HelperService,
        private readonly _modalHelper: ModalHelperService,
        //private readonly _toastService: ToastService
    ) {

    }

    public async ngOnInit(): Promise<void> {
        this._acitvatedRoute.paramMap.subscribe(async params => {
            const userId = params.get('id');

            if (userId) {
                this.getUser(+userId);
            }
        });
    }

    private async getUser(id: number): Promise<void> {
        try {
            this.isLoading = true;
            this.user = await this._userService.get(id);

            if (this.user) {
                await Promise.all([
                    this.loadRoles(),
                    this.loadUserRoles(id)
                ]);
            }
        } finally {
            this.isLoading = false;
        }
    }

    private async loadRoles(): Promise<void> {
        const result = await this._roleService.getAll({ pageNumber: 1, pageSize: MAX_PAGE_SIZE }, {});
        this.roleOptions = result.data?.map(r => ({ id: r.id, text: r.name })) ?? [];
    }

    private async loadUserRoles(userId: number): Promise<void> {
        const userRoles = await this._userRoleService.getAll(userId);
        this._originalRoleIds = userRoles.map(ur => ur.role!.id!);
        this.selectedRoleIds = [...this._originalRoleIds];
    }

    public resendInvitation(): void {
        this._modalHelper.confirm({
            messages: ['Are you sure you want to resend the invitation to this user?']
        }).result().subscribe(async () => {
            try {
                this.isLoading = true;
                await this._userService.resendInvitation(this.user.id!);
            } finally {
                this.isLoading = false;
            }
        });
    }

    public async save(form: NgForm): Promise<void> {
        if (form.valid) {
            try {
                this.isLoading = true;

                await this._userService.update(this.user);

                const userId = this.user.id!;
                const addedIds = this.selectedRoleIds.filter(id => !this._originalRoleIds.includes(id));
                const deletedIds = this._originalRoleIds.filter(id => !this.selectedRoleIds.includes(id));

                await Promise.all([
                    ...addedIds.map(id => this._userRoleService.add({ user: { id: userId }, role: { id } })),
                    ...deletedIds.map(id => this._userRoleService.delete(userId, id))
                ]);

                this._originalRoleIds = [...this.selectedRoleIds];

                //this._toastService.successfullySaved();
            } catch (error) {
                if (!this._helper.parseValidationErrors(error, this.validationErrors)) {
                    throw error;
                }

                //this._toastService.failedToSave();
            } finally {
                this.isLoading = false;
            }
        }
    }
}