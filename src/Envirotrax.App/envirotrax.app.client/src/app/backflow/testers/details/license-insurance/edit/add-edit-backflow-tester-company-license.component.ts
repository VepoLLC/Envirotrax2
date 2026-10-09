import { Component, OnInit } from "@angular/core";
import { NgForm } from "@angular/forms";
import { ModalReference } from "@developer-partners/ngx-modal-dialog";
import { ProfessionalType } from "../../../../../shared/models/professionals/licenses/professional-user-license";
import { ProfessionalLicense } from "../../../../../shared/models/professionals/licenses/professional-license";
import { LicenseScope, ProfessionalLicenseType } from "../../../../../shared/models/professionals/licenses/professional-license-type";
import { BackflowTesterLicensesService } from "../../../../../shared/services/backflow/backflow-tester-licenses.service";
import { HelperService } from "../../../../../shared/services/helpers/helper.service";
import { ToastService, InputOption } from '@envirotrax/common-ui';

export interface BackflowCompanyLicenseModalData {
    testerId: number;
    license: ProfessionalLicense;
}

@Component({
    standalone: false,
    templateUrl: './add-edit-backflow-tester-company-license.component.html'
})
export class AddEditBackflowTesterCompanyLicenseComponent implements OnInit {
    public license: ProfessionalLicense;
    public isEditMode: boolean;
    public isLoading: boolean = false;
    public validationErrors: string[] = [];
    public licenseTypes: InputOption<ProfessionalLicenseType>[] = [];

    constructor(
        private readonly _modalReference: ModalReference<BackflowCompanyLicenseModalData, ProfessionalLicense>,
        private readonly _licensesService: BackflowTesterLicensesService,
        private readonly _helper: HelperService,
        private readonly _toastService: ToastService
    ) {
        this.license = { ...this._modalReference.config.model!.license };
        this.license.professionalType = ProfessionalType.Bpat;
        this.isEditMode = !!this.license.id;
    }

    public async ngOnInit(): Promise<void> {
        try {
            this.isLoading = true;

            const types = await this._licensesService.getLicenseTypes();

            this.licenseTypes = types.filter(t => t.data?.professionalType == ProfessionalType.Bpat && t.data?.licenseScope == LicenseScope.Company);
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

                const { testerId } = this._modalReference.config.model!;

                let result: ProfessionalLicense;

                if (this.isEditMode) {
                    result = await this._licensesService.updateCompanyLicense(testerId, this.license);
                } else {
                    result = await this._licensesService.addCompanyLicense(testerId, this.license);
                }

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
