import { NgModule } from "@angular/core";
import { RouterModule } from "@angular/router";
import { CommonModule } from "@angular/common";
import { LoginRedirectComponent } from "./login-redirect/login-redirect.component";
import { SignOutComponent } from "./sign-out/sign-out.component";
import { UnauthorizedComponent } from "./unauthorized/unauthorized.component";
import { AppLoadingSpinnerModule } from "../shared/components/loading-spinner/app-loading-spinner.module";

@NgModule({
    declarations: [
        LoginRedirectComponent,
        SignOutComponent
    ],
    imports: [
        CommonModule,
        AppLoadingSpinnerModule,
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
            }
        ])
    ],
    exports: [
        RouterModule
    ]
})
export class AppAuthModule {

}