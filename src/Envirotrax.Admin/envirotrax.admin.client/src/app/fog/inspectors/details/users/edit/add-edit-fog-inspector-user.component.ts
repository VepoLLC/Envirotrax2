import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { ModalReference } from '@developer-partners/ngx-modal-dialog';
import { HelperService, ToastService } from '@envirotrax/common-ui';
import { SharedComponentsModule } from '../../../../../shared/components/shared.components.module';
import { ProfessionalUser } from '../../../../../shared/models/professionals/professional';
import { FogInspectorUserService } from '../../../../../shared/services/fog/fog-inspector-user.service';

export interface FogInspectorUserModalData {
    professionalId: number;
    user: ProfessionalUser;
}

@Component({
    templateUrl: './add-edit-fog-inspector-user.component.html',
    imports: [
        CommonModule,
        FormsModule,
        SharedComponentsModule
    ],
})
export class AddEditFogInspectorUserComponent {
    public user: ProfessionalUser;
    public isLoading: boolean = false;
    public validationErrors: string[] = [];

    public readonly isEditMode: boolean;

    constructor(
        private readonly _modalReference: ModalReference<FogInspectorUserModalData, ProfessionalUser>,
        private readonly _userService: FogInspectorUserService,
        private readonly _helper: HelperService,
        private readonly _toastService: ToastService
    ) {
        this.user = { ...this._modalReference.config.model!.user };
        this.isEditMode = !!this.user.id;
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
