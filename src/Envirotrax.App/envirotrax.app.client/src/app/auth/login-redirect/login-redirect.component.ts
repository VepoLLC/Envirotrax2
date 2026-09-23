import { Component } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";
import { PagedData } from "../../shared/models/paged-data";
import { AuthService } from "../../shared/services/auth/auth.service";
import { MySupplierHierarchyDto, WaterSupplier } from "../../shared/models/water-suppliers/water-supplier";
import { WaterSupplierService } from "../../shared/services/water-suppliers/water-supplier.service";
import { ProfesisonalService } from "../../shared/services/professionals/professional.service";
import { Professional } from "../../shared/models/professionals/professional";
import { isSafeReturnUrl } from "../../shared/utils/return-url.util";

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
        private readonly _router: Router,
        private readonly _route: ActivatedRoute) {

    }

    public async ngOnInit(): Promise<void> {
        // A dashboard "View" on a sub account opens a new tab straight to this route with
        // ?waterSupplierId=<id>&returnUrl=<listPageUrl> to switch into that water supplier before
        // this tab has ever done an OIDC round trip. Everything else hitting this route is the OIDC
        // callback leg (it carries code/state/error), which the existing flow below handles.
        const params = this._route.snapshot.queryParamMap;
        const waterSupplierIdParam = params.get('waterSupplierId');
        const isOidcCallback = params.has('code') || params.has('state') || params.has('error');

        if (waterSupplierIdParam && !isOidcCallback) {
            await this.initiateWaterSupplierSwitch(Number(waterSupplierIdParam), params.get('returnUrl'));
            return;
        }

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

    private async initiateWaterSupplierSwitch(targetWaterSupplierId: number, rawReturnUrl: string | null): Promise<void> {
        const returnUrl = isSafeReturnUrl(rawReturnUrl) ? rawReturnUrl : '/';

        const currentWaterSupplierId = await this._authService.getWaterSupplierId();
        if (currentWaterSupplierId === targetWaterSupplierId) {
            window.location.replace(returnUrl);
            return;
        }

        try {
            this.isLoading = true;
            await this._authService.signIn(targetWaterSupplierId, undefined, returnUrl, true);
        } finally {
            this.isLoading = false;
        }
    }

    private loginWithExistingSystem(): void {
        this._authService.setLoggedIn(true);
        const returnUrl = isSafeReturnUrl(this.returnUrl) ? this.returnUrl : '/';

        window.location.replace(returnUrl);
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