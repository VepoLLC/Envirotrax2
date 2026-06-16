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

    constructor(
    ) {
        this._userManager = this.createUserManager();
    }

    private createUserManager(): UserManager {
        return new UserManager({
            authority: environment.authUrl,
            loadUserInfo: true,
            scope: 'openid profile offline_access envirotrax_admin',
            client_id: 'envirotrax-admin',
            redirect_uri: window.location.origin + '/auth/login-redirect',
            post_logout_redirect_uri: window.location.origin + '/auth/sign-out',
            response_type: 'code'
        });
    }

    public signOut(): void {
        this._userManager.signoutRedirect();
        this.setLoggedIn(false);
    }

    public async signIn(): Promise<void> {
        await this._userManager.removeUser();
        return this._userManager.signinRedirect();
    }

    public async signInCallback(): Promise<void> {
        await this._userManager.signinCallback();
    }

    public navigateToProfile(): void {
        window.open(`${environment.authUrl}/Identity/Account/Manage`, '_blank');
    }

    private async getProfileField(fieldName: string): Promise<any> {
        const user = await this._userManager.getUser();

        if (user) {
            const profile = user.profile as any;

            if (profile) {
                return profile[fieldName];
            }
        }

        return undefined;
    }

    public async isAuthenticated(): Promise<boolean> {
        const user = await this._userManager.getUser();
        return !!user;
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

    public async getUserEmail(): Promise<string | undefined> {
        const user = await this._userManager.getUser();
        return user?.profile?.email;
    }
}