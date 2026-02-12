import { NgModule } from "@angular/core";
import { RouterModule, Routes } from "@angular/router";
import { WaterSuppliersComponent } from "./water-supppliers/water-supliers.component";

const routes: Routes = [
    {
        path: 'water-suppliers',
        title: 'Water Supplier Registration',
        component: WaterSuppliersComponent
    }
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class ProfessionalRoutingModule {

}
