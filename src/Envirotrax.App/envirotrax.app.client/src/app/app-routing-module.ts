import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AuthGuard } from './shared/guards/auth.guard';

const routes: Routes = [
  {
    path: '',
    title: '',
    canActivate: [AuthGuard],
    children: [
      {
        path: 'admin',
        loadChildren: () => import('./admin/admin.module').then(m => m.AdminModule)
      },
      {
        path: 'sites',
        loadChildren: () => import('./sites/site.module').then(m => m.SiteModule)
      },
      {
        path: 'professionals',
        loadChildren: () => import('./professionals/professional-routing.module').then(m => m.ProfessionalRoutingModule)
      }
    ]
  },
  {
    path: 'auth',
    title: '',
    loadChildren: () => import('./auth/auth.module').then(m => m.AppAuthModule)
  },
  {
    path: 'profile',
    title: '',
    loadChildren: () => import('./profile/profile.module').then(m => m.ProfileModule)
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
