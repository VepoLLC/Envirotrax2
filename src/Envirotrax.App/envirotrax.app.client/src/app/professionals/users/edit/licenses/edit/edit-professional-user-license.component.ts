import { Component, OnInit } from "@angular/core";
import { NgForm } from "@angular/forms";
import { ModalReference } from "@developer-partners/ngx-modal-dialog";
import { ProfessionalUserLicense, ProfessionalType, professionalTypeLabels } from "../../../../../shared/models/professionals/professional-user-license";
import { ProfessionalUserLicenseService } from "../../../../../shared/services/professionals/professional-user-license.service";
import { HelperService } from "../../../../../shared/services/helpers/helper.service";
import { InputOption } from "../../../../../shared/components/input/input.component";

@Component({
    standalone: false,
    templateUrl: './edit-professional-user-license.component.html'
})
export class EditProfessionalUserLicenseComponent implements OnInit {
    public isLoading: boolean = false;
    public validationErrors: string[] = [];
    public license: ProfessionalUserLicense = {};

    public readonly professionalTypeOptions: InputOption[] = Object.values(ProfessionalType)
        .filter(v => typeof v === 'number')
        .map(v => ({ id: v as ProfessionalType, text: professionalTypeLabels[v as ProfessionalType] }));

    constructor(
        private readonly _modalReference: ModalReference<ProfessionalUserLicense, ProfessionalUserLicense>,
        private readonly _licenseService: ProfessionalUserLicenseService,
        private readonly _helper: HelperService
    ) { }

    public async ngOnInit(): Promise<void> {
        try {
            this.isLoading = true;
            this.license = await this._licenseService.get(this._modalReference.config.model!.id!);
        } finally {
            this.isLoading = false;
        }
    }

    public async save(form: NgForm): Promise<void> {
        if (form.valid) {
            try {
                this.isLoading = true;
                this.validationErrors = [];

                const result = await this._licenseService.update(this.license);
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
