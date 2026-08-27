import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { CsiInspectionListComponent } from './inspections/list/csi-inspection-list.component';
import { CsiInspectorListComponent } from './inspectors/list/csi-inspector-list.component';

const routes: Routes = [
    {
        path: 'inspections',
        title: 'CSI Inspection Search',
        component: CsiInspectionListComponent,
    },
    {
        path: 'inspectors',
        title: 'CSI Inspector Search',
        component: CsiInspectorListComponent,
    },
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule],
})
export class CsiRoutingModule { }
