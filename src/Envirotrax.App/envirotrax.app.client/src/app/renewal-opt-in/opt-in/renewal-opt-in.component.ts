import { Component, OnInit } from "@angular/core";
import { ActivatedRoute } from "@angular/router";
import { NgForm } from "@angular/forms";
import { InputOption } from "@envirotrax/common-ui";
import { RenewalOptInService } from "../../shared/services/sites/renewal-opt-in.service";
import { RenewalOptInResult, RenewalOptInType } from "../../shared/models/sites/renewal-opt-in";

@Component({
    standalone: false,
    templateUrl: './renewal-opt-in.component.html'
})
export class RenewalOptInComponent implements OnInit {
    public readonly deliveryOptions: InputOption[] = [
        { id: String(RenewalOptInType.OptedOut), text: 'Via Physical Mail' },
        { id: String(RenewalOptInType.OptedIn), text: 'Via Email' }
    ];

    public siteId: number = 0;
    public isSiteFound: boolean = false;
    public isLoaded: boolean = false;
    public isLoading: boolean = false;

    public passcode: string = '';
    public zipCodePrefix: string = '';
    public optInType: string = String(RenewalOptInType.OptedOut);
    public isEmailDelivery: boolean = false;
    public emailAddresses: string = '';

    public result?: RenewalOptInResult;

    constructor(
        private readonly _renewalOptInService: RenewalOptInService,
        private readonly _activatedRoute: ActivatedRoute
    ) {

    }

    public async ngOnInit(): Promise<void> {
        this.siteId = Number(this._activatedRoute.snapshot.paramMap.get('siteId')) || 0;

        if (!this.siteId) {
            this.isLoaded = true;
            return;
        }

        try {
            this.isLoading = true;

            const site = await this._renewalOptInService.getSite(this.siteId);

            this.isSiteFound = true;
            this.optInType = String(site.optInType);
            this.onOptInTypeChanged();
        } finally {
            this.isLoading = false;
            this.isLoaded = true;
        }
    }

    public onOptInTypeChanged(): void {
        this.isEmailDelivery = this.optInType === String(RenewalOptInType.OptedIn);
    }

    public async submit(form: NgForm): Promise<void> {
        form.form.markAllAsTouched();

        if (!form.valid) {
            return;
        }

        try {
            this.isLoading = true;

            this.result = await this._renewalOptInService.save(this.siteId, {
                passcode: this.passcode,
                zipCodePrefix: this.zipCodePrefix,
                optInType: Number(this.optInType),
                emailAddresses: this.isEmailDelivery ? this.emailAddresses : undefined
            });
        } finally {
            this.isLoading = false;
        }
    }
}
