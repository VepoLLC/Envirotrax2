import { Component, Input, Optional, SkipSelf } from '@angular/core';
import { ControlContainer, NgForm } from '@angular/forms';
import { InputOption } from '@envirotrax/common-ui';
import { SiteDetail } from '../../../shared/models/sites/site-detail';

/**
 * GIS Data section of the Edit Site view. Latitude, Longitude and GIS Status are editable client-side;
 * GIS Date is read-only. The map reuses the shared @envirotrax/common-ui vp-map component, which loads
 * the Google Maps key from GET /api/google-maps/api-key (proxied by Admin.Server).
 *
 * Its ngModel controls register into the parent SiteEditComponent's NgForm via the ControlContainer
 * viewProviders pattern. Editing mutates only the parent-provided editable site model — no API calls,
 * no persistence logic (GIS is saved later through its own dedicated endpoint, coordinated by the
 * parent). Internal fields (GisAreaId, GisOutOfArea) are intentionally not exposed.
 */
@Component({
    selector: 'app-site-gis-section',
    standalone: false,
    templateUrl: './site-gis-section.component.html',
    viewProviders: [
        {
            provide: ControlContainer,
            useFactory: (container: ControlContainer) => container,
            deps: [[new SkipSelf(), new Optional(), ControlContainer]]
        }
    ]
})
export class SiteGisSectionComponent {
    @Input() site!: SiteDetail;

    // The parent SiteEditComponent's NgForm — used only so vp-input can display validation state.
    @Input() form!: NgForm;

    // Reuses the exact GisStatusType values and labels from the App Edit Site component.
    public readonly gisStatusOptions: InputOption[] = [
        { id: -1, text: 'Error' },
        { id: 0, text: 'Not Set' },
        { id: 1, text: 'Geocoded' }
    ];
}
