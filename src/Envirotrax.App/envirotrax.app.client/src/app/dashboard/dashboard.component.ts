import { Component, OnInit } from "@angular/core";
import { AuthService } from "../shared/services/auth/auth.service";
import { WaterSupplierDashboardService } from "../shared/services/water-suppliers/water-supplier-dashboard.service";
import { WaterSupplierDashboardStats } from "../shared/models/water-suppliers/water-supplier-dashboard-stats";
import { CsiSubmissionStats } from "../shared/models/water-suppliers/csi-submission-stats";
import { BackflowSubmissionStats } from "../shared/models/water-suppliers/backflow-submission-stats";
import { FogInspectionSubmissionStats } from "../shared/models/water-suppliers/fog-inspection-submission-stats";
import { FogTripTicketSubmissionStats } from "../shared/models/water-suppliers/fog-trip-ticket-submission-stats";
import { FeatureType } from "../shared/models/feature-type";

@Component({
    templateUrl: './dashboard.component.html',
    styleUrls: ['./dashboard.component.scss'],
    standalone: false
})
export class DashboardComponent implements OnInit {
    public waterSupplierId?: number;
    public stats?: WaterSupplierDashboardStats;
    public csiVm?: CsiStatsVm;
    public backflowVm?: BackflowStatsVm;
    public fogInspectionVm?: FogInspectionStatsVm;
    public fogTripTicketVm?: FogTripTicketStatsVm;
    public isLoading: boolean = false;

    
    public hasCsi: boolean = false;
    public hasBackflow: boolean = false;
    public hasFogInspection: boolean = false;
    public hasFogTransportation: boolean = false;

    public get hasAnyProgram(): boolean {
        return this.hasCsi || this.hasBackflow || this.hasFogInspection || this.hasFogTransportation;
    }

    constructor(
        private readonly _authService: AuthService,
        private readonly _dashboardService: WaterSupplierDashboardService
    ) { }

