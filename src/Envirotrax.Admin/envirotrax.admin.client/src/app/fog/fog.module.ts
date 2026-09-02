import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { SharedComponentsModule } from '../shared/components/shared.components.module';
import { FogRoutingModule } from './fog-routing.module';
import { FogInspectionListComponent } from './inspections/list/fog-inspection-list.component';

@NgModule({
    declarations: [
        FogInspectionListComponent,
    ],
    imports: [
        CommonModule,
        FormsModule,
        SharedComponentsModule,
        FogRoutingModule,
    ],
})
export class FogModule { }
