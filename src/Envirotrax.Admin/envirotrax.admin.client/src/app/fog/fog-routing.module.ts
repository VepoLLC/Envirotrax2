import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { FogInspectionListComponent } from './inspections/list/fog-inspection-list.component';

const routes: Routes = [
    {
        path: 'inspections',
        title: 'FOG Inspection Search',
        component: FogInspectionListComponent,
    },
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule],
})
export class FogRoutingModule { }
