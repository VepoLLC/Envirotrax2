import { Component } from "@angular/core";
import { Router } from "@angular/router";
import { PagedData } from "../../shared/models/paged-data";
import { AuthService } from "../../shared/services/auth/auth.service";
import { WaterSupplier } from "../../shared/models/water-suppliers/water-supplier";
import { WaterSupplierService } from "../../shared/services/water-suppliers/water-supplier.service";

@Component({
    templateUrl: './login-redirect.component.html',
    standalone: false
})
export class LoginRedirectComponent {
    public isLoading: boolean = false;
    public suppliers?: PagedData<WaterSupplier>;

    constructor(
        private readonly _authService: AuthService,
        private readonly _supplierService: WaterSupplierService,
        private readonly _router: Router) {

    }

    public async ngOnInit(): Promise<void> {
        try {
            this.isLoading = true;

            await this._authService.signInCallback();

            const waterSupplierId = await this._authService.getWaterSupplierId();

            if (waterSupplierId) {
                this._authService.setLoggedIn(true);

                this._router.navigateByUrl('/', {
                    replaceUrl: true
                });
            } else {
                this.suppliers = await this._supplierService.getAllMySuppliers(this.suppliers?.pageInfo || {}, {});

                if (this.suppliers.data.length == 1) {
                    await this._authService.signIn(this.suppliers.data[0].id);
                }
            }
        } finally {
            this.isLoading = false;
        }
    }
}