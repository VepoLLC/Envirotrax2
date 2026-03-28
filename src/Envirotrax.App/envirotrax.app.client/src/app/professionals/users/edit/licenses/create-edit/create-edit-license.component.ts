import { Component, OnInit } from "@angular/core";
import { NgForm } from "@angular/forms";
import { ModalReference } from "@developer-partners/ngx-modal-dialog";
import { ProfessionalUserLicense, ProfessionalType, professionalTypeLabels } from "../../../../../shared/models/professionals/licenses/professional-user-license";
import { ProfessionalUserLicenseService } from "../../../../../shared/services/professionals/professional-user-license.service";
import { HelperService } from "../../../../../shared/services/helpers/helper.service";
import { InputOption } from "../../../../../shared/components/input/input.component";
import { ProfessionalLicenseType } from "../../../../../shared/models/professionals/licenses/professional-license-type";
import { ProfesisonalService } from "../../../../../shared/services/professionals/professional.service";

@Component({
    standalone: false,
    templateUrl: './create-edit-license.component.html'
})
export class CreateEditLicenseComponent implements OnInit {
    private _allLicenseTypes: InputOption<ProfessionalLicenseType>[] = [];

    public isLoading: boolean = false;
    public validationErrors: string[] = [];
    public license: ProfessionalUserLicense = {};

    public licenseTypes: InputOption<ProfessionalLicenseType>[] = [];

    public readonly professionalTypeOptions: InputOption<ProfessionalType>[] = Object.values(ProfessionalType)
        .filter(v => typeof v === 'number')
        .map(v => ({
            id: v as ProfessionalType,
            text: professionalTypeLabels[v as ProfessionalType],
            data: v as ProfessionalType
        }));

    public get isEditMode(): boolean {
        return !!this._modalReference.config.model?.id;
    }

    constructor(
        private readonly _modalReference: ModalReference<ProfessionalUserLicense, ProfessionalUserLicense>,
        private readonly _licenseService: ProfessionalUserLicenseService,
        private readonly _helper: HelperService,
        private readonly _professionalService: ProfesisonalService
    ) {
        this.license = this._modalReference.config.model!;
    }

    public async ngOnInit(): Promise<void> {
        try {
            this.isLoading = true;

            const [license, professional, licenseTypes] = await Promise.all([
                this.getLicense(),
                this._professionalService.getLoggedInProfessional(),
                this._licenseService.getAllTypesAsOptions({}, true)
            ]);

            this.license = license;
            this._allLicenseTypes = licenseTypes;
            this.licenseTypes = this._allLicenseTypes.filter(l => l.data?.state?.id == professional.state?.id);
        } finally {
            this.isLoading = false;
        }
    }

    private async getLicense(): Promise<ProfessionalUserLicense> {
        if (this.isEditMode) {
            return this._licenseService.get(this._modalReference.config.model!.id!);
        }

        return this.license;
    }

    public async save(form: NgForm): Promise<void> {
        if (form.valid) {
            try {
                this.isLoading = true;
                this.validationErrors = [];

                const result = this.isEditMode
                    ? await this._licenseService.update(this.license)
                    : await this._licenseService.add(this.license);

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

    public licenseTypeChange(typeId: number): void {
        if (typeId) {
            this.license.licenseType = {
                id: typeId
            };
        } else {
            this.license.licenseType = undefined;
        }
    }

    public professionChange(): void {
        this.licenseTypes = this._allLicenseTypes.filter(t => t.data?.professionalType == this.license.professionalType);
        this.license.licenseType = undefined;
    }
}
