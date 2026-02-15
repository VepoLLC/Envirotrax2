import { RouterModule, Routes } from "@angular/router";
import { SiteListComponent } from "./list/site-list.component";
import { NgModule } from "@angular/core";
import { EditSiteComponent } from './edit/edit-site-component';

const routes: Routes = [
    {
        path: '',
        title: 'Property Record Search',
        component: SiteListComponent
    },
    {
        path: ':id/edit',
        title: 'Edit site',
        component: EditSiteComponent
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
