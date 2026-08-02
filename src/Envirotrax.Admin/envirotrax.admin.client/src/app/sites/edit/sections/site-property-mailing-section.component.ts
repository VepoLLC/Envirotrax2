import { Component, Input, Optional, SkipSelf } from '@angular/core';
import { ControlContainer, NgForm } from '@angular/forms';
import { InputOption } from '@envirotrax/common-ui';
import { PropertyType } from '../../../shared/models/sites/site';
import { SiteDetail } from '../../../shared/models/sites/site-detail';

@Component({
    selector: 'app-site-property-mailing-section',
    standalone: false,
    templateUrl: './site-property-mailing-section.component.html',
    viewProviders: [
        {
            provide: ControlContainer,
            useFactory: (container: ControlContainer) => container,
            deps: [[new SkipSelf(), new Optional(), ControlContainer]]
        }
    ]
})
export class SitePropertyMailingSectionComponent {
    @Input() site!: SiteDetail;

    @Input() form!: NgForm;

    @Input() stateOptions: InputOption[] = [];

    public readonly propertyTypeOptions: InputOption[] = [
        { id: PropertyType.Residential, text: 'Residential' },
        { id: PropertyType.Commercial, text: 'Commercial' }
    ];

    // Copies the property address into the mailing address (Vepo: imgCopyPropertyToMailing).
    // Business Name → Company Name only for Commercial properties, matching the legacy rule.
    // Client-side only — no API call. Contact name / phone / email are intentionally not copied.
    public copyPropertyToMailing(): void {
        this.site.mailingStreetNumber = this.site.streetNumber;
        this.site.mailingStreetName = this.site.streetName;
        this.site.mailingNumber = this.site.propertyNumber;
        this.site.mailingCity = this.site.city;
        this.site.mailingState = this.site.state ? { ...this.site.state } : undefined;
        this.site.mailingZipCode = this.site.zipCode;

        if (this.site.propertyType === PropertyType.Commercial) {
            this.site.mailingCompanyName = this.site.businessName;
        }
    }

    // Copies the mailing address into the property address (Vepo: imgCopyMailingToProperty).
    // Company Name → Business Name only for Commercial properties, matching the legacy rule.
    // Client-side only — no API call. Contact name / phone / email are intentionally not copied.
    public copyMailingToProperty(): void {
        this.site.streetNumber = this.site.mailingStreetNumber;
        this.site.streetName = this.site.mailingStreetName;
        this.site.propertyNumber = this.site.mailingNumber;
        this.site.city = this.site.mailingCity;
        this.site.state = this.site.mailingState ? { ...this.site.mailingState } : undefined;
        this.site.zipCode = this.site.mailingZipCode;

        if (this.site.propertyType === PropertyType.Commercial) {
            this.site.businessName = this.site.mailingCompanyName;
        }
    }
}
