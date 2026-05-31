import { Component, OnDestroy, OnInit } from "@angular/core";
import { ActivatedRoute } from "@angular/router";
import { Subscription } from "rxjs";
import { Professional } from "../../../shared/models/professionals/professional";
import { FogInspectorAccountInfoService } from "../../../shared/services/fog/fog-inspector-account-info.service";

@Component({
    standalone: false,
    templateUrl: './fog-inspector-details.component.html'
})
export class FogInspectorDetailsComponent implements OnInit, OnDestroy {
    public id: number | null = null;
    public accountInfo: Professional | null = null;
    public isAccountLoading: boolean = false;
    public formattedAddress: string = '';

    private _routeSub?: Subscription;

    constructor(
        private readonly _activatedRoute: ActivatedRoute,
        private readonly _accountInfoService: FogInspectorAccountInfoService
    ) {}

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
            this.accountInfo = await this._accountInfoService.getAccountInfo(this.id);
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
