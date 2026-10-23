import { Component, Input } from "@angular/core";
import { Router } from "@angular/router";
import { ProfessionalCheckoutReceipt } from "../../../../shared/models/payments/professional-checkout";
import { ProfessionalTransactionService } from "../../../../shared/services/professionals/professional-transaction.service";
import { DownloadService } from "../../../../shared/services/download.service";

@Component({
    selector: 'vp-checkout-receipt',
    standalone: false,
    templateUrl: './checkout-receipt.component.html'
})
export class CheckoutReceiptComponent<TItem> {
    @Input({ required: true }) public receipt!: ProfessionalCheckoutReceipt<TItem>;

    constructor(
        private readonly _router: Router,
        private readonly _transactionService: ProfessionalTransactionService,
        private readonly _downloadService: DownloadService
    ) {

    }

    public async printReceipt(): Promise<void> {
        await this._downloadService.openBlobInNewWindow(() => this._transactionService.getReceiptPdf(this.receipt.transactionId));
    }

    public returnToAccountOverview(): void {
        this._router.navigate(['/']);
    }
}
