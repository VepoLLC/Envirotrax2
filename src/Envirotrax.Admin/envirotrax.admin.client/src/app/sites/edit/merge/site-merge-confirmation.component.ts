import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { ModalReference } from '@developer-partners/ngx-modal-dialog';
import { HelperService } from '@envirotrax/common-ui';
import { PropertyType } from '../../../shared/models/sites/site';
import { SiteDetail } from '../../../shared/models/sites/site-detail';

export interface SiteMergeConfirmationModel {
    sourceSite: SiteDetail;
    destinationSite: SiteDetail;
}

interface SiteMergeSummary {
    id?: number;
    propertyType: string;
    businessName?: string;
    address: string;
    cityStateZip: string;
}

@Component({
    templateUrl: './site-merge-confirmation.component.html',
    imports: [CommonModule]
})
export class SiteMergeConfirmationComponent {
    public readonly sourceSite: SiteMergeSummary;
    public readonly destinationSite: SiteMergeSummary;

    constructor(
        private readonly _modalReference: ModalReference<SiteMergeConfirmationModel, void>,
        private readonly _helperService: HelperService
    ) {
        const { sourceSite, destinationSite } = this._modalReference.config.model!;

        this.sourceSite = this.buildSummary(sourceSite);
        this.destinationSite = this.buildSummary(destinationSite);
    }

    public confirm(): void {
        this._modalReference.closeSuccess();
    }

    public cancel(): void {
        this._modalReference.cancel();
    }

    private buildSummary(site: SiteDetail): SiteMergeSummary {
        let propertyType = 'Residential';

        if (site.propertyType === PropertyType.Commercial) {
            propertyType = 'Commercial';
        }

        return {
            id: site.id,
            propertyType,
            businessName: site.businessName,
            address: this._helperService.buildStreetAddress(site.streetNumber, site.streetName, site.propertyNumber),
            cityStateZip: this._helperService.buildCityStateZip(site.city, site.state?.code, site.zipCode)
        };
    }
}
