import { NgModule } from "@angular/core";
import { RouterModule, Routes } from "@angular/router";
import { ProfessionalFogVehicleListComponent } from "./vehicles/list/professional-fog-vehicle-list.component";
import { ProfessionalFogDisposalSiteListComponent } from "./disposal-sites/list/professional-fog-disposal-site-list.component";
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
        path: 'disposal-sites',
        title: 'Disposal Site Management',
        component: ProfessionalFogDisposalSiteListComponent,
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
