import { NgModule } from "@angular/core";
import { RouterModule, Routes } from "@angular/router";
import { PermissionGuard } from "../shared/guards/permission.guard";
import { PermissionAction, PermissionType } from "../shared/models/permission-type";
import { FogInspectorListComponent } from "./inspectors/list/fog-inspector-list.component";
import { FogInspectorDetailsComponent } from "./inspectors/details/fog-inspector-details.component";
import { FogInspectionListComponent } from "./inspections/list/fog-inspection-list.component";

const routes: Routes = [
    {
        path: 'inspections',
        title: 'FOG Inspection Search',
        component: FogInspectionListComponent,
        canActivate: [PermissionGuard],
        data: {
            permissions: [
                {
                    type: PermissionType.FogInspections,
                    action: PermissionAction.CanView
                }
            ]
        }
    },
    {
        path: 'inspectors/details/:id',
        title: 'Inspector Details',
        component: FogInspectorDetailsComponent,
        canActivate: [PermissionGuard],
        data: {
            permissions: [
                {
                    type: PermissionType.FogInspectors,
                    action: PermissionAction.CanView
                }
            ]
        }
    },
    {
        path: 'inspectors',
        title: 'Inspector Management',
        component: FogInspectorListComponent,
        canActivate: [PermissionGuard],
        data: {
            permissions: [
                {
                    type: PermissionType.FogInspectors,
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
export class FogRoutingModule {}
