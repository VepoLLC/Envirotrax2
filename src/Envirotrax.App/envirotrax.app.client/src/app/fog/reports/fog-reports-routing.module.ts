import { NgModule } from "@angular/core";
import { RouterModule, Routes } from "@angular/router";
import { PermissionGuard } from "../../shared/guards/permission.guard";
import { PermissionAction, PermissionType } from "../../shared/models/permission-type";
import { FogReportComponent } from "./fog-report.component";
import { FogTripTicketReportTabComponent } from "./tabs/trip-tickets/fog-trip-ticket-report-tab.component";
import { FogInspectionReportTabComponent } from "./tabs/inspections/fog-inspection-report-tab.component";
import { FogInspectionComplianceManagementComponent } from "./compliance/fog-inspection-compliance-management.component";
import { FogPermitComplianceManagementComponent } from "./compliance/fog-permit-compliance-management.component";
import { FogTripTicketComplianceManagementComponent } from "./compliance/fog-trip-ticket-compliance-management.component";

const compliancePermissions = {
    permissions: [
        {
            type: PermissionType.FogReports,
            action: PermissionAction.CanView
        }
    ]
};

// The compliance routes are declared BEFORE the '' shell route: '' carries children, so leaving it first
// would let the router commit to it and fail to match the remaining segment instead of falling through.
const routes: Routes = [
    {
        path: 'inspection-compliance',
        title: 'FOG Inspection Compliance Management',
        component: FogInspectionComplianceManagementComponent,
        canActivate: [PermissionGuard],
        data: compliancePermissions
    },
    {
        path: 'permit-compliance',
        title: 'FOG Permit Compliance Management',
        component: FogPermitComplianceManagementComponent,
        canActivate: [PermissionGuard],
        data: compliancePermissions
    },
    {
        path: 'trip-ticket-compliance',
        title: 'FOG Trip Ticket Compliance Management',
        component: FogTripTicketComplianceManagementComponent,
        canActivate: [PermissionGuard],
        data: compliancePermissions
    },
    {
        path: '',
        component: FogReportComponent,
        children: [
            { path: '', pathMatch: 'full', redirectTo: 'trip-tickets' },
            { path: 'trip-tickets', title: 'FOG Trip Ticket Reports', component: FogTripTicketReportTabComponent },
            { path: 'inspections', title: 'FOG Inspection Reports', component: FogInspectionReportTabComponent }
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
export class FogReportsRoutingModule {}
