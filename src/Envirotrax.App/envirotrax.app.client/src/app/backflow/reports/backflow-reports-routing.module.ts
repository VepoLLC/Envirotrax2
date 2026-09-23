import { NgModule } from "@angular/core";
import { RouterModule, Routes } from "@angular/router";
import { BackflowReportComponent } from "./backflow-report.component";
import { BackflowTestReportsTabComponent } from "./tabs/test-reports/backflow-test-reports-tab.component";
import { BackflowCurrentComplianceTabComponent } from "./tabs/current-compliance/backflow-current-compliance-tab.component";
import { BackflowComplianceHistoryTabComponent } from "./tabs/compliance-history/backflow-compliance-history-tab.component";
import { BackflowNewRemovedTabComponent } from "./tabs/new-removed/backflow-new-removed-tab.component";

const routes: Routes = [
    {
        // Shell hosting the report pill navigation + <router-outlet>. Each report is its own routed
        // child page below. The permission guard lives on the parent (lazy) route in backflow-routing.
        path: '',
        component: BackflowReportComponent,
        children: [
            { path: '', pathMatch: 'full', redirectTo: 'test-reports' },
            { path: 'test-reports', title: 'Backflow Test Reports', component: BackflowTestReportsTabComponent },
            { path: 'current-compliance', title: 'Current Compliance Report', component: BackflowCurrentComplianceTabComponent },
            { path: 'compliance-history', title: 'Compliance History Report', component: BackflowComplianceHistoryTabComponent },
            { path: 'new-removed', title: 'New/Removed Assemblies Report', component: BackflowNewRemovedTabComponent }
        ]
    }
];

@NgModule({
    imports: [
        RouterModule.forChild(routes)
    ],
    exports: [
        RouterModule
    ]
})
export class BackflowReportsRoutingModule {}
