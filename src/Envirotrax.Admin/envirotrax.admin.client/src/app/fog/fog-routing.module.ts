import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { FogInspectionListComponent } from './inspections/list/fog-inspection-list.component';
import { FogInspectorListComponent } from './inspectors/list/fog-inspector-list.component';
import { FogDisposalSiteListComponent } from './disposal-sites/list/fog-disposal-site-list.component';

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
    {
        path: 'disposal-sites',
        title: 'FOG Disposal Sites',
        component: FogDisposalSiteListComponent,
    },
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule],
})
export class FogRoutingModule { }
