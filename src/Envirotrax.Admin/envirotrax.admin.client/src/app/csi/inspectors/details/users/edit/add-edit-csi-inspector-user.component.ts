import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { ModalReference } from '@developer-partners/ngx-modal-dialog';
import { HelperService, ToastService } from '@envirotrax/common-ui';
import { SharedComponentsModule } from '../../../../../shared/components/shared.components.module';
import { ProfessionalUser } from '../../../../../shared/models/professionals/professional';
import { CsiInspectorUserService } from '../../../../../shared/services/csi/csi-inspector-user.service';

export interface CsiInspectorUserModalData {
    professionalId: number;
    user: ProfessionalUser;
}

@Component({
    templateUrl: './add-edit-csi-inspector-user.component.html',
    imports: [
        CommonModule,
        FormsModule,
        SharedComponentsModule
    ],
})
export class AddEditCsiInspectorUserComponent {
    public user: ProfessionalUser;
    public isLoading: boolean = false;
    public validationErrors: string[] = [];

    public get isEditMode(): boolean {
        return !!this._modalReference.config.model?.user?.id;
    }

    constructor(
        private readonly _modalReference: ModalReference<CsiInspectorUserModalData, ProfessionalUser>,
        private readonly _userService: CsiInspectorUserService,
        private readonly _helper: HelperService,
        private readonly _toastService: ToastService
    ) {
        this.user = { ...this._modalReference.config.model!.user };
    }

    public async save(form: NgForm): Promise<void> {
        this.validationErrors = [];

        if (!form.valid) {
            return;
        }

        try {
            this.isLoading = true;

            const { professionalId } = this._modalReference.config.model!;

            const saved = this.isEditMode
                ? await this._userService.update(professionalId, this.user)
                : await this._userService.add(professionalId, this.user);

            this._toastService.successfullySaved('User Account');
            this._modalReference.closeSuccess(saved);
        } catch (error) {
            if (!this._helper.parseValidationErrors(error, this.validationErrors)) {
                throw error;
            }

            this._toastService.failedToSave('User Account');
        } finally {
            this.isLoading = false;
        }
    }

    public cancel(): void {
        this._modalReference.cancel();
    }
}
