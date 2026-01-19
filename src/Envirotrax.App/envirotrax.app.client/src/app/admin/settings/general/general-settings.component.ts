import { Component, OnInit } from "@angular/core";
import { GeneralSettings } from "../../../shared/models/settings/general-settings";
import { GeneralSettingsService } from "../../../shared/services/settings/general-settings.service";
import { HelperService } from "../../../shared/services/helpers/helper.service";
import { NgForm } from "@angular/forms";

@Component({
    templateUrl: './general-settings.component.html',
    standalone: false
})
export class GeneralSettingsComponent implements OnInit {
    public settings: GeneralSettings = {};
    public isLoading: boolean = false;
    public validationErrors: string[] = [];

    constructor(
        private readonly _generalSettingsService: GeneralSettingsService,
        private readonly _helper: HelperService,
    ) {

    }

    public async ngOnInit(): Promise<void> {
        await this.getSettings();
    }

    private async getSettings(): Promise<void> {
        try {
            this.isLoading = true;
            const response = await this._generalSettingsService.getAll({ pageNumber: 1, pageSize: 1 }, { sort: {}, filter: [] });

            if (response.data && response.data.length > 0) {
                this.settings = response.data[0];
            }
        } catch (error) {
            // If no settings exist yet, keep the empty object
            console.log('No settings found, will create new on save', error);
        } finally {
            this.isLoading = false;
        }
    }

    public async save(form: NgForm): Promise<void> {
        if (form.valid) {
            try {
                this.isLoading = true;

                let result;
                if (this.settings.waterSupplierId) {
                    result = await this._generalSettingsService.update(this.settings);
                } else {
                    result = await this._generalSettingsService.add(this.settings);
                }

                if (result) {
                    this.settings = result;
                }

                //this._toastService.successfullySaved();
            } catch (error) {
                if (!this._helper.parseValidationErrors(error, this.validationErrors)) {
                    throw error;
                }

                //this._toastService.failedToSave();
            } finally {
                this.isLoading = false;
            }
        }
    }
}
