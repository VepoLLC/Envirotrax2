import { NgModule } from "@angular/core";
import { LoadingSpinnerConfig, LoadingSpinnerModule } from "@developer-partners/ngx-loading-spinner";
import { LoadingSpinnerComponent } from "./loading-spinner.component";

@NgModule({
    declarations: [
        LoadingSpinnerComponent
    ],
    imports: [
        LoadingSpinnerModule
    ],
    exports: [
        LoadingSpinnerModule
    ],
    providers: [
        {
            provide: LoadingSpinnerConfig,
            useFactory: () => {
                return {
                    component: LoadingSpinnerComponent,
                    defaultMessage: 'Loading Data...'
                }
            }
        }
    ],
})
export class AppLoadingSpinnerModule {

}