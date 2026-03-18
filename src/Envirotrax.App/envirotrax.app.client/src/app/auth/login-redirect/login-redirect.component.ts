import { Component } from "@angular/core";
import { Router } from "@angular/router";
import { PagedData } from "../../shared/models/paged-data";
import { AuthService } from "../../shared/services/auth/auth.service";
import { MySupplierHierarchyDto, WaterSupplier, WaterSupplierHierarchy } from "../../shared/models/water-suppliers/water-supplier";
import { WaterSupplierService } from "../../shared/services/water-suppliers/water-supplier.service";
import { ProfesisonalService } from "../../shared/services/professionals/professional.service";
import { Professional } from "../../shared/models/professionals/professional";

@Component({
    templateUrl: './login-redirect.component.html',
    standalone: false
})
export class LoginRedirectComponent {
    public isLoading: boolean = false;
    public suppliers?: MySupplierHierarchyDto;
    public professionals?: PagedData<Professional>;

    constructor(
        private readonly _authService: AuthService,
        private readonly _supplierService: WaterSupplierService,
        private readonly _professionalService: ProfesisonalService,
        private readonly _router: Router) {

    }

    public async ngOnInit(): Promise<void> {
        try {
            this.isLoading = true;

            await this._authService.signInCallback();

            const [profesisonalId, waterSupplierId] = await Promise.all([
                this._authService.getProfessionalId(),
                this._authService.getWaterSupplierId()
            ]);

            if (profesisonalId || waterSupplierId) {
                this.loginWithExistingSystem();
            } else {
                await this.loadSystems();
            }
        } finally {
            this.isLoading = false;
        }
    }

    private loginWithExistingSystem(): void {
        this._authService.setLoggedIn(true);

        this._router.navigateByUrl('/', {
            replaceUrl: true
        });
    }

    private checkIfOneSupplier(myHierarchy: MySupplierHierarchyDto): WaterSupplier | null {
        if (myHierarchy.hierarchy.length == 1) {
            if (myHierarchy.hierarchy[0].waterSuppliers.length == 1) {
                if (myHierarchy.hierarchy[0].waterSuppliers[0].children.length == 0) {
                    return myHierarchy.hierarchy[0].waterSuppliers[0].waterSupplier;
                }
            }
        }

        return null;
    }

    private async loadSystems(): Promise<void> {
        this.professionals = await this._professionalService.getAllMy(this.professionals?.pageInfo || {}, {});

        if (this.professionals.data.length == 1) {
            await this._authService.signIn(undefined, this.professionals.data[0].id);
            return;
        }

        this.suppliers = await this._supplierService.getAllMySuppliers();

        const onlySupplier = this.checkIfOneSupplier(this.suppliers);

        if (onlySupplier) {
            await this._authService.signIn(onlySupplier.id);
            return
        }

        // User must be a registered professional who self-registered, but hasn't fileld out their company information yet.
        if (this.professionals.data.length == 0 && !this.suppliers.adminAccount && this.suppliers.hierarchy.length == 0) {
            this._router.navigate(['/profile'], {
                replaceUrl: true
            });
        }
    }

    public async selectSupplier(supplier: WaterSupplier): Promise<void> {
        try {
            this.isLoading = true;
            await this._authService.signIn(supplier.id);
        } finally {
            this.isLoading = false;
        }
    }

    public async selectProfessional(professional: Professional): Promise<void> {
        try {
            this.isLoading = true;
            await this._authService.signIn(undefined, professional.id);
        } finally {
            this.isLoading = false;
        }
    }
}