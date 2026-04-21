import { NgModule } from "@angular/core";
import { CommonModule } from "@angular/common";
import { FormsModule } from "@angular/forms";
import { RouterModule } from "@angular/router";
import { SharedComponentsModule } from "../shared/components/shared.components.module";
import { BackflowRoutingModule } from "./backflow-routing.module";
import { BackflowTesterListComponent } from "./testers/list/backflow-tester-list.component";
import { BackflowTestListComponent } from "./tests/backflow-test-list.component";

@NgModule({
    declarations: [
        BackflowTesterListComponent,
        BackflowTestListComponent
    ],
    imports: [
        CommonModule,
        FormsModule,
        RouterModule,
        SharedComponentsModule,
        BackflowRoutingModule
    ]
})
export class BackflowModule {}
