import { RouterModule, Routes } from "@angular/router";
import { SiteListComponent } from "./list/site-list.component";
import { NgModule } from "@angular/core";
import { PermissionGuard } from "../shared/guards/permission.guard";
import { PermissionAction, PermissionType } from "../shared/models/permission-type";

const routes: Routes = [
    {
        path: '',
        title: 'Property Record Search',
        component: SiteListComponent,
        canActivate: [PermissionGuard],
        data: {
            permissions: [
                {
                    type: PermissionType.Sites,
                    action: PermissionAction.CanView
                }
            ]
        }
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
