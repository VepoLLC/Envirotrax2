import { NgModule } from "@angular/core";
import { CommonModule } from "@angular/common";
import { FormsModule } from "@angular/forms";
import { SharedComponentsModule } from "../../shared/components/shared.components.module";
import { FogReportsRoutingModule } from "./fog-reports-routing.module";
import { FogReportComponent } from "./fog-report.component";
import { FogTripTicketReportTabComponent } from "./tabs/trip-tickets/fog-trip-ticket-report-tab.component";
import { FogInspectionReportTabComponent } from "./tabs/inspections/fog-inspection-report-tab.component";

@NgModule({
    declarations: [
        FogReportComponent,
        FogTripTicketReportTabComponent,
        FogInspectionReportTabComponent
    ],
    imports: [
        CommonModule,
        FormsModule,
        SharedComponentsModule,
        FogReportsRoutingModule
    ]
})
export class FogReportsModule {}
