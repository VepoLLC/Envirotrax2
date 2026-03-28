import { Component, OnInit } from '@angular/core';
import { Site } from '../../shared/models/sites/site';
import { NgForm } from "@angular/forms";
import { State } from "../../shared/models/lookup/state";
import { PageInfo } from "../../shared/models/page-info";
import { LookupService } from "../../shared/services/lookup/lookup.service";
import { PropertyType } from "../../shared/enums/property-type.enum";
import { SiteService } from "../../shared/services/sites/site.service";
import { HelperService } from "../../shared/services/helpers/helper.service";
import { ActivatedRoute, Router } from "@angular/router";
import { UserService } from "../../shared/services/water-suppliers/user.service";
import { FacilityType } from '../../shared/enums/facility-type.enum';
import { InputOption } from "../../shared/components/input/input.component";
import { GreaseTrapType } from '../../shared/enums/grease-trap-type.enum';
import { ToastService, ToastType } from '../../shared/services/toast.service';

@Component({
    selector: 'app-edit-site-component',
    standalone: false,
    templateUrl: './edit-site-component.html'
})
export class EditSiteComponent implements OnInit {
    public validationErrors: string[] = [];

    public site: Site = {
        backflowScheduleMonth: 0,
        greaseTrapType: 0,
        facilityType: FacilityType.Other
    };

    public currentSite?: Site = {};

    public users: any[] = [];
    public greaseTrapOptions: any[] = [];

    public csiUsers: InputOption[] = [];
    public backflowUsers: InputOption[] = [];
    public fogUsers: InputOption[] = [];
    public stateOptions: InputOption[] = [];

    public sectionLoading = {
        siteSettings: false,
        facilityType: false,
        location: false,
        mailing: false,
    };

    constructor(
        private readonly _siteService: SiteService,
        private readonly _stateService: LookupService,
        private readonly _acitvatedRoute: ActivatedRoute,
        private readonly _router: Router,
        private readonly _helper: HelperService,
        private readonly _userService: UserService,
        private readonly _toastService: ToastService
    ) {
    }

    public async ngOnInit(): Promise<void> {
        await this.loadStates();
        await this.getUsers();
        this._acitvatedRoute.paramMap.subscribe(async params => {
            const siteId = params.get('id');
            if (siteId) {
                await this.getSite(+siteId);
                this.currentSite = { ...this.site };
            }
        });
    }

    public propertyTypeOptions: InputOption[] = [
        { id: PropertyType.Residential, text: 'Residential' },
        { id: PropertyType.Commercial, text: 'Commercial' }
    ];


    public facilityTypes: InputOption[] = [
        { id: FacilityType.Restaurant, text: "Restaurant" },
        { id: FacilityType.FastFoodEstablishment, text: "Fast Food Establishment" },
        { id: FacilityType.HotelMotel, text: "Hotel/Motel" },
        { id: FacilityType.CarWash, text: "Car Wash" },
        { id: FacilityType.SchoolUniversity, text: "School/University" },
        { id: FacilityType.GroceryStore, text: "Grocery Store" },
        { id: FacilityType.ConvenienceStore, text: "Convenience Store" },
        { id: FacilityType.AssistedLivingFacility, text: "Assisted Living Facility" },
        { id: FacilityType.MedicalFacility, text: "Medical Facility" },
        { id: FacilityType.Industrial, text: "Industrial" },
        { id: FacilityType.CityOwnedFacility, text: "City Owned Facility" },
        { id: FacilityType.Other, text: "Other" },
    ];

    public mailingStateChanged(stateId: number): void {
        this.site.mailingStateId = stateId;
    }

    public copyFromPropertyAddress(): void {
        this.site.mailingStreetNumber = this.site.streetNumber;
        this.site.mailingStreetName = this.site.streetName;
        this.site.propertyNumber = this.site.propertyNumber;
        this.site.mailingCity = this.site.city;
        this.site.mailingStateId = this.site.stateId;
        this.site.mailingZipCode = this.site.zipCode;
        this.site.mailingPhoneNumber = this.site.fogGeneratorPhoneNumber;
        this.site.mailingEmailAddress = this.site.fogGeneratorEmailAddress;
        this.site.mailingNumber = this.site.propertyNumber;
    }

