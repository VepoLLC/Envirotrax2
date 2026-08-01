import { Component, OnInit } from "@angular/core";
import { ActivatedRoute } from "@angular/router";
import { BackflowTest } from "../../../../shared/models/backflow/backflow-test";
import { BackflowTestService } from "../../../../shared/services/backflow/backflow-test.service";
import { BackflowSettingsService } from "../../../../shared/services/backflow/backflow-settings.service";
import { BackflowTestingSettings } from "../../../../shared/models/backflow/backflow-testing-settings";
import { DownloadService } from "../../../../shared/services/download.service";
import { HelperService } from "../../../../shared/services/helpers/helper.service";
import { ToastService, ToastType } from "../../../../shared/services/toast.service";

@Component({
    selector: 'app-professional-backflow-test-details',
    standalone: false,
    templateUrl: './professional-backflow-test-details.component.html'
})
export class ProfessionalBackflowTestDetailsComponent implements OnInit {
    public id: number = 0;
    public test: BackflowTest | null = null;
    public isLoading: boolean = false;
    public settings: BackflowTestingSettings | null = null;

    constructor(
        private readonly _activatedRoute: ActivatedRoute,
        private readonly _testService: BackflowTestService,
        private readonly _settingsService: BackflowSettingsService,
        private readonly _downloadService: DownloadService,
        private readonly _helper: HelperService,
        private readonly _toastService: ToastService
    ) { }

    public ngOnInit(): void {
        this.initialize();
    }

    private initialize(): void {
        this._activatedRoute.paramMap.subscribe(async params => {
            const id = params.get('id');

            if (id) {
                this.id = +id;
                await this.loadTest();
            }
        });
    }

    private async loadTest(): Promise<void> {
        try {
            this.isLoading = true;
            this.test = await this._testService.getForProfessional(this.id);

            const waterSupplierId = this.test?.waterSupplier?.id;
            if (waterSupplierId) {
                this.settings = await this._settingsService.getTestingSettings(waterSupplierId);
            }
        } finally {
            this.isLoading = false;
        }
    }

    public async exportPdf(): Promise<void> {
        if (this.test == null || !this.test.transactionId) {
            return;
        }

        try {
            this.isLoading = true;
            const blob = await this._testService.getPdfForProfessional(this.test.id);
            this._downloadService.downloadFileFromBlob(blob);
        } catch (e) {
            const validationErrors: string[] = [];
            if (this._helper.parseValidationErrors(e, validationErrors)) {
                this._toastService.show({ text: validationErrors[0], type: ToastType.Error });
            } else {
                throw e;
            }
        } finally {
            this.isLoading = false;
        }
    }
}
