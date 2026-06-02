import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LicenseManagementComponent } from './license-management.component';

const routes: Routes = [
    {
        path: '',
        title: 'License Management',
        component: LicenseManagementComponent
    }
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class LicensesRoutingModule {}