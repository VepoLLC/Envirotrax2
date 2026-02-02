import { Component, OnInit } from "@angular/core";
import { Professional } from "../../shared/models/professionals/professional";
import { Router } from "@angular/router";
import { HelperService } from "../../shared/services/helpers/helper.service";
import { State } from "../../shared/models/states/state";
import { LookupService } from "../../shared/services/lookup/lookup.service";
import { ProfesisonalService } from "../../shared/services/professionals/professional.service";
import { NgForm } from "@angular/forms";
import { ProfessionalUser } from "../../shared/models/professionals/professional-user";
import { AuthService } from "../../shared/services/auth/auth.service";

@Component({
    standalone: false,
    templateUrl: './company.component.html'
})
export class CompanyComponent implements OnInit {
    public isLoading: boolean = false;
    public loadingMessage: string = '';
    public validationErrors: string[] = [];

    public professional: Professional = {};
    public user: ProfessionalUser = {};
    public states: State[] = [];

    constructor(
        private readonly _helper: HelperService,
        private readonly _lookupService: LookupService,
        private readonly _professionalService: ProfesisonalService,
        private readonly _router: Router,
        private readonly _authService: AuthService
    ) {

    }

    public async ngOnInit(): Promise<void> {
        try {
            this.isLoading = true;
            this.loadingMessage = 'Loading Profile';

            this.states = await this._lookupService.getAllStates();
        } finally {
            this.isLoading = false;
            this.loadingMessage = '';
        }
    }

    public stateChanged(stateId: number): void {
        if (stateId) {
            this.professional.state = this.professional.state || {};
            this.professional.state.id = stateId;
        } else {
            this.professional.state = undefined;
        }
    }

    public async save(form: NgForm): Promise<void> {
        if (form.valid) {
            try {
                this.isLoading = true;
                this.loadingMessage = 'Saving Profile';
                this.validationErrors = [];

                const addedProfessional = await this._professionalService.addMyData({
                    professional: this.professional,
                    user: this.user
                });

                await this._authService.signIn(undefined, addedProfessional.id)
            } catch (e) {
                if (!this._helper.parseValidationErrors(e, this.validationErrors)) {
                    throw e;
                }
            } finally {
                this.isLoading = false;
                this.loadingMessage = '';
            }
        }
    }
}