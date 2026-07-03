import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { PropertySearchComponent } from './property-search.component';

const routes: Routes = [
    {
        path: '',
        title: 'Property Search',
        component: PropertySearchComponent,
    },
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule],
})
export class PropertySearchRoutingModule { }
