import { Component, Input } from "@angular/core";
import { Router } from "@angular/router";
import { ProfessionalCheckoutReceipt } from "../../../../shared/models/payments/professional-checkout";
import { ProfessionalTransactionService } from "../../../../shared/services/professionals/professional-transaction.service";

@Component({
    selector: 'vp-checkout-receipt',
    standalone: false,
    templateUrl: './checkout-receipt.component.html'
})
export class CheckoutReceiptComponent<TItem> {
    @Input({ required: true }) public receipt!: ProfessionalCheckoutReceipt<TItem>;

    public isLoading = false;

    constructor(
        private readonly _router: Router,
        private readonly _transactionService: ProfessionalTransactionService
    ) {

    }

    public async printReceipt(): Promise<void> {
        try {
            this.isLoading = true;
            await this._transactionService.openReceiptPdf(this.receipt.transactionId);
        } finally {
            this.isLoading = false;
        }
    }

    public returnToAccountOverview(): void {
        this._router.navigate(['/']);
    }
}
