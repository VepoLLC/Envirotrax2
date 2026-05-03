import { Component, OnInit } from "@angular/core";
import { NgForm } from "@angular/forms";
import { ModalReference } from "@developer-partners/ngx-modal-dialog";
import { ProfessionalUserLicense, ProfessionalType, professionalTypeLabels } from "../../../../../shared/models/professionals/licenses/professional-user-license";
import { CsiInspectorLicensesService } from "../../../../../shared/services/csi/csi-inspector-licenses.service";
import { CsiInspectorSubAccountsService } from "../../../../../shared/services/csi/csi-inspector-user.service";
import { InputOption } from "../../../../../shared/components/input/input.component";
import { ProfessionalLicenseType } from "../../../../../shared/models/professionals/licenses/professional-license-type";
import { ProfessionalUser } from "../../../../../shared/models/professionals/professional-user";
import { HelperService } from "../../../../../shared/services/helpers/helper.service";
import { ToastService } from "../../../../../shared/services/toast.service";

export interface CsiLicenseModalData {
    inspectorId: number;
    license: ProfessionalUserLicense;
}

@Component({
    standalone: false,
    templateUrl: './add-edit-csi-inspector-license.component.html'
})
export class CsiInspectorAddEditLicenseComponent implements OnInit {
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

    public get isEditMode(): boolean {
        return !!this._modalReference.config.model?.license?.id;
    }

    constructor(
        private readonly _modalReference: ModalReference<CsiLicenseModalData, ProfessionalUserLicense>,
        private readonly _licensesService: CsiInspectorLicensesService,
        private readonly _subAccountsService: CsiInspectorSubAccountsService,
        private readonly _helper: HelperService,
        private readonly _toastService: ToastService
    ) {
        this.license = { ...this._modalReference.config.model!.license };
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