    public greaseTrapTypes: InputOption[] = [
        { id: GreaseTrapType.TrapNotRequired, text: "Trap Not Required" },
        { id: GreaseTrapType.HasGreaseTrap, text: "Has Grease Trap" },
        { id: GreaseTrapType.ShouldHaveGreaseTrap, text: "Should Have Grease Trap" },
        { id: GreaseTrapType.MightHaveGreaseTrap, text: "Might Have Grease Trap" },
    ];

    public backflowScheduleMonths: InputOption[] = [
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

    async getUsers() {
        const pageInfo: PageInfo = {
            pageNumber: 1,
            pageSize: 999999
        };
        const query: any = {};
        const result = await this._userService.getAll(pageInfo, query);
        this.users = result.data;

        this.csiUsers = [
            { id: null, text: 'Unassigned' },
            ...this.users.map(user => ({ id: user.id, text: user.emailAddress }))
        ];

        this.backflowUsers = [
            { id: null, text: 'Unassigned' },
            ...this.users.map(user => ({ id: user.id, text: user.emailAddress }))
        ];

        this.fogUsers = [
            { id: null, text: 'Unassigned' },
            ...this.users.map(user => ({ id: user.id, text: user.emailAddress }))
        ];
    }

    public async updateFacilityType(form: NgForm) {
        if (form.valid) {
            try {
                this.sectionLoading.facilityType = true;
                this.validationErrors = [];

                console.log(this.site.facilityType);

                if (this.currentSite) {
                    this.currentSite.facilityType = this.site.facilityType;
                    this.currentSite.greaseTrapType = this.site.greaseTrapType;
                    this.currentSite.hasKnownBackflowAssemblies = this.site.hasKnownBackflowAssemblies;
                    this.currentSite.hasOnSiteSewageFacility = this.site.hasOnSiteSewageFacility;
                    this.currentSite.hasAuxWaterSupply = this.site.hasAuxWaterSupply;
                    this.currentSite.hasFireSystem = this.site.hasFireSystem;
                    this.currentSite.fireSeparateWater = this.site.fireSeparateWater;
                    this.currentSite.hasGritTrap = this.site.hasGritTrap;
                    this.currentSite.hasIrrigation = this.site.hasIrrigation;
                    this.currentSite.irrigationSeparateWater = this.site.irrigationSeparateWater;
                    this.currentSite.hasDomesticPremisesIsolation = this.site.hasDomesticPremisesIsolation;
                    this.currentSite.requiresDomesticPremisesIsolation = this.site.requiresDomesticPremisesIsolation;

                    const result = await this._siteService.update(this.currentSite);

                    this._toastService.successfullySaved('Facility Type');
                }
            } catch (e) {
                if (!this._helper.parseValidationErrors(e, this.validationErrors)) {
                    throw e;
                }

                this._toastService.failedToSave('Facility Type');
            } finally {
                this.sectionLoading.facilityType = false;
            }
        }
    }

    public async updateSiteSettings(form: NgForm): Promise<void> {
        if (form.valid) {
            try {
                this.sectionLoading.siteSettings = true;
                this.validationErrors = [];

                if (this.currentSite) {
                    this.currentSite.id = this.site.id;
                    this.currentSite.backflowScheduleMonth = this.site.backflowScheduleMonth;
                    this.currentSite.lastTripTicketDate = this.site.lastTripTicketDate;
                    this.currentSite.tripTicketInterval = this.site.tripTicketInterval;
                    this.currentSite.active = this.site.active;
                    this.currentSite.invalidMailingAddress = this.site.invalidMailingAddress;
                    this.currentSite.outOfArea = this.site.outOfArea;
                    this.currentSite.isFeeExempt = this.site.isFeeExempt;
                    this.currentSite.needsCsiInspection = this.site.needsCsiInspection;
                    this.currentSite.needsFogInspection = this.site.needsFogInspection;
                    this.currentSite.needsFogPermit = this.site.needsFogPermit;
                    this.currentSite.csiAccountAssignmentId = this.site.csiAccountAssignmentId;
                    this.currentSite.backflowAccountAssignmentId = this.site.backflowAccountAssignmentId;
                    this.currentSite.fogAccountAssignmentId = this.site.fogAccountAssignmentId;

                    const result = await this._siteService.update(this.currentSite);

                    this._toastService.successfullySaved('Site Settings');
                }
            } catch (e) {
                if (!this._helper.parseValidationErrors(e, this.validationErrors)) {
                    throw e;
                }

                this._toastService.failedToSave('Site Settings');
            } finally {
                this.sectionLoading.siteSettings = false;
            }
        }
    }

    public async updateLocation(form: NgForm): Promise<void> {
        if (form.valid) {
            try {
                this.sectionLoading.location = true;
                this.validationErrors = [];
                if (this.currentSite) {
                    this.currentSite.id = this.site.id;
                    this.currentSite.accountNumber = this.site.accountNumber;
                    this.currentSite.propertyType = this.site.propertyType;
                    this.currentSite.businessName = this.site.businessName;
                    this.currentSite.streetNumber = this.site.streetNumber;
                    this.currentSite.streetName = this.site.streetName;
                    this.currentSite.propertyNumber = this.site.propertyNumber;
                    this.currentSite.city = this.site.city;
                    this.currentSite.stateId = this.site.stateId;
                    this.currentSite.zipCode = this.site.zipCode;
                    this.currentSite.fogGeneratorPhoneNumber = this.site.fogGeneratorPhoneNumber;
                    this.currentSite.fogGeneratorEmailAddress = this.site.fogGeneratorEmailAddress;
                    const result = await this._siteService.update(this.currentSite);

                    this._toastService.successfullySaved('Location');
                }
            } catch (e) {
                if (!this._helper.parseValidationErrors(e, this.validationErrors)) {
                    throw e;
                }

                this._toastService.failedToSave('Location');
            } finally {
                this.sectionLoading.location = false;
            }
        }
    }


    public async updateMailingInformation(form: NgForm): Promise<void> {
        if (form.valid) {
            try {
                this.sectionLoading.mailing = true;

                this.validationErrors = [];
                if (this.currentSite) {
                    this.currentSite.id = this.site.id;
                    this.currentSite.mailingCompanyName = this.site.mailingCompanyName;
                    this.currentSite.mailingContactName = this.site.mailingContactName;
                    this.currentSite.mailingStreetNumber = this.site.mailingStreetNumber;
                    this.currentSite.mailingStreetName = this.site.mailingStreetName;
                    this.currentSite.propertyNumber = this.site.propertyNumber;
                    this.currentSite.mailingCity = this.site.mailingCity;
                    this.currentSite.mailingStateId = this.site.mailingStateId;
                    this.currentSite.mailingZipCode = this.site.mailingZipCode;
                    this.currentSite.mailingPhoneNumber = this.site.mailingPhoneNumber;
                    this.currentSite.mailingEmailAddress = this.site.mailingEmailAddress;
                    const result = await this._siteService.update(this.currentSite);

                    this._toastService.successfullySaved('Mailing Information');
                }
            } catch (e) {
                if (!this._helper.parseValidationErrors(e, this.validationErrors)) {
                    throw e;
                }

                this._toastService.failedToSave('Mailing Information');
            } finally {
                this.sectionLoading.mailing = false;
            }
        }
    }

    private async getSite(id: number): Promise<void> {
        try {

            this.sectionLoading.facilityType = true;
            this.sectionLoading.location = true;
            this.sectionLoading.mailing = true;
            this.sectionLoading.siteSettings = true;

            const apiSite = await this._siteService.get(id);

            this.site = {
                ...apiSite,
            };

        } finally {
            this.sectionLoading.facilityType = false;
            this.sectionLoading.location = false;
            this.sectionLoading.mailing = false;
            this.sectionLoading.siteSettings = false;
        }
    }

    private async loadStates(): Promise<void> {
        this.stateOptions = await this._stateService.getAllStatesAsOptions(true);
    }
}