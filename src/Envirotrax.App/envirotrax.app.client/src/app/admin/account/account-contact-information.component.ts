import { Component, OnInit } from "@angular/core";
import { NgForm } from "@angular/forms";
import { WaterSupplier } from "../../shared/models/water-suppliers/water-supplier";
import { WaterSupplierService } from "../../shared/services/water-suppliers/water-supplier.service";
import { HelperService } from "../../shared/services/helpers/helper.service";
import { LookupService } from "../../shared/services/lookup/lookup.service";
import { AuthService } from "../../shared/services/auth/auth.service";
import { State } from "../../shared/models/lookup/state";
import { PermissionAction, PermissionType } from "../../shared/models/permission-type";
import { ToastService, InputOption } from '@envirotrax/common-ui';

@Component({
    templateUrl: './account-contact-information.component.html',
    standalone: false
})
export class AccountContactInformationComponent implements OnInit {
    public supplier: WaterSupplier = new WaterSupplier();
    public states: InputOption<State>[] = [];
    public canModify: boolean = false;

    public isLoading: boolean = false;
    public validationErrors: string[] = [];

    constructor(
        private readonly _supplierService: WaterSupplierService,
        private readonly _helper: HelperService,
        private readonly _stateService: LookupService,
        private readonly _authService: AuthService,
        private readonly _toastService: ToastService
    ) {

    }

    public async ngOnInit(): Promise<void> {
        try {
            this.isLoading = true;

            const [states, supplier, canModify] = await Promise.all([
                this._stateService.getAllStatesAsOptions(true),
                this._supplierService.getLoggedInSupplier(),
                this._authService.hasAnyPermisison(PermissionAction.CanModify, PermissionType.AccountInformation)
            ]);

            this.states = states;
            this.supplier = { ...supplier };
            this.canModify = canModify;
        } finally {
            this.isLoading = false;
        }
    }

    public async save(form: NgForm): Promise<void> {
        if (!this.canModify || !form.valid) {
            return;
        }

        try {
            this.isLoading = true;
            this.supplier = await this._supplierService.updateLoggedInSupplier(this.supplier);

            this._toastService.successfullySaved('Account Information');
        } catch (error) {
            if (!this._helper.parseValidationErrors(error, this.validationErrors)) {
                throw error;
            }

            this._toastService.failedToSave('Account Information');
        } finally {
            this.isLoading = false;
        }
    }
}
