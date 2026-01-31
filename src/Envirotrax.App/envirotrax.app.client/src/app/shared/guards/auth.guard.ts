import { Injectable } from "@angular/core";
import { CanActivate, Router, UrlTree } from "@angular/router";
import { AuthService } from "../services/auth/auth.service";

@Injectable({
    providedIn: 'root'
})
export class AuthGuard implements CanActivate {
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
                return true;
            }

            // If the user is logged in and has no supplier or professional ID, 
            // they are a registered professional who self-registered, but hasn't finished setting up their profile
            return this._router.createUrlTree(['/profile']);
        }

        this._authService.signIn();

        return false;
    }
}