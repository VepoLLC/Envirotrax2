import { Injectable } from '@angular/core';
import { BackflowTesterDetailsComponent } from '../backflow/testers/details/backflow-tester-details.component';
import { CsiInspectorDetailsComponent } from '../csi/inspectors/details/csi-inspector-details.component';
import { FogInspectorDetailsComponent } from '../fog/inspectors/details/fog-inspector-details.component';
import { CsiInspectorAccount } from '../shared/models/csi/csi-inspector-account';
import { MODULE_COLORS } from '../shared/models/module-colors';
import { ProfessionalType } from '../shared/models/professionals/licenses/professional-user-license';
import { Professional } from '../shared/models/professionals/professional';
import { WindowService } from '../shared/services/window.service';

export interface ProfessionalAccount {
    professionalType?: ProfessionalType;
    professionalId?: number;
    userId?: number;
    companyName?: string;
    contactName?: string;
    userEmail?: string;
}

const ACCOUNT_WINDOW_TYPES: ProfessionalType[] = [
    ProfessionalType.Bpat,
    ProfessionalType.CsiInspector,
    ProfessionalType.FogInspector
];

@Injectable({
    providedIn: 'root'
})
export class ProfessionalAccountWindowService {
    constructor(private readonly _windowService: WindowService) {

    }

    public canOpen(professionalType?: ProfessionalType): boolean {
        if (professionalType == null) {
            return false;
        }

        return ACCOUNT_WINDOW_TYPES.includes(professionalType);
    }

    public open(account: ProfessionalAccount): void {
        const title = this.buildTitle(account);

        if (account.professionalType === ProfessionalType.Bpat) {
            this._windowService.addWindow<BackflowTesterDetailsComponent, Professional>(BackflowTesterDetailsComponent, {
                title,
                color: MODULE_COLORS.BACKFLOW,
                model: { id: account.professionalId }
            });

            return;
        }

        if (account.professionalType === ProfessionalType.CsiInspector) {
            this._windowService.addWindow<CsiInspectorDetailsComponent, CsiInspectorAccount>(CsiInspectorDetailsComponent, {
                title,
                color: MODULE_COLORS.CSI,
                model: { professionalId: account.professionalId, id: account.userId }
            });

            return;
        }

        if (account.professionalType === ProfessionalType.FogInspector) {
            this._windowService.addWindow<FogInspectorDetailsComponent, Professional>(FogInspectorDetailsComponent, {
                title,
                color: MODULE_COLORS.FOG,
                model: { id: account.professionalId }
            });
        }
    }

    private buildTitle(account: ProfessionalAccount): string {
        return [account.companyName, account.contactName, account.userEmail]
            .filter(part => part)
            .join(' - ');
    }
}
