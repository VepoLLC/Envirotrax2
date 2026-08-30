import { Injectable } from "@angular/core";
import { ActivatedRouteSnapshot, CanActivate, RouterStateSnapshot } from "@angular/router";
import { AuthService } from "../services/auth/auth.service";

// Lets a URL carry ?waterSupplierId=<id> to log the user in as that specific water supplier -
// e.g. a dashboard "View" link into a sub account. If the current session isn't already that
// water supplier, re-authenticates (full redirect through the auth server) and lands back on
// this same URL, at which point the check below passes and the page loads normally.
//
// isolateTab=true: this switch is scoped to just this tab (see AuthService's TAB_ISOLATED_KEY) -
// it doesn't announce itself to, or get overridden by, any other open tab.
@Injectable({
    providedIn: 'root'
})
export class WaterSupplierSwitchGuard implements CanActivate {
    constructor(
        private readonly _authService: AuthService
    ) {
    }

    public async canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Promise<boolean> {
        const requestedId = route.queryParamMap.get('waterSupplierId');

        if (!requestedId) {
            return true;
        }

        const targetWaterSupplierId = Number(requestedId);
        const currentWaterSupplierId = await this._authService.getWaterSupplierId();

        if (currentWaterSupplierId === targetWaterSupplierId) {
            return true;
        }

        await this._authService.signIn(targetWaterSupplierId, undefined, state.url, true);

        return false;
    }
}
