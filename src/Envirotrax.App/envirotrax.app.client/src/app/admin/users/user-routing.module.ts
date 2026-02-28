import { RouterModule, Routes } from "@angular/router";
import { UserListComponent } from "./list/user-list.component";
import { NgModule } from "@angular/core";
import { EditUserComponent } from "./edit/edit-user.component";
import { RoleListComponent } from "./roles/list/role-list.component";
import { EditRoleComponent } from "./roles/edit/edit-role.component";

const routes: Routes = [
    {
        path: '',
        title: 'Users',
        component: UserListComponent
    },
    {
        path: ':id/edit',
        title: 'Edit User',
        component: EditUserComponent
    },
    {
        path: 'roles',
        title: 'Roles',
        component: RoleListComponent
    },
    {
        path: 'roles/:id/edit',
        title: 'Edit Role',
        component: EditRoleComponent
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