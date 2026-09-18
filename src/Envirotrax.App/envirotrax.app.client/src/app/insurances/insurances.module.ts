import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { SharedComponentsModule } from '../shared/components/shared.components.module';
import { InsurancesRoutingModule } from './insurances-routing.module';
import { InsuranceManagementComponent } from './insurance-management.component';

@NgModule({
    declarations: [InsuranceManagementComponent],
    imports: [CommonModule, FormsModule, RouterModule, SharedComponentsModule, InsurancesRoutingModule]
})
export class InsurancesModule {}
