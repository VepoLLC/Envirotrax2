import { NgModule } from "@angular/core";
import { RouterModule, Routes } from "@angular/router";
import { CompanyComponent } from "./company/company.component";
import { ProfileGuard } from "./profile.guard";

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
            }
        ]
    }
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class ProfileRoutingModule { }
