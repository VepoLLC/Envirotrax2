import { Injectable } from "@angular/core";
import { ActivatedRouteSnapshot, CanActivate, Router, RouterStateSnapshot, UrlTree } from "@angular/router";
import { AuthService } from "../services/auth/auth.service";
import { PermissionAction, PermissionType } from "../models/permission-type";

@Injectable({
    providedIn: 'root'
})
export class PermissionGuard implements CanActivate {
    constructor(
        private readonly _authService: AuthService,
        private readonly _router: Router
    ) {

    }

    public async canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Promise<boolean | UrlTree> {
        const permissions = route.data['permissions'] as Permission[];

        if (permissions) {
            for (const permission of permissions) {
                if (await this._authService.hasAnyPermisison(permission.action, permission.type)) {
                    return true;
                }
            }
        }

        return this._router.createUrlTree(['auth', "unauthorized"]);
    }

}

interface Permission {
    type: PermissionType,
    action: PermissionAction
}