import { NgModule } from "@angular/core";
import { RouterModule, Routes } from "@angular/router";
import { ProfessionalFogInspectionListComponent } from "./inspections/list/professional-fog-inspection-list.component";
import { FogInspectionViewComponent } from "./inspections/view/fog-inspection-view.component";
import { ProfessionalFogSubmissionPropertySearchComponent } from "./inspections/create/professional-fog-submission-property-search.component";
import { ProfessionalFogSubmissionCreateComponent } from "./inspections/create/professional-fog-submission-create.component";
import { ProfessionalFogTripTicketListComponent } from "./trip-tickets/list/professional-fog-trip-ticket-list.component";

const routes: Routes = [
    {
        path: 'inspections',
        title: 'FOG Inspection Search',
        component: ProfessionalFogInspectionListComponent
    },
    {
        path: 'inspections/create',
        title: 'Submit FOG Inspection',
        component: ProfessionalFogSubmissionPropertySearchComponent
    },
    {
        path: 'inspections/create/:siteId',
        title: 'Submit FOG Inspection',
        component: ProfessionalFogSubmissionCreateComponent
    },
    {
        path: 'inspections/:id',
        title: 'View FOG Inspection',
        component: FogInspectionViewComponent
    },
    {
        path: 'trip-tickets',
        title: 'FOG Trip Ticket Search',
        component: ProfessionalFogTripTicketListComponent
    },
    {
        path: 'trip-tickets/:id',
        title: 'View FOG Trip Ticket',
        component: ProfessionalFogTripTicketListComponent
    }
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class ProfessionalsFogRoutingModule {}
