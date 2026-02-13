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
import { ProfesionalUserService } from "../../shared/services/professionals/professional-user.service";

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
        private readonly _professionalUserService: ProfesionalUserService,
        private readonly _authService: AuthService
    ) {

    }

    public async ngOnInit(): Promise<void> {
        try {
            this.isLoading = true;
            this.loadingMessage = 'Loading Profile';

            const [states, professional, user] = await Promise.all([
                this._lookupService.getAllStates(),
                this._professionalService.getLoggedInProfessional(),
                this._professionalUserService.getMyData()
            ]);

            this.states = states;
            this.professional = professional || {};
            this.user = user || {};
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

    private validateServices(): boolean {
        if (!this.professional.hasWiseGuys &&
            !this.professional.hasBackflowTesting &&
            !this.professional.hasCsiInspection &&
            !this.professional.hasFogInspection &&
            !this.professional.hasFogTransportation) {

            this.validationErrors.push('Please select at least one service.');
            return false;
        }

        return true;
    }

    public async save(form: NgForm): Promise<void> {
        if (form.valid) {
            try {
                this.isLoading = true;
                this.loadingMessage = 'Saving Profile';
                this.validationErrors = [];

                if (this.validateServices()) {
                    const updatedProfessional = this.professional.id
                        ? await this._professionalService.updateMyData(this.professional)
                        : await this._professionalService.addMyData({
                            professional: this.professional,
                            user: this.user
                        });

                    if (this.professional.id) {
                        await this._professionalUserService.updateMyData(this.user);
                    }

                    await this._authService.signIn(undefined, updatedProfessional.id)
                }
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