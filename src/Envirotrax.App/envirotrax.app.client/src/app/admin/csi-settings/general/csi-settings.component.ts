import { Component, OnInit } from "@angular/core";
import { NgForm } from "@angular/forms";
import { CsiSettings } from "../../../shared/models/settings/csi-settings";
import { CsiSettingsService } from "../../../shared/services/settings/csi-settings.service";
import { HelperService } from "../../../shared/services/helpers/helper.service";

@Component({
    templateUrl: './csi-settings.component.html',
    standalone: false
})
export class CsiSettingsComponent implements OnInit {
    public settings: CsiSettings = {
        newlyCreatedBackflowTestExpirationDays: 0,
        impendingNotice1: 0,
        impendingNotice2: 0,
        pastDueNotice1: 0,
        pastDueNotice2: 0,
        nonCompliant1: 0,
        nonCompliant2: 0,
        impendingLettersBackgroundColor: '#d3d3d3',
        impendingLettersForegroundColor: '#000000',
        impendingLettersBorderColor: '#000000',
        pastDueLettersBackgroundColor: '#d3d3d3',
        pastDueLettersForegroundColor: '#000000',
        pastDueLettersBorderColor: '#000000',
        nonCompliantLettersBackgroundColor: '#d3d3d3',
        nonCompliantLettersForegroundColor: '#000000',
        nonCompliantLettersBorderColor: '#000000',
    };
    public isLoading: boolean = false;
    public validationErrors: string[] = [];

    // None, 1-7 days
    public readonly gracePeriodOptions: { value: number | null; label: string }[] = [
        { value: null, label: 'None' },
        { value: 1, label: '1' },
        { value: 2, label: '2' },
        { value: 3, label: '3' },
        { value: 4, label: '4' },
        { value: 5, label: '5' },
        { value: 6, label: '6' },
        { value: 7, label: '7' },
    ];

    // 0 = Immediate; 30/60/90/120/150/180 = days
    public readonly expirationDaysOptions: { value: number; label: string }[] = [
        { value: 0, label: 'Immediate' },
        { value: 30, label: '30' },
        { value: 60, label: '60' },
        { value: 90, label: '90' },
        { value: 120, label: '120' },
        { value: 150, label: '150' },
        { value: 180, label: '180' },
    ];

    public readonly impendingNoticeOptions: { value: number; label: string }[] = [
        { value: 0, label: 'Off' },
        { value: 1, label: 'Impending in 2 Months' },
        { value: 2, label: 'Impending Next Month' },
        { value: 3, label: 'Impending This Month' },
    ];

    public readonly pastDueNoticeOptions: { value: number; label: string }[] = [
        { value: 0, label: 'Off' },
        { value: 1, label: 'Past Due Last Month' },
        { value: 2, label: 'Past Due 2 Months Ago' },
        { value: 3, label: 'Past Due 3 Months Ago' },
        { value: 4, label: 'Past Due This Month' },
    ];

    public readonly nonCompliantOptions: { value: number; label: string }[] = [
        { value: 0, label: 'Off' },
        { value: 1, label: 'Past Due 30+ Days' },
        { value: 2, label: 'Past Due 60+ Days' },
        { value: 3, label: 'Past Due 90+ Days' },
        { value: 4, label: 'Past Due 120+ Days' },
        { value: 5, label: 'Past Due 150+ Days' },
        { value: 6, label: 'Past Due 180+ Days' },
        { value: 7, label: 'Past Due Last Month' },
        { value: 8, label: 'Expired 2 Months Ago' },
    ];

    constructor(
        private readonly _csiSettingsService: CsiSettingsService,
        private readonly _helper: HelperService,
    ) {
    }

    public async ngOnInit(): Promise<void> {
        await this.getSettings();
    }

    private async getSettings(): Promise<void> {
        this.isLoading = true;

        try {
            this.settings = await this._csiSettingsService.get();
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
                    result = await this._csiSettingsService.update(this.settings);
                } else {
                    result = await this._csiSettingsService.add(this.settings);
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
