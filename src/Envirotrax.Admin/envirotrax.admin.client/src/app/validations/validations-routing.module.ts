import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { SubmissionValidationListComponent } from './list/submission-validation-list.component';

const routes: Routes = [
    {
        path: '',
        title: 'Validations',
        component: SubmissionValidationListComponent,
    },
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule],
})
export class ValidationsRoutingModule { }
