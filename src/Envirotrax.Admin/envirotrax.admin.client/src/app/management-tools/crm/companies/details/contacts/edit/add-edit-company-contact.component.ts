import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { ModalReference } from '@developer-partners/ngx-modal-dialog';
import { HelperService, InputOption, ToastService } from '@envirotrax/common-ui';
import { SharedComponentsModule } from '../../../../../../shared/components/shared.components.module';
import { CompanyContact } from '../../../../../../shared/models/crm/company-contact';
import { State } from '../../../../../../shared/models/lookup/state';
import { CompanyContactService } from '../../../../../../shared/services/crm/company-contact.service';

export interface CompanyContactModalData {
    companyId: number;
    contact: CompanyContact;

    // The company window's states, with the blank option first.
    states: InputOption<State>[];
}

@Component({
    templateUrl: './add-edit-company-contact.component.html',
    imports: [
        CommonModule,
        FormsModule,
        SharedComponentsModule
    ],
})
export class AddEditCompanyContactComponent {
    public contact: CompanyContact;

    // The State select binds the string id of its option and maps it back to the contact on change.
    public stateId: string;

    public isLoading: boolean = false;
    public validationErrors: string[] = [];

    public readonly isEditMode: boolean;
    public readonly states: InputOption<State>[];

    constructor(
        private readonly _modalReference: ModalReference<CompanyContactModalData, CompanyContact>,
        private readonly _contactService: CompanyContactService,
        private readonly _helper: HelperService,
        private readonly _toastService: ToastService
    ) {
        const { contact, states } = this._modalReference.config.model!;

        this.contact = { ...contact };
        this.isEditMode = !!contact.id;
        this.states = states;
        this.stateId = contact.state?.id == null ? '' : String(contact.state.id);
    }

    public onStateChange(value: string): void {
        this.contact.state = this.states.find(option => option.id === value)?.data ?? null;
    }

    public async save(form: NgForm): Promise<void> {
        this.validationErrors = [];

        if (!form.valid) {
            return;
        }

        try {
            this.isLoading = true;

            const { companyId } = this._modalReference.config.model!;

            const saved = this.isEditMode
                ? await this._contactService.update(companyId, this.contact)
                : await this._contactService.add(companyId, this.contact);

            this._toastService.successfullySaved('Contact');
            this._modalReference.closeSuccess(saved);
        } catch (error) {
            if (!this._helper.parseValidationErrors(error, this.validationErrors)) {
                throw error;
            }

            this._toastService.failedToSave('Contact');
        } finally {
            this.isLoading = false;
        }
    }

    public cancel(): void {
        this._modalReference.cancel();
    }
}
