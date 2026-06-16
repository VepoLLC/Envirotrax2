import { NgModule } from "@angular/core";
import { EnvirotraxComponentsModule } from "@envirotrax/common-ui";
import { FormsModule } from "@angular/forms";
import { CommonModule } from "@angular/common";

@NgModule({
    declarations: [
    ],
    imports: [
        EnvirotraxComponentsModule,
        FormsModule,
        CommonModule
    ],
    exports: [
        EnvirotraxComponentsModule,
    ]
})
export class SharedComponentsModule {

}