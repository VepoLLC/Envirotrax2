import { NgModule } from "@angular/core";
import { RouterModule, Routes } from "@angular/router";
import { CsiSettingsComponent } from "./general/csi-settings.component";

const routes: Routes = [
    {
        path: '',
        redirectTo: 'general',
        pathMatch: 'full'
    },
    {
        path: 'general',
        title: 'CSI Settings',
        component: CsiSettingsComponent
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
export class CsiSettingsRoutingModule {

}
