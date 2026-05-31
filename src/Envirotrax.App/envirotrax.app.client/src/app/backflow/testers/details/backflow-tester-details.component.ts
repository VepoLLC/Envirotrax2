import { Component, OnDestroy, OnInit } from "@angular/core";

import { ActivatedRoute } from "@angular/router";
import { Subscription } from "rxjs";
import { Professional } from "../../../shared/models/professionals/professional";
import { BackflowTesterManagementService } from "../../../shared/services/backflow/backflow-tester-management.service";

@Component({
    selector: 'app-backflow-tester-details',
    standalone: false,
    templateUrl: './backflow-tester-details.component.html'
})
export class BackflowTesterDetailsComponent implements OnInit, OnDestroy {
    public id: number | null = null;
    public isAccountLoading: boolean = false;
    public accountInfo: Professional | null = null;
    public formattedAddress: string = '';

    private _routeSub?: Subscription;

    constructor(
        private readonly _activatedRoute: ActivatedRoute,
        private readonly _backflowTesterManagementService: BackflowTesterManagementService,
    ) { }

    public ngOnInit(): void {
        this._routeSub = this._activatedRoute.paramMap.subscribe(async params => {
            const idParam = params.get('id');
            this.id = idParam ? Number(idParam) : null;
            if (this.id !== null) {
                await this.loadAccountInfo();
            }
        });
    }

    public ngOnDestroy(): void {
        this._routeSub?.unsubscribe();
    }

    private async loadAccountInfo(): Promise<void> {
        if (this.id === null) {
            return;
        }
        try {
            this.isAccountLoading = true;
            this.accountInfo = await this._backflowTesterManagementService.getAccountInfo(this.id);
            this.formattedAddress = this.buildFormattedAddress();
        } finally {
            this.isAccountLoading = false;
        }
    }

    private buildFormattedAddress(): string {
        if (!this.accountInfo) {
            return '';
        }
        return [
            this.accountInfo.address,
            this.accountInfo.city,
            this.accountInfo.state?.name,
            this.accountInfo.zipCode
        ].filter(p => p).join(', ');
    }
}
