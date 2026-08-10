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

@NgModule({
    declarations: [
        CheckoutComponent,
        CheckoutBackflowComponent,
        CheckoutCsiComponent,
        CheckoutFogInspectionComponent,
        CheckoutFogTransportComponent
    ],
    imports: [
        CommonModule,
        FormsModule,
        SharedComponentsModule,
        ProfessionalsCheckoutRoutingModule
    ]
})
export class ProfessionalsCheckoutModule {}
