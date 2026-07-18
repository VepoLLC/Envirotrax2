import { Component } from "@angular/core";
import { NgForm } from "@angular/forms";
import { ModalReference } from "@developer-partners/ngx-modal-dialog";
import { BackflowTest } from "../../../../shared/models/backflow/backflow-test";
import { BackflowTestService } from "../../../../shared/services/backflow/backflow-test.service";
import { HelperService } from "../../../../shared/services/helpers/helper.service";
@Component({
    standalone: false,
    templateUrl: './backflow-test-reject.component.html'
})
export class BackflowTestRejectComponent {
    public rejectedReason: string = '';
    public sendEmailNotification: boolean = true;
    public isLoading: boolean = false;
    public validationErrors: string[] = [];

    constructor(
        private readonly _modalReference: ModalReference<BackflowTest>,
        private readonly _testService: BackflowTestService,
        private readonly _helper: HelperService
    ) {}

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

            const updated = await this._testService.updateRejection(this.test.id!, {
                rejected: true,
                rejectedReason: this.rejectedReason || null
            });

            if (this.sendEmailNotification) {
                await this.prepareEmail(updated);
            }

            this._modalReference.closeSuccess(updated);
        } catch (e) {
            if (!this._helper.parseValidationErrors(e, this.validationErrors)) {
                throw e;
            }
        } finally {
            this.isLoading = false;
        }
    }

    private async prepareEmail(updated: BackflowTest): Promise<void> {
        const wsName = updated.waterSupplier?.name ?? '';
        const dateStr = new Date().toLocaleString('en-US', {
            month: '2-digit', day: '2-digit', year: 'numeric',
            hour: 'numeric', minute: '2-digit', hour12: true
        });

        const nl = '%0D%0A';

        let body = `The following backflow test report has been rejected by ${wsName} on ${dateStr}:${nl}${nl}`;

        body += `Location:  ${nl}`;
        if (updated.propertyType === 1 && updated.propertyBusinessName) {
            body += `   ${updated.propertyBusinessName}${nl}`;
        }
        const address = [updated.propertyStreetNumber, updated.propertyStreetName].filter(Boolean).join(' ');
        body += `   ${address}${nl}`;
        const cityStateZip = [updated.propertyCity, updated.propertyState?.code, updated.propertyZip].filter(Boolean).join(' ');
        body += `   ${cityStateZip}${nl}`;
        body += nl;

        const testDateStr = updated.testDate ? new Date(updated.testDate).toLocaleDateString('en-US') : '';
        body += `Test Date:  ${testDateStr}${nl}`;
        body += `Device Type:  ${updated.deviceType ?? ''}${nl}`;

        const hasBypassAssembly = ['DCD', 'DCD2', 'RPPD', 'RPPD2'].includes(updated.deviceType ?? '');
        if (hasBypassAssembly) {
            body += `Main Assembly:${nl}`;
            body += `   Manufacturer:  ${updated.manufacturer ?? ''}${nl}`;
            body += `   Model:  ${updated.model ?? ''}${nl}`;
            body += `   Size:  ${updated.size ?? ''}${nl}`;
            body += `   Serial Number:  ${updated.serialNumber ?? ''}${nl}`;
            body += `Bypass Assembly:${nl}`;
            body += `   Manufacturer:  ${updated.manufacturer2 ?? ''}${nl}`;
            body += `   Model:  ${updated.model2 ?? ''}${nl}`;
            body += `   Size:  ${updated.size2 ?? ''}${nl}`;
            body += `   Serial Number:  ${updated.serialNumber2 ?? ''}${nl}`;
        } else {
            body += `Manufacturer:  ${updated.manufacturer ?? ''}${nl}`;
            body += `Model:  ${updated.model ?? ''}${nl}`;
            body += `Size:  ${updated.size ?? ''}${nl}`;
            body += `Serial Number:  ${updated.serialNumber ?? ''}${nl}`;
        }
        body += nl;

        body += `Reason for Rejection:${nl}`;
        body += this.rejectedReason;

        const subject = encodeURIComponent(`${wsName} - Notice of Backflow Test Rejection`);
        const toEmail = updated.bpat?.emailAddress ?? '';
        const link = `mailto:${toEmail}?subject=${subject}&body=${body}`;

        window.open(link);
    }

    public cancel(): void {
        this._modalReference.cancel();
    }
}
