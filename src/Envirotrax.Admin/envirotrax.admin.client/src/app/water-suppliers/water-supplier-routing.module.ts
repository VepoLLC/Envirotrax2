import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { WaterSupplierListComponent } from './list/water-supplier-list.component';

const routes: Routes = [
    {
        path: '',
        title: 'Water Suppliers',
        component: WaterSupplierListComponent,
    },
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule],
})
export class WaterSupplierRoutingModule { }
