import { NgModule } from "@angular/core";
import { HomeComponent } from "./home/home.component";
import { CommonModule } from "@angular/common";
import { FormsModule } from "@angular/forms";
import { SharedComponentsModule } from "../shared/components/shared.components.module";
import { AdminRoutingModule } from "./admin-routing.module";
import { RouterModule } from "@angular/router";
import { GisAreaListComponent } from "./gis-areas/list/gis-area-list.component";

@NgModule({
    declarations: [
        HomeComponent,
        GisAreaListComponent
    ],
    imports: [
        CommonModule,
        FormsModule,
        SharedComponentsModule,
        AdminRoutingModule,
        RouterModule
    ]
})
export class AdminModule {

}