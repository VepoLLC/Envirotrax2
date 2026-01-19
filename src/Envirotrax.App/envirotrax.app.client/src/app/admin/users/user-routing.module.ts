import { RouterModule, Routes } from "@angular/router";
import { UserListComponent } from "./list/user-list.component";
import { NgModule } from "@angular/core";

const routes: Routes = [
    {
        path: '',
        title: 'Users',
        component: UserListComponent
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