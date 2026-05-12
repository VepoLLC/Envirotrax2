import { Component, OnInit } from "@angular/core";
import { NgForm } from "@angular/forms";
import { BackflowSettings } from "../../../shared/models/settings/backflow-settings";
import { BackflowExpiredType, BackflowExpiringType, BackflowNonCompliantType, BackflowOutOfServiceType, BackflowTestingMethodType } from "../../../shared/models/settings/backflow-testing-settings-enum";
import { BackflowTestingSettingsService } from "../../../shared/services/settings/backflow-testing-settings.service";
import { HelperService } from "../../../shared/services/helpers/helper.service";
import { ToastService } from "../../../shared/services/toast.service";

@Component({
    templateUrl: './backflow-letter-message-settings.component.html',
    standalone: false
})
export class BackflowLetterMessageSettingsComponent implements OnInit {
    public settings: BackflowSettings = {
        testingMethod: BackflowTestingMethodType.USC,
        gracePeriodDays: null,
        outOfServiceType: BackflowOutOfServiceType.VepoManaged,
        expiringNotice1: BackflowExpiringType.Off,
        expiringNotice2: BackflowExpiringType.Off,
        expiredNotice1: BackflowExpiredType.Off,
        expiredNotice2: BackflowExpiredType.Off,
        backflowNonCompliant1: BackflowNonCompliantType.Off,
        backflowNonCompliant2: BackflowNonCompliantType.Off,
        expiringLettersBackgroundColor: '#d3d3d3',
        expiringLettersForegroundColor: '#000000',
        expiringLettersBorderColor: '#000000',
        expiredLettersBackgroundColor: '#d3d3d3',
        expiredLettersForegroundColor: '#000000',
        expiredLettersBorderColor: '#000000',
        nonCompliantLettersBackgroundColor: '#d3d3d3',
        nonCompliantLettersForegroundColor: '#000000',
        nonCompliantLettersBorderColor: '#000000',
    };
    public isLoading: boolean = false;
    public validationErrors: string[] = [];

    public readonly fontOptions = [
        { id: 'Arial', text: 'Arial' },
        { id: 'Times New Roman', text: 'Times New Roman' },
        { id: 'Courier New', text: 'Courier New' },
        { id: 'Verdana', text: 'Verdana' },
        { id: 'Georgia', text: 'Georgia' },
        { id: 'Tahoma', text: 'Tahoma' },
        { id: 'Trebuchet MS', text: 'Trebuchet MS' },
    ];

    public readonly fontSizeOptions = [
        { id: 8, text: '8' },
        { id: 9, text: '9' },
        { id: 10, text: '10' },
        { id: 11, text: '11' },
        { id: 12, text: '12' },
        { id: 14, text: '14' },
        { id: 16, text: '16' },
        { id: 18, text: '18' },
        { id: 20, text: '20' },
        { id: 22, text: '22' },
        { id: 24, text: '24' },
    ];

    constructor(
        private readonly _backflowSettingsService: BackflowTestingSettingsService,
        private readonly _helper: HelperService,
        private readonly _toastService: ToastService,
    ) { }

    public async ngOnInit(): Promise<void> {
        await this.getSettings();
    }

    private async getSettings(): Promise<void> {
        this.isLoading = true;
        try {
            this.settings = await this._backflowSettingsService.get();
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
                    result = await this._backflowSettingsService.update(this.settings);
                } else {
                    result = await this._backflowSettingsService.add(this.settings);
                }
                if (result) {
                    this.settings = result;
                }
                this._toastService.successfullySaved('Backflow Letter Message Settings');
            } catch (error) {
                if (!this._helper.parseValidationErrors(error, this.validationErrors)) {
                    throw error;
                }
                this._toastService.failedToSave('Backflow Letter Message Settings');
            } finally {
                this.isLoading = false;
            }
        }
    }
}
