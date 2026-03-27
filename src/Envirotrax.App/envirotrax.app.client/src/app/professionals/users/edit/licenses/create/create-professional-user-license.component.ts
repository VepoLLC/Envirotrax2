import { Component } from "@angular/core";
import { NgForm } from "@angular/forms";
import { ModalReference } from "@developer-partners/ngx-modal-dialog";
import { ProfessionalUserLicense, ProfessionalType, professionalTypeLabels } from "../../../../../shared/models/professionals/professional-user-license";
import { ProfessionalUserLicenseService } from "../../../../../shared/services/professionals/professional-user-license.service";
import { HelperService } from "../../../../../shared/services/helpers/helper.service";
import { InputOption } from "../../../../../shared/components/input/input.component";

@Component({
    standalone: false,
    templateUrl: './create-professional-user-license.component.html'
})
export class CreateProfessionalUserLicenseComponent {
    public isLoading: boolean = false;
    public validationErrors: string[] = [];
    public license: ProfessionalUserLicense = {};

    public readonly professionalTypeOptions: InputOption[] = Object.values(ProfessionalType)
        .filter(v => typeof v === 'number')
        .map(v => ({ id: v as ProfessionalType, text: professionalTypeLabels[v as ProfessionalType] }));

    constructor(
        private readonly _modalReference: ModalReference<number, ProfessionalUserLicense>,
        private readonly _licenseService: ProfessionalUserLicenseService,
        private readonly _helper: HelperService
    ) { }

    public async save(form: NgForm): Promise<void> {
        if (form.valid) {
            try {
                this.isLoading = true;
                this.validationErrors = [];

                const userId = this._modalReference.config.model!;
                const result = await this._licenseService.add({
                    ...this.license,
                    user: { id: userId }
                });

                this._modalReference.closeSuccess(result);
            } catch (e) {
                if (!this._helper.parseValidationErrors(e, this.validationErrors)) {
                    throw e;
                }
            } finally {
                this.isLoading = false;
            }
        }
    }

    public cancel(): void {
        this._modalReference.cancel();
    }
}
