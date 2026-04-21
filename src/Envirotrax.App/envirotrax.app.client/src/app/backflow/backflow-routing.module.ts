import { NgModule } from "@angular/core";
import { RouterModule, Routes } from "@angular/router";
import { PermissionGuard } from "../shared/guards/permission.guard";
import { PermissionAction, PermissionType } from "../shared/models/permission-type";
import { BackflowTesterListComponent } from "./testers/list/backflow-tester-list.component";
import { BackflowTestListComponent } from "./tests/backflow-test-list.component";

const routes: Routes = [
    {
        path: 'tests',
        title: 'Backflow Test Search',
        component: BackflowTestListComponent,
        canActivate: [PermissionGuard],
        data: {
            permissions: [
                {
                    type: PermissionType.BackflowTests,
                    action: PermissionAction.CanView
                }
            ]
        }
    },
    {
        path: 'testers',
        title: 'BPAT Management',
        component: BackflowTesterListComponent,
        canActivate: [PermissionGuard],
        data: {
            permissions: [
                {
                    type: PermissionType.BackflowTesters,
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
export class BackflowRoutingModule {}
