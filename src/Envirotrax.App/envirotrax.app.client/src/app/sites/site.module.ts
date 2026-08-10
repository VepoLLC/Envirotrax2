import { CommonModule } from "@angular/common";
import { NgModule } from "@angular/core";
import { FormsModule } from "@angular/forms";
import { RouterModule } from "@angular/router";
import { SharedComponentsModule } from "../shared/components/shared.components.module";
import { SiteListComponent } from "./list/site-list.component";
import { SiteRoutingModule } from "./site-routing.module";
import { CreateSiteComponent } from './create/create-site-component';
import { EditSiteComponent } from './edit/edit-site-component';
import { SiteCsiInspectionsComponent } from './edit/tabs/csi-inspections/site-csi-inspections.component';
import { SiteBackflowTestsComponent } from './edit/tabs/backflow-tests/site-backflow-tests.component';
import { SiteBackflowOutOfServiceComponent } from './edit/tabs/backflow-out-of-service/site-backflow-out-of-service.component';
import { SiteFogInspectionsComponent } from './edit/tabs/fog-inspections/site-fog-inspections.component';
import { SiteLogHistoryComponent } from './edit/tabs/log-history/site-log-history.component';
import { PropertyLogManagementComponent } from './reports/property-log-management/property-log-management.component';

@NgModule({
    declarations: [
        SiteListComponent,
        CreateSiteComponent,
        EditSiteComponent,
        SiteCsiInspectionsComponent,
        SiteBackflowTestsComponent,
        SiteBackflowOutOfServiceComponent,
        SiteFogInspectionsComponent,
        SiteLogHistoryComponent,
        PropertyLogManagementComponent
    ],
    imports: [
        CommonModule,
        FormsModule,
        RouterModule,
        SharedComponentsModule,
        SiteRoutingModule
    ]
})
export class SiteModule {

}
