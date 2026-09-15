import { Component } from "@angular/core";
import { Router } from "@angular/router";
import { PagedData } from "../../shared/models/paged-data";
import { AuthService } from "../../shared/services/auth/auth.service";
import { MySupplierHierarchyDto, WaterSupplier, WaterSupplierHierarchy } from "../../shared/models/water-suppliers/water-supplier";
import { WaterSupplierService } from "../../shared/services/water-suppliers/water-supplier.service";
import { ProfesisonalService } from "../../shared/services/professionals/professional.service";
import { Professional } from "../../shared/models/professionals/professional";

export type LoginAccountType = 'professional' | 'waterSupplier';

@Component({
    templateUrl: './login-redirect.component.html',
    standalone: false
})
export class LoginRedirectComponent {
    public isLoading: boolean = false;
    public suppliers?: MySupplierHierarchyDto;
    public professionals?: PagedData<Professional>;

    public accountType: LoginAccountType | null = null;

    public showAccountTypeChoice: boolean = false;
    public showProfessionalSelection: boolean = false;
    public showWaterSupplierSelection: boolean = false;

    public returnUrl: string | null = null;

    constructor(
        private readonly _authService: AuthService,
        private readonly _supplierService: WaterSupplierService,
        private readonly _professionalService: ProfesisonalService,
        private readonly _router: Router) {

    }

    public async ngOnInit(): Promise<void> {
        try {
            this.isLoading = true;

            this.returnUrl = await this._authService.signInCallback();

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

        this._router.navigateByUrl(this.returnUrl ?? '/', {
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
        [this.professionals, this.suppliers] = await Promise.all([
            this._professionalService.getAllMy(this.professionals?.pageInfo || {}, {}),
            this._supplierService.getAllMySuppliers()
        ]);

        const hasProfessional = this.professionals.data.length > 0;
        const hasWaterSupplier = !!this.suppliers.adminAccount || this.suppliers.hierarchy.length > 0;

        if (hasProfessional && hasWaterSupplier) {
            this.updateView();
            return;
        }

        if (hasProfessional) {
            if (this.professionals.data.length == 1) {
                await this._authService.signIn(undefined, this.professionals.data[0].id, this.returnUrl ?? undefined);
                return;
            }

            this.updateView();
            return;
        }

        if (hasWaterSupplier) {
            const onlySupplier = this.checkIfOneSupplier(this.suppliers);

            if (onlySupplier) {
                await this._authService.signIn(onlySupplier.id, undefined, this.returnUrl ?? undefined);
                return;
            }

            this.updateView();
            return;
        }

        // User must be a registered professional who self-registered, but hasn't fileld out their company information yet.
        this._router.navigate(['/profile'], {
            replaceUrl: true
        });
    }

    private updateView(): void {
        const professionalCount = this.professionals?.data.length ?? 0;
        const hasProfessional = professionalCount > 0;
        const hasWaterSupplier = !!this.suppliers && (!!this.suppliers.adminAccount || this.suppliers.hierarchy.length > 0);

        this.showAccountTypeChoice = hasProfessional && hasWaterSupplier && this.accountType == null;

        this.showProfessionalSelection = professionalCount > 1
            && (!hasWaterSupplier || this.accountType == 'professional');

        this.showWaterSupplierSelection = hasWaterSupplier
            && (!hasProfessional || this.accountType == 'waterSupplier');
    }

    public async selectAccountType(accountType: LoginAccountType): Promise<void> {
        this.accountType = accountType;

        if (accountType == 'professional' && this.professionals && this.professionals.data.length == 1) {
            await this.selectProfessional(this.professionals.data[0]);
            return;
        }

        if (accountType == 'waterSupplier' && this.suppliers) {
            const onlySupplier = this.checkIfOneSupplier(this.suppliers);

            if (onlySupplier) {
                await this.selectSupplier(onlySupplier);
                return;
            }
        }

        this.updateView();
    }

    public async selectSupplier(supplier: WaterSupplier): Promise<void> {
        try {
            this.isLoading = true;
            await this._authService.signIn(supplier.id, undefined, this.returnUrl ?? undefined);
        } finally {
            this.isLoading = false;
        }
    }

    public async selectProfessional(professional: Professional): Promise<void> {
        try {
            this.isLoading = true;
            await this._authService.signIn(undefined, professional.id, this.returnUrl ?? undefined);
        } finally {
            this.isLoading = false;
        }
    }
}