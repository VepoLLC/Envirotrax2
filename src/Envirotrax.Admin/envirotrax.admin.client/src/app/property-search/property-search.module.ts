import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { SharedComponentsModule } from '../shared/components/shared.components.module';
import { PropertySearchRoutingModule } from './property-search-routing.module';
import { PropertySearchComponent } from './property-search.component';

@NgModule({
    declarations: [
        PropertySearchComponent,
    ],
    imports: [
        CommonModule,
        FormsModule,
        SharedComponentsModule,
        PropertySearchRoutingModule,
    ],
})
export class PropertySearchModule { }
