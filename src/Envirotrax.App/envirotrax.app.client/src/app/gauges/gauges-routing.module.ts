import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { GaugeManagementComponent } from './gauge-management.component';

const routes: Routes = [
    {
        path: '',
        title: 'Gauge Management',
        component: GaugeManagementComponent
    }
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class GaugesRoutingModule {}
