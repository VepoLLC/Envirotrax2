import { NgModule } from "@angular/core";
import { CommonModule } from "@angular/common";
import { FormsModule } from "@angular/forms";
import { BaseChartDirective, provideCharts, withDefaultRegisterables } from "ng2-charts";
import { SharedComponentsModule } from "../../shared/components/shared.components.module";
import { BackflowReportsRoutingModule } from "./backflow-reports-routing.module";
import { BackflowReportComponent } from "./backflow-report.component";
import { BackflowTestReportsTabComponent } from "./tabs/test-reports/backflow-test-reports-tab.component";
import { BackflowCurrentComplianceTabComponent } from "./tabs/current-compliance/backflow-current-compliance-tab.component";
import { BackflowComplianceHistoryTabComponent } from "./tabs/compliance-history/backflow-compliance-history-tab.component";
import { BackflowNewRemovedTabComponent } from "./tabs/new-removed/backflow-new-removed-tab.component";

@NgModule({
    declarations: [
        BackflowReportComponent,
        BackflowTestReportsTabComponent,
        BackflowCurrentComplianceTabComponent,
        BackflowComplianceHistoryTabComponent,
        BackflowNewRemovedTabComponent
    ],
    imports: [
        CommonModule,
        FormsModule,
        SharedComponentsModule,
        BaseChartDirective,
        BackflowReportsRoutingModule
    ],
    // Chart.js is only used by these reports, so register it here (not in the eager AppModule) — it
    // then loads on demand with this lazy module instead of in the initial app bundle.
    providers: [
        provideCharts(withDefaultRegisterables())
    ]
})
export class BackflowReportsModule {}
