import { Injectable } from "@angular/core";
import { BehaviorSubject, from, lastValueFrom, merge, Observable, shareReplay } from "rxjs";
import { User, UserManager } from "oidc-client-ts";
import { environment } from "../../../../environments/environment";
import { FeatureType } from "../../models/feature-type";
import { UrlResolverService } from "../helpers/url-resolver.service";
import { HttpClient } from "@angular/common/http";
import { PermissionAction, PermissionType } from "../../models/permission-type";

const AUTH_BROADCAST_CHANNEL_NAME = 'envirotrax-auth-sync';
const CROSS_TAB_SYNC_TIMEOUT_MS = 250;
const TAB_ISOLATED_KEY = 'vp-tab-isolated';

type AuthBroadcastMessage =
    | { type: 'user-changed'; user: string | null }
    | { type: 'request-user' }
    | { type: 'user-response'; user: string | null };

@Injectable({
    providedIn: 'root'
})
export class AuthService {
    private _isLoggedIn$: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false);
    private _userManager!: UserManager;
    private _userAcces$?: Observable<UserAccess>;

    private readonly _channel: BroadcastChannel | null = typeof BroadcastChannel !== 'undefined'
        ? new BroadcastChannel(AUTH_BROADCAST_CHANNEL_NAME)
        : null;

    private _suppressBroadcast: boolean = false;
    private _syncPromise?: Promise<void>;

    constructor(
        private readonly _http: HttpClient,
        private readonly _urlResolver: UrlResolverService
    ) {
        this.attachUserManager(this.createUserManager());

        if (this._channel) {
            this._channel.onmessage = (event: MessageEvent<AuthBroadcastMessage>) => {
                void this.handleBroadcastMessage(event.data);
            };
        }
    }

    private attachUserManager(userManager: UserManager): void {
        this._userManager = userManager;

        this._userManager.events.addUserLoaded(user => {
            this.broadcastUserChanged(user.toStorageString());

            if (this.hasResolvedTenant(user)) {
                this.setLoggedIn(true);
            }
        });

        this._userManager.events.addUserUnloaded(() => {
            this.setLoggedIn(false);
            this.broadcastUserChanged(null);
        });
    }

    private hasResolvedTenant(user: User): boolean {
        const profile = user.profile as any;

        return !!profile?.wsId || !!profile?.prfId;
    }

    private isTabIsolated(): boolean {
        return sessionStorage.getItem(TAB_ISOLATED_KEY) === '1';
    }

    private broadcastUserChanged(user: string | null): void {
        if (this._suppressBroadcast || !this._channel || this.isTabIsolated()) {
            return;
        }

        this._channel.postMessage({ type: 'user-changed', user } as AuthBroadcastMessage);
    }

    private async handleBroadcastMessage(message: AuthBroadcastMessage): Promise<void> {
        if (this.isTabIsolated()) {
            return;
        }

        if (message.type !== 'user-changed' && message.type !== 'request-user') {
            return;
        }

        if (message.type === 'request-user') {
            const user = await this._userManager.getUser();

            this._channel?.postMessage({
                type: 'user-response',
                user: user ? user.toStorageString() : null
            } as AuthBroadcastMessage);

            return;
        }

        this._suppressBroadcast = true;

        try {
            await this._userManager.storeUser(message.user ? User.fromStorageString(message.user) : null);
        } finally {
            this._suppressBroadcast = false;
        }
    }

    private requestUserFromOtherTabs(): Promise<User | null> {
        const channel = this._channel;

        if (!channel) {
            return Promise.resolve(null);
        }

        return new Promise<User | null>(resolve => {
            let settled = false;

            const finish = (user: User | null) => {
                if (settled) {
                    return;
                }

                settled = true;
                channel.removeEventListener('message', listener);
                resolve(user);
            };

            const listener = (event: MessageEvent<AuthBroadcastMessage>) => {
                // Only a positive reply short-circuits the wait — with more than one other tab
                // open, an empty tab's "no user" reply shouldn't win a race against a tab that
                // actually has a session, so a negative reply just keeps waiting for the timeout.
                if (event.data?.type === 'user-response' && event.data.user) {
                    finish(User.fromStorageString(event.data.user));
                }
            };

            channel.addEventListener('message', listener);
            channel.postMessage({ type: 'request-user' } as AuthBroadcastMessage);

            setTimeout(() => finish(null), CROSS_TAB_SYNC_TIMEOUT_MS);
        });
    }

    public ensureSynced(): Promise<void> {
        if (!this._syncPromise) {
            this._syncPromise = this.syncFromOtherTabs();
        }

        return this._syncPromise;
    }

    private async syncFromOtherTabs(): Promise<void> {
        // This tab intentionally logged in as a different water supplier (WaterSupplierSwitchGuard).
        // Don't let it copy another tab's user - that would undo the switch it just made.
        if (this.isTabIsolated()) {
            return;
        }

        // This tab is on /auth/login-redirect, which is in the middle of finishing its own login
        // (signInCallback below) and is about to save the newly-authenticated user. If we copied
        // another tab's user right now, we could overwrite that new user immediately after it's saved.
        if (window.location.pathname === '/auth/login-redirect') {
            return;
        }

        const localUser = await this._userManager.getUser();

        if (localUser) {
            return;
        }

        const remoteUser = await this.requestUserFromOtherTabs();

        if (!remoteUser) {
            return;
        }

        this._suppressBroadcast = true;

        try {
            await this._userManager.storeUser(remoteUser);
        } finally {
            this._suppressBroadcast = false;
        }
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
            scope: 'openid profile offline_access envirotrax_app',
            client_id: 'envirotrax-app',
            redirect_uri: window.location.origin + '/auth/login-redirect',
            post_logout_redirect_uri: window.location.origin + '/auth/sign-out',
            response_type: 'code',
            extraTokenParams: {
                'acr_values': acrValues
            }
        });
    }

    public async signOut(): Promise<void> {
        await this._userManager.removeUser();

        this._userManager.signoutRedirect();
        this.setLoggedIn(false);
    }

    public async signIn(waterSupplierId?: number, professionalId?: number, returnUrl?: string, isolateTab: boolean = false): Promise<void> {
        if (isolateTab) {
            sessionStorage.setItem(TAB_ISOLATED_KEY, '1');
        }

        this._suppressBroadcast = true;

        try {
            await this._userManager.removeUser();
        } finally {
            this._suppressBroadcast = false;
        }

        if (waterSupplierId || professionalId) {
            this.attachUserManager(this.createUserManager(waterSupplierId, professionalId));
        }

        return this._userManager.signinRedirect({
            state: { returnUrl }
        });
    }

    public async signInCallback(): Promise<string | null> {
        const user = await this._userManager.signinCallback();

        if (user?.state) {
            return (<any>user.state).returnUrl ?? null;
        }

        return null;
    }

    public navigateToProfile(): void {
        window.open(`${environment.authUrl}/Identity/Account/Manage`, '_blank');
    }

    public navigateToSecuritySettings(): void {
        window.open(`${environment.authUrl}/Identity/Account/Manage/ChangePassword`, '_blank');
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
            from(this.ensureSynced().then(() => this.isAuthenticated(true)))
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

    private getMyAccess(): Promise<UserAccess> {
        const url = this._urlResolver.resolveUrl('/api/users/access/my');

        if (!this._userAcces$) {
            this._userAcces$ = this._http.get<UserAccess>(url).pipe(shareReplay(1));
        }

        return lastValueFrom(this._userAcces$);
    }

    public async hasAnyFeatures(...featuresToCheck: FeatureType[]): Promise<boolean> {
        const myAccess = await this.getMyAccess();

        for (let featureToCheck of featuresToCheck) {
            if (myAccess.features.indexOf(featureToCheck) >= 0) {
                return true;
            }
        }

        return false;
    }

    private hasPermission(myPermissions: RolePermission[], action: PermissionAction, type: PermissionType): boolean {
        let matchingPermission = myPermissions.find(p => p.permission == type);

        if (matchingPermission) {
            switch (action) {
                case PermissionAction.CanModify:
                    return matchingPermission.canModify!;
                case PermissionAction.CanDelete:
                    return matchingPermission.canDelete!;
                case PermissionAction.CanView:
                    return matchingPermission.canModify! || matchingPermission.canView!;
            }
        }

        return false;
    }

    public async hasAnyPermisison(action: PermissionAction, ...types: PermissionType[]): Promise<boolean> {
        const access = await this.getMyAccess();

        for (let type of types) {
            if (this.hasPermission(access.permissions, action, type)) {
                return true;
            }
        }

        return false;
    }

    public async hasAnyRoles(...roles: string[]): Promise<boolean> {
        const access = await this.getMyAccess();

        for (let roleToCheck of roles) {
            if (access.roles.indexOf(roleToCheck) >= 0) {
                return true;
            }
        }

        return false;
    }
}

interface UserAccess {
    features: FeatureType[];
    permissions: RolePermission[];
    roles: string[];
}

interface RolePermission {
    permission: PermissionType;

    canView?: boolean;
    canModify?: boolean;
    canDelete?: boolean;
}