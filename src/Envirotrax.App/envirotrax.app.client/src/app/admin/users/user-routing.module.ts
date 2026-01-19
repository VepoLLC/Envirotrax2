import { RouterModule, Routes } from "@angular/router";
import { UserListComponent } from "./list/user-list.component";
import { NgModule } from "@angular/core";
import { EditUserComponent } from "./edit/edit-user.component";

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