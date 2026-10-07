import { NgModule } from "@angular/core";
import { CommonModule } from "@angular/common";
import { FormsModule } from "@angular/forms";
import { SharedComponentsModule } from "../shared/components/shared.components.module";
import { BackflowTestDetailsSectionsModule } from "../backflow/tests/details/backflow-test-details-sections.module";
import { PublicSearchRoutingModule } from "./public-search-routing.module";
import { PublicSearchComponent } from "./search/public-search.component";
import { PublicBackflowTestViewComponent } from "./backflow-test-view/public-backflow-test-view.component";
import { PublicCsiInspectionViewComponent } from "./csi-inspection-view/public-csi-inspection-view.component";

@NgModule({
    declarations: [
        PublicSearchComponent,
        PublicBackflowTestViewComponent,
        PublicCsiInspectionViewComponent
    ],
    imports: [
        PublicSearchRoutingModule,
        CommonModule,
        FormsModule,
        SharedComponentsModule,
        BackflowTestDetailsSectionsModule
    ]
})
export class PublicSearchModule {

}
