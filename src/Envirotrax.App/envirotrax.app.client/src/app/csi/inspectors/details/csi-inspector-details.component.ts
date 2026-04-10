import { Component, OnInit } from "@angular/core";
import { ActivatedRoute } from "@angular/router";
import { Professional } from "../../../shared/models/professionals/professional";
import { CsiInspectorAccountInfoService } from "../../../shared/services/csi/csi-inspector-account-info.service";

@Component({
    selector: 'app-csi-inspector-details',
    standalone: false,
    templateUrl: './csi-inspector-details.component.html'
})
export class CsiInspectorDetailsComponent implements OnInit {
    public id: number | null = null;
    public accountInfo: Professional | null = null;
    public isAccountLoading: boolean = false;

    constructor(
        private readonly _activatedRoute: ActivatedRoute,
        private readonly _accountInfoService: CsiInspectorAccountInfoService
    ) {}

    public async ngOnInit(): Promise<void> {
        this.setIdFromRoute();
        if (this.id !== null) {
            await this.loadAccountInfo();
        }
    }

     private setIdFromRoute(): void {
        const idParam = this._activatedRoute.snapshot.paramMap.get('id');
        this.id = idParam ? Number(idParam) : null;
    }

    private async loadAccountInfo(): Promise<void> {
        if (this.id === null) {
            return;
        }
        try {
            this.isAccountLoading = true;
            this.accountInfo = await this._accountInfoService.getAccountInfo(this.id);
        } finally {
            this.isAccountLoading = false;
        }
    }

    public getFormattedAddress(): string {
        if (!this.accountInfo) {
            return '';
        }
        const parts = [
            this.accountInfo.address,
            this.accountInfo.city,
            this.accountInfo.state?.name,
            this.accountInfo.zipCode
        ].filter(p => p);
        return parts.join(', ');
    }
}
