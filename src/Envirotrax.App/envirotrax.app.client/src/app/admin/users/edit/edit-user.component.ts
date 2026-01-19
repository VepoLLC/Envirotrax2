import { Component } from "@angular/core";
import { WaterSupplierUser } from "../../../shared/models/users/water-supplier-user";
import { UserService } from "../../../shared/services/water-suppliers/user.service";
import { ActivatedRoute } from "@angular/router";
import { HelperService } from "../../../shared/services/helpers/helper.service";
import { NgForm } from "@angular/forms";


@Component({
    standalone: false,
    templateUrl: './edit-user.component.html'
})
export class EditUserComponent {
    public user: WaterSupplierUser = {};
    public isLoading: boolean = false;
    public validationErrors: string[] = [];

    constructor(
        private readonly _userService: UserService,
        private readonly _acitvatedRoute: ActivatedRoute,
        private readonly _helper: HelperService,
        //private readonly _toastService: ToastService
    ) {

    }

    public async ngOnInit(): Promise<void> {
        this._acitvatedRoute.paramMap.subscribe(async params => {
            const userId = params.get('id');

            if (userId) {
                await this.getUser(+userId);
            }
        });
    }

    private async getUser(id: number): Promise<void> {
        try {
            this.isLoading = true;
            this.user = await this._userService.get(id);
        } finally {
            this.isLoading = false;
        }
    }

    public async save(form: NgForm): Promise<void> {
        if (form.valid) {
            try {
                this.isLoading = true;

                await this._userService.update(this.user);

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