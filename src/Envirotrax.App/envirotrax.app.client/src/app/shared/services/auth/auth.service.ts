import { Injectable } from "@angular/core";
import { BehaviorSubject, from, merge, Observable } from "rxjs";
import { UserManager } from "oidc-client-ts";
import { environment } from "../../../../environments/environment";

@Injectable({
    providedIn: 'root'
})
export class AuthService {
    private _isLoggedIn$: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false);
    private _userManager: UserManager;

    constructor() {
        this._userManager = this.createUserManager();
    }

    private createUserManager(waterSupplierId?: number, professionalId?: number): UserManager {
        let acrValues = '';

        if (waterSupplierId) {
            acrValues += `waterSupplierId:${waterSupplierId} `;
        }

        if (professionalId) {
            acrValues + `professional:${professionalId} `;
        }

        return new UserManager({
            authority: environment.authUrl,
            loadUserInfo: true,
            scope: 'envirotrax_app',
            client_id: 'envirotrax-app',
            redirect_uri: window.location.origin + '/auth/login-redirect',
            post_logout_redirect_uri: window.location.origin + '/auth/sign-out',
            response_type: 'code',
            extraTokenParams: {
                'acr_values': acrValues
            }
        });
    }

    public signOut(): void {
        this._userManager.signoutRedirect();
        this.setLoggedIn(false);
    }

    public signIn(waterSupplierId?: number): Promise<void> {
        if (waterSupplierId) {
            this._userManager = this.createUserManager(waterSupplierId);
        }

        return this._userManager.signinRedirect();
    }

    public async signInCallback(): Promise<void> {
        await this._userManager.signinCallback();
    }

    public navigateToProfile(): void {
        window.open(`${environment.authUrl}/Identity/Account/Manage`, '_blank');
    }

    public async getWaterSupplierId(): Promise<number | null> {
        const user = await this._userManager.getUser();

        if (user) {
            const profile = user.profile as any;

            if (profile.wsId) {
                return parseInt(profile.wsId);
            }
        }

        return null;
    }

    public async isAuthenticated(): Promise<boolean> {
        const supplierId = await this.getWaterSupplierId();
        return !!supplierId;
    }

    public onLoggedIn(): Observable<boolean> {
        return merge(
            this._isLoggedIn$.asObservable(),
            from(this.isAuthenticated())
        );
    }

    public setLoggedIn(isLoggedIn: boolean): void {
        this._isLoggedIn$.next(isLoggedIn);
    }

    public async getAccessToken(): Promise<string | undefined> {
        const user = await this._userManager.getUser();
        return user?.access_token;
    }
}