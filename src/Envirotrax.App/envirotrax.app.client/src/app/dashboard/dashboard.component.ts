import { Component, OnInit } from "@angular/core";
import { AuthService } from "../shared/services/auth/auth.service";
import { WaterSupplierDashboardService } from "../shared/services/water-suppliers/water-supplier-dashboard.service";
import { WaterSupplierDashboardStats } from "../shared/models/water-suppliers/water-supplier-dashboard-stats";
import { CsiSubmissionStats, CsiSubAccountStats } from "../shared/models/water-suppliers/csi-submission-stats";
import { BackflowSubmissionStats, BackflowSubAccountStats } from "../shared/models/water-suppliers/backflow-submission-stats";
import { FogInspectionSubmissionStats, FogInspectionSubAccountStats } from "../shared/models/water-suppliers/fog-inspection-submission-stats";
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
    public backflowStats?: BackflowSubmissionStats;
    public fogInspectionStats?: FogInspectionSubmissionStats;
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

    public get backflowTotalTests(): number {
        if (this.backflowStats) {
            return this.backflowStats.dailyStats.reduce((s, d) => s + d.totalTests, 0);
        } else {
            return 0;
        }
    }

    public get backflowTotalPaidTests(): number {
        if (this.backflowStats) {
            return this.backflowStats.dailyStats.reduce((s, d) => s + d.totalPaidTests, 0);
        } else {
            return 0;
        }
    }

    public get backflowSubAccountTotalTests(): number {
        if (this.backflowStats?.subAccountStats) {
            return this.backflowStats.subAccountStats.reduce((s, sub) => s + this.getBackflowSubAccountTotal(sub, 'totalTests'), 0);
        } else {
            return 0;
        }
    }

    public get backflowSubAccountTotalPaidTests(): number {
        if (this.backflowStats?.subAccountStats) {
            return this.backflowStats.subAccountStats.reduce((s, sub) => s + this.getBackflowSubAccountTotal(sub, 'totalPaidTests'), 0);
        } else {
            return 0;
        }
    }

    public get fogTotalInspections(): number {
        if (this.fogInspectionStats) {
            return this.fogInspectionStats.dailyStats.reduce((s, d) => s + d.totalInspections, 0);
        } else {
            return 0;
        }
    }

    public get fogTotalPaidInspections(): number {
        if (this.fogInspectionStats) {
            return this.fogInspectionStats.dailyStats.reduce((s, d) => s + d.totalPaidInspections, 0);
        } else {
            return 0;
        }
    }

    public get fogSubAccountTotalInspections(): number {
        if (this.fogInspectionStats?.subAccountStats) {
            return this.fogInspectionStats.subAccountStats.reduce((s, sub) => s + this.getFogSubAccountTotal(sub, 'totalInspections'), 0);
        } else {
            return 0;
        }
    }

    public get fogSubAccountTotalPaidInspections(): number {
        if (this.fogInspectionStats?.subAccountStats) {
            return this.fogInspectionStats.subAccountStats.reduce((s, sub) => s + this.getFogSubAccountTotal(sub, 'totalPaidInspections'), 0);
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

            if (this.hasBackflow) {
                requests.push(this._dashboardService.getBackflowSubmissionStats().then(s => this.backflowStats = s));
            }

            if (this.hasFogInspection) {
                requests.push(this._dashboardService.getFogInspectionSubmissionStats().then(s => this.fogInspectionStats = s));
            }

            await Promise.all(requests);
        } finally {
            this.isLoading = false;
        }
    }

    public getPropertyLogPastDueBgClass(count: number): string {
        return count > 0 ? 'bg-danger' : 'bg-success';
    }

    public getPropertyLogPastDueTextClass(count: number): string {
        return count > 0 ? 'text-danger' : 'text-success';
    }

    public getPropertyLogExpiringBgClass(count: number): string {
        return count > 0 ? 'bg-warning' : 'bg-success';
    }

    public getPropertyLogExpiringTextClass(count: number): string {
        return count > 0 ? 'text-warning' : 'text-success';
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

    public getBackflowBarPercent(value: number): number {
        return this.backflowTotalTests > 0 ? Math.round((value / this.backflowTotalTests) * 100) : 0;
    }

    public getFogBarPercent(value: number): number {
        return this.fogTotalInspections > 0 ? Math.round((value / this.fogTotalInspections) * 100) : 0;
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

    public getBackflowSubAccountTotal(sub: BackflowSubAccountStats, field: 'totalTests' | 'totalPaidTests'): number {
        return sub.dailyStats.reduce((sum, d) => sum + d[field], 0);
    }

    public getFogSubAccountTotal(sub: FogInspectionSubAccountStats, field: 'totalInspections' | 'totalPaidInspections'): number {
        return sub.dailyStats.reduce((sum, d) => sum + d[field], 0);
    }
}
