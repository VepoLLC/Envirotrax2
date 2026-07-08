import { NgModule } from "@angular/core";
import { CommonModule } from "@angular/common";
import { FormsModule } from "@angular/forms";
import { SharedComponentsModule } from "../../../shared/components/shared.components.module";
import { BackflowTestWaterSupplierComponent } from "./water-supplier/backflow-test-water-supplier.component";
import { BackflowTestRemarksComponent } from "./remarks/backflow-test-remarks.component";
import { BackflowTestImagesComponent } from "./images/backflow-test-images.component";
import { BackflowTestInfoComponent } from "./test-info/backflow-test-info.component";
import { BackflowTestAdditionalInfoComponent } from "./additional-info/backflow-test-additional-info.component";

@NgModule({
    declarations: [
        BackflowTestWaterSupplierComponent,
        BackflowTestRemarksComponent,
        BackflowTestImagesComponent,
        BackflowTestInfoComponent,
        BackflowTestAdditionalInfoComponent
    ],
    imports: [CommonModule, FormsModule, SharedComponentsModule],
    exports: [
        BackflowTestWaterSupplierComponent,
        BackflowTestRemarksComponent,
        BackflowTestImagesComponent,
        BackflowTestInfoComponent,
        BackflowTestAdditionalInfoComponent
    ]
})
export class BackflowTestDetailsSectionsModule {}
