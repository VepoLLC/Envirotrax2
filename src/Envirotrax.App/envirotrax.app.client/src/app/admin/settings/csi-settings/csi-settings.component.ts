import { Component, OnInit } from "@angular/core";
import { NgForm } from "@angular/forms";
import { CsiSettings } from "../../../shared/models/settings/csi-settings";
import { CsiImpendingType, CsiNonCompliantType, CsiPastDueType } from "../../../shared/models/settings/csi-settings-enums";
import { CsiSettingsService } from "../../../shared/services/settings/csi-settings.service";
import { HelperService } from "../../../shared/services/helpers/helper.service";
import { ToastService } from "../../../shared/services/toast.service";

@Component({
    templateUrl: './csi-settings.component.html',
    standalone: false,
    styles: [`
        .nav-tabs .nav-link { color: var(--bs-body-color) !important; }
        .nav-tabs .nav-link.active { color: var(--bs-body-color) !important; }
    `]
})
export class CsiSettingsComponent implements OnInit {
    public settings: CsiSettings = {
        newlyCreatedBackflowTestExpirationDays: 0,
        impendingNotice1: CsiImpendingType.Off,
        impendingNotice2: CsiImpendingType.Off,
        pastDueNotice1: CsiPastDueType.Off,
        pastDueNotice2: CsiPastDueType.Off,
        nonCompliant1: CsiNonCompliantType.Off,
        nonCompliant2: CsiNonCompliantType.Off,
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
    public readonly gracePeriodOptions = [
        { id: null, text: 'None' },
        { id: 1, text: '1' },
        { id: 2, text: '2' },
        { id: 3, text: '3' },
        { id: 4, text: '4' },
        { id: 5, text: '5' },
        { id: 6, text: '6' },
        { id: 7, text: '7' },
    ];

    // 0 = Immediate; 30/60/90/120/150/180 = days
    public readonly expirationDaysOptions = [
        { id: 0, text: 'Immediate' },
        { id: 30, text: '30' },
        { id: 60, text: '60' },
        { id: 90, text: '90' },
        { id: 120, text: '120' },
        { id: 150, text: '150' },
        { id: 180, text: '180' },
    ];

    public readonly impendingNoticeOptions = [
        { id: CsiImpendingType.Off, text: 'Off' },
        { id: CsiImpendingType.TwoMonths, text: 'Impending in 2 Months' },
        { id: CsiImpendingType.NextMonth, text: 'Impending Next Month' },
        { id: CsiImpendingType.ThisMonth, text: 'Impending This Month' },
    ];

    public readonly pastDueNoticeOptions = [
        { id: CsiPastDueType.Off, text: 'Off' },
        { id: CsiPastDueType.LastMonth, text: 'Past Due Last Month' },
        { id: CsiPastDueType.TwoMonthsAgo, text: 'Past Due 2 Months Ago' },
        { id: CsiPastDueType.ThreeMonthsAgo, text: 'Past Due 3 Months Ago' },
        { id: CsiPastDueType.ThisMonth, text: 'Past Due This Month' },
    ];

    public readonly nonCompliantOptions = [
        { id: CsiNonCompliantType.Off, text: 'Off' },
        { id: CsiNonCompliantType.ThirtyPlusDays, text: 'Past Due 30+ Days' },
        { id: CsiNonCompliantType.SixtyPlusDays, text: 'Past Due 60+ Days' },
        { id: CsiNonCompliantType.NinetyPlusDays, text: 'Past Due 90+ Days' },
        { id: CsiNonCompliantType.OneTwentyPlusDays, text: 'Past Due 120+ Days' },
        { id: CsiNonCompliantType.OneFiftyPlusDays, text: 'Past Due 150+ Days' },
        { id: CsiNonCompliantType.OneEightyPlusDays, text: 'Past Due 180+ Days' },
        { id: CsiNonCompliantType.PastDueLastMonth, text: 'Past Due Last Month' },
        { id: CsiNonCompliantType.ExpiredTwoMonthsAgo, text: 'Expired 2 Months Ago' },
    ];

    constructor(
        private readonly _csiSettingsService: CsiSettingsService,
        private readonly _helper: HelperService,
        private readonly _toastService: ToastService,
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

                this._toastService.successfullySaved('CSI Settings');

            } catch (error) {
                if (!this._helper.parseValidationErrors(error, this.validationErrors)) {
                    throw error;
                }

                this._toastService.failedToSave('CSI Settings');

            } finally {
                this.isLoading = false;
            }
        }
    }
}
