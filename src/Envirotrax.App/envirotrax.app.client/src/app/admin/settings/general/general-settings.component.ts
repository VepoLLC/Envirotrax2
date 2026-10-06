import { Component, OnInit } from "@angular/core";
import { GeneralSettings } from "../../../shared/models/settings/general-settings";
import { GeneralSettingsService } from "../../../shared/services/settings/general-settings.service";
import { HelperService } from "../../../shared/services/helpers/helper.service";
import { NgForm } from "@angular/forms";
import { ToastService } from '@envirotrax/common-ui';
import { SettingsSection } from "../../../shared/models/settings/settings-section";
import { SettingsCopyService } from "../../../shared/services/settings/settings-copy.service";

@Component({
    templateUrl: './general-settings.component.html',
    standalone: false,
    styles: [`
        :host ::ng-deep .input-group vp-input {
            flex: 1 1 auto;
            width: 1%;
            min-width: 0;
        }
        :host ::ng-deep .input-group vp-input > div {
            display: contents;
        }
    `]
})
export class GeneralSettingsComponent implements OnInit {
    public settings: GeneralSettings = {};
    public isLoading: boolean = false;
    public validationErrors: string[] = [];

    public showProgramUpdateWarning: boolean = false;

    public canCopyFromParent: boolean = false;

    constructor(
        private readonly _generalSettingsService: GeneralSettingsService,
        private readonly _helper: HelperService,
        private readonly _toastService: ToastService,
        private readonly _settingsCopyService: SettingsCopyService
    ) {

    }

    public async ngOnInit(): Promise<void> {
        await this.getSettings();
        this.canCopyFromParent = await this._settingsCopyService.canCopyFromParent();
    }

    private async getSettings(): Promise<void> {
        this.isLoading = true;

        try {
            this.settings = await this._generalSettingsService.get();
        } finally {
            this.isLoading = false;
        }
    }

    public async copyFromParent(): Promise<void> {
        try {
            this.isLoading = true;

            const copied = await this._settingsCopyService.confirmAndCopyFromParent(SettingsSection.General);

            if (copied) {
                this.validationErrors = [];
                this.showProgramUpdateWarning = false;

                await this.getSettings();
            }
        } finally {
            this.isLoading = false;
        }
    }

    public async save(form: NgForm): Promise<void> {
        if (form.valid) {
            try {
                this.isLoading = true;

                let result;
                if (this.settings.id) {
                    result = await this._generalSettingsService.update(this.settings);
                } else {
                    result = await this._generalSettingsService.add(this.settings);
                }

                if (result) {
                    this.settings = result;
                }

                this._toastService.successfullySaved('General Settings');
            } catch (error) {
                if (!this._helper.parseValidationErrors(error, this.validationErrors)) {
                    throw error;
                }

                this._toastService.failedToSave('General Settings');
            } finally {
                this.isLoading = false;
            }
        }
    }
}
