import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { BackflowTestListComponent } from './tests/list/backflow-test-list.component';
import { BackflowTesterListComponent } from './testers/list/backflow-tester-list.component';

const routes: Routes = [
    {
        path: 'tests',
        title: 'Backflow Test Search',
        component: BackflowTestListComponent,
    },
    {
        path: 'testers',
        title: 'Backflow Tester Search',
        component: BackflowTesterListComponent,
    },
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule],
})
export class BackflowRoutingModule { }
