import { NgModule } from "@angular/core";
import { RouterModule, Routes } from "@angular/router";
import { FogReportComponent } from "./fog-report.component";
import { FogTripTicketReportTabComponent } from "./tabs/trip-tickets/fog-trip-ticket-report-tab.component";
import { FogInspectionReportTabComponent } from "./tabs/inspections/fog-inspection-report-tab.component";

const routes: Routes = [
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
