import { Injectable } from "@angular/core";
import { CanActivate, Router, UrlTree } from "@angular/router";
import { AuthService } from "../services/auth/auth.service";
import { ProfesisonalService } from "../services/professionals/professional.service";
import { ProfesionalUserService } from "../services/professionals/professional-user.service";

@Injectable({
    providedIn: 'root'
})
export class AuthGuard implements CanActivate {
    constructor(
        private readonly _authService: AuthService,
        private readonly _router: Router,
        private readonly _professionalService: ProfesisonalService,
        private readonly _professionalUserService: ProfesionalUserService
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
            // Let's check if their profile is filled
            if (!professionalId) {
                return this._router.createUrlTree(['/profile/company']);
            }

            const professional = await this._professionalService.getLoggedInProfessional();

            if (!professional) {
                return this._router.createUrlTree(['/profile/company']);
            }

            const professionalUser = await this._professionalUserService.getMyData();

            if (!professionalUser || !professionalUser.contactName) {
                return this._router.createUrlTree(['/profile/user']);
            }

            return true;
        }

        this._authService.signIn();

        return false;
    }
}