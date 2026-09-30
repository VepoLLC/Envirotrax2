import { Component, Input } from "@angular/core";
import { CheckoutAmounts } from "../../../../shared/utils/checkout-amounts.util";

@Component({
    selector: 'vp-checkout-amounts',
    standalone: false,
    templateUrl: './checkout-amounts.component.html'
})
export class CheckoutAmountsComponent {
    @Input() public amounts: CheckoutAmounts = { total: 0, fromBalance: 0, cardCharge: 0 };
    @Input() public accountBalance = 0;
}
