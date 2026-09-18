import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { SharedComponentsModule } from '../shared/components/shared.components.module';
import { RegistrationsRoutingModule } from './registrations-routing.module';
import { RegistrationManagementComponent } from './registration-management.component';

@NgModule({
    declarations: [RegistrationManagementComponent],
    imports: [CommonModule, FormsModule, RouterModule, SharedComponentsModule, RegistrationsRoutingModule]
})
export class RegistrationsModule {}
