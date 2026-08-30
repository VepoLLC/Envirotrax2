import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { SharedComponentsModule } from '../shared/components/shared.components.module';
import { BackflowRoutingModule } from './backflow-routing.module';
import { BackflowTestListComponent } from './tests/list/backflow-test-list.component';

@NgModule({
    declarations: [
        BackflowTestListComponent,
    ],
    imports: [
        CommonModule,
        FormsModule,
        SharedComponentsModule,
        BackflowRoutingModule,
    ],
})
export class BackflowModule { }
