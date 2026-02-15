import { Component, OnInit } from '@angular/core';
import { Site } from '../../shared/models/sites/site';
import { NgForm } from "@angular/forms";
import { State } from "../../shared/models/states/state";
import { LookupService } from "../../shared/services/lookup/lookup.service";
import { PropertyType } from "../../shared/enums/property-type.enum";
import { SiteService } from "../../shared/services/sites/site.service";
import { HelperService } from "../../shared/services/helpers/helper.service";
import { ActivatedRoute, Router } from "@angular/router";

@Component({
  selector: 'app-edit-site-component',
  standalone: false,
  templateUrl: './edit-site-component.html'
})
export class EditSiteComponent implements OnInit {

    public isLoading: boolean = false;
    public validationErrors: string[] = [];
    public site: Site = {};

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
        private readonly _acitvatedRoute: ActivatedRoute,
        private readonly _router: Router,
        private readonly _helper: HelperService
    ) {
    }

    public async ngOnInit(): Promise<void> {
        await this.loadStates();
        this._acitvatedRoute.paramMap.subscribe(async params => {
            const siteId = params.get('id');
            if (siteId) {
                await this.getUser(+siteId);
            }
        });
    }


    public async save(form: NgForm): Promise<void> {
        if (form.valid) {
            try {
                this.isLoading = true;
                this.validationErrors = [];

                const result = await this._siteService.update(this.site);

                this._router.navigateByUrl('sites');

            } catch (e) {
                if (!this._helper.parseValidationErrors(e, this.validationErrors)) {
                    throw e;
                }
            } finally {
                this.isLoading = false;
            }
        }

    }


    public cancel(): void {
        this._router.navigateByUrl('sites');
    }


    private async getUser(id: number): Promise<void> {
        try {
            this.isLoading = true;
            const apiSite = await this._siteService.get(id);

            this.site = {
                ...apiSite,
            };

        } finally {
            this.isLoading = false;
        }
    }

    private async loadStates(): Promise<void> {
        this.states = await this._stateService.getAllStates();
    }

}
