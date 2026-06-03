import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { SharedComponentsModule } from '../shared/components/shared.components.module';
import { LicensesRoutingModule } from './licenses-routing.module';
import { LicenseManagementComponent } from './license-management.component';
import { EditWaterSupplierLicenseComponent } from './edit/edit-water-supplier-license.component';

@NgModule({
    declarations: [LicenseManagementComponent, EditWaterSupplierLicenseComponent],
    imports: [CommonModule, FormsModule, RouterModule, SharedComponentsModule, LicensesRoutingModule]
})
export class LicensesModule {}
