import { NgModule } from "@angular/core";
import { CommonModule } from "@angular/common";
import { FormsModule } from "@angular/forms";
import { RouterModule } from "@angular/router";
import { SharedComponentsModule } from "../shared/components/shared.components.module";
import { BackflowRoutingModule } from "./backflow-routing.module";
import { BackflowTesterListComponent } from "./testers/list/backflow-tester-list.component";
import { BackflowTestListComponent } from "./tests/backflow-test-list.component";
import { BackflowTesterDetailsComponent } from "./testers/details/backflow-tester-details.component";
import { BackflowTesterWaterSuppliersComponent } from "./testers/details/water-suppliers/list/backflow-tester-water-suppliers.component";
import { BackflowTesterUsersComponent } from "./testers/details/users/list/backflow-tester-users.component";
import { BackflowTesterLicenseInsuranceComponent } from "./testers/details/license-insurance/list/backflow-tester-license-insurance.component";
import { ProfessionalModule } from "../professionals/professional.module";
import { BackflowTesterGaugeComponent } from "./testers/details/gauge/list/backflow-tester-gauge.component";

@NgModule({
    declarations: [
        BackflowTesterListComponent,
        BackflowTestListComponent,
        BackflowTesterDetailsComponent,
        BackflowTesterWaterSuppliersComponent,
        BackflowTesterUsersComponent,
        BackflowTesterLicenseInsuranceComponent,
        BackflowTesterGaugeComponent
    ],
    imports: [
        CommonModule,
        FormsModule,
        RouterModule,
        SharedComponentsModule,
        BackflowRoutingModule,
        ProfessionalModule
    ]
})
export class BackflowModule {}
