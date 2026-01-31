import { Injectable } from "@angular/core";
import { CanActivate, Router, UrlTree } from "@angular/router";
import { AuthService } from "../shared/services/auth/auth.service";

@Injectable({
    providedIn: 'root'
})
export class ProfileGuard implements CanActivate {
    constructor(
        private readonly _authService: AuthService,
        private readonly _router: Router
    ) {

    }

    public async canActivate(): Promise<boolean | UrlTree> {
        if (await this._authService.isAuthenticated(false)) {
            const [supplierId, professionalId] = await Promise.all([
                this._authService.getWaterSupplierId(),
                this._authService.getProfessionalId()
            ]);

            if (supplierId && professionalId) {
                return this._router.createUrlTree(['/']);
            }

            return true;
        }

        this._authService.signIn();

        return false;
    }
}