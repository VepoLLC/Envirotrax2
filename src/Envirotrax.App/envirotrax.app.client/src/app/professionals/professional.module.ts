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
import { ProfessionalUserLicenseListComponent } from "./users/edit/licenses/list/professional-user-license-list.component";
import { CreateEditLicenseComponent } from "./users/edit/licenses/create-edit/create-edit-license.component";
import { SiteListComponent } from "./sites/site-list.component";
import { SiteDetailsComponent } from "./sites/details/site-details.component";
import { InsuranceListComponent } from "./insurances/list/insurance-list.component";
import { EditInsuranceComponent } from "./insurances/edit/edit-insurance.component";
import { LicenseHelpTextComponent } from "./users/edit/licenses/help-text/license-help-text.component";
import { LicenseListComponent } from "./licenses/license-list.component";

@NgModule({
    declarations: [
        WaterSuppliersComponent,
        WaterSupplierRegistrationComponent,
        ProfessionalUserListComponent,
        CreateProfessionalUserComponent,
        EditProfessionalUserComponent,
        ProfessionalUserLicenseListComponent,
        LicenseHelpTextComponent,
        CreateEditLicenseComponent,
        SiteListComponent,
        SiteDetailsComponent,
        InsuranceListComponent,
        EditInsuranceComponent,
        LicenseListComponent
    ],
    imports: [
        ProfessionalRoutingModule,
        CommonModule,
        FormsModule,
        SharedComponentsModule,
    ],
    exports:[
        LicenseHelpTextComponent
    ]
})
export class ProfessionalModule {

}