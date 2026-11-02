import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LetterGeneratorComponent } from './letter-generator/letter-generator.component';

const routes: Routes = [
    {
        path: 'letter-generator',
        title: 'Letter Generator',
        component: LetterGeneratorComponent,
    },
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule],
})
export class ManagementToolsRoutingModule { }
