import { Component, OnInit } from "@angular/core";
import { Professional } from "../../shared/models/professionals/professional";
import { ActivatedRoute } from "@angular/router";
import { HelperService } from "../../shared/services/helpers/helper.service";
import { State } from "../../shared/models/states/state";
import { LookupService } from "../../shared/services/lookup/lookup.service";

@Component({
    standalone: false,
    templateUrl: './company.component.html'
})
export class CompanyComponent implements OnInit {
    public isLoading: boolean = false;
    public validationErrors: string[] = [];
    public professional: Professional = {};
    public states: State[] = [];

    constructor(
        private readonly _acitvatedRoute: ActivatedRoute,
        private readonly _helper: HelperService,
        private readonly _lookupService: LookupService
    ) {

    }

    public async ngOnInit(): Promise<void> {
        try {
            this.isLoading = true;
            this.states = await this._lookupService.getAllStates();
        } finally {
            this.isLoading = false;
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
}