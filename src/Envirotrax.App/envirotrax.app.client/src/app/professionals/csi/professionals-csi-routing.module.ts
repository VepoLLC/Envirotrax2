import { NgModule } from "@angular/core";
import { RouterModule, Routes } from "@angular/router";
import { CsiSubmissionPropertySearchComponent } from "./inspections/create/csi-submission-property-search.component";
import { CsiSubmissionCreateComponent } from "./inspections/create/csi-submission-create.component";
import { CsiInspectionListComponent } from "./inspections/list/csi-inspection-list.component";
import { CsiInspectionViewComponent } from "./inspections/view/csi-inspection-view.component";

const routes: Routes = [
    {
        path: 'inspections',
        title: 'CSI Search',
        component: CsiInspectionListComponent
    },
    {
        path: 'inspections/create',
        title: 'Submit CSI',
        component: CsiSubmissionPropertySearchComponent
    },
    {
        path: 'inspections/create/:siteId',
        title: 'Submit CSI',
        component: CsiSubmissionCreateComponent
    },
    {
        path: 'inspections/:id',
        title: 'View CSI Inspection',
        component: CsiInspectionViewComponent
    }
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class ProfessionalsCsiRoutingModule {}
