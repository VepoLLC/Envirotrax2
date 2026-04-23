import { NgModule } from "@angular/core";
import { RouterModule, Routes } from "@angular/router";
import { HomeComponent } from "./home/home.component";
import { GisAreaListComponent } from "./gis-areas/list/gis-area-list.component";
import { PermissionGuard } from "../shared/guards/permission.guard";
import { PermissionAction, PermissionType } from "../shared/models/permission-type";

const routes: Routes = [
    {
        path: '',
        component: HomeComponent,
        title: 'Administration'
    },
    {
        path: 'water-suppliers',
        loadChildren: () => import('./water-suppliers/water-supplier.module').then(m => m.WaterSupplierModule)
    },
    {
        path: 'settings',
        loadChildren: () => import('./settings/settings.module').then(m => m.SettingsModule)
    },
    {
        path: 'users',
        loadChildren: () => import('./users/user.module').then(m => m.UserModule)
    },
    {
        path: 'gis-areas',
        component: GisAreaListComponent,
        canActivate: [PermissionGuard],
        data: {
            permissions: [
                {
                    type: PermissionType.Settings,
                    action: PermissionAction.CanView
                }
            ]
        }
    }
];

@NgModule({
    imports: [
        RouterModule.forChild(routes)
    ],
    exports: [
        RouterModule
    ]
})
export class AdminRoutingModule {

}