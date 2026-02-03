import { CommonModule } from "@angular/common";
import { NgModule } from "@angular/core";
import { FormsModule } from "@angular/forms";
import { RouterModule } from "@angular/router";
import { SharedComponentsModule } from "../shared/components/shared.components.module";
import { SiteListComponent } from "./list/site-list.component";
import { SiteRoutingModule } from "./site-routing.module";

@NgModule({
    declarations: [
        SiteListComponent
    ],
    imports: [
        CommonModule,
        FormsModule,
        RouterModule,
        SharedComponentsModule,
        SiteRoutingModule
    ]
})
export class SiteModule {

}
