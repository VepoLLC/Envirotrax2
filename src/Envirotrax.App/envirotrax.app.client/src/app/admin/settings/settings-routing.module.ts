import { NgModule } from "@angular/core";
import { RouterModule, Routes } from "@angular/router";
import { GeneralSettingsComponent } from "./general/general-settings.component";

const routes: Routes = [
    {
        path: '',
        redirectTo: 'general',
        pathMatch: 'full'
    },
    {
        path: 'general',
        title: 'General Settings & Fees',
        component: GeneralSettingsComponent
    }
];

@NgModule({
    imports: [
        RouterModule.forChild(routes)
    ],
    exports: [
        RouterModule
    ]
})
export class SettingsRoutingModule {

}
