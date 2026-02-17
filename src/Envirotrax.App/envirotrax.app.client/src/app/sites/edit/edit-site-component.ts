import { Component, OnInit } from '@angular/core';
import { Site } from '../../shared/models/sites/site';
import { NgForm } from "@angular/forms";
import { State } from "../../shared/models/states/state";
import { PageInfo } from "../../shared/models/page-info";
import { LookupService } from "../../shared/services/lookup/lookup.service";
import { PropertyType } from "../../shared/enums/property-type.enum";
import { SiteService } from "../../shared/services/sites/site.service";
import { HelperService } from "../../shared/services/helpers/helper.service";
import { ActivatedRoute, Router } from "@angular/router";
import { UserService } from "../../shared/services/water-suppliers/user.service";

@Component({
    selector: 'app-edit-site-component',
    standalone: false,
    templateUrl: './edit-site-component.html'
})
export class EditSiteComponent implements OnInit {

    public isLoading: boolean = false;
    public validationErrors: string[] = [];
    public site: Site = {
        hasGreaseTrap: 0,   
        facilityType: 'Other'  
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


    monthsnumbers = Array.from({ length: 12 }, (_, i) => i + 1);

    days = Array.from({ length: 31 }, (_, i) => i + 1);

    years = Array.from(
        { length: 50 },
        (_, i) => new Date().getFullYear() - i
    );


    months: any[] = [];
    users: any[] = [];
    facilityTypes: any[] = [];
    greaseTrapOptions: any[] = [];

    constructor(
        private readonly _siteService: SiteService,
        private readonly _stateService: LookupService,
        private readonly _acitvatedRoute: ActivatedRoute,
        private readonly _router: Router,
        private readonly _helper: HelperService,
        private readonly _userService: UserService
    ) {
    }

    public async ngOnInit(): Promise<void> {
        await this.loadStates();
        await this.getUsers();
        this.loadDropdowns();
        this._acitvatedRoute.paramMap.subscribe(async params => {
            const siteId = params.get('id');
            if (siteId) {
                await this.getSite(+siteId);
                console.log(this.site);
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

    public stateChanged(stateId: number): void {
        if (stateId) {
            //  this.professional.state = this.professional.state || {};
            this.site.stateId = stateId;
        } else {
            this.site.stateId = undefined;
        }
    }


    private async getSite(id: number): Promise<void> {
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


    loadDropdowns(): void {

        this.months = [
            { value: 1, text: 'January' },
            { value: 2, text: 'February' },
            { value: 3, text: 'March' },
            { value: 4, text: 'April' },
            { value: 5, text: 'May' },
            { value: 6, text: 'June' },
            { value: 7, text: 'July' },
            { value: 8, text: 'August' },
            { value: 9, text: 'September' },
            { value: 10, text: 'October' },
            { value: 11, text: 'November' },
            { value: 12, text: 'December' }
        ];

          this.facilityTypes = [
                { id: 'Restaurant', name: 'Restaurant' },
                { id: 'Fast Food Establishment', name: 'Fast Food Establishment' },
                { id: 'Hotel/Motel', name: 'Hotel/Motel' },
                { id: 'Car Wash', name: 'Car Wash' },
                { id: 'School/University', name: 'School/University' },
                { id: 'Grocery Store', name: 'Grocery Store' },
                { id: 'Convenience Store', name: 'Convenience Store' },
                { id: 'Assisted Living Facility', name: 'Assisted Living Facility' },
                { id: 'Medical Facility', name: 'Medical Facility' },
                { id: 'Industrial', name: 'Industrial' },
                { id: 'City Owned Facility', name: 'City Owned Facility' },
                { id: 'Other', name: 'Other' }
            ];
       
        this.greaseTrapOptions = [
            { id: 0, name: 'Trap Not Required' },
            { id: 1, name: 'Has Grease Trap' },
            { id: 2, name: 'Should Have Grease Trap' },
            { id: 3, name: 'Might Have Grease Trap' }
        ];

    }

    updateSiteSettings(form: NgForm): void {

        if (!form.valid) return;

        this.isLoading = true;

        console.log('Updating Site Settings', this.site);

        // TODO API CALL

        setTimeout(() => {
            this.isLoading = false;
        }, 500);
    }

    updateFacilityType(form: NgForm): void {

        if (!form.valid) return;

        this.isLoading = true;

        console.log('Updating Facility Type', this.site);

        // TODO API CALL

        setTimeout(() => {
            this.isLoading = false;
        }, 500);
    }

    async getUsers() {
        const pageInfo: PageInfo = {
            pageNumber: 1,
            pageSize: 999999
        };

        const query: any = {};
        const result = await this._userService.getAll(pageInfo, query);
        this.users = result.data;
        console.log(this.users);
    }


    // updateFacilityType(form: NgForm): Promise<void> {

    //    if (!form.valid) return;

    //    try {
    //        this.isLoading = true;

    //        const payload = {
    //            id: this.site.id,
    //            facilityType: this.site.facilityType,
    //            hasGreaseTrap: this.site.hasGreaseTrap,
    //            hasKnownBackflowAssemblies: this.site.hasKnownBackflowAssemblies,
    //            hasOnSiteSewageFacility: this.site.hasOnSiteSewageFacility,
    //            hasAuxWaterSupply: this.site.hasAuxWaterSupply,
    //            hasFireSystem: this.site.hasFireSystem,
    //            fireSeparateWater: this.site.fireSeparateWater,
    //            hasGritTrap: this.site.hasGritTrap,
    //            hasIrrigation: this.site.hasIrrigation,
    //            irrigationSeparateWater: this.site.irrigationSeparateWater,
    //            hasDomesticPremisesIsolation: this.site.hasDomesticPremisesIsolation,
    //            requiresDomesticPremisesIsolation: this.site.requiresDomesticPremisesIsolation
    //        };

    //        await this._siteService.updateFacility(payload);

    //    } finally {
    //        this.isLoading = false;
    //    }
    //}


    //async updateSiteSettings(form: NgForm): Promise<void> {

    //    if (!form.valid) return;

    //    try {
    //        this.isLoading = true;

    //        const payload = {
    //            id: this.site.id,
    //            backflowScheduleMonth: this.site.backflowScheduleMonth,
    //            lastTripTicketDate: this.site.lastTripTicketDate,
    //            tripTicketInterval: this.site.tripTicketInterval,
    //            active: this.site.active,
    //            invalidMailingAddress: this.site.invalidMailingAddress,
    //            outOfArea: this.site.outOfArea,
    //            isFeeExempt: this.site.isFeeExempt,
    //            needsCsiInspection: this.site.needsCsiInspection,
    //            needsFogInspection: this.site.needsFogInspection,
    //            needsFogPermit: this.site.needsFogPermit,
    //            csiAccountAssignment: this.site.csiAccountAssignment,
    //            backflowAccountAssignment: this.site.backflowAccountAssignment,
    //            fogAccountAssignment: this.site.fogAccountAssignment
    //        };

    //        await this._siteService.updateSettings(payload);

    //    } finally {
    //        this.isLoading = false;
    //    }
    //}
}
