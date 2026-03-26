import { NgModule } from "@angular/core";
import { ProfessionalRoutingModule } from "./professional-routing.module";
import { CommonModule } from "@angular/common";
import { FormsModule } from "@angular/forms";
import { SharedComponentsModule } from "../shared/components/shared.components.module";
import { WaterSuppliersComponent } from "./water-supppliers/water-suppliers.component";
import { WaterSupplierRegistrationComponent } from "./water-supppliers/registration/water-supplier-registration.component";
import { ProfessionalUserListComponent } from "./users/list/professional-user-list.component";
import { CreateProfessionalUserComponent } from "./users/create/create-professional-user.component";
import { EditProfessionalUserComponent } from "./users/edit/edit-professional-user.component";
import { AddProfessionalUserLicenseComponent } from "./users/edit/add-professional-user-license.component";

@NgModule({
    declarations: [
        WaterSuppliersComponent,
        WaterSupplierRegistrationComponent,
        ProfessionalUserListComponent,
        CreateProfessionalUserComponent,
        EditProfessionalUserComponent,
        AddProfessionalUserLicenseComponent
    ],
    imports: [
        ProfessionalRoutingModule,
        CommonModule,
        FormsModule,
        SharedComponentsModule,
    ]
})
export class ProfessionalModule {

}