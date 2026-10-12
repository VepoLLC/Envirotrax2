import { Directive, OnInit } from "@angular/core";
import { ActivatedRoute } from "@angular/router";
import { RenewalOptInResult, RenewalOptInTokenRequest } from "../../shared/models/sites/renewal-opt-in";

/**
 * Pages opened from a link in a renewal email: they read the id and token from the query string,
 * send them once and show the outcome.
 */
@Directive()
export abstract class RenewalEmailLinkComponent implements OnInit {
    public abstract readonly title: string;

    public isLoading: boolean = false;
    public isLoaded: boolean = false;
    public result?: RenewalOptInResult;

    constructor(private readonly _activatedRoute: ActivatedRoute) {

    }

    public async ngOnInit(): Promise<void> {
        const queryParams = this._activatedRoute.snapshot.queryParamMap;
        const id = Number(queryParams.get('id')) || 0;
        const token = queryParams.get('token');

        if (!id || !token) {
            this.isLoaded = true;
            return;
        }

        try {
            this.isLoading = true;
            this.result = await this.process({ id, token });
        } finally {
            this.isLoading = false;
            this.isLoaded = true;
        }
    }

    protected abstract process(request: RenewalOptInTokenRequest): Promise<RenewalOptInResult>;
}
