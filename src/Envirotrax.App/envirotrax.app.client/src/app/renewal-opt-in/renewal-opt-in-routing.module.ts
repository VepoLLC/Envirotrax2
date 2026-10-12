import { NgModule } from "@angular/core";
import { RouterModule, Routes } from "@angular/router";
import { RenewalOptInComponent } from "./opt-in/renewal-opt-in.component";
import { RenewalVerifyEmailComponent } from "./email-link/renewal-verify-email.component";
import { RenewalUnsubscribeComponent } from "./email-link/renewal-unsubscribe.component";

const routes: Routes = [
    {
        path: 'verify-email',
        title: 'Email Verification',
        component: RenewalVerifyEmailComponent
    },
    {
        path: 'unsubscribe',
        title: 'Unsubscribe',
        component: RenewalUnsubscribeComponent
    },
    {
        path: ':siteId',
        title: 'Renewal Opt-In / Opt-Out',
        component: RenewalOptInComponent
    }
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class RenewalOptInRoutingModule {

}
