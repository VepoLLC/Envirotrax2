import { RouterModule, Routes } from "@angular/router";
import { SiteListComponent } from "./list/site-list.component";
import { NgModule } from "@angular/core";

const routes: Routes = [
    {
        path: '',
        title: 'Property Record Search',
        component: SiteListComponent
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
export class SiteRoutingModule {

}
