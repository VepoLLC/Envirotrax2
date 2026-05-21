import { Component } from "@angular/core";
import { NgForm } from "@angular/forms";
import { ModalReference } from "@developer-partners/ngx-modal-dialog";
import { CsiInspection } from "../../../../shared/models/csi/csi-inspection";
import { CsiInspectionService } from "../../../../shared/services/csi/csi-inspection.service";
import { HelperService } from "../../../../shared/services/helpers/helper.service";
import { AuthService } from "../../../../shared/services/auth/auth.service";

@Component({
    standalone: false,
    templateUrl: './disapprove-csi-inspection.component.html'
})
export class DisapproveCsiInspectionComponent {
    public disapprovedReason: string = '';
    public sendEmailNotification: boolean = true;
    public isLoading: boolean = false;
    public validationErrors: string[] = [];

    constructor(
        private readonly _modalReference: ModalReference<CsiInspection>,
        private readonly _inspectionService: CsiInspectionService,
        private readonly _authService: AuthService,
        private readonly _helper: HelperService
    ) {}

    public get inspection(): CsiInspection {
        return this._modalReference.config.model!;
    }

    public async save(form: NgForm): Promise<void> {
        if (!form.valid) {
            return;
        }

        try {
            this.isLoading = true;
            this.validationErrors = [];
            const updated = await this._inspectionService.updateApproval(this.inspection.id!, {
                disapproved: true,
                disapprovedReason: this.disapprovedReason || null
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

    private async prepareEmail(updated: CsiInspection): Promise<void> {
        const adminEmail = await this._authService.getUserEmail();
        const wsName = updated.waterSupplier?.name ?? '';
        const dateStr = new Date().toLocaleString('en-US', {
            month: '2-digit', day: '2-digit', year: 'numeric',
            hour: 'numeric', minute: '2-digit', hour12: true
        });

        const nl = '%0D%0A';
        const cityStateZip = [updated.propertyCity, updated.propertyState, updated.propertyZip]
            .filter(Boolean).join(' ');

        const body = `The following CSI certificate has been disapproved by ${wsName} on ${dateStr}.${nl}${nl}` +
            `Location:  ${nl}` +
            `   ${updated.propertyBusinessName ?? ''}${nl}` +
            `   ${updated.propertyStreetNumber ?? ''} ${updated.propertyStreetName ?? ''}${nl}` +
            `   ${cityStateZip}${nl}` +
            `${nl}` +
            `Reason for Disapproval:${nl}` +
            `${this.disapprovedReason}${nl}${nl}` +
            `Sincerely,${nl}${adminEmail} - Envirotrax`;

        const subject = encodeURIComponent(`${wsName} – Notice of CSI Certificate Disapproval`);
        const link = `mailto:${updated.mailingEmailAddress ?? ''}?subject=${subject}&body=${body}`;

        window.open(link);
    }

    public cancel(): void {
        this._modalReference.cancel();
    }
}
