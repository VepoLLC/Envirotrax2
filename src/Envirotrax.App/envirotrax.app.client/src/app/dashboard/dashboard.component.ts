import { Component, OnInit } from "@angular/core";
import { AuthService } from "../shared/services/auth/auth.service";
import { WaterSupplierDashboardService } from "../shared/services/water-suppliers/water-supplier-dashboard.service";
import { WaterSupplierDashboardStats } from "../shared/models/water-suppliers/water-supplier-dashboard-stats";
import { FeatureType } from "../shared/models/feature-type";

@Component({
    templateUrl: './dashboard.component.html',
    styleUrls: ['./dashboard.component.scss'],
    standalone: false
})
export class DashboardComponent implements OnInit {
    public waterSupplierId?: number;
    public stats?: WaterSupplierDashboardStats;
    public isLoading: boolean = false;

    public hasWiseGuys: boolean = false;
    public hasCsi: boolean = false;
    public hasBackflow: boolean = false;
    public hasFogInspection: boolean = false;
    public hasFogTransportation: boolean = false;

    public get hasAnyProgram(): boolean {
        return this.hasWiseGuys || this.hasCsi || this.hasBackflow || this.hasFogInspection || this.hasFogTransportation;
    }

    constructor(
        private readonly _authService: AuthService,
        private readonly _dashboardService: WaterSupplierDashboardService
    ) { }

    public async ngOnInit(): Promise<void> {
        [
            this.waterSupplierId,
            this.hasWiseGuys,
            this.hasCsi,
            this.hasBackflow,
            this.hasFogInspection,
            this.hasFogTransportation
        ] = await Promise.all([
            this._authService.getWaterSupplierId(),
            this._authService.hasAnyFeatures(FeatureType.WiseGuys),
            this._authService.hasAnyFeatures(FeatureType.CsiInspection),
            this._authService.hasAnyFeatures(FeatureType.BackflowTesting),
            this._authService.hasAnyFeatures(FeatureType.FogInspection),
            this._authService.hasAnyFeatures(FeatureType.FogTransportation)
        ]);

        if(this.hasAnyProgram){
            await this.loadPageData();
        }

    }

    public async loadPageData(): Promise<void> {
        try {
            this.isLoading = true;
            this.stats = await this._dashboardService.getStats();
        } finally {
            this.isLoading = false;
        }
    }
}


