import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { CsiInspectionListComponent } from './inspections/list/csi-inspection-list.component';

const routes: Routes = [
    {
        path: 'inspections',
        title: 'CSI Inspection Search',
        component: CsiInspectionListComponent,
    },
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule],
})
export class CsiRoutingModule { }
