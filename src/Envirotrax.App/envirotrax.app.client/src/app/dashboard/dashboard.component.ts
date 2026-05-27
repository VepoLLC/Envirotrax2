import { Component, OnInit } from "@angular/core";
import { AuthService } from "../shared/services/auth/auth.service";
import { WaterSupplierDashboardService } from "../shared/services/water-suppliers/water-supplier-dashboard.service";
import { WaterSupplierDashboardStats } from "../shared/models/water-suppliers/water-supplier-dashboard-stats";
import { CsiSubmissionStats, CsiSubAccountStats } from "../shared/models/water-suppliers/csi-submission-stats";
import { FeatureType } from "../shared/models/feature-type";

@Component({
    templateUrl: './dashboard.component.html',
    styleUrls: ['./dashboard.component.scss'],
    standalone: false
})
export class DashboardComponent implements OnInit {
    public waterSupplierId?: number;
    public stats?: WaterSupplierDashboardStats;
    public csiStats?: CsiSubmissionStats;
    public isLoading: boolean = false;

    public hasWiseGuys: boolean = false;
    public hasCsi: boolean = false;
    public hasBackflow: boolean = false;
    public hasFogInspection: boolean = false;
    public hasFogTransportation: boolean = false;

    public get hasAnyProgram(): boolean {
        return this.hasWiseGuys || this.hasCsi || this.hasBackflow || this.hasFogInspection || this.hasFogTransportation;
    }

    public get csiTotalInspections(): number {
        if (this.csiStats) {
            return this.csiStats.dailyStats.reduce((s, d) => s + d.totalInspections, 0);
        } else {
            return 0;
        }
    }

    public get csiTotalPaidInspections(): number {
        if (this.csiStats) {
            return this.csiStats.dailyStats.reduce((s, d) => s + d.totalPaidInspections, 0);
        } else {
            return 0;
        }
    }

    public get csiSubAccountTotalInspections(): number {
        if (this.csiStats?.subAccountStats) {
            return this.csiStats.subAccountStats.reduce((s, sub) => s + this.getSubAccountTotal(sub, 'totalInspections'), 0);
        } else {
            return 0;
        }
    }

    public get csiSubAccountTotalPaidInspections(): number {
        if (this.csiStats?.subAccountStats) {
            return this.csiStats.subAccountStats.reduce((s, sub) => s + this.getSubAccountTotal(sub, 'totalPaidInspections'), 0);
        } else {
            return 0;
        }
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

        if (this.hasAnyProgram) {
            await this.loadPageData();
        }
    }

    public async loadPageData(): Promise<void> {
        try {
            this.isLoading = true;
            const requests: Promise<unknown>[] = [this._dashboardService.getStats().then(s => this.stats = s)];
            if (this.hasCsi) {
                requests.push(this._dashboardService.getCsiSubmissionStats().then(s => this.csiStats = s));
            }
            await Promise.all(requests);
        } finally {
            this.isLoading = false;
        }
    }

    public getThresholdBgClass(count: number): string {
        if (count === 0) {
            return 'bg-success';
        } else if (count < 5) {
            return 'bg-warning';
        } else {
            return 'bg-danger';
        }
    }

    public getThresholdTextClass(count: number): string {
        if (count === 0) {
            return 'text-success';
        } else if (count < 5) {
            return 'text-warning';
        } else {
            return 'text-danger';
        }
    }

    public getBarPercent(value: number): number {
        return this.csiTotalInspections > 0 ? Math.round((value / this.csiTotalInspections) * 100) : 0;
    }

    public getDayName(date: string): string {
        return new Date(date + 'T00:00:00').toLocaleDateString('en-US', { weekday: 'short' });
    }

    public getFormattedDate(date: string): string {
        return new Date(date + 'T00:00:00').toLocaleDateString('en-US', { month: 'numeric', day: 'numeric' });
    }

    public getSubAccountTotal(sub: CsiSubAccountStats, field: 'totalInspections' | 'totalPaidInspections'): number {
        return sub.dailyStats.reduce((sum, d) => sum + d[field], 0);
    }
}
