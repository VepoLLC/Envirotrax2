import { NgModule } from "@angular/core";
import { RouterModule, Routes } from "@angular/router";
import { CsiSearchComponent } from "./search/csi-search.component";
import { PermissionGuard } from "../shared/guards/permission.guard";
import { PermissionAction, PermissionType } from "../shared/models/permission-type";
import { InspectorManagementComponent } from "./inspector-management/inspector-management.component";

const routes: Routes = [
    {
        path: 'inspectors',
        title: 'Inspector Management',
        component: InspectorManagementComponent
    },
    {
        path: '',
        title: 'CSI Search',
        component: CsiSearchComponent,
        canActivate: [PermissionGuard],
        data: {
            permissions: [
                {
                    type: PermissionType.CsiInspections,
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
export class CsiRoutingModule {

}
