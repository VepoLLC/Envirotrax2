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
import { FogTransporterUserService } from '../../../../../shared/services/fog/fog-transporter-user.service';
import { FogTransporterService } from '../../../../../shared/services/fog/fog-transporter.service';
import { WindowReference } from '../../../../../window/window-config';
import { FogTransporterLicenseListComponent } from '../../licenses/list/fog-transporter-license-list.component';
import { FogTransporterWaterSupplierListComponent } from '../../water-suppliers/list/fog-transporter-water-supplier-list.component';
import { AddEditFogTransporterUserComponent, FogTransporterUserModalData } from '../edit/add-edit-fog-transporter-user.component';

/**
 * One user of a FOG transporter company: the user's account and comment log. Transporter registrations and water
 * supplier registrations belong to the company, so they are listed read-only.
 */
@Component({
    templateUrl: './fog-transporter-user-details.component.html',
    imports: [
        CommonModule,
        FormsModule,
        SharedComponentsModule,
        FogTransporterWaterSupplierListComponent,
        FogTransporterLicenseListComponent,
        ProfessionalNotesComponent
    ],
})
export class FogTransporterUserDetailsComponent implements OnInit {
    public professionalId: number = 0;
    public userId: number = 0;

    public readonly professionalType: ProfessionalType = ProfessionalType.FogTransporter;

    public professional: Professional = {};
    public user?: ProfessionalUser;

    public isLoading: boolean = false;
    public isUserLoading: boolean = false;

    constructor(
        private readonly _windowReference: WindowReference<ProfessionalUserWindowModel>,
        private readonly _transporterService: FogTransporterService,
        private readonly _userService: FogTransporterUserService,
        private readonly _modalHelper: ModalHelperService
    ) {

    }

    public async ngOnInit(): Promise<void> {
        this.professionalId = this._windowReference.config.model?.professionalId ?? 0;
        this.userId = this._windowReference.config.model?.userId ?? 0;

        try {
            this.isLoading = true;

            const [professional, user] = await allSettled([
                this._transporterService.get(this.professionalId),
                this._userService.get(this.professionalId, this.userId)
            ]);

            this.professional = professional;
            this.user = user;
        } finally {
            this.isLoading = false;
        }
    }

    public editUser(): void {
        this._modalHelper.show<FogTransporterUserModalData, ProfessionalUser>(AddEditFogTransporterUserComponent, {
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
