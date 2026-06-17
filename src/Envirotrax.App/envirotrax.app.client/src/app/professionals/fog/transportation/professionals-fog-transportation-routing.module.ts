import { NgModule } from "@angular/core";
import { RouterModule, Routes } from "@angular/router";
import { ProfessionalFogVehicleListComponent } from "./vehicles/list/professional-fog-vehicle-list.component";
import { RoleGuard } from "../../../shared/guards/role.guard";
import { ROLE_DEFINITIONS } from "../../../shared/models/role-definitions";

const routes: Routes = [
    {
        path: 'vehicles',
        title: 'Vehicle Management',
        component: ProfessionalFogVehicleListComponent,
        canActivate: [RoleGuard],
        data: {
            roles: [ROLE_DEFINITIONS.PROFESSIONAL]
        }
    },
    {
        path: '',
        redirectTo: 'vehicles',
        pathMatch: 'full'
    }
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class ProfessionalsFogTransportationRoutingModule {}
