import { Component, OnInit } from "@angular/core";
import { WaterSupplier, WaterSupplierHierarchy } from "../../shared/models/water-suppliers/water-supplier";
import { WaterSupplierService } from "../../shared/services/water-suppliers/water-supplier.service";

@Component({
    templateUrl: './water-supplier-list.component.html',
    standalone: false
})
export class WaterSupplierListComponent implements OnInit {
    public suppliers: WaterSupplierHierarchy[] = [];
    public isLoading: boolean = false;

    constructor(
        private readonly _supplierService: WaterSupplierService
    ) {

    }

    public async ngOnInit(): Promise<void> {
        try {
            this.isLoading = true;
            this.suppliers = await this._supplierService.getAllMySuppliers();
        } finally {
            this.isLoading = false;
        }
    }
}