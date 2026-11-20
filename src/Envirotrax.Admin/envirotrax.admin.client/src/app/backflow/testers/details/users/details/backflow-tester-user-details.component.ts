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
import { BackflowTesterUserService } from '../../../../../shared/services/backflow/backflow-tester-user.service';
import { BackflowTesterService } from '../../../../../shared/services/backflow/backflow-tester.service';
import { WindowReference } from '../../../../../window/window-config';
import { BackflowTesterGaugeListComponent } from '../../gauges/list/backflow-tester-gauge-list.component';
import { BackflowTesterLicenseListComponent } from '../../licenses/list/backflow-tester-license-list.component';
import { BackflowTesterWaterSupplierListComponent } from '../../water-suppliers/list/backflow-tester-water-supplier-list.component';
import { AddEditBackflowTesterUserComponent, BackflowTesterUserModalData } from '../edit/add-edit-backflow-tester-user.component';

/**
 * One user of a backflow tester company: the user's account, licenses, gauges and comment log, with the company's
 * registrations read-only, like V1's sub-account page.
 */
@Component({
    templateUrl: './backflow-tester-user-details.component.html',
    imports: [
        CommonModule,
        FormsModule,
        SharedComponentsModule,
        BackflowTesterWaterSupplierListComponent,
        BackflowTesterLicenseListComponent,
        BackflowTesterGaugeListComponent,
        ProfessionalNotesComponent
    ],
})
export class BackflowTesterUserDetailsComponent implements OnInit {
    public professionalId: number = 0;
    public userId: number = 0;

    public readonly professionalType: ProfessionalType = ProfessionalType.Bpat;

    public professional: Professional = {};
    public user?: ProfessionalUser;

    public isLoading: boolean = false;
    public isUserLoading: boolean = false;

    constructor(
        private readonly _windowReference: WindowReference<ProfessionalUserWindowModel>,
        private readonly _testerService: BackflowTesterService,
        private readonly _userService: BackflowTesterUserService,
        private readonly _modalHelper: ModalHelperService
    ) {

    }

    public async ngOnInit(): Promise<void> {
        this.professionalId = this._windowReference.config.model?.professionalId ?? 0;
        this.userId = this._windowReference.config.model?.userId ?? 0;

        try {
            this.isLoading = true;

            const [professional, user] = await allSettled([
                this._testerService.get(this.professionalId),
                this._userService.get(this.professionalId, this.userId)
            ]);

            this.professional = professional;
            this.user = user;
        } finally {
            this.isLoading = false;
        }
    }

    public editUser(): void {
        this._modalHelper.show<BackflowTesterUserModalData, ProfessionalUser>(AddEditBackflowTesterUserComponent, {
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
