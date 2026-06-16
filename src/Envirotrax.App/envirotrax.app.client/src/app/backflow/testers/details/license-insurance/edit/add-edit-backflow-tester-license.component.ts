import { Component, OnInit } from "@angular/core";
import { NgForm } from "@angular/forms";
import { ModalReference } from "@developer-partners/ngx-modal-dialog";
import { ProfessionalUserLicense, ProfessionalType } from "../../../../../shared/models/professionals/licenses/professional-user-license";
import { BackflowTesterLicensesService } from "../../../../../shared/services/backflow/backflow-tester-licenses.service";
import { BackflowTesterUserService } from "../../../../../shared/services/backflow/backflow-tester-user.service";
import { ProfessionalLicenseType } from "../../../../../shared/models/professionals/licenses/professional-license-type";
import { ProfessionalUser } from "../../../../../shared/models/professionals/professional-user";
import { HelperService } from "../../../../../shared/services/helpers/helper.service";
import { ToastService } from "../../../../../shared/services/toast.service";
import { InputOption } from "@envirotrax/common-ui";

export interface BackflowLicenseModalData {
    testerId: number;
    license: ProfessionalUserLicense;
}

@Component({
    standalone: false,
    templateUrl: './add-edit-backflow-tester-license.component.html'
})
export class AddEditBackflowTesterLicenseComponent implements OnInit {
    private _allLicenseTypes: InputOption<ProfessionalLicenseType>[] = [];

    public license: ProfessionalUserLicense;
    public isLoading: boolean = false;
    public validationErrors: string[] = [];
    public licenseTypes: InputOption<ProfessionalLicenseType>[] = [];
    public userOptions: InputOption<ProfessionalUser>[] = [];

    public get isEditMode(): boolean {
        return !!this._modalReference.config.model?.license?.id;
    }

    constructor(
        private readonly _modalReference: ModalReference<BackflowLicenseModalData, ProfessionalUserLicense>,
        private readonly _licensesService: BackflowTesterLicensesService,
        private readonly _subAccountsService: BackflowTesterUserService,
        private readonly _helper: HelperService,
        private readonly _toastService: ToastService
    ) {
        this.license = { ...this._modalReference.config.model!.license };
        if (this.license.professionalType == null) {
            this.license.professionalType = ProfessionalType.Bpat;
        }
    }

    public async ngOnInit(): Promise<void> {
        const { testerId } = this._modalReference.config.model!;
        try {
            this.isLoading = true;
            const [types, users] = await Promise.all([
                this._licensesService.getLicenseTypes(),
                this._subAccountsService.getSubAccounts(testerId, { pageNumber: 1, pageSize: 1000 }, {})
            ]);
            this._allLicenseTypes = types;
            this.userOptions = users.data.map((u: ProfessionalUser) => ({ id: u.id, text: u.emailAddress ?? u.contactName, data: u }));
            this.licenseTypes = this._allLicenseTypes.filter(t => t.data?.professionalType == this.license.professionalType);
        } finally {
            this.isLoading = false;
        }
    }

    public userChange(userId: number): void {
        this.license.user = userId ? { id: userId } : undefined;
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

                const result = this.isEditMode
                    ? await this._licensesService.update(testerId, this.license)
                    : await this._licensesService.add(testerId, this.license);

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
