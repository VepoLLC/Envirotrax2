import { Injectable } from "@angular/core";
import { ActivatedRouteSnapshot, CanActivate, Router, RouterStateSnapshot, UrlTree } from "@angular/router";
import { AuthService } from "../services/auth/auth.service";
import { FeatureType } from "../models/feature-type";

@Injectable({
    providedIn: 'root'
})
export class FeatureGuard implements CanActivate {
    constructor(
        private readonly _authService: AuthService,
        private readonly _router: Router
    ) {

    }

    public async canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Promise<boolean | UrlTree> {
        const features = route.data['features'] as FeatureType[];

        if (features) {
            if (await this._authService.hasAnyFeatures(...features)) {
                return true;
            }
        }

        return this._router.createUrlTree(['auth', "unauthorized"]);
    }

}