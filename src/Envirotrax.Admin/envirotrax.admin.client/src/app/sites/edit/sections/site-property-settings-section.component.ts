import { Component, Input, Optional, SkipSelf } from '@angular/core';
import { ControlContainer, NgForm } from '@angular/forms';
import { InputOption } from '@envirotrax/common-ui';
import { FacilityType, GreaseTrapType } from '../../../shared/models/sites/site';
import { SiteDetail } from '../../../shared/models/sites/site-detail';

/**
 * Property Settings section of the Edit Site view. Renders and binds the property-settings fields on
 * the parent-provided editable site model.
 *
 * Its ngModel controls register into the parent SiteEditComponent's single NgForm via the
 * ControlContainer viewProviders pattern (same approach as the App's gis-area-lookup component), so
 * the parent owns whole-page validity and dirty state. This component makes no API calls and holds no
 * persistence logic — it only renders the section and its local UI behavior (the conditional CSI/FOG
 * date fields).
 */
@Component({
    selector: 'app-site-property-settings-section',
    standalone: false,
    templateUrl: './site-property-settings-section.component.html',
    viewProviders: [
        {
            provide: ControlContainer,
            useFactory: (container: ControlContainer) => container,
            deps: [[new SkipSelf(), new Optional(), ControlContainer]]
        }
    ]
})
export class SitePropertySettingsSectionComponent {
    @Input() site!: SiteDetail;

    // The parent SiteEditComponent's NgForm — used only so vp-input can display validation state.
    // Control registration into the form happens through the ControlContainer viewProviders above.
    @Input() form!: NgForm;

    // 0 = Not Applicable (Site.BackflowScheduleMonth default / legacy index 0), then Jan–Dec.
    public readonly backflowScheduleMonths: InputOption[] = [
        { id: 0, text: 'Not Applicable' },
        { id: 1, text: 'January' },
        { id: 2, text: 'February' },
        { id: 3, text: 'March' },
        { id: 4, text: 'April' },
        { id: 5, text: 'May' },
        { id: 6, text: 'June' },
        { id: 7, text: 'July' },
        { id: 8, text: 'August' },
        { id: 9, text: 'September' },
        { id: 10, text: 'October' },
        { id: 11, text: 'November' },
        { id: 12, text: 'December' }
    ];

    // Reuses the exact FacilityType enum values and labels from the App Edit Site component.
    public readonly facilityTypes: InputOption[] = [
        { id: FacilityType.Restaurant, text: 'Restaurant' },
        { id: FacilityType.FastFoodEstablishment, text: 'Fast Food Establishment' },
        { id: FacilityType.HotelMotel, text: 'Hotel/Motel' },
        { id: FacilityType.CarWash, text: 'Car Wash' },
        { id: FacilityType.SchoolUniversity, text: 'School/University' },
        { id: FacilityType.GroceryStore, text: 'Grocery Store' },
        { id: FacilityType.ConvenienceStore, text: 'Convenience Store' },
        { id: FacilityType.AssistedLivingFacility, text: 'Assisted Living Facility' },
        { id: FacilityType.MedicalFacility, text: 'Medical Facility' },
        { id: FacilityType.Industrial, text: 'Industrial' },
        { id: FacilityType.CityOwnedFacility, text: 'City Owned Facility' },
        { id: FacilityType.Other, text: 'Other' }
    ];

    // Reuses the exact GreaseTrapType enum values and labels from the App Edit Site component.
    public readonly greaseTrapTypes: InputOption[] = [
        { id: GreaseTrapType.TrapNotRequired, text: 'Trap Not Required' },
        { id: GreaseTrapType.HasGreaseTrap, text: 'Has Grease Trap' },
        { id: GreaseTrapType.ShouldHaveGreaseTrap, text: 'Should Have Grease Trap' },
        { id: GreaseTrapType.MightHaveGreaseTrap, text: 'Might Have Grease Trap' }
    ];
}
