import { CommonModule } from "@angular/common";
import { NgModule } from "@angular/core";
import { FormsModule } from "@angular/forms";
import { RouterModule } from "@angular/router";
import { SharedComponentsModule } from "../../shared/components/shared.components.module";
import { UserListComponent } from "./list/user-list.component";
import { UserRoutingModule } from "./user-routing.module";
import { CreateUserComponent } from "./create/create-user.component";

@NgModule({
    declarations: [
        UserListComponent,
        CreateUserComponent
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