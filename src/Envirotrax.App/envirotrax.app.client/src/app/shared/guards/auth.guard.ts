import { Injectable } from "@angular/core";
import { CanActivate } from "@angular/router";
import { AuthService } from "../services/auth/auth.service";

@Injectable({
    providedIn: 'root'
})
export class AuthGuard implements CanActivate {
    constructor(
        private readonly _authService: AuthService
    ) {

    }

    public async canActivate(): Promise<boolean> {
        if (await this._authService.isAuthenticated()) {
            return true;
        }

        this._authService.signIn();

        return false;
    }
}