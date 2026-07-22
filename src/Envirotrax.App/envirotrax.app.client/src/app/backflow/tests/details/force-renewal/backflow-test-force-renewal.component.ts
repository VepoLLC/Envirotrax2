import { Component } from "@angular/core";
import { NgForm } from "@angular/forms";
import { ModalReference } from "@developer-partners/ngx-modal-dialog";
import { BackflowTest } from "../../../../shared/models/backflow/backflow-test";
import { BackflowTestService } from "../../../../shared/services/backflow/backflow-test.service";
import { HelperService } from "../../../../shared/services/helpers/helper.service";

@Component({
    standalone: false,
    templateUrl: './backflow-test-force-renewal.component.html'
})
export class BackflowTestForceRenewalComponent {
    public forceRenewal: boolean = false;
    public forceRenewalYears: number | null = null;
    public isLoading: boolean = false;
    public validationErrors: string[] = [];

    public readonly yearOptions: { label: string; value: number | null }[] = [
        { label: '6 months', value: 0},
        { label: '1 Year', value: 1 },
        { label: '2 Years', value: 2 },
        { label: '3 Years', value: 3 },
        { label: '4 Years', value: 4 },
        { label: '5 Years', value: 5 }
    ];

    constructor(
        private readonly _modalReference: ModalReference<BackflowTest>,
        private readonly _testService: BackflowTestService,
        private readonly _helper: HelperService
    ) {
        this.forceRenewal = this._modalReference.config.model?.forceRenewal ?? false;
        this.forceRenewalYears = this._modalReference.config.model?.forceRenewalYears ?? null;
    }

    public get test(): BackflowTest {
        return this._modalReference.config.model!;
    }

    public async save(form: NgForm): Promise<void> {
        if (!form.valid) {
            return;
        }

        try {
            this.isLoading = true;
            this.validationErrors = [];

            const updated = await this._testService.updateForceRenewal(this.test.id!, {
                forceRenewal: this.forceRenewal,
                forceRenewalYears: this.forceRenewalYears
            });

            this._modalReference.closeSuccess(updated);
        } catch (e) {
            if (!this._helper.parseValidationErrors(e, this.validationErrors)) {
                throw e;
            }
        } finally {
            this.isLoading = false;
        }
    }

    public cancel(): void {
        this._modalReference.cancel();
    }
}
