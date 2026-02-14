import { Component } from '@angular/core';
import { Site } from '../../shared/models/sites/site';
import { NgForm } from "@angular/forms";
import { State } from "../../shared/models/states/state";
import { LookupService } from "../../shared/services/lookup/lookup.service";
import { PropertyType } from "../../shared/enums/property-type.enum";
import { SiteService } from "../../shared/services/sites/site.service";
import { ModalReference } from "@developer-partners/ngx-modal-dialog";
import { HelperService } from "../../shared/services/helpers/helper.service";

@Component({
  selector: 'app-create-site-component',
  standalone: false,
  templateUrl: './create-site-component.html'
})
export class CreateSiteComponent {
    public isLoading: boolean = false;
    public validationErrors: string[] = [];
    public site: Site = {
        propertyType: PropertyType.Residential 
    };

    public states: State[] = [];

    public selectedPropertyType = {
        propertyType: PropertyType.Residential
    };

    public propertyTypes = [
        PropertyType.Residential,
        PropertyType.Commercial
    ];
    public PropertyType = PropertyType;
    constructor(
        private readonly _siteService: SiteService,
        private readonly _stateService: LookupService,
        private readonly _modalReference: ModalReference<Site>,
        private readonly _helper: HelperService
    ) {

    }

    public async ngOnInit(): Promise<void> {
        await this.loadStates();
    }


    public async save(form: NgForm): Promise<void> {
        if (form.valid) {
            try {
                this.isLoading = true;
                this.validationErrors = [];

                const result = await this._siteService.add(this.site);
                this._modalReference.closeSuccess(result);
            } catch (e) {
                if (!this._helper.parseValidationErrors(e, this.validationErrors)) {
                    throw e;
                }
            } finally {
                this.isLoading = false;
            }
        }

    }
    public cancel(): void { }

    private async loadStates(): Promise<void> {
        this.states = await this._stateService.getAllStates();
    }
}
