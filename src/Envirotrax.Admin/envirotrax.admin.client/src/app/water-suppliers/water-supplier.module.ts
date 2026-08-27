import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { SharedComponentsModule } from '../shared/components/shared.components.module';
import { WaterSupplierRoutingModule } from './water-supplier-routing.module';
import { WaterSupplierListComponent } from './list/water-supplier-list.component';

@NgModule({
    declarations: [
        WaterSupplierListComponent,
    ],
    imports: [
        CommonModule,
        FormsModule,
        SharedComponentsModule,
        WaterSupplierRoutingModule,
    ],
})
export class WaterSupplierModule { }
