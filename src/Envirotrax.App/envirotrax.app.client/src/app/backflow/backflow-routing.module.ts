import { NgModule } from "@angular/core";
import { RouterModule, Routes } from "@angular/router";
import { PermissionGuard } from "../shared/guards/permission.guard";
import { PermissionAction, PermissionType } from "../shared/models/permission-type";
import { BackflowTesterListComponent } from "./testers/list/backflow-tester-list.component";
import { BackflowTestListComponent } from "./tests/backflow-test-list.component";
import { BackflowTestDetailsComponent } from "./tests/details/backflow-test-details.component";
import { BackflowTesterDetailsComponent } from "./testers/details/backflow-tester-details.component";
import { BackflowReportComponent } from "./reports/backflow-report.component";
import { BackflowTestReportsTabComponent } from "./reports/tabs/test-reports/backflow-test-reports-tab.component";
import { BackflowCurrentComplianceTabComponent } from "./reports/tabs/current-compliance/backflow-current-compliance-tab.component";
import { BackflowComplianceHistoryTabComponent } from "./reports/tabs/compliance-history/backflow-compliance-history-tab.component";
import { BackflowNewRemovedTabComponent } from "./reports/tabs/new-removed/backflow-new-removed-tab.component";
import { BackflowOutOfServiceListComponent } from "./out-of-service/backflow-out-of-service-list.component";

const routes: Routes = [
    {
        // Shell hosting the report pill navigation + <router-outlet>. Each report is its own routed
        // child page below; the permission guard on the parent covers every child.
        path: 'reports',
        component: BackflowReportComponent,
        canActivate: [PermissionGuard],
        data: {
            permissions: [
                {
                    type: PermissionType.BackflowReports,
                    action: PermissionAction.CanView
                }
            ]
        },
        children: [
            { path: '', pathMatch: 'full', redirectTo: 'test-reports' },
            { path: 'test-reports', title: 'Backflow Test Reports', component: BackflowTestReportsTabComponent },
            { path: 'current-compliance', title: 'Current Compliance Report', component: BackflowCurrentComplianceTabComponent },
            { path: 'compliance-history', title: 'Compliance History Report', component: BackflowComplianceHistoryTabComponent },
            { path: 'new-removed', title: 'New/Removed Assemblies Report', component: BackflowNewRemovedTabComponent }
        ]
    },
    {
        path: 'tests',
        title: 'Backflow Test Search',
        component: BackflowTestListComponent,
        canActivate: [PermissionGuard],
        data: {
            permissions: [
                {
                    type: PermissionType.BackflowTests,
                    action: PermissionAction.CanView
                }
            ]
        }
    },
    {
        path: 'tests/:id/view',
        title: 'Backflow Test Details',
        component: BackflowTestDetailsComponent,
        canActivate: [PermissionGuard],
        data: {
            permissions: [
                {
                    type: PermissionType.BackflowTests,
                    action: PermissionAction.CanView
                }
            ]
        }
    },
    {
        path: 'out-of-service',
        title: 'Out of Service Requests',
        component: BackflowOutOfServiceListComponent,
        canActivate: [PermissionGuard],
        data: {
            permissions: [
                {
                    type: PermissionType.BackflowOutOfService,
                    action: PermissionAction.CanView
                }
            ]
        }
    },
    {
        path: 'testers',
        title: 'BPAT Management',
        component: BackflowTesterListComponent,
        canActivate: [PermissionGuard],
        data: {
            permissions: [
                {
                    type: PermissionType.BackflowTesters,
                    action: PermissionAction.CanView
                }
            ]
        }
    },
    {
        path: 'testers/details/:id',
        title: 'BPAT Details',
        component: BackflowTesterDetailsComponent,
        canActivate: [PermissionGuard],
        data: {
            permissions: [
                {
                    type: PermissionType.BackflowTesters,
                    action: PermissionAction.CanView
                }
            ]
        }
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
export class BackflowRoutingModule {}
