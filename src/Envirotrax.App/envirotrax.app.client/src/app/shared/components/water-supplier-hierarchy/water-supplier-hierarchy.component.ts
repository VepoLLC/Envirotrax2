import { Component, Input, OnInit } from "@angular/core";
import { MySupplierHierarchyDto, WaterSupplier } from "../../models/water-suppliers/water-supplier";
import { AuthService } from "../../services/auth/auth.service";
import { AppContainerHelperService } from "../../services/helpers/app-contaner-helper.service";

@Component({
    selector: 'vp-water-supplier-hierarchy',
    standalone: false,
    templateUrl: './water-supplier-hierarchy.component.html'
})
export class WaterSupplierHierarchyComponent implements OnInit {
    public isLoading: boolean = false;

    @Input()
    public mySuppliers?: MySupplierHierarchyDto;

    constructor(
        private readonly _authService: AuthService,
        private readonly _containerHelper: AppContainerHelperService
    ) {

    }

    public ngOnInit(): void {
        this._containerHelper.setContainerVisibility(false);
    }

    public async selectSupplier(supplier: WaterSupplier): Promise<void> {
        try {
            this.isLoading = true;
            await this._authService.signIn(supplier.id);
        } finally {
            this.isLoading = false;
        }
    }
}