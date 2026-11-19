import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LetterGeneratorComponent } from './letter-generator/letter-generator.component';
import { UserLogComponent } from './user-log/user-log.component';
import { CompanyListComponent } from './crm/companies/list/company-list.component';

const routes: Routes = [
    {
        path: 'letter-generator',
        title: 'Letter Generator',
        component: LetterGeneratorComponent,
    },
    {
        path: 'user-log',
        title: 'User Log',
        component: UserLogComponent,
    },
    {
        path: 'companies',
        title: 'Contact Manager',
        component: CompanyListComponent,
    },
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule],
})
export class ManagementToolsRoutingModule { }
