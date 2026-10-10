import { NgModule } from "@angular/core";
import { CommonModule } from "@angular/common";
import { FormsModule } from "@angular/forms";
import { SharedComponentsModule } from "../../../shared/components/shared.components.module";
import { BackflowTestWaterSupplierComponent } from "./water-supplier/backflow-test-water-supplier.component";
import { BackflowTestRemarksComponent } from "./remarks/backflow-test-remarks.component";
import { BackflowTestImagesComponent } from "./images/backflow-test-images.component";
import { BackflowTestInfoComponent } from "./test-info/backflow-test-info.component";
import { BackflowTestAdditionalInfoComponent } from "./additional-info/backflow-test-additional-info.component";
import { ProfessionalBackflowTestBpatInfoComponent } from "../../../professionals/backflow/tests/details/bpat-info/professional-backflow-test-bpat-info.component";
import { ProfessionalBackflowTestPropertyInfoComponent } from "../../../professionals/backflow/tests/details/property-info/professional-backflow-test-property-info.component";
import { ProfessionalBackflowTestMailingInfoComponent } from "../../../professionals/backflow/tests/details/mailing-info/professional-backflow-test-mailing-info.component";
import { ProfessionalBackflowTestBackflowInfoComponent } from "../../../professionals/backflow/tests/details/backflow-info/professional-backflow-test-backflow-info.component";

@NgModule({
    declarations: [
        BackflowTestWaterSupplierComponent,
        BackflowTestRemarksComponent,
        BackflowTestImagesComponent,
        BackflowTestInfoComponent,
        BackflowTestAdditionalInfoComponent,
        ProfessionalBackflowTestBpatInfoComponent,
        ProfessionalBackflowTestPropertyInfoComponent,
        ProfessionalBackflowTestMailingInfoComponent,
        ProfessionalBackflowTestBackflowInfoComponent
    ],
    imports: [CommonModule, FormsModule, SharedComponentsModule],
    exports: [
        BackflowTestWaterSupplierComponent,
        BackflowTestRemarksComponent,
        BackflowTestImagesComponent,
        BackflowTestInfoComponent,
        BackflowTestAdditionalInfoComponent,
        ProfessionalBackflowTestBpatInfoComponent,
        ProfessionalBackflowTestPropertyInfoComponent,
        ProfessionalBackflowTestMailingInfoComponent,
        ProfessionalBackflowTestBackflowInfoComponent
    ]
})
export class BackflowTestDetailsSectionsModule {}
