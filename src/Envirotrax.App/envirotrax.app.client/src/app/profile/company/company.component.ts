import { Component, OnInit } from "@angular/core";
import { Professional } from "../../shared/models/professionals/professional";
import { Router } from "@angular/router";
import { HelperService } from "../../shared/services/helpers/helper.service";
import { State } from "../../shared/models/lookup/state";
import { LookupService } from "../../shared/services/lookup/lookup.service";
import { ProfesisonalService } from "../../shared/services/professionals/professional.service";
import { NgForm } from "@angular/forms";
import { ProfessionalUser } from "../../shared/models/professionals/professional-user";
import { AuthService } from "../../shared/services/auth/auth.service";
import { ProfesionalUserService } from "../../shared/services/professionals/professional-user.service";
import { InputOption } from "../../shared/components/input/input.component";
import { HttpErrorResponse } from "@angular/common/http";
import { ROLE_DEFINITIONS } from "../../shared/models/role-definitions";

@Component({
    standalone: false,
    templateUrl: './company.component.html'
})
export class CompanyComponent implements OnInit {
    public isLoading: boolean = false;
    public loadingMessage: string = '';
    public validationErrors: string[] = [];
    public isAdmin: boolean = false;

    public professional: Professional = {};
    public user: ProfessionalUser = {};
    public states: InputOption<State>[] = [];

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

            const [states, professional, user, isAdmin] = await Promise.all([
                this._lookupService.getAllStatesAsOptions(true),
                this.getLoggedInProfessional(),
                this.getMyData(),
                this._authService.hasAnyRoles(ROLE_DEFINITIONS.PROFESSIONALS.ADMIN)
            ]);

            this.states = states;
            this.professional = professional;
            this.user = user;
            this.isAdmin = isAdmin;
        } finally {
            this.isLoading = false;
            this.loadingMessage = '';
        }
    }

    private async getLoggedInProfessional(): Promise<Professional> {
        try {
            return await this._professionalService.getLoggedInProfessional();
        } catch (e) {
            if (this._helper.isNotFoundError(e)) {
                return {};
            }

            throw e;
        }
    }

    private async getMyData(): Promise<ProfessionalUser> {
        try {
            return await this._professionalUserService.getMyData();
        } catch (e) {
            if (this._helper.isNotFoundError(e)) {
                return {
                    emailAddress: await this._authService.getUserEmail()
                };
            }

            throw e;
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

                if (this.isAdmin) {
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

                        await this._authService.signIn(undefined, updatedProfessional.id);
                    }
                } else {
                    await this._professionalUserService.updateMyData(this.user);
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