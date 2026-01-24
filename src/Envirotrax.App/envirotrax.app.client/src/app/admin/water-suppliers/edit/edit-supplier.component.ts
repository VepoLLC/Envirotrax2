import { Component } from "@angular/core";
import { WaterSupplier } from "../../../shared/models/water-suppliers/water-supplier";
import { WaterSupplierService } from "../../../shared/services/water-suppliers/water-supplier.service";
import { ActivatedRoute, Router } from "@angular/router";
import { HelperService } from "../../../shared/services/helpers/helper.service";
import { NgForm } from "@angular/forms";
import { LookupService } from "../../../shared/services/lookup/lookup.service";
import { State } from "../../../shared/models/states/state";


@Component({
    templateUrl: './edit-supplier.component.html',
    standalone: false
})
export class EditSupplierComponent {
    public supplier: WaterSupplier = new WaterSupplier();
    public states: State[] = [];

    public isLoading: boolean = false;
    public validationErrors: string[] = [];

    constructor(
        private readonly _supplierService: WaterSupplierService,
        private readonly _acitvatedRoute: ActivatedRoute,
        private readonly _helper: HelperService,
        private readonly _router: Router,
        private readonly _stateService: LookupService,
        //private readonly _toastService: ToastService
    ) {

    }

    public async ngOnInit(): Promise<void> {
        await this.loadStates();
        this._acitvatedRoute.paramMap.subscribe(async params => {
            const supplierId = params.get('id');
            if (supplierId) {
                await this.getSupplier(+supplierId);
            }
        });
    }

    private async getSupplier(id: number): Promise<void> {
        try {
            this.isLoading = true;

            const apiSupplier = await this._supplierService.get(id);


            this.supplier = {
                ...apiSupplier,
            };
        } finally {
            this.isLoading = false;
        }
    }

    public async save(form: NgForm): Promise<void> {
        if (form.valid) {
            try {
                this.isLoading = true;
                await this._supplierService.update(this.supplier);

                // this._toastService.successfullySaved();
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

    private async loadStates(): Promise<void> {
        this.states = await this._stateService.getAllStates();
    }

}
