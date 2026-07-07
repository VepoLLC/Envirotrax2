import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AuthGuard } from './shared/guards/auth.guard';

const routes: Routes = [
  {
    path: '',
    title: '',
    canActivate: [AuthGuard],
    children: [
      // regular routes go here.
    ]
  },
  {
    path: 'auth',
    title: '',
    loadChildren: () => import('./auth/auth.module').then(m => m.AppAuthModule)
  },
  {
    path: 'water-suppliers',
    title: 'Water Suppliers',
    loadChildren: () => import('./water-suppliers/water-supplier.module').then(m => m.WaterSupplierModule)
  },
  {
    path: 'sites',
    title: 'Property Search',
    loadChildren: () => import('./sites/site.module').then(m => m.SiteModule)
  },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
