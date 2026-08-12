import { Component, OnInit } from "@angular/core";
import { NgForm } from "@angular/forms";
import { ModalReference } from "@developer-partners/ngx-modal-dialog";
import { ProfessionalUserLicense, ProfessionalType, professionalTypeLabels } from "../../../../../shared/models/professionals/licenses/professional-user-license";
import { FogInspectorLicensesService } from "../../../../../shared/services/fog/fog-inspector-licenses.service";
import { FogInspectorSubAccountsService } from "../../../../../shared/services/fog/fog-inspector-user.service";
import { ProfessionalLicenseType } from "../../../../../shared/models/professionals/licenses/professional-license-type";
import { ProfessionalUser } from "../../../../../shared/models/professionals/professional-user";
import { HelperService } from "../../../../../shared/services/helpers/helper.service";
import { ToastService, InputOption } from '@envirotrax/common-ui';

export interface FogLicenseModalData {
    inspectorId: number;
    license: ProfessionalUserLicense;
}

@Component({
    standalone: false,
    templateUrl: './edit-fog-inspector-license.component.html'
})
export class EditFogInspectorLicenseComponent implements OnInit {
    private _allLicenseTypes: InputOption<ProfessionalLicenseType>[] = [];

    public license: ProfessionalUserLicense;
    public isLoading: boolean = false;
    public validationErrors: string[] = [];
    public licenseTypes: InputOption<ProfessionalLicenseType>[] = [];
    public userOptions: InputOption<ProfessionalUser>[] = [];

    public readonly professionalTypeOptions: InputOption<ProfessionalType>[] = Object.values(ProfessionalType)
        .filter(v => typeof v === 'number')
        .map(v => ({
            id: v as ProfessionalType,
            text: professionalTypeLabels[v as ProfessionalType],
            data: v as ProfessionalType
        }));

    public isEditMode: boolean = false;

    constructor(
        private readonly _modalReference: ModalReference<FogLicenseModalData, ProfessionalUserLicense>,
        private readonly _licensesService: FogInspectorLicensesService,
        private readonly _subAccountsService: FogInspectorSubAccountsService,
        private readonly _helper: HelperService,
        private readonly _toastService: ToastService
    ) {
        this.license = { ...this._modalReference.config.model!.license };
        this.isEditMode = !!this.license.id;
    }

    public async ngOnInit(): Promise<void> {
        const { inspectorId } = this._modalReference.config.model!;
        try {
            this.isLoading = true;
            const [types, users] = await Promise.all([
                this._licensesService.getLicenseTypes(),
                this._subAccountsService.getSubAccounts(inspectorId, { pageNumber: 1, pageSize: 1000 }, {})
            ]);
            this._allLicenseTypes = types;
            this.userOptions = users.data.map((u: ProfessionalUser) => ({ id: u.id, text: u.emailAddress ?? u.contactName, data: u }));

            if (this.license.professionalType !== undefined) {
                this.licenseTypes = this._allLicenseTypes.filter(t => t.data?.professionalType == this.license.professionalType);
            }
        } finally {
            this.isLoading = false;
        }
    }

    public userChange(userId: number): void {
        this.license.user = userId ? { id: userId } : undefined;
    }

    public professionalTypeChange(): void {
        this.licenseTypes = this._allLicenseTypes.filter(t => t.data?.professionalType == this.license.professionalType);
        this.license.licenseType = undefined;
    }

    public licenseTypeChange(typeId: number): void {
        this.license.licenseType = typeId ? { id: typeId } : undefined;
    }

    public async save(form: NgForm): Promise<void> {
        this.validationErrors = [];

        if (form.valid) {
            try {
                this.isLoading = true;

                const { inspectorId } = this._modalReference.config.model!;

                const result = this.isEditMode
                    ? await this._licensesService.update(inspectorId, this.license)
                    : await this._licensesService.add(inspectorId, this.license);

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
