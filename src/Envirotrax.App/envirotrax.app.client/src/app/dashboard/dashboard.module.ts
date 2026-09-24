import { NgModule } from "@angular/core";
import { CommonModule } from "@angular/common";
import { RouterModule } from "@angular/router";
import { BaseChartDirective, provideCharts, withDefaultRegisterables } from "ng2-charts";
import { SharedComponentsModule } from "../shared/components/shared.components.module";
import { DashboardRoutingModule } from "./dashboard-routing.module";
import { DashboardComponent } from "./dashboard.component";

@NgModule({
    declarations: [DashboardComponent],
    imports: [
        CommonModule,
        RouterModule,
        SharedComponentsModule,
        BaseChartDirective,
        DashboardRoutingModule
    ],
    providers: [
        provideCharts(withDefaultRegisterables())
    ]
})
export class DashboardModule {}