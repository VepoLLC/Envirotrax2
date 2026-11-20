import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ModalSize } from '@developer-partners/ngx-modal-dialog';
import { CheckboxCellComponent, ColumnType, CurrencyCellComponent, ModalHelperService, TableColumn } from '@envirotrax/common-ui';
import { SharedComponentsModule } from '../../../../../shared/components/shared.components.module';
import { ProfessionalNotesComponent } from '../../../../../shared/components/professional-notes/list/professional-notes.component';
import { CsiInspectorAccountDetails } from '../../../../../shared/models/csi/csi-inspector-account';
import { ProfessionalType } from '../../../../../shared/models/professionals/licenses/professional-user-license';
import { Professional, ProfessionalUser } from '../../../../../shared/models/professionals/professional';
import { ProfessionalUserWindowModel } from '../../../../../shared/models/professionals/professional-user-window';
import { ProfessionalWaterSupplier } from '../../../../../shared/models/professionals/professional-water-supplier';
import { CsiInspectorService } from '../../../../../shared/services/csi/csi-inspector.service';
import { WindowReference } from '../../../../../window/window-config';
import { CsiInspectorLicenseListComponent } from '../../licenses/list/csi-inspector-license-list.component';
import { AddEditCsiInspectorUserComponent, CsiInspectorUserModalData } from '../edit/add-edit-csi-inspector-user.component';

/**
 * One user of a CSI inspector company: the user's account, licenses and comment log, with the company's
 * registrations read-only, like V1's sub-account page. The CSI details endpoint returns the company, the user and
 * the registrations together.
 */
@Component({
    templateUrl: './csi-inspector-user-details.component.html',
    imports: [
        CommonModule,
        FormsModule,
        SharedComponentsModule,
        CsiInspectorLicenseListComponent,
        ProfessionalNotesComponent
    ],
})
export class CsiInspectorUserDetailsComponent implements OnInit {
    public professionalId: number = 0;
    public userId: number = 0;

    public readonly professionalType: ProfessionalType = ProfessionalType.CsiInspector;

    public professional: Professional = {};
    public user?: ProfessionalUser;
    public registrations: ProfessionalWaterSupplier[] = [];
    public registrationsHeader: string = '';

    // The CSI window edits these inline; here they are read-only, with the shared cell components of the other
    // registration grids.
    public readonly registrationColumns: TableColumn<ProfessionalWaterSupplier>[] = [
        { field: 'waterSupplier.name', caption: 'Water Supplier', type: ColumnType.text },
        { field: 'csiCommercialInspectionFee', caption: 'Comm Fee', cellComponent: CurrencyCellComponent, type: ColumnType.number },
        { field: 'csiResidentialInspectionFee', caption: 'Res Fee', cellComponent: CurrencyCellComponent, type: ColumnType.number },
        { field: 'hasCsiInspection', caption: 'Active', cellComponent: CheckboxCellComponent, type: ColumnType.text },
        { field: 'isCsiInspectionSuspended', caption: 'Banned', cellComponent: CheckboxCellComponent, type: ColumnType.text }
    ];

    public isLoading: boolean = false;
    public isUserLoading: boolean = false;

    constructor(
        private readonly _windowReference: WindowReference<ProfessionalUserWindowModel>,
        private readonly _inspectorService: CsiInspectorService,
        private readonly _modalHelper: ModalHelperService
    ) {

    }

    public async ngOnInit(): Promise<void> {
        this.professionalId = this._windowReference.config.model?.professionalId ?? 0;
        this.userId = this._windowReference.config.model?.userId ?? 0;

        try {
            this.isLoading = true;
            this.applyDetails(await this._inspectorService.getDetails(this.professionalId, this.userId));
        } finally {
            this.isLoading = false;
        }
    }

    public editUser(): void {
        this._modalHelper.show<CsiInspectorUserModalData, ProfessionalUser>(AddEditCsiInspectorUserComponent, {
            title: 'Edit User Account',
            model: { professionalId: this.professionalId, user: this.user! },
            size: ModalSize.medium
        }).result().subscribe(() => this.reloadDetails());
    }

    private async reloadDetails(): Promise<void> {
        try {
            this.isUserLoading = true;
            this.applyDetails(await this._inspectorService.getDetails(this.professionalId, this.userId));
        } finally {
            this.isUserLoading = false;
        }
    }

    private applyDetails(details: CsiInspectorAccountDetails): void {
        this.professional = details.professional ?? {};
        this.user = details.user;
        this.registrations = details.registrations ?? [];
        this.registrationsHeader = `Water Supplier Registrations (${this.registrations.length})`;
    }
}
