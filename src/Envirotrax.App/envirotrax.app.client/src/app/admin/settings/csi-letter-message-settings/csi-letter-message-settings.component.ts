import { Component, OnInit } from "@angular/core";
import { NgForm } from "@angular/forms";
import { CsiSettings } from "../../../shared/models/settings/csi-settings";
import { CsiImpendingType, CsiNonCompliantType, CsiPastDueType } from "../../../shared/models/settings/csi-settings-enums";
import { CsiSettingsService } from "../../../shared/services/settings/csi-settings.service";
import { HelperService } from "../../../shared/services/helpers/helper.service";
import { ToastService } from "../../../shared/services/toast.service";

@Component({
    templateUrl: './csi-letter-message-settings.component.html',
    standalone: false,
    styles: ['.vp-tabs .nav-link { color: var(--bs-body-color) !important; } .vp-tabs .nav-link.active { color: var(--bs-body-color) !important; }']
})
export class CsiLetterMessageSettingsComponent implements OnInit {
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
        private readonly _csiSettingsService: CsiSettingsService,
        private readonly _helper: HelperService,
        private readonly _toastService: ToastService,
    ) {}

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
                this._toastService.successfullySaved('CSI Letter Message Settings');
            } catch (error) {
                if (!this._helper.parseValidationErrors(error, this.validationErrors)) {
                    throw error;
                }
                this._toastService.failedToSave('CSI Letter Message Settings');
            } finally {
                this.isLoading = false;
            }
        }
    }
}
