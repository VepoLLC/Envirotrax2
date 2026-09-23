import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { ModalReference } from '@developer-partners/ngx-modal-dialog';
import { HelperService, InputOption, ToastService } from '@envirotrax/common-ui';
import { SharedComponentsModule } from '../../../../../shared/components/shared.components.module';
import {
    ProfessionalLicenseType,
    ProfessionalType,
    ProfessionalUserLicense
} from '../../../../../shared/models/professionals/licenses/professional-user-license';
import { ProfessionalUser } from '../../../../../shared/models/professionals/professional';
import { CsiInspectorLicenseService } from '../../../../../shared/services/csi/csi-inspector-license.service';
import { CsiInspectorUserService } from '../../../../../shared/services/csi/csi-inspector-user.service';

export interface CsiInspectorLicenseModalData {
    professionalId: number;
    license: ProfessionalUserLicense;
}

@Component({
    templateUrl: './add-edit-csi-inspector-license.component.html',
    imports: [
        CommonModule,
        FormsModule,
        SharedComponentsModule
    ],
})
export class AddEditCsiInspectorLicenseComponent implements OnInit {
    public license: ProfessionalUserLicense;
    public isLoading: boolean = false;
    public validationErrors: string[] = [];

    public licenseTypes: InputOption<ProfessionalLicenseType>[] = [];
    public users: InputOption<ProfessionalUser>[] = [];

    public get isEditMode(): boolean {
        return !!this._modalReference.config.model?.license?.id;
    }

    constructor(
        private readonly _modalReference: ModalReference<CsiInspectorLicenseModalData, ProfessionalUserLicense>,
        private readonly _licenseService: CsiInspectorLicenseService,
        private readonly _userService: CsiInspectorUserService,
        private readonly _helper: HelperService,
        private readonly _toastService: ToastService
    ) {
        // This grid only holds CSI licenses. The server pins the type too, but the DTO marks it
        // required, so it has to be on the payload or model binding rejects the request first.
        this.license = {
            ...this._modalReference.config.model!.license,
            professionalType: ProfessionalType.CsiInspector
        };
    }

    public async ngOnInit(): Promise<void> {
        const { professionalId } = this._modalReference.config.model!;

        try {
            this.isLoading = true;

            const [licenseTypes, users] = await Promise.all([
                this._licenseService.getTypesAsOptions(),
                this._userService.getAllAsOptions(professionalId)
            ]);

            this.licenseTypes = licenseTypes;
            this.users = users;
        } finally {
            this.isLoading = false;
        }
    }

    public onUserChange(userId: number): void {
        this.license.user = userId ? { id: userId } : undefined;
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

            this._toastService.successfullySaved('License');
            this._modalReference.closeSuccess(saved);
        } catch (error) {
            if (!this._helper.parseValidationErrors(error, this.validationErrors)) {
                throw error;
            }

            this._toastService.failedToSave('License');
        } finally {
            this.isLoading = false;
        }
    }

    public cancel(): void {
        this._modalReference.cancel();
    }
}
