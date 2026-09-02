import { NgModule } from "@angular/core";
import { CommonModule } from "@angular/common";
import { FormsModule } from "@angular/forms";
import { SharedComponentsModule } from "../shared/components/shared.components.module";
import { RegisteredProfessionalsRoutingModule } from "./registered-professionals-routing.module";
import { RegisteredProfessionalSearchComponent } from "./search/registered-professional-search.component";

@NgModule({
    declarations: [
        RegisteredProfessionalSearchComponent
    ],
    imports: [
        RegisteredProfessionalsRoutingModule,
        CommonModule,
        FormsModule,
        SharedComponentsModule
    ]
})
export class RegisteredProfessionalsModule {

}
