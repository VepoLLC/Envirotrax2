import { Component } from "@angular/core";
import { ActivatedRoute } from "@angular/router";
import { RenewalEmailLinkComponent } from "./renewal-email-link.component";
import { RenewalOptInService } from "../../shared/services/sites/renewal-opt-in.service";
import { RenewalOptInResult, RenewalOptInTokenRequest } from "../../shared/models/sites/renewal-opt-in";

@Component({
    standalone: false,
    templateUrl: './renewal-email-link.component.html'
})
export class RenewalVerifyEmailComponent extends RenewalEmailLinkComponent {
    public readonly title: string = 'Email Verification';

    constructor(
        private readonly _renewalOptInService: RenewalOptInService,
        activatedRoute: ActivatedRoute
    ) {
        super(activatedRoute);
    }

    protected process(request: RenewalOptInTokenRequest): Promise<RenewalOptInResult> {
        return this._renewalOptInService.verifyEmail(request);
    }
}
