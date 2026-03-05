import { CommonModule } from "@angular/common";
import { NgModule } from "@angular/core";
import { FormsModule } from "@angular/forms";
import { RouterModule } from "@angular/router";
import { SharedComponentsModule } from "../../shared/components/shared.components.module";
import { UserListComponent } from "./list/user-list.component";
import { UserRoutingModule } from "./user-routing.module";
import { CreateUserComponent } from "./create/create-user.component";
import { EditUserComponent } from "./edit/edit-user.component";
import { RoleListComponent } from "./roles/list/role-list.component";
import { CreateRoleComponent } from "./roles/create/create-role.component";
import { EditRoleComponent } from "./roles/edit/edit-role.component";
import { RolePermissionListComponent } from "./roles/edit/permissions/role-permission-list.component";

@NgModule({
    declarations: [
        UserListComponent,
        CreateUserComponent,
        EditUserComponent,
        RoleListComponent,
        CreateRoleComponent,
        EditRoleComponent,
        RolePermissionListComponent
    ],
    imports: [
        CommonModule,
        FormsModule,
        RouterModule,
        SharedComponentsModule,
        UserRoutingModule
    ]
})
export class UserModule {

}