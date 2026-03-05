import { RouterModule, Routes } from "@angular/router";
import { UserListComponent } from "./list/user-list.component";
import { NgModule } from "@angular/core";
import { EditUserComponent } from "./edit/edit-user.component";
import { RoleListComponent } from "./roles/list/role-list.component";
import { EditRoleComponent } from "./roles/edit/edit-role.component";
import { PermissionGuard } from "../../shared/guards/permission.guard";
import { PermissionAction, PermissionType } from "../../shared/models/permission-type";

const routes: Routes = [
    {
        path: '',
        title: 'Users',
        component: UserListComponent,
        canActivate: [PermissionGuard],
        data: {
            permissions: [
                {
                    type: PermissionType.Users,
                    action: PermissionAction.CanView
                }
            ]
        }
    },
    {
        path: ':id/edit',
        title: 'Edit User',
        component: EditUserComponent,
        canActivate: [PermissionGuard],
        data: {
            permissions: [
                {
                    type: PermissionType.Users,
                    action: PermissionAction.CanEdit
                }
            ]
        }
    },
    {
        path: 'roles',
        title: 'Roles',
        component: RoleListComponent,
        canActivate: [PermissionGuard],
        data: {
            permissions: [
                {
                    type: PermissionType.Roles,
                    action: PermissionAction.CanView
                }
            ]
        }
    },
    {
        path: 'roles/:id/edit',
        title: 'Edit Role',
        component: EditRoleComponent,
        canActivate: [PermissionGuard],
        data: {
            permissions: [
                {
                    type: PermissionType.Roles,
                    action: PermissionAction.CanEdit
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
export class UserRoutingModule {

}