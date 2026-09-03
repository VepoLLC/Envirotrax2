import { NgModule } from "@angular/core";
import { EnvirotraxComponentsModule } from "@envirotrax/common-ui";
import { WaterSupplierHierarchyComponent } from "./water-supplier-hierarchy/water-supplier-hierarchy.component";
import { ProfessionalUserLookupComponent } from "./lookups/professional-user-lookup/professional-user-lookup.component";
import { WaterSupplierUserLookupComponent } from "./lookups/water-supplier-user-lookup/water-supplier-user-lookup.component";
import { GisAreaLookupComponent } from "./lookups/gis-areas/gis-area-lookup.component";
import { GisAreaSelectionModalComponent } from "./lookups/gis-areas/gis-area-selection-modal.component";
import { DownloadManagerComponent } from "./data-components/download-manager/download-manager.component";
import { PropertyLogCellComponent } from "./data-components/table-cells/property-log-cell.component";
import { SiteLogEditComponent } from "./site-log/site-log-edit.component";
import { FormsModule } from "@angular/forms";
import { CommonModule } from "@angular/common";

@NgModule({
    declarations: [
        WaterSupplierHierarchyComponent,
        ProfessionalUserLookupComponent,
        WaterSupplierUserLookupComponent,
        GisAreaLookupComponent,
        GisAreaSelectionModalComponent,
        DownloadManagerComponent,
        PropertyLogCellComponent,
        SiteLogEditComponent
    ],
    imports: [
        EnvirotraxComponentsModule,
        FormsModule,
        CommonModule
    ],
    exports: [
        EnvirotraxComponentsModule,
        WaterSupplierHierarchyComponent,
        GisAreaLookupComponent,
        ProfessionalUserLookupComponent,
        WaterSupplierUserLookupComponent,
        DownloadManagerComponent
    ]
})
export class SharedComponentsModule {

}