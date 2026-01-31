import { Component, OnInit } from "@angular/core";
import { Professional } from "../../shared/models/professionals/professional";
import { ActivatedRoute, Router } from "@angular/router";
import { HelperService } from "../../shared/services/helpers/helper.service";
import { State } from "../../shared/models/states/state";
import { LookupService } from "../../shared/services/lookup/lookup.service";
import { ProfesisonalService } from "../../shared/services/professionals/professional.service";
import { HttpErrorResponse } from "@angular/common/http";
import { NgForm } from "@angular/forms";

@Component({
    standalone: false,
    templateUrl: './company.component.html'
})
export class CompanyComponent implements OnInit {
    public isLoading: boolean = false;
    public loadingMessage: string = '';
    public validationErrors: string[] = [];
    public professional: Professional = {};
    public states: State[] = [];

    constructor(
        private readonly _acitvatedRoute: ActivatedRoute,
        private readonly _helper: HelperService,
        private readonly _lookupService: LookupService,
        private readonly _professionalService: ProfesisonalService,
        private readonly _router: Router
    ) {

    }

    private async getLoggedInProfessional(): Promise<Professional> {
        try {
            return await this._professionalService.getLoggedInProfessional();
        } catch (e) {
            // If profile is not filled out, getting logged in professional will return Not Found.
            if (e instanceof HttpErrorResponse) {
                if (e.status != 404) {
                    throw e;
                }
            }
        }

        return {};
    }

    public async ngOnInit(): Promise<void> {
        try {
            this.isLoading = true;
            this.loadingMessage = 'Loading Company Information';

            [this.states, this.professional] = await Promise.all([
                this._lookupService.getAllStates(),
                this.getLoggedInProfessional()
            ]);

            if (this.professional) {
                await this.navigateToUserInfo();
            }

            this.professional = {};
        } finally {
            this.isLoading = false;
            this.loadingMessage = '';
        }
    }

    private async navigateToUserInfo(): Promise<void> {
        await this._router.navigate(['user'], {
            relativeTo: this._acitvatedRoute
        });
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
                this.loadingMessage = 'Saving Company Information';

                this.navigateToUserInfo();
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