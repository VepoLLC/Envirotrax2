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
            acrValues += `professionalId:${professionalId} `;
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

    public signIn(waterSupplierId?: number, professionalId?: number): Promise<void> {
        if (waterSupplierId || professionalId) {
            this._userManager = this.createUserManager(waterSupplierId, professionalId);
        }

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

    private async getProfileInteger(fieldName: string): Promise<number | undefined> {
        const id = await this.getProfileField(fieldName);

        return id
            ? parseInt(id)
            : undefined;
    }

    public async getWaterSupplierId(): Promise<number | undefined> {
        return this.getProfileInteger("wsId");
    }

    public getProfessionalId(): Promise<number | undefined> {
        return this.getProfileInteger("prfId");
    }

    public async isAuthenticated(checkTenantOrProfessional: boolean): Promise<boolean> {
        if (checkTenantOrProfessional) {
            const supplierId = await this.getWaterSupplierId();
            const professionalId = await this.getProfessionalId();

            return !!supplierId || !!professionalId;
        }

        const user = await this._userManager.getUser();

        return !!user;
    }

    public onLoggedIn(): Observable<boolean> {
        return merge(
            this._isLoggedIn$.asObservable(),
            from(this.isAuthenticated(true))
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