    public async ngOnInit(): Promise<void> {
        [
            this.waterSupplierId,
            this.hasCsi,
            this.hasBackflow,
            this.hasFogInspection,
            this.hasFogTransportation
        ] = await Promise.all([
            this._authService.getWaterSupplierId(),
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
                requests.push(this._dashboardService.getCsiSubmissionStats().then(s => this.csiVm = this.buildCsiVm(s)));
            }

            if (this.hasBackflow) {
                requests.push(this._dashboardService.getBackflowSubmissionStats().then(s => this.backflowVm = this.buildBackflowVm(s)));
            }

            if (this.hasFogInspection) {
                requests.push(this._dashboardService.getFogInspectionSubmissionStats().then(s => this.fogInspectionVm = this.buildFogInspectionVm(s)));
            }

            if (this.hasFogTransportation) {
                requests.push(this._dashboardService.getFogTripTicketSubmissionStats().then(s => this.fogTripTicketVm = this.buildFogTripTicketVm(s)));
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

    private buildCsiVm(stats: CsiSubmissionStats): CsiStatsVm {
        const totalInspections = stats.dailyStats.reduce((s, d) => s + d.totalInspections, 0);
        const totalPaidInspections = stats.dailyStats.reduce((s, d) => s + d.totalPaidInspections, 0);

        const dailyStats = stats.dailyStats.map(d => ({
            date: d.date,
            dayName: this.formatDayName(d.date),
            formattedDate: this.formatDate(d.date),
            isWeekend: d.isWeekend,
            totalInspections: d.totalInspections,
            totalPaidInspections: d.totalPaidInspections,
            barPercent: totalInspections > 0 ? Math.round((d.totalInspections / totalInspections) * 100) : 0
        }));

        const subAccountStats = (stats.subAccountStats ?? []).map(sub => ({
            waterSupplierId: sub.waterSupplierId,
            waterSupplierName: sub.waterSupplierName,
            totalInspections: sub.dailyStats.reduce((s, d) => s + d.totalInspections, 0),
            totalPaidInspections: sub.dailyStats.reduce((s, d) => s + d.totalPaidInspections, 0)
        }));

        return {
            dailyStats,
            totalInspections,
            totalPaidInspections,
            subAccountStats,
            subAccountTotalInspections: subAccountStats.reduce((s, sub) => s + sub.totalInspections, 0),
            subAccountTotalPaidInspections: subAccountStats.reduce((s, sub) => s + sub.totalPaidInspections, 0),
            rangeStartDate: dailyStats[0]?.date,
            rangeEndDate: dailyStats[dailyStats.length - 1]?.date
        };
    }

    private buildBackflowVm(stats: BackflowSubmissionStats): BackflowStatsVm {
        const totalTests = stats.dailyStats.reduce((s, d) => s + d.totalTests, 0);
        const totalPaidTests = stats.dailyStats.reduce((s, d) => s + d.totalPaidTests, 0);

        const dailyStats = stats.dailyStats.map(d => ({
            date: d.date,
            dayName: this.formatDayName(d.date),
            formattedDate: this.formatDate(d.date),
            isWeekend: d.isWeekend,
            totalTests: d.totalTests,
            totalPaidTests: d.totalPaidTests,
            barPercent: totalTests > 0 ? Math.round((d.totalTests / totalTests) * 100) : 0
        }));

        const subAccountStats = (stats.subAccountStats ?? []).map(sub => ({
            waterSupplierId: sub.waterSupplierId,
            waterSupplierName: sub.waterSupplierName,
            totalTests: sub.dailyStats.reduce((s, d) => s + d.totalTests, 0),
            totalPaidTests: sub.dailyStats.reduce((s, d) => s + d.totalPaidTests, 0)
        }));

        return {
            dailyStats,
            totalTests,
            totalPaidTests,
            subAccountStats,
            subAccountTotalTests: subAccountStats.reduce((s, sub) => s + sub.totalTests, 0),
            subAccountTotalPaidTests: subAccountStats.reduce((s, sub) => s + sub.totalPaidTests, 0),
            rangeStartDate: dailyStats[0]?.date,
            rangeEndDate: dailyStats[dailyStats.length - 1]?.date
        };
    }

    private buildFogInspectionVm(stats: FogInspectionSubmissionStats): FogInspectionStatsVm {
        const totalInspections = stats.dailyStats.reduce((s, d) => s + d.totalInspections, 0);
        const totalPaidInspections = stats.dailyStats.reduce((s, d) => s + d.totalPaidInspections, 0);

        const dailyStats = stats.dailyStats.map(d => ({
            date: d.date,
            dayName: this.formatDayName(d.date),
            formattedDate: this.formatDate(d.date),
            isWeekend: d.isWeekend,
            totalInspections: d.totalInspections,
            totalPaidInspections: d.totalPaidInspections,
            barPercent: totalInspections > 0 ? Math.round((d.totalInspections / totalInspections) * 100) : 0
        }));

        const subAccountStats = (stats.subAccountStats ?? []).map(sub => ({
            waterSupplierId: sub.waterSupplierId,
            waterSupplierName: sub.waterSupplierName,
            totalInspections: sub.dailyStats.reduce((s, d) => s + d.totalInspections, 0),
            totalPaidInspections: sub.dailyStats.reduce((s, d) => s + d.totalPaidInspections, 0)
        }));

        return {
            dailyStats,
            totalInspections,
            totalPaidInspections,
            subAccountStats,
            subAccountTotalInspections: subAccountStats.reduce((s, sub) => s + sub.totalInspections, 0),
            subAccountTotalPaidInspections: subAccountStats.reduce((s, sub) => s + sub.totalPaidInspections, 0),
            rangeStartDate: dailyStats[0]?.date,
            rangeEndDate: dailyStats[dailyStats.length - 1]?.date
        };
    }

    private buildFogTripTicketVm(stats: FogTripTicketSubmissionStats): FogTripTicketStatsVm {
        const totalTripTickets = stats.dailyStats.reduce((s, d) => s + d.totalTripTickets, 0);
        const totalPaidTripTickets = stats.dailyStats.reduce((s, d) => s + d.totalPaidTripTickets, 0);

        const dailyStats = stats.dailyStats.map(d => ({
            date: d.date,
            dayName: this.formatDayName(d.date),
            formattedDate: this.formatDate(d.date),
            isWeekend: d.isWeekend,
            totalTripTickets: d.totalTripTickets,
            totalPaidTripTickets: d.totalPaidTripTickets,
            barPercent: totalTripTickets > 0 ? Math.round((d.totalTripTickets / totalTripTickets) * 100) : 0
        }));

        const subAccountStats = (stats.subAccountStats ?? []).map(sub => ({
            waterSupplierId: sub.waterSupplierId,
            waterSupplierName: sub.waterSupplierName,
            totalTripTickets: sub.dailyStats.reduce((s, d) => s + d.totalTripTickets, 0),
            totalPaidTripTickets: sub.dailyStats.reduce((s, d) => s + d.totalPaidTripTickets, 0)
        }));

        return {
            dailyStats,
            totalTripTickets,
            totalPaidTripTickets,
            subAccountStats,
            subAccountTotalTripTickets: subAccountStats.reduce((s, sub) => s + sub.totalTripTickets, 0),
            subAccountTotalPaidTripTickets: subAccountStats.reduce((s, sub) => s + sub.totalPaidTripTickets, 0),
            rangeStartDate: dailyStats[0]?.date,
            rangeEndDate: dailyStats[dailyStats.length - 1]?.date
        };
    }

    public buildReturnUrl(basePath: string, startDate: string, endDate: string): string {
        return `${basePath}?startDate=${encodeURIComponent(startDate)}&endDate=${encodeURIComponent(endDate)}`;
    }

    private formatDayName(date: string): string {
        return new Date(date + 'T00:00:00').toLocaleDateString('en-US', { weekday: 'short' });
    }

    private formatDate(date: string): string {
        return new Date(date + 'T00:00:00').toLocaleDateString('en-US', { month: 'numeric', day: 'numeric' });
    }
}

interface CsiDailyStatsVm {
    date: string;
    dayName: string;
    formattedDate: string;
    isWeekend: boolean;
    totalInspections: number;
    totalPaidInspections: number;
    barPercent: number;
}

interface CsiSubAccountVm {
    waterSupplierId: number;
    waterSupplierName: string;
    totalInspections: number;
    totalPaidInspections: number;
}

interface CsiStatsVm {
    dailyStats: CsiDailyStatsVm[];
    totalInspections: number;
    totalPaidInspections: number;
    subAccountStats: CsiSubAccountVm[];
    subAccountTotalInspections: number;
    subAccountTotalPaidInspections: number;
    rangeStartDate: string;
    rangeEndDate: string;
}

interface BackflowDailyStatsVm {
    date: string;
    dayName: string;
    formattedDate: string;
    isWeekend: boolean;
    totalTests: number;
    totalPaidTests: number;
    barPercent: number;
}

interface BackflowSubAccountVm {
    waterSupplierId: number;
    waterSupplierName: string;
    totalTests: number;
    totalPaidTests: number;
}

interface BackflowStatsVm {
    dailyStats: BackflowDailyStatsVm[];
    totalTests: number;
    totalPaidTests: number;
    subAccountStats: BackflowSubAccountVm[];
    subAccountTotalTests: number;
    subAccountTotalPaidTests: number;
    rangeStartDate: string;
    rangeEndDate: string;
}

interface FogInspectionDailyStatsVm {
    date: string;
    dayName: string;
    formattedDate: string;
    isWeekend: boolean;
    totalInspections: number;
    totalPaidInspections: number;
    barPercent: number;
}

interface FogInspectionSubAccountVm {
    waterSupplierId: number;
    waterSupplierName: string;
    totalInspections: number;
    totalPaidInspections: number;
}

interface FogInspectionStatsVm {
    dailyStats: FogInspectionDailyStatsVm[];
    totalInspections: number;
    totalPaidInspections: number;
    subAccountStats: FogInspectionSubAccountVm[];
    subAccountTotalInspections: number;
    subAccountTotalPaidInspections: number;
    rangeStartDate: string;
    rangeEndDate: string;
}

interface FogTripTicketDailyStatsVm {
    date: string;
    dayName: string;
    formattedDate: string;
    isWeekend: boolean;
    totalTripTickets: number;
    totalPaidTripTickets: number;
    barPercent: number;
}

interface FogTripTicketSubAccountVm {
    waterSupplierId: number;
    waterSupplierName: string;
    totalTripTickets: number;
    totalPaidTripTickets: number;
}

interface FogTripTicketStatsVm {
    dailyStats: FogTripTicketDailyStatsVm[];
    totalTripTickets: number;
    totalPaidTripTickets: number;
    subAccountStats: FogTripTicketSubAccountVm[];
    subAccountTotalTripTickets: number;
    subAccountTotalPaidTripTickets: number;
    rangeStartDate: string;
    rangeEndDate: string;
}
