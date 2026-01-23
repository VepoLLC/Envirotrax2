import { NgModule } from "@angular/core";
import { RouterModule, Routes } from "@angular/router";
import { HomeComponent } from "./home/home.component";

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