import { NgModule } from "@angular/core";
import { RouterModule, Routes } from "@angular/router";
import { ProfessionalFogInspectionListComponent } from "./inspections/list/professional-fog-inspection-list.component";
import { ProfessionalFogSubmissionPropertySearchComponent } from "./inspections/create/professional-fog-submission-property-search.component";
import { ProfessionalFogSubmissionCreateComponent } from "./inspections/create/professional-fog-submission-create.component";

const routes: Routes = [
    {
        path: 'inspections',
        title: 'FOG Inspection Search',
        component: ProfessionalFogInspectionListComponent
    },
    {
        path: 'inspections/create',
        title: 'Submit FOG Inspection',
        component: ProfessionalFogSubmissionPropertySearchComponent
    },
    {
        path: 'inspections/create/:siteId',
        title: 'Submit FOG Inspection',
        component: ProfessionalFogSubmissionCreateComponent
    }
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class ProfessionalsFogRoutingModule {}
