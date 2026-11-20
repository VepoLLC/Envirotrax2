import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ModalSize } from '@developer-partners/ngx-modal-dialog';
import { ModalHelperService, allSettled } from '@envirotrax/common-ui';
import { SharedComponentsModule } from '../../../../../shared/components/shared.components.module';
import { ProfessionalNotesComponent } from '../../../../../shared/components/professional-notes/list/professional-notes.component';
import { ProfessionalType } from '../../../../../shared/models/professionals/licenses/professional-user-license';
import { Professional, ProfessionalUser } from '../../../../../shared/models/professionals/professional';
import { ProfessionalUserWindowModel } from '../../../../../shared/models/professionals/professional-user-window';
import { FogInspectorUserService } from '../../../../../shared/services/fog/fog-inspector-user.service';
import { FogInspectorService } from '../../../../../shared/services/fog/fog-inspector.service';
import { WindowReference } from '../../../../../window/window-config';
import { FogInspectorLicenseListComponent } from '../../licenses/list/fog-inspector-license-list.component';
import { FogInspectorWaterSupplierListComponent } from '../../water-suppliers/list/fog-inspector-water-supplier-list.component';
import { AddEditFogInspectorUserComponent, FogInspectorUserModalData } from '../edit/add-edit-fog-inspector-user.component';

/**
 * One user of a FOG inspector company: the user's account, licenses and comment log, with the company's
 * registrations read-only, like V1's sub-account page.
 */
@Component({
    templateUrl: './fog-inspector-user-details.component.html',
    imports: [
        CommonModule,
        FormsModule,
        SharedComponentsModule,
        FogInspectorWaterSupplierListComponent,
        FogInspectorLicenseListComponent,
        ProfessionalNotesComponent
    ],
})
export class FogInspectorUserDetailsComponent implements OnInit {
    public professionalId: number = 0;
    public userId: number = 0;

    public readonly professionalType: ProfessionalType = ProfessionalType.FogInspector;

    public professional: Professional = {};
    public user?: ProfessionalUser;

    public isLoading: boolean = false;
    public isUserLoading: boolean = false;

    constructor(
        private readonly _windowReference: WindowReference<ProfessionalUserWindowModel>,
        private readonly _inspectorService: FogInspectorService,
        private readonly _userService: FogInspectorUserService,
        private readonly _modalHelper: ModalHelperService
    ) {

    }

    public async ngOnInit(): Promise<void> {
        this.professionalId = this._windowReference.config.model?.professionalId ?? 0;
        this.userId = this._windowReference.config.model?.userId ?? 0;

        try {
            this.isLoading = true;

            const [professional, user] = await allSettled([
                this._inspectorService.get(this.professionalId),
                this._userService.get(this.professionalId, this.userId)
            ]);

            this.professional = professional;
            this.user = user;
        } finally {
            this.isLoading = false;
        }
    }

    public editUser(): void {
        this._modalHelper.show<FogInspectorUserModalData, ProfessionalUser>(AddEditFogInspectorUserComponent, {
            title: 'Edit User Account',
            model: { professionalId: this.professionalId, user: this.user! },
            size: ModalSize.medium
        }).result().subscribe(() => this.loadUser());
    }

    private async loadUser(): Promise<void> {
        try {
            this.isUserLoading = true;
            this.user = await this._userService.get(this.professionalId, this.userId);
        } finally {
            this.isUserLoading = false;
        }
    }
}
