import { NgModule } from "@angular/core";
import { RouterModule } from "@angular/router";
import { CommonModule } from "@angular/common";
import { LoginRedirectComponent } from "./login-redirect/login-redirect.component";
import { SignOutComponent } from "./sign-out/sign-out.component";
import { UnauthorizedComponent } from "./unauthorized/unauthorized.component";
import { WaterSupplierListComponent } from "./water-suppliers/water-supplier-list.component";
import { SharedComponentsModule } from "../shared/components/shared.components.module";

@NgModule({
    declarations: [
        LoginRedirectComponent,
        SignOutComponent,
        UnauthorizedComponent,
        WaterSupplierListComponent,
    ],
    imports: [
        CommonModule,
        SharedComponentsModule,
        RouterModule.forChild([
            {
                path: 'login-redirect',
                title: 'Sign In',
                component: LoginRedirectComponent
            },
            {
                path: 'sign-out',
                title: 'Signed Out',
                component: SignOutComponent
            },
            {
                path: 'unauthorized',
                title: 'Unauthorized',
                component: UnauthorizedComponent
            },
            {
                path: 'water-suppliers',
                title: 'Select Water Supplier',
                component: WaterSupplierListComponent
            }
        ])
    ],
    exports: [
        RouterModule
    ]
})
export class AppAuthModule {

}