import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SharedComponentsModule } from '../shared/components/shared.components.module';
import { WaterSupplierRoutingModule } from './water-supplier-routing.module';
import { WaterSupplierListComponent } from './list/water-supplier-list.component';

@NgModule({
    declarations: [
        WaterSupplierListComponent,
    ],
    imports: [
        CommonModule,
        SharedComponentsModule,
        WaterSupplierRoutingModule,
    ],
})
export class WaterSupplierModule { }
