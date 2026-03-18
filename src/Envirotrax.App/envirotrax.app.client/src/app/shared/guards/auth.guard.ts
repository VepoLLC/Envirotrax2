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

            if (supplierId) {
                return true;
            }

            // If user is logged in, but they don't have a supplierId, they are a professional
            // Let's navigate to profile page to collect the missing information
            if (!professionalId) {
                return this._router.createUrlTree(['/profile']);
            }

            return true;
        }

        this._authService.signIn();

        return false;
    }
}