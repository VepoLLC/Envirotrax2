import { Component, OnInit } from "@angular/core";
import { BackflowTesterAccountInfoService } from "../../../shared/services/backflow/backflow-tester-account-info.service";
import { ActivatedRoute } from "@angular/router";
import { Professional } from "../../../shared/models/professionals/professional";

@Component({
    selector: 'app-backflow-tester-details',
    standalone: false,
    templateUrl: './backflow-tester-details.component.html'
})
export class BackflowTesterDetailsComponent implements OnInit {
    public id: number | null = null;
    public isAccountLoading: boolean = false;
    public accountInfo: Professional | null = null;

    constructor(
        private readonly _activatedRoute: ActivatedRoute,
        private readonly _accountInfoService: BackflowTesterAccountInfoService
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
