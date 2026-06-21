import { NgModule } from "@angular/core";
import { RouterModule, Routes } from "@angular/router";
import { ProfessionalFogInspectionListComponent } from "./inspections/list/professional-fog-inspection-list.component";
import { FogInspectionViewComponent } from "./inspections/view/fog-inspection-view.component";

const routes: Routes = [
    {
        path: 'inspections',
        title: 'FOG Inspection Search',
        component: ProfessionalFogInspectionListComponent
    },
    {
        path: 'inspections/:id',
        title: 'View FOG Inspection',
        component: FogInspectionViewComponent
    }
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class ProfessionalsFogRoutingModule {}
