import { NgModule } from "@angular/core";
import { CommonModule } from "@angular/common";
import { FormsModule } from "@angular/forms";
import { RouterModule } from "@angular/router";
import { SharedComponentsModule } from "../shared/components/shared.components.module";
import { BackflowRoutingModule } from "./backflow-routing.module";
import { BackflowTesterListComponent } from "./testers/list/backflow-tester-list.component";
import { BackflowTestListComponent } from "./tests/backflow-test-list.component";
import { BackflowTestDetailsComponent } from "./tests/details/backflow-test-details.component";
import { BackflowTesterDetailsComponent } from "./testers/details/backflow-tester-details.component";
import { BackflowOutOfServiceListComponent } from "./out-of-service/backflow-out-of-service-list.component";
import { BackflowTesterWaterSuppliersComponent } from "./testers/details/water-suppliers/list/backflow-tester-water-suppliers.component";
import { EditBackflowTesterWaterSupplierComponent } from "./testers/details/water-suppliers/edit/edit-backflow-tester-water-supplier.component";
import { BackflowTesterUsersComponent } from "./testers/details/users/list/backflow-tester-users.component";
import { AddEditBackflowTesterUserComponent } from "./testers/details/users/edit/add-edit-backflow-tester-user.component";
import { BackflowTesterLicenseInsuranceComponent } from "./testers/details/license-insurance/list/backflow-tester-license-insurance.component";
import { AddEditBackflowTesterLicenseComponent } from "./testers/details/license-insurance/edit/add-edit-backflow-tester-license.component";
import { AddEditBackflowTesterInsuranceComponent } from "./testers/details/license-insurance/edit/add-edit-backflow-tester-insurance.component";
import { ProfessionalModule } from "../professionals/professional.module";
import { BackflowTesterGaugeComponent } from "./testers/details/gauge/list/backflow-tester-gauge.component";
import { AddEditBackflowTesterGaugeComponent } from "./testers/details/gauge/edit/add-edit-backflow-tester-gauge.component";
import { BackflowTestBpatInfoComponent } from "./tests/details/bpat-info/backflow-test-bpat-info.component";
import { BackflowTestPropertyInfoComponent } from "./tests/details/property-info/backflow-test-property-info.component";
import { BackflowTestMailingInfoComponent } from "./tests/details/mailing-info/backflow-test-mailing-info.component";
import { BackflowTestBackflowInfoComponent } from "./tests/details/backflow-info/backflow-test-backflow-info.component";
import { BackflowTestHistoryComponent } from "./tests/details/history/backflow-test-history.component";
import { BackflowTestDetailsSectionsModule } from "./tests/details/backflow-test-details-sections.module";
import { BackflowReportComponent } from "./reports/backflow-report.component";
import { BackflowTestReportsTabComponent } from "./reports/tabs/test-reports/backflow-test-reports-tab.component";
import { BackflowCurrentComplianceTabComponent } from "./reports/tabs/current-compliance/backflow-current-compliance-tab.component";
import { BackflowComplianceHistoryTabComponent } from "./reports/tabs/compliance-history/backflow-compliance-history-tab.component";
import { BackflowNewRemovedTabComponent } from "./reports/tabs/new-removed/backflow-new-removed-tab.component";
import { BaseChartDirective } from "ng2-charts";

@NgModule({
    declarations: [
        BackflowReportComponent,
        BackflowTestReportsTabComponent,
        BackflowCurrentComplianceTabComponent,
        BackflowComplianceHistoryTabComponent,
        BackflowNewRemovedTabComponent,
        BackflowTesterListComponent,
        BackflowTestListComponent,
        BackflowOutOfServiceListComponent,
        BackflowTestDetailsComponent,
        BackflowTestBpatInfoComponent,
        BackflowTestPropertyInfoComponent,
        BackflowTestMailingInfoComponent,
        BackflowTestBackflowInfoComponent,
        BackflowTestHistoryComponent,
        BackflowTesterDetailsComponent,
        BackflowTesterWaterSuppliersComponent,
        EditBackflowTesterWaterSupplierComponent,
        BackflowTesterUsersComponent,
        AddEditBackflowTesterUserComponent,
        BackflowTesterLicenseInsuranceComponent,
        AddEditBackflowTesterLicenseComponent,
        AddEditBackflowTesterInsuranceComponent,
        BackflowTesterGaugeComponent,
        AddEditBackflowTesterGaugeComponent
    ],
    imports: [
        CommonModule,
        FormsModule,
        RouterModule,
        SharedComponentsModule,
        BackflowRoutingModule,
        ProfessionalModule,
        BackflowTestDetailsSectionsModule,
        BaseChartDirective
    ]
})
export class BackflowModule {}
