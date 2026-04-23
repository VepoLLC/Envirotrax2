import { NgModule } from "@angular/core";
import { RouterModule, Routes } from "@angular/router";
import { RoleGuard } from "../../shared/guards/role.guard";
import { ROLE_DEFINITIONS } from "../../shared/models/role-definitions";
import { CsiSubmissionPropertySearchComponent } from "./inspections/create/csi-submission-property-search.component";
import { CsiSubmissionCreateComponent } from "./inspections/create/csi-submission-create.component";

const routes: Routes = [
    {
        path: 'inspections/create/:siteId',
        title: 'Submit CSI',
        component: CsiSubmissionCreateComponent,
        canActivate: [RoleGuard],
        data: { roles: [ROLE_DEFINITIONS.PROFESSIONAL] }
    },
    {
        path: 'inspections/create',
        title: 'Submit CSI',
        component: CsiSubmissionPropertySearchComponent,
        canActivate: [RoleGuard],
        data: { roles: [ROLE_DEFINITIONS.PROFESSIONAL] }
    }
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class ProfessionalsCsiRoutingModule {}
