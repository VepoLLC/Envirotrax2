import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ManagementToolsRoutingModule } from './management-tools-routing.module';
import { LetterGeneratorComponent } from './letter-generator/letter-generator.component';

@NgModule({
    imports: [
        CommonModule,
        ManagementToolsRoutingModule,
        LetterGeneratorComponent,
    ],
})
export class ManagementToolsModule { }
