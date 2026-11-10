import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LicensesRoutingModule } from './licenses-routing.module';
import { LicenseManagementComponent } from './license-management.component';

@NgModule({
    imports: [
        CommonModule,
        LicensesRoutingModule,
        LicenseManagementComponent,
    ],
})
export class LicensesModule { }
