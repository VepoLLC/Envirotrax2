import { Component } from "@angular/core";
import { NgForm } from "@angular/forms";
import { ModalReference } from "@developer-partners/ngx-modal-dialog";
import { TableSetting, TableSettingFilters } from "../../../models/table-setting";
import { TableSettingService } from "../../../services/table-setting/table-setting.service";
import { HelperService } from "../../../services/helpers/helper.service";
import { ToastService } from "../../../services/toast/toast.service";

export interface EditTableSettingConfig {
    setting: TableSetting;
    currentFilters: TableSettingFilters;
}

@Component({
    templateUrl: './edit-table-setting.component.html',
    standalone: false
})
export class EditTableSettingComponent {
    public isLoading: boolean = false;
    public validationErrors: string[] = [];
    public setting: TableSetting;
    public isNew: boolean;
    public saveCurrentFilters: boolean = false;

    private readonly _currentFilters: TableSettingFilters;

    constructor(
        private readonly _modalReference: ModalReference<EditTableSettingConfig, TableSetting>,
        private readonly _tableSettingService: TableSettingService,
        private readonly _helper: HelperService,
        private readonly _toastService: ToastService
    ) {
        const config = _modalReference.config.model!;

        this.setting = config.setting;
        this.isNew = !config.setting.id;
        this._currentFilters = config.currentFilters;
    }

    public async save(form: NgForm): Promise<void> {
        if (form.valid) {
            try {
                this.isLoading = true;
                this.validationErrors = [];

                // A new template always saves the current search criteria. An existing one only when the user asks to.
                if (this.isNew || this.saveCurrentFilters) {
                    this.setting.filters = JSON.stringify(this._currentFilters);
                }

                const saved = this.isNew
                    ? await this._tableSettingService.add(this.setting)
                    : await this._tableSettingService.update(this.setting);

                this._toastService.successfullySaved('Search Template');
                this._modalReference.closeSuccess(saved);
            } catch (error) {
                if (!this._helper.parseValidationErrors(error, this.validationErrors)) {
                    throw error;
                }

                this._toastService.failedToSave('Search Template');
            } finally {
                this.isLoading = false;
            }
        }
    }

    public cancel(): void {
        this._modalReference.cancel();
    }
}
