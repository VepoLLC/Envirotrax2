import { Component, Input } from '@angular/core';
import { SiteDetail } from '../../../shared/models/sites/site-detail';

/**
 * Read-only Water Supplier header section of the Edit Site view. Renders the water-supplier reference
 * carried on the loaded site. Has no editable controls, makes no API calls, and holds no persistence
 * logic — it only displays the parent-provided site model.
 */
@Component({
    selector: 'app-site-water-supplier-section',
    standalone: false,
    templateUrl: './site-water-supplier-section.component.html',
})
export class SiteWaterSupplierSectionComponent {
    @Input() site!: SiteDetail;
}
