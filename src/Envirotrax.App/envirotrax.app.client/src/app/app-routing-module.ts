import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AuthGuard } from './shared/guards/auth.guard';
import { FeatureGuard } from './shared/guards/feature.guard';
import { FeatureType } from './shared/models/feature-tyype';
import { RoleGuard } from './shared/guards/role.guard';
import { ROLE_DEFINITIONS } from './shared/models/role-definitions';
const routes: Routes = [
  {
    path: '',
    title: '',
    canActivate: [AuthGuard],
    children: [
      {
        path: 'admin',
        loadChildren: () => import('./admin/admin.module').then(m => m.AdminModule),
        canActivate: [RoleGuard],
        data: {
          roles: [ROLE_DEFINITIONS.WATER_SUPPLIER]
        }
      },
      {
        path: 'sites',
        loadChildren: () => import('./sites/site.module').then(m => m.SiteModule),
        canActivate: [RoleGuard],
        data: {
          roles: [ROLE_DEFINITIONS.WATER_SUPPLIER]
        }
      },
      {
        path: 'professionals',
        loadChildren: () => import('./professionals/professional.module').then(m => m.ProfessionalModule),
        canActivate: [RoleGuard],
        data: {
          roles: [ROLE_DEFINITIONS.PROFESSIONAL]
        }
      },
      {
        path: 'csi',
        loadChildren: () => import('./csi/csi.module').then(m => m.CsiModule),
        canActivate: [FeatureGuard, RoleGuard],
        data: {
          features: [FeatureType.CsiInspection],
          roles: [ROLE_DEFINITIONS.WATER_SUPPLIER, ROLE_DEFINITIONS.PROFESSIONAL]
        }
      },
      {
        path: 'backflow',
        loadChildren: () => import('./backflow/backflow.module').then(m => m.BackflowModule),
        canActivate: [FeatureGuard, RoleGuard],
        data: {
          features: [FeatureType.BackflowTesting],
          roles: [ROLE_DEFINITIONS.WATER_SUPPLIER]
        }
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
