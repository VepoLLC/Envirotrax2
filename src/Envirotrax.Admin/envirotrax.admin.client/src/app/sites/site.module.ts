import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { SharedComponentsModule } from '../shared/components/shared.components.module';
import { SiteRoutingModule } from './site-routing.module';
import { SiteListComponent } from './list/site-list.component';

@NgModule({
    declarations: [
        SiteListComponent,
    ],
    imports: [
        CommonModule,
        FormsModule,
        SharedComponentsModule,
        SiteRoutingModule,
    ],
})
export class SiteModule { }
