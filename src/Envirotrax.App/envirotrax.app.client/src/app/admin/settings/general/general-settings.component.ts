import { Component, OnInit } from "@angular/core";
import { GeneralSettings } from "../../../shared/models/settings/general-settings";
import { GeneralSettingsService } from "../../../shared/services/settings/general-settings.service";
import { HelperService } from "../../../shared/services/helpers/helper.service";
import { NgForm } from "@angular/forms";

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

    constructor(
        private readonly _generalSettingsService: GeneralSettingsService,
        private readonly _helper: HelperService,
    ) {

    }

    public async ngOnInit(): Promise<void> {
        await this.getSettings();
    }

    private async getSettings(): Promise<void> {
        this.isLoading = true;

        try {
            this.settings = await this._generalSettingsService.get();
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

            } catch (error) {
                if (!this._helper.parseValidationErrors(error, this.validationErrors)) {
                    throw error;
                }

            } finally {
                this.isLoading = false;
            }
        }
    }
}
