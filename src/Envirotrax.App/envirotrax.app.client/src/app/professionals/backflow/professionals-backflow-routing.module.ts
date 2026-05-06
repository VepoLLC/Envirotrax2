import { NgModule } from "@angular/core";
import { RouterModule, Routes } from "@angular/router";
import { RoleGuard } from "../../shared/guards/role.guard";
import { ROLE_DEFINITIONS } from "../../shared/models/role-definitions";
import { GaugeListComponent } from "./gauges/gauge-list.component";
import { ProfessionalBackflowTestListComponent } from "./tests/professional-backflow-test-list.component";

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
        component: ProfessionalBackflowTestListComponent
    }
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class ProfessionalsBackflowRoutingModule {}
