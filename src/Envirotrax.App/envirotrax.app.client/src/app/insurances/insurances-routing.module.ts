import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { InsuranceManagementComponent } from './insurance-management.component';

const routes: Routes = [
    {
        path: '',
        title: 'Insurance Management',
        component: InsuranceManagementComponent
    }
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class InsurancesRoutingModule {}
