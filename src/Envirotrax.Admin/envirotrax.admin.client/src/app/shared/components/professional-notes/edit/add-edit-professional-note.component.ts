import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { ModalReference } from '@developer-partners/ngx-modal-dialog';
import { HelperService, ToastService } from '@envirotrax/common-ui';
import { SharedComponentsModule } from '../../shared.components.module';
import { ProfessionalNote } from '../../../models/professionals/professional-note';
import { ProfessionalType } from '../../../models/professionals/licenses/professional-user-license';
import { ProfessionalNoteService } from '../../../services/professionals/professional-note.service';

export interface ProfessionalNoteModalData {
    professionalType: ProfessionalType;
    professionalId: number;
    note: ProfessionalNote;
}

@Component({
    templateUrl: './add-edit-professional-note.component.html',
    imports: [
        CommonModule,
        FormsModule,
        SharedComponentsModule
    ],
})
export class AddEditProfessionalNoteComponent {
    public note: ProfessionalNote;
    public isLoading: boolean = false;
    public validationErrors: string[] = [];

    constructor(
        private readonly _modalReference: ModalReference<ProfessionalNoteModalData, ProfessionalNote>,
        private readonly _service: ProfessionalNoteService,
        private readonly _helper: HelperService,
        private readonly _toastService: ToastService
    ) {
        this.note = { ...this._modalReference.config.model!.note };
    }

    public async save(form: NgForm): Promise<void> {
        this.validationErrors = [];

        if (!form.valid) {
            return;
        }

        try {
            this.isLoading = true;

            const { professionalType, professionalId } = this._modalReference.config.model!;

            const saved = this.note.id
                ? await this._service.update(professionalType, professionalId, this.note)
                : await this._service.add(professionalType, professionalId, this.note);

            this._toastService.successfullySaved('Comment');
            this._modalReference.closeSuccess(saved);
        } catch (error) {
            if (!this._helper.parseValidationErrors(error, this.validationErrors)) {
                throw error;
            }

            this._toastService.failedToSave('Comment');
        } finally {
            this.isLoading = false;
        }
    }

    public cancel(): void {
        this._modalReference.cancel();
    }
}
