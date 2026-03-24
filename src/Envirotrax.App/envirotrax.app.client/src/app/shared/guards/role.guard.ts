import { Injectable } from "@angular/core";
import { ActivatedRouteSnapshot, CanActivate, Router, RouterStateSnapshot, UrlTree } from "@angular/router";
import { AuthService } from "../services/auth/auth.service";


@Injectable({
    providedIn: 'root'
})
export class RoleGuard implements CanActivate {
    constructor(
        private readonly _authService: AuthService,
        private readonly _router: Router
    ) {

    }

    public async canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Promise<boolean | UrlTree> {
        const roles = route.data['roles'] as string[];

        if (roles) {
            if (await this._authService.hasAnyRoles(...roles)) {
                return true;
            }
        }

        return this._router.createUrlTree(['auth', "unauthorized"]);
    }

}