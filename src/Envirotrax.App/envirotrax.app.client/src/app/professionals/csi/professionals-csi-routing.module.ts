import { NgModule } from "@angular/core";
import { RouterModule, Routes } from "@angular/router";
import { RoleGuard } from "../../shared/guards/role.guard";
import { ROLE_DEFINITIONS } from "../../shared/models/role-definitions";
import { CsiSubmissionPropertySearchComponent } from "./inspections/create/csi-submission-property-search.component";
import { CsiSubmissionCreateComponent } from "./inspections/create/csi-submission-create.component";
import { CsiInspectionListComponent } from "./inspections/list/csi-inspection-list.component";
import { CsiInspectionViewComponent } from "./inspections/view/csi-inspection-view.component";

const routes: Routes = [
    {
        path: 'inspections',
        title: 'CSI Search',
        component: CsiInspectionListComponent,
        canActivate: [RoleGuard],
        data: { roles: [ROLE_DEFINITIONS.PROFESSIONAL] }
    },
    {
        path: 'inspections/create',
        title: 'Submit CSI',
        component: CsiSubmissionPropertySearchComponent,
        canActivate: [RoleGuard],
        data: { roles: [ROLE_DEFINITIONS.PROFESSIONAL] }
    },
    {
        path: 'inspections/create/:siteId',
        title: 'Submit CSI',
        component: CsiSubmissionCreateComponent,
        canActivate: [RoleGuard],
        data: { roles: [ROLE_DEFINITIONS.PROFESSIONAL] }
    },
    {
        path: 'inspections/:id',
        title: 'View CSI Inspection',
        component: CsiInspectionViewComponent,
        canActivate: [RoleGuard],
        data: { roles: [ROLE_DEFINITIONS.PROFESSIONAL] }
    }
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class ProfessionalsCsiRoutingModule {}
