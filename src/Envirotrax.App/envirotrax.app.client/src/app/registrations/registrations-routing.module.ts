import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { RegistrationManagementComponent } from './registration-management.component';

const routes: Routes = [
    {
        path: '',
        title: 'Registration Management',
        component: RegistrationManagementComponent
    }
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class RegistrationsRoutingModule {}
