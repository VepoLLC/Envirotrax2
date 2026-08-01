import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { SharedComponentsModule } from '../../../shared/components/shared.components.module';
import { SiteWaterSupplierSectionComponent } from './site-water-supplier-section.component';
import { SitePropertySettingsSectionComponent } from './site-property-settings-section.component';
import { SitePropertyMailingSectionComponent } from './site-property-mailing-section.component';
import { SiteGisSectionComponent } from './site-gis-section.component';

/**
 * Declares the (non-standalone) Edit Site child section components and provides their template scope
 * (CommonModule / FormsModule / shared vp-* components). Imported by the standalone SiteEditComponent.
 *
 * Note: SiteEditComponent itself must be standalone because it is instantiated dynamically by the
 * windowing system (ViewContainerRef.createComponent), which does not apply an NgModule's scope to the
 * created root — a non-standalone root fails at runtime with NG0301/NG0303. The child sections are
 * rendered inside that root via normal template instantiation, so they get their scope from this
 * module normally.
 */
@NgModule({
    declarations: [
        SiteWaterSupplierSectionComponent,
        SitePropertySettingsSectionComponent,
        SitePropertyMailingSectionComponent,
        SiteGisSectionComponent,
    ],
    imports: [
        CommonModule,
        FormsModule,
        SharedComponentsModule,
    ],
    exports: [
        SiteWaterSupplierSectionComponent,
        SitePropertySettingsSectionComponent,
        SitePropertyMailingSectionComponent,
        SiteGisSectionComponent,
    ],
})
export class SiteEditSectionsModule { }
