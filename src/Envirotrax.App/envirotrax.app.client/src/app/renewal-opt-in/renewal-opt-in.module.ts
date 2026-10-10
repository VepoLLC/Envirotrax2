import { NgModule } from "@angular/core";
import { CommonModule } from "@angular/common";
import { FormsModule } from "@angular/forms";
import { SharedComponentsModule } from "../shared/components/shared.components.module";
import { RenewalOptInRoutingModule } from "./renewal-opt-in-routing.module";
import { RenewalOptInComponent } from "./opt-in/renewal-opt-in.component";
import { RenewalVerifyEmailComponent } from "./email-link/renewal-verify-email.component";
import { RenewalUnsubscribeComponent } from "./email-link/renewal-unsubscribe.component";

@NgModule({
    declarations: [
        RenewalOptInComponent,
        RenewalVerifyEmailComponent,
        RenewalUnsubscribeComponent
    ],
    imports: [
        RenewalOptInRoutingModule,
        CommonModule,
        FormsModule,
        SharedComponentsModule
    ]
})
export class RenewalOptInModule {

}
