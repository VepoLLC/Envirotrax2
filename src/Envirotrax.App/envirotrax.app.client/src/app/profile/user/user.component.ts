import { Component, OnInit } from "@angular/core";
import { ProfessionalUser } from "../../shared/models/professionals/professional-user";
import { HelperService } from "../../shared/services/helpers/helper.service";
import { ProfesionalUserService } from "../../shared/services/professionals/professional-user.service";
import { HttpErrorResponse } from "@angular/common/http";
import { NgForm } from "@angular/forms";
import { ActivatedRoute, Router } from "@angular/router";
import { ProfesisonalService } from "../../shared/services/professionals/professional.service";
import { Professional } from "../../shared/models/professionals/professional";

@Component({
    standalone: false,
    templateUrl: './user.component.html'
})
export class UserComponent implements OnInit {
    private _professional?: Professional = { name: 'Test' };

    public isLoading: boolean = false;
    public loadingMessage: string = '';
    public validationErrors: string[] = [];
    public user: ProfessionalUser = {};

    constructor(
        private readonly _activatedRoute: ActivatedRoute,
        private readonly _helper: HelperService,
        private readonly _userService: ProfesionalUserService,
        private readonly _professionalService: ProfesisonalService,
        private readonly _router: Router
    ) {
    }

    private async getLoggedInUser(): Promise<ProfessionalUser> {
        try {
            return await this._userService.getMyData();
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

    private async getLoggedInProfessional(): Promise<Professional | undefined> {
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

        return undefined;
    }

    public async ngOnInit(): Promise<void> {
        try {
            this.isLoading = true;
            this.loadingMessage = 'Loading User Information';

            this._professional = await this.getLoggedInProfessional();

            if (!this._professional) {
                // If company information is not set yet, go to that page first.
                this._router.navigate(['../company'], {
                    relativeTo: this._activatedRoute,
                    replaceUrl: true
                });
                return;
            }

            this.user = await this.getLoggedInUser();

            if (this.user.contactName && this.user.jobTitle) {
                this._router.navigateByUrl('/', {
                    replaceUrl: true
                });
            }
        } finally {
            this.isLoading = false;
            this.loadingMessage = '';
        }
    }

    public async save(form: NgForm): Promise<void> {
        if (form.valid) {
            try {
                this.isLoading = true;
                this.loadingMessage = 'Saving User Information';
                this.validationErrors = [];

                await this._userService.updateMyData(this.user);
                this.user = await this._userService.getMyData();

                this._router.navigateByUrl('/', {
                    replaceUrl: true
                });
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