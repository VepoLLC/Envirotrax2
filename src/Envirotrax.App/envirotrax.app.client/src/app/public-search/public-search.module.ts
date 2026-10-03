import { NgModule } from "@angular/core";
import { CommonModule } from "@angular/common";
import { FormsModule } from "@angular/forms";
import { SharedComponentsModule } from "../shared/components/shared.components.module";
import { PublicSearchRoutingModule } from "./public-search-routing.module";
import { PublicSearchComponent } from "./search/public-search.component";

@NgModule({
    declarations: [
        PublicSearchComponent
    ],
    imports: [
        PublicSearchRoutingModule,
        CommonModule,
        FormsModule,
        SharedComponentsModule
    ]
})
export class PublicSearchModule {

}
