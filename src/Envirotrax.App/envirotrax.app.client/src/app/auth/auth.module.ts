import { NgModule } from "@angular/core";
import { RouterModule } from "@angular/router";
import { LoadingSpinnerModule } from "@developer-partners/ngx-loading-spinner";
import { CommonModule } from "@angular/common";
import { LoginRedirectComponent } from "./login-redirect/login-redirect.component";
import { SignOutComponent } from "./sign-out/sign-out.component";
import { UnauthorizedComponent } from "./unauthorized/unauthorized.component";

@NgModule({
    declarations: [
        LoginRedirectComponent,
        SignOutComponent
    ],
    imports: [
        CommonModule,
        LoadingSpinnerModule,
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