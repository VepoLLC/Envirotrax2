import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { ModalReference } from '@developer-partners/ngx-modal-dialog';
import { HelperService, InputOption, ToastService } from '@envirotrax/common-ui';
import { SharedComponentsModule } from '../../../../../shared/components/shared.components.module';
import { ProfessionalLicense } from '../../../../../shared/models/professionals/licenses/professional-license';
import {
    ProfessionalLicenseType,
    ProfessionalType
} from '../../../../../shared/models/professionals/licenses/professional-user-license';
import { FogTransporterLicenseService } from '../../../../../shared/services/fog/fog-transporter-license.service';

export interface FogTransporterLicenseModalData {
    professionalId: number;
    license: ProfessionalLicense;
}

/**
 * Transporter registrations are company-level licenses, so unlike the FOG inspector dialog there is no user.
 */
@Component({
    templateUrl: './add-edit-fog-transporter-license.component.html',
    imports: [
        CommonModule,
        FormsModule,
        SharedComponentsModule
    ],
})
export class AddEditFogTransporterLicenseComponent implements OnInit {
    public license: ProfessionalLicense;
    public isLoading: boolean = false;
    public validationErrors: string[] = [];

    public licenseTypes: InputOption<ProfessionalLicenseType>[] = [];

    public readonly isEditMode: boolean;

    constructor(
        private readonly _modalReference: ModalReference<FogTransporterLicenseModalData, ProfessionalLicense>,
        private readonly _licenseService: FogTransporterLicenseService,
        private readonly _helper: HelperService,
        private readonly _toastService: ToastService
    ) {
        this.license = {
            ...this._modalReference.config.model!.license,
            professionalType: ProfessionalType.FogTransporter
        };
        this.isEditMode = !!this.license.id;
    }

    public async ngOnInit(): Promise<void> {
        await this.loadLicenseTypes();
    }

    public onLicenseTypeChange(licenseTypeId: number): void {
        this.license.licenseType = licenseTypeId ? { id: licenseTypeId } : undefined;
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
                ? await this._licenseService.update(professionalId, this.license)
                : await this._licenseService.add(professionalId, this.license);

            this._toastService.successfullySaved('Registration');
            this._modalReference.closeSuccess(saved);
        } catch (error) {
            if (!this._helper.parseValidationErrors(error, this.validationErrors)) {
                throw error;
            }

            this._toastService.failedToSave('Registration');
        } finally {
            this.isLoading = false;
        }
    }

    public cancel(): void {
        this._modalReference.cancel();
    }

    private async loadLicenseTypes(): Promise<void> {
        try {
            this.isLoading = true;
            this.licenseTypes = await this._licenseService.getTypesAsOptions();
        } finally {
            this.isLoading = false;
        }
    }
}
