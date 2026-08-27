import { Component, EventEmitter, Input, Output } from '@angular/core';
import { SiteDetail } from '../../../shared/models/sites/site-detail';

/**
 * Water Supplier header section of the Edit Site view. Renders the water-supplier reference carried on
 * the loaded site and raises changeWaterSupplier when the lookup is clicked. Makes no API calls and
 * holds no persistence logic — reassignment is owned by the parent.
 */
@Component({
    selector: 'app-site-water-supplier-section',
    standalone: false,
    templateUrl: './site-water-supplier-section.component.html',
})
export class SiteWaterSupplierSectionComponent {
    @Input() site!: SiteDetail;

    @Output() changeWaterSupplier: EventEmitter<void> = new EventEmitter();
}
