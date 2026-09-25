import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { SharedComponentsModule } from '../shared/components/shared.components.module';
import { GaugesRoutingModule } from './gauges-routing.module';
import { GaugeManagementComponent } from './gauge-management.component';

@NgModule({
    declarations: [GaugeManagementComponent],
    imports: [CommonModule, FormsModule, RouterModule, SharedComponentsModule, GaugesRoutingModule]
})
export class GaugesModule {}
