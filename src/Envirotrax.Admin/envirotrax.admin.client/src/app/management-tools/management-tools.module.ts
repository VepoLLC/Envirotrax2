import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ManagementToolsRoutingModule } from './management-tools-routing.module';
import { LetterGeneratorComponent } from './letter-generator/letter-generator.component';
import { UserLogComponent } from './user-log/user-log.component';

@NgModule({
    imports: [
        CommonModule,
        ManagementToolsRoutingModule,
        LetterGeneratorComponent,
        UserLogComponent,
    ],
})
export class ManagementToolsModule { }
