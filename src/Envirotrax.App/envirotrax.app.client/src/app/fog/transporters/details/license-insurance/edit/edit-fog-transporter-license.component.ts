import { Component, OnInit } from "@angular/core";
import { NgForm } from "@angular/forms";
import { ModalReference } from "@developer-partners/ngx-modal-dialog";
import { ProfessionalType } from "../../../../../shared/models/professionals/licenses/professional-user-license";
import { ProfessionalLicense } from "../../../../../shared/models/professionals/licenses/professional-license";
import { FogTransporterLicensesService } from "../../../../../shared/services/fog/fog-transporter-licenses.service";
import { LicenseScope, ProfessionalLicenseType } from "../../../../../shared/models/professionals/licenses/professional-license-type";
import { HelperService } from "../../../../../shared/services/helpers/helper.service";
import { InputOption, ToastService } from "@envirotrax/common-ui";

export interface FogLicenseModalData {
    transporterId: number;
    license: ProfessionalLicense;
}

@Component({
    standalone: false,
    templateUrl: './edit-fog-transporter-license.component.html'
})
export class EditFogTransporterLicenseComponent implements OnInit {
    public license: ProfessionalLicense;
    public isLoading: boolean = false;
    public validationErrors: string[] = [];
    public licenseTypes: InputOption<ProfessionalLicenseType>[] = [];

    public isEditMode: boolean = false;

    constructor(
        private readonly _modalReference: ModalReference<FogLicenseModalData, ProfessionalLicense>,
        private readonly _licensesService: FogTransporterLicensesService,
        private readonly _helper: HelperService,
        private readonly _toastService: ToastService
    ) {
        this.license = { ...this._modalReference.config.model!.license };
        this.license.professionalType = ProfessionalType.FogTransporter;
        this.isEditMode = !!this.license.id;
    }

    public async ngOnInit(): Promise<void> {
        try {
            this.isLoading = true;

            const types = await this._licensesService.getLicenseTypes();

            this.licenseTypes = types.filter(t => t.data?.professionalType == ProfessionalType.FogTransporter && t.data?.licenseScope == LicenseScope.Company);
        } finally {
            this.isLoading = false;
        }
    }

    public licenseTypeChange(typeId: number): void {
        this.license.licenseType = typeId ? { id: typeId } : undefined;
    }

    public async save(form: NgForm): Promise<void> {
        this.validationErrors = [];

        if (form.valid) {
            try {
                this.isLoading = true;

                const { transporterId } = this._modalReference.config.model!;

                const result = this.isEditMode
                    ? await this._licensesService.update(transporterId, this.license)
                    : await this._licensesService.add(transporterId, this.license);

                this._toastService.successfullySaved('License');
                this._modalReference.closeSuccess(result);
            } catch (error) {
                if (!this._helper.parseValidationErrors(error, this.validationErrors)) {
                    throw error;
                }
                this._toastService.failedToSave('License');
            } finally {
                this.isLoading = false;
            }
        }
    }

    public cancel(): void {
        this._modalReference.cancel();
    }
}
