import { NgModule } from "@angular/core";
import { RouterModule, Routes } from "@angular/router";
import { RoleGuard } from "../../shared/guards/role.guard";
import { ROLE_DEFINITIONS } from "../../shared/models/role-definitions";
import { GaugeListComponent } from "./gauges/gauge-list.component";
import { ProfessionalBackflowTestListComponent } from "./tests/professional-backflow-test-list.component";
import { BackflowTestAssemblySearchComponent } from "./submit/backflow-test-assembly-search.component";
import { BackflowTestSubmitComponent } from "./submit/backflow-test-submit.component";

const routes: Routes = [
    {
        path: 'gauges',
        title: 'Gauge Management',
        component: GaugeListComponent,
        canActivate: [RoleGuard],
        data: { roles: [ROLE_DEFINITIONS.PROFESSIONAL] }
    },
    {
        path: 'tests',
        title: 'Backflow Test Search',
        component: ProfessionalBackflowTestListComponent,
        canActivate: [RoleGuard],
        data: { roles: [ROLE_DEFINITIONS.PROFESSIONAL] }
    },
    {
        path: 'submit',
        title: 'Submit Backflow Test',
        component: BackflowTestAssemblySearchComponent,
        canActivate: [RoleGuard],
        data: { roles: [ROLE_DEFINITIONS.PROFESSIONAL] }
    },
    {
        path: 'submit/:testId',
        title: 'Submit Backflow Test',
        component: BackflowTestSubmitComponent,
        canActivate: [RoleGuard],
        data: { roles: [ROLE_DEFINITIONS.PROFESSIONAL] }
    }
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class ProfessionalsBackflowRoutingModule {}
