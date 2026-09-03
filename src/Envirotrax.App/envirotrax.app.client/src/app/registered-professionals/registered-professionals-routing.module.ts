import { NgModule } from "@angular/core";
import { RouterModule, Routes } from "@angular/router";
import { RegisteredProfessionalSearchComponent } from "./search/registered-professional-search.component";

// Public routes — deliberately outside the AuthGuard branch of app-routing-module.ts. The marketing
// site links straight to /registered-professionals/{accountType}, so these must render signed out.
const routes: Routes = [
    {
        path: '',
        title: 'Registered Accounts Search',
        component: RegisteredProfessionalSearchComponent
    },
    {
        path: ':accountType',
        component: RegisteredProfessionalSearchComponent
    }
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class RegisteredProfessionalsRoutingModule {

}
