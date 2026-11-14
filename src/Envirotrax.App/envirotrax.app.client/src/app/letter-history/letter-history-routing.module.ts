import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LetterHistoryComponent } from './letter-history.component';
import { PermissionGuard } from '../shared/guards/permission.guard';
import { PermissionAction, PermissionType } from '../shared/models/permission-type';

const routes: Routes = [
    {
        path: '',
        title: 'Letter Generation History',
        component: LetterHistoryComponent,
        canActivate: [PermissionGuard],
        data: {
            permissions: [
                {
                    type: PermissionType.AccountInformation,
                    action: PermissionAction.CanView
                }
            ]
        }
    }
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class LetterHistoryRoutingModule {}
