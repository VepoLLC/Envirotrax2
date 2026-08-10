import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { SharedComponentsModule } from '../shared/components/shared.components.module';
import { CsiRoutingModule } from './csi-routing.module';
import { CsiInspectionListComponent } from './inspections/list/csi-inspection-list.component';
import { CsiInspectionDetailsComponent } from './inspections/details/csi-inspection-details.component';

@NgModule({
    declarations: [
        CsiInspectionListComponent,
    ],
    imports: [
        CommonModule,
        FormsModule,
        SharedComponentsModule,
        CsiRoutingModule,
        CsiInspectionDetailsComponent,
    ],
})
export class CsiModule { }
