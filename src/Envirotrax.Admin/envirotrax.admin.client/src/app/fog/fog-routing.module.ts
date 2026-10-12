import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { FogInspectionListComponent } from './inspections/list/fog-inspection-list.component';
import { FogInspectorListComponent } from './inspectors/list/fog-inspector-list.component';

const routes: Routes = [
    {
        path: 'inspections',
        title: 'FOG Inspection Search',
        component: FogInspectionListComponent,
    },
    {
        path: 'inspectors',
        title: 'FOG Inspector Search',
        component: FogInspectorListComponent,
    },
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule],
})
export class FogRoutingModule { }
