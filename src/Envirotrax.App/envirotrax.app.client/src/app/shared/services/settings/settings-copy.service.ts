import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { lastValueFrom, map, Observable, shareReplay } from "rxjs";
import { ModalHelperService, ToastService, ToastType } from "@envirotrax/common-ui";
import { UrlResolverService } from "../helpers/url-resolver.service";
import { HelperService } from "../helpers/helper.service";
import { AuthService } from "../auth/auth.service";
import { PermissionAction, PermissionType } from "../../models/permission-type";
import { SettingsSection } from "../../models/settings/settings-section";

interface SettingsCopyDescription {
    name: string;
    confirmationNote?: string;
    successNote?: string;
}

@Injectable({
    providedIn: 'root'
})
export class SettingsCopyService {
    private static readonly descriptions: Record<SettingsSection, SettingsCopyDescription> = {
        [SettingsSection.General]: {
            name: 'general settings',
            successNote: 'Users may need to log out and log back in to see program changes.'
        },
        [SettingsSection.Csi]: {
            name: 'CSI settings'
        },
        [SettingsSection.CsiLetterMessage]: {
            name: 'CSI letter message settings'
        },
        [SettingsSection.Backflow]: {
            name: 'backflow testing settings',
            confirmationNote: 'The existing backflow renewal requirements of this account will be replaced with the ones from the parent account.'
        },
        [SettingsSection.BackflowLetterMessage]: {
            name: 'backflow letter message settings'
        }
    };

    private _parentAvailability$: Observable<boolean> | null = null;
    private _isCopying: boolean = false;

    constructor(
        private readonly _urlResolver: UrlResolverService,
        private readonly _http: HttpClient,
        private readonly _authService: AuthService,
        private readonly _modalHelper: ModalHelperService,
        private readonly _toastService: ToastService,
        private readonly _helper: HelperService
    ) {
    }

    public async canCopyFromParent(): Promise<boolean> {
        const canModify = await this._authService.hasAnyPermisison(PermissionAction.CanModify, PermissionType.Settings);

        if (!canModify) {
            return false;
        }

        return this.getParentAvailability();
    }

    public async confirmAndCopyFromParent(section: SettingsSection): Promise<boolean> {
        if (this._isCopying) {
            return false;
        }

        const description = SettingsCopyService.descriptions[section];
        const confirmed = await this.confirm(description);

        if (!confirmed) {
            return false;
        }

        try {
            this._isCopying = true;

            await this.copyFromParent(section);

            this._toastService.show({ text: this.buildSuccessMessage(description), type: ToastType.Success });

            return true;
        } catch (error) {
            const validationErrors: string[] = [];

            if (!this._helper.parseValidationErrors(error, validationErrors)) {
                throw error;
            }

            this._toastService.show({ text: validationErrors[0], type: ToastType.Error });

            return false;
        } finally {
            this._isCopying = false;
        }
    }

    private getParentAvailability(): Promise<boolean> {
        if (!this._parentAvailability$) {
            const url = this._urlResolver.resolveUrl('/api/settings-copy/availability');

            this._parentAvailability$ = this._http.get<boolean>(url).pipe(
                shareReplay(1)
            );
        }

        return lastValueFrom(this._parentAvailability$);
    }

    private copyFromParent(section: SettingsSection): Promise<void> {
        const url = this._urlResolver.resolveUrl(`/api/settings-copy/${section}`);
        const observable = this._http.post<void>(url, null);

        return lastValueFrom(observable);
    }

    private confirm(description: SettingsCopyDescription): Promise<boolean> {
        const messages = [`Are you sure that you want to copy ${description.name} from the parent water supplier account to this account?`];

        if (description.confirmationNote) {
            messages.push(description.confirmationNote);
        }

        const confirmed$ = this._modalHelper.confirm({ messages }).result().pipe(
            map(() => true)
        );

        return lastValueFrom(confirmed$, { defaultValue: false });
    }

    private buildSuccessMessage(description: SettingsCopyDescription): string {
        const message = `Successfully copied ${description.name} from the parent account!`;

        if (description.successNote) {
            return `${message} ${description.successNote}`;
        }

        return message;
    }
}
