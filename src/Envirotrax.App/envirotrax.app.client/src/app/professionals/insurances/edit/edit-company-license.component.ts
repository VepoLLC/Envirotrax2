import { Component, OnInit } from "@angular/core";
import { NgForm } from "@angular/forms";
import { ModalReference } from "@developer-partners/ngx-modal-dialog";
import { InputOption, ToastService } from '@envirotrax/common-ui';
import { ProfessionalLicense } from "../../../shared/models/professionals/licenses/professional-license";
import { LicenseScope, ProfessionalLicenseType } from "../../../shared/models/professionals/licenses/professional-license-type";
import { ProfessionalLicenseService } from "../../../shared/services/professionals/professional-license.service";
import { ProfessionalUserLicenseService } from "../../../shared/services/professionals/professional-user-license.service";
import { HelperService } from "../../../shared/services/helpers/helper.service";

@Component({
    standalone: false,
    templateUrl: './edit-company-license.component.html'
})
export class EditCompanyLicenseComponent implements OnInit {
    public license: ProfessionalLicense = {};
    public licenseTypes: InputOption<ProfessionalLicenseType>[] = [];
    public isLoading: boolean = false;
    public validationErrors: string[] = [];

    constructor(
        private readonly _licenseService: ProfessionalLicenseService,
        private readonly _userLicenseService: ProfessionalUserLicenseService,
        private readonly _modalReference: ModalReference<ProfessionalLicense, ProfessionalLicense>,
        private readonly _helper: HelperService,
        private readonly _toastService: ToastService
    ) {
    }

    public async ngOnInit(): Promise<void> {
        try {
            this.isLoading = true;

            const [license, licenseTypes] = await Promise.all([
                this._licenseService.get(this._modalReference.config.model!.id!),
                this._userLicenseService.getAllTypesAsOptions({ sort: {}, filter: [{ columnName: 'licenseScope', comparisonOperator: 'Eq', value: LicenseScope.Company.toString() }] }, false)
            ]);

            this.license = license;
            this.licenseTypes = licenseTypes.filter(type => type.data?.professionalType == license.professionalType);
        } finally {
            this.isLoading = false;
        }
    }

    public licenseTypeChange(typeId: number): void {
        if (typeId) {
            this.license.licenseType = {
                id: typeId
            };
        } else {
            this.license.licenseType = undefined;
        }
    }

    public async save(form: NgForm): Promise<void> {
        this.validationErrors = [];

        if (form.valid) {
            try {
                this.isLoading = true;

                const result = await this._licenseService.update(this.license);

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
