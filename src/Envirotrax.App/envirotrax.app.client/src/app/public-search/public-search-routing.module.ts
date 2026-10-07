import { NgModule } from "@angular/core";
import { RouterModule, Routes } from "@angular/router";
import { PublicSearchComponent } from "./search/public-search.component";
import { PublicBackflowTestViewComponent } from "./backflow-test-view/public-backflow-test-view.component";
import { PublicCsiInspectionViewComponent } from "./csi-inspection-view/public-csi-inspection-view.component";

const routes: Routes = [
    {
        path: '',
        title: 'Public Search',
        component: PublicSearchComponent
    },
    {
        path: 'backflow-tests/:id/view',
        title: 'Completed Test',
        component: PublicBackflowTestViewComponent
    },
    {
        path: 'csi-inspections/:id/view',
        title: 'CSI Inspection Submission',
        component: PublicCsiInspectionViewComponent
    }
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class PublicSearchRoutingModule {

}
