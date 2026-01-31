import { NgModule } from "@angular/core";
import { ProfileRoutingModule } from "./profile-routing.module";
import { CommonModule } from "@angular/common";
import { FormsModule } from "@angular/forms";
import { SharedComponentsModule } from "../shared/components/shared.components.module";
import { CompanyComponent } from "./company/company.component";


@NgModule({
    declarations: [
        CompanyComponent
    ],
    imports: [
        ProfileRoutingModule,
        CommonModule,
        FormsModule,
        SharedComponentsModule,
    ]
})
export class ProfileModule {

}