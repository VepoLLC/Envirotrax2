import { NgModule } from "@angular/core";
import { RouterModule, Routes } from "@angular/router";
import { CompanyComponent } from "./company/company.component";
import { ProfileGuard } from "./profile.guard";
import { UserComponent } from "./user/user.component";

const routes: Routes = [
    {
        path: '',
        title: '',
        canActivate: [ProfileGuard],
        children: [
            {
                path: '',
                component: CompanyComponent,
                title: 'Company Information'
            },
            {
                path: 'company',
                component: CompanyComponent,
                title: 'Company Information'
            },
            {
                path: 'user',
                component: UserComponent,
                title: 'User Information'
            }
        ]
    }
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class ProfileRoutingModule { }
