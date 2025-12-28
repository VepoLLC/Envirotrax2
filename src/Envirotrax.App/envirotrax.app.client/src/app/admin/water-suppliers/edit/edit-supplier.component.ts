import { Component } from "@angular/core";
import { WaterSupplier } from "../../../shared/models/water-suppliers/water-supplier";
import { createEmptyWaterSupplier } from "../../../shared/factories/water-supplier/water-supplier.factory";
import { WaterSupplierService } from "../../../shared/services/water-suppliers/water-supplier.service";
import { ActivatedRoute } from "@angular/router";
import { HelperService } from "../../../shared/services/helpers/helper.service";
import { NgForm } from "@angular/forms";

@Component({
    templateUrl: './edit-supplier.component.html',
    standalone: false
})
export class EditSupplierComponent {
    public supplier: WaterSupplier = createEmptyWaterSupplier();

    public isLoading: boolean = false;
    public validationErrors: string[] = [];

    constructor(
        private readonly _supplierService: WaterSupplierService,
        private readonly _acitvatedRoute: ActivatedRoute,
        private readonly _helper: HelperService,
        //private readonly _toastService: ToastService
    ) {

    }

    public async ngOnInit(): Promise<void> {
        this._acitvatedRoute.paramMap.subscribe(async params => {
          const supplierId = params.get('id');
          console.log(supplierId);
            if (supplierId) {
                await this.getSupplier(+supplierId);
            }
        });
    }

    //private async getSupplier(id: number): Promise<void> {
    //    try {
    //        this.isLoading = true;
    //        this.supplier = await this._supplierService.get(id);
    //    } finally {
    //        this.isLoading = false;
    //    }
  //}

    private async getSupplier(id: number): Promise<void> {
        try {
          this.isLoading = true;

          const apiSupplier = await this._supplierService.get(id);

          this.supplier = {
            ...createEmptyWaterSupplier(),
            ...apiSupplier,
            letterReturn: {
              ...createEmptyWaterSupplier().letterReturn,
              ...apiSupplier.letterReturn
            }
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

                //this._toastService.successfullySaved();
            } catch (error) {
                if (!this._helper.parseValidationErrors(error, this.validationErrors)) {
                    throw error;
                }

                //this._toastService.failedToSave();
            }
        }
    }
}
