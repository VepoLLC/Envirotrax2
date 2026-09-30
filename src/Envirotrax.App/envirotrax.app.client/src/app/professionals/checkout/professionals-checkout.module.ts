import { NgModule } from "@angular/core";
import { CommonModule } from "@angular/common";
import { FormsModule } from "@angular/forms";
import { SharedComponentsModule } from "../../shared/components/shared.components.module";
import { ProfessionalsCheckoutRoutingModule } from "./professionals-checkout-routing.module";
import { CheckoutComponent } from "./checkout.component";
import { CheckoutBackflowComponent } from "./backflow/checkout-backflow.component";
import { CheckoutCsiComponent } from "./csi/checkout-csi.component";
import { CheckoutFogInspectionComponent } from "./fog-inspection/checkout-fog-inspection.component";
import { CheckoutFogTransportComponent } from "./fog-transport/checkout-fog-transport.component";
import { CheckoutAmountsComponent } from "./shared/checkout-amounts/checkout-amounts.component";
import { CheckoutPaymentComponent } from "./shared/checkout-payment/checkout-payment.component";
import { CheckoutReceiptComponent } from "./shared/checkout-receipt/checkout-receipt.component";

@NgModule({
    declarations: [
        CheckoutComponent,
        CheckoutBackflowComponent,
        CheckoutCsiComponent,
        CheckoutFogInspectionComponent,
        CheckoutFogTransportComponent,
        CheckoutAmountsComponent,
        CheckoutPaymentComponent,
        CheckoutReceiptComponent
    ],
    imports: [
        CommonModule,
        FormsModule,
        SharedComponentsModule,
        ProfessionalsCheckoutRoutingModule
    ]
})
export class ProfessionalsCheckoutModule {}
