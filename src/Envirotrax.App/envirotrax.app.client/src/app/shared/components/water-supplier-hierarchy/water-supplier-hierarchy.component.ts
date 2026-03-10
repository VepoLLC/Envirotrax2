import { Component, Input } from "@angular/core";
import { WaterSupplier, WaterSupplierHierarchy } from "../../models/water-suppliers/water-supplier";
import { AuthService } from "../../services/auth/auth.service";

@Component({
    selector: 'vp-water-supplier-hierarchy',
    standalone: false,
    templateUrl: './water-supplier-hierarchy.component.html'
})
export class WaterSupplierHierarchyComponent {
    public isLoading: boolean = false;

    @Input()
    public suppliers: WaterSupplierHierarchy[] = [];

    constructor(private readonly _authService: AuthService) {

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