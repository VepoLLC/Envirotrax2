import { NgModule } from "@angular/core";
import { RouterModule, Routes } from "@angular/router";
import { PermissionGuard } from "../shared/guards/permission.guard";
import { PermissionAction, PermissionType } from "../shared/models/permission-type";
import { CsiInspectionListComponent } from "./inspections/list/csi-inspection-list.component";
import { CsiInspectorListComponent } from "./inspectors/list/csi-inspector-list.component";
import { CsiInspectorDetailsComponent } from "./inspectors/details/csi-inspector-details.component";
import { CsiInspectionDetailsComponent } from "./inspections/details/csi-inspection-details.component";

const routes: Routes = [

    {
        path: ':id/view',
        title: 'Inspection Details',
        component: CsiInspectionDetailsComponent,
        canActivate: [PermissionGuard],
        data: {
            permissions: [
                {
                    type: PermissionType.CsiInspections,
                    action: PermissionAction.CanView
                }
            ]
        }
    },
    {
        path: 'inspectors/details/:id',
        title: 'Inspector Details',
        component: CsiInspectorDetailsComponent,
        canActivate: [PermissionGuard],
        data: {
            permissions: [
                {
                    type: PermissionType.CsiInspectors,
                    action: PermissionAction.CanView
                }
            ]
        }
    },
    {
        path: 'inspectors',
        title: 'Inspector Management',
        component: CsiInspectorListComponent,
        canActivate: [PermissionGuard],
        data: {
            permissions: [
                {
                    type: PermissionType.CsiInspectors,
                    action: PermissionAction.CanView
                }
            ]
        }
    },
    {
        path: '',
        title: 'CSI Search',
        component: CsiInspectionListComponent,
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
export class CsiRoutingModule {}
