import { NgModule } from "@angular/core";
import { RouterModule, Routes } from "@angular/router";
import { PermissionGuard } from "../shared/guards/permission.guard";
import { PermissionAction, PermissionType } from "../shared/models/permission-type";
import { FogInspectorListComponent } from "./inspectors/list/fog-inspector-list.component";
import { FogInspectorDetailsComponent } from "./inspectors/details/fog-inspector-details.component";
import { FogInspectionListComponent } from "./inspections/list/fog-inspection-list.component";
import { FogInspectionViewComponent } from "./inspections/view/fog-inspection-view.component";
import { FogTripTicketListComponent } from "./trip-tickets/list/fog-trip-ticket-list.component";
import { FogTripTicketViewComponent } from "./trip-tickets/view/fog-trip-ticket-view.component";
import { FogTransporterListComponent } from "./transporters/list/fog-transporter-list.component";
import { FogTransporterDetailsComponent } from "./transporters/details/fog-transporter-details.component";

const routes: Routes = [
    {
        path: 'inspections',
        title: 'FOG Inspection Search',
        component: FogInspectionListComponent,
        canActivate: [PermissionGuard],
        data: {
            permissions: [
                {
                    type: PermissionType.FogInspections,
                    action: PermissionAction.CanView
                }
            ]
        }
    },
    {
        path: 'inspections/:id',
        title: 'View FOG Inspection',
        component: FogInspectionViewComponent,
        canActivate: [PermissionGuard],
        data: {
            permissions: [
                {
                    type: PermissionType.FogInspections,
                    action: PermissionAction.CanView
                }
            ]
        }
    },
    {
        path: 'trip-tickets',
        title: 'FOG Trip Ticket Search',
        component: FogTripTicketListComponent,
        canActivate: [PermissionGuard],
        data: {
            permissions: [
                {
                    type: PermissionType.FogTripTickets,
                    action: PermissionAction.CanView
                }
            ]
        }
    },
    {
        path: 'trip-tickets/:id',
        title: 'View FOG Trip Ticket',
        component: FogTripTicketViewComponent,
        canActivate: [PermissionGuard],
        data: {
            permissions: [
                {
                    type: PermissionType.FogTripTickets,
                    action: PermissionAction.CanView
                }
            ]
        }
    },
    {
        path: 'inspectors/details/:id',
        title: 'Inspector Details',
        component: FogInspectorDetailsComponent,
        canActivate: [PermissionGuard],
        data: {
            permissions: [
                {
                    type: PermissionType.FogInspectors,
                    action: PermissionAction.CanView
                }
            ]
        }
    },
    {
        path: 'inspectors',
        title: 'Inspector Management',
        component: FogInspectorListComponent,
        canActivate: [PermissionGuard],
        data: {
            permissions: [
                {
                    type: PermissionType.FogInspectors,
                    action: PermissionAction.CanView
                }
            ]
        }
    },
    {
        path: 'transporters/details/:id',
        title: 'Transporter Details',
        component: FogTransporterDetailsComponent,
        canActivate: [PermissionGuard],
        data: {
            permissions: [
                {
                    type: PermissionType.FogTransporters,
                    action: PermissionAction.CanView
                }
            ]
        }
    },
    {
        path: 'transporters',
        title: 'Transporter Management',
        component: FogTransporterListComponent,
        canActivate: [PermissionGuard],
        data: {
            permissions: [
                {
                    type: PermissionType.FogTransporters,
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
export class FogRoutingModule { }
