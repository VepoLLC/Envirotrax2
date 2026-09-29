import { Component, Input } from "@angular/core";
import { Router } from "@angular/router";
import { ProfessionalCheckoutReceipt } from "../../../../shared/models/payments/professional-checkout";

@Component({
    selector: 'vp-checkout-receipt',
    standalone: false,
    templateUrl: './checkout-receipt.component.html'
})
export class CheckoutReceiptComponent<TItem> {
    @Input({ required: true }) public receipt!: ProfessionalCheckoutReceipt<TItem>;

    constructor(private readonly _router: Router) {

    }

    public printReceipt(): void {
        window.print();
    }

    public returnToAccountOverview(): void {
        this._router.navigate(['/']);
    }
}
