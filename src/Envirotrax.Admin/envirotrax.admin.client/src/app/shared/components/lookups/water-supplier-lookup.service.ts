import { Injectable } from "@angular/core";
import { ModalReference, ModalSize } from "@developer-partners/ngx-modal-dialog";
import { ModalHelperService, PagedData } from "@envirotrax/common-ui";
import { WaterSupplier } from "../../models/water-suppliers/water-supplier";
import { WaterSupplierService } from "../../services/water-suppliers/water-supplier.service";
import { WaterSupplierLookupComponent } from "./water-supplier-lookup.component";

@Injectable({
    providedIn: 'root'
})
export class WaterSupplierLookupService {
    constructor(
        private readonly _waterSupplierService: WaterSupplierService,
        private readonly _modalHelper: ModalHelperService
    ) {

    }

    public async open(): Promise<ModalReference<PagedData<WaterSupplier>, WaterSupplier>> {
        const suppliers = await this._waterSupplierService.getAll({}, { sort: { name: 'Asc' }, filter: [] });

        return this._modalHelper.show<PagedData<WaterSupplier>, WaterSupplier>(WaterSupplierLookupComponent, {
            title: 'Water Suppliers',
            size: ModalSize.large,
            model: suppliers
        });
    }
}
