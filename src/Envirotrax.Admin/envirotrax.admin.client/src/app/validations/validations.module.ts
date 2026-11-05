import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ValidationsRoutingModule } from './validations-routing.module';
import { SubmissionValidationListComponent } from './list/submission-validation-list.component';

@NgModule({
    imports: [
        CommonModule,
        ValidationsRoutingModule,
        SubmissionValidationListComponent,
    ],
})
export class ValidationsModule { }
