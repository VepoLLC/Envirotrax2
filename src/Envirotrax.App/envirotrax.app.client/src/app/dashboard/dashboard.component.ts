import { Component, OnDestroy, OnInit, ViewChild } from "@angular/core";
import { BaseChartDirective } from "ng2-charts";
import { ChartConfiguration, ChartData, Plugin } from "chart.js";
import { AuthService } from "../shared/services/auth/auth.service";
import { WaterSupplierDashboardService } from "../shared/services/water-suppliers/water-supplier-dashboard.service";
import { WaterSupplierDashboardStats } from "../shared/models/water-suppliers/water-supplier-dashboard-stats";
import { CsiSubmissionStats } from "../shared/models/water-suppliers/csi-submission-stats";
import { BackflowSubmissionStats } from "../shared/models/water-suppliers/backflow-submission-stats";
import { BackflowComplianceSnapshot } from "../shared/models/backflow/backflow-compliance-snapshot";
import { FogInspectionSubmissionStats } from "../shared/models/water-suppliers/fog-inspection-submission-stats";
import { FogTripTicketSubmissionStats } from "../shared/models/water-suppliers/fog-trip-ticket-submission-stats";
import { FeatureType } from "../shared/models/feature-type";
import { OutOfServiceType } from "../shared/models/backflow/out-of-service-type.enum";
import { onThemeChange, readCssVar } from "../shared/utils/chart-theme.util";

@Component({
    templateUrl: './dashboard.component.html',
    styleUrls: ['./dashboard.component.scss'],
    standalone: false
})
export class DashboardComponent implements OnInit, OnDestroy {
    @ViewChild(BaseChartDirective) private _complianceChart?: BaseChartDirective;

    private _disposeThemeObserver?: () => void;

    public readonly OutOfServiceType = OutOfServiceType;

    public waterSupplierId?: number;
    public statsVm: DashboardStatsVm = this.buildStatsVm();
    public csiVm?: CsiStatsVm;
    public backflowVm?: BackflowStatsVm;
    public backflowComplianceVm: BackflowComplianceVm | null = null;
    public fogInspectionVm?: FogInspectionStatsVm;
    public fogTripTicketVm?: FogTripTicketStatsVm;
    public isLoading: boolean = false;


    public hasCsi: boolean = false;
    public hasBackflow: boolean = false;
    public hasFogInspection: boolean = false;
    public hasFogTransportation: boolean = false;
    public hasAnyProgram: boolean = false;

    public readonly complianceTotalColor = '#adb5bd';
    public readonly complianceCompliantColor = '#20a845';
    public readonly complianceNonCompliantColor = '#dc3545';

    private readonly compliancePercentFont = '700 1.4rem sans-serif';
    private readonly complianceLabelFont = '400 0.75rem sans-serif';
    private readonly compliancePercentOffsetY = -7;
    private readonly complianceLabelOffsetY = 14;

    public readonly complianceDoughnutType = 'doughnut' as const;
    public readonly complianceDoughnutOptions: ChartConfiguration<'doughnut'>['options'] = {
        responsive: true,
        maintainAspectRatio: false,
        cutout: '62%',
        plugins: {
            legend: { display: false },
            tooltip: { enabled: false }
        }
    };

    public readonly complianceDoughnutPlugins: Plugin<'doughnut'>[] = [{
        id: 'dashboardComplianceCenterText',
        afterDraw: chart => {
            const arc = chart.getDatasetMeta(0).data[0] as unknown as { x: number; y: number } | undefined;

            if (!arc) {
                return;
            }

            const percentColor = readCssVar('--bf-doughnut-percent-color', '#212529');
            const labelColor = readCssVar('--bf-doughnut-label-color', '#6c757d');
            const ctx = chart.ctx;

            ctx.save();
            ctx.textAlign = 'center';
            ctx.textBaseline = 'middle';

            ctx.fillStyle = percentColor;
            ctx.font = this.compliancePercentFont;
            ctx.fillText(`${this.backflowComplianceVm?.compliantPercentage ?? 0}%`, arc.x, arc.y + this.compliancePercentOffsetY);

            ctx.fillStyle = labelColor;
            ctx.font = this.complianceLabelFont;
            ctx.fillText('Compliant', arc.x, arc.y + this.complianceLabelOffsetY);

            ctx.restore();
        }
    }];

    constructor(
        private readonly _authService: AuthService,
        private readonly _dashboardService: WaterSupplierDashboardService
    ) { }

    public async ngOnInit(): Promise<void> {
        this._disposeThemeObserver = onThemeChange(() => this._complianceChart?.update());

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

        this.hasAnyProgram = this.hasCsi || this.hasBackflow || this.hasFogInspection || this.hasFogTransportation;

        if (this.hasAnyProgram) {
            await this.loadPageData();
        }
    }

    public ngOnDestroy(): void {
        this._disposeThemeObserver?.();
    }

    public async loadPageData(): Promise<void> {
        try {
            this.isLoading = true;
            const requests: Promise<unknown>[] = [this._dashboardService.getStats().then(s => this.statsVm = this.buildStatsVm(s))];

            if (this.hasCsi) {
                requests.push(this._dashboardService.getCsiSubmissionStats().then(s => this.csiVm = this.buildCsiVm(s)));
            }

            if (this.hasBackflow) {
                requests.push(this._dashboardService.getBackflowSubmissionStats().then(s => this.backflowVm = this.buildBackflowVm(s)));
                requests.push(this._dashboardService.getBackflowCompliance().then(c => this.backflowComplianceVm = this.buildBackflowComplianceVm(c)));
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

    private buildStatsVm(stats?: WaterSupplierDashboardStats): DashboardStatsVm {
        return {
            pastDuePropertyLogs: this.buildPresenceBadge(stats?.pastDuePropertyLogCount ?? 0, 'danger'),
            expiringPropertyLogs: this.buildPresenceBadge(stats?.expiringPropertyLogCount ?? 0, 'warning'),
            allPropertyLogCount: stats?.allPropertyLogCount ?? 0,

            bpatCount: stats?.bpatCount ?? 0,
            csiInspectorCount: stats?.csiInspectorCount ?? 0,
            fogInspectorCount: stats?.fogInspectorCount ?? 0,
            fogTransporterCount: stats?.fogTransporterCount ?? 0,

            unverifiedLicenses: this.buildThresholdBadge(stats?.unverifiedLicenseCount ?? 0),
            expiredLicenses: this.buildThresholdBadge(stats?.expiredLicenseCount ?? 0),
            expiringLicenses: this.buildThresholdBadge(stats?.expiringLicenseCount ?? 0),

            insurancePolicies: this.buildThresholdBadge(stats?.insurancePolicyCount ?? 0),
            testGauges: this.buildThresholdBadge(stats?.testGaugeCount ?? 0),
            transporterRegistrations: this.buildThresholdBadge(stats?.transporterRegistrationCount ?? 0),

            outOfServiceReplaced: this.buildThresholdBadge(stats?.outOfServiceReplacedCount ?? 0),
            outOfServiceRemoved: this.buildThresholdBadge(stats?.outOfServiceRemovedCount ?? 0)
        };
    }

    private buildPresenceBadge(count: number, presentColor: string): CountBadgeVm {
        return this.buildBadge(count, count > 0 ? presentColor : 'success');
    }

    private buildThresholdBadge(count: number): CountBadgeVm {
        if (count === 0) {
            return this.buildBadge(count, 'success');
        }

        if (count < 5) {
            return this.buildBadge(count, 'warning');
        }

        return this.buildBadge(count, 'danger');
    }

    private buildBadge(count: number, color: string): CountBadgeVm {
        return {
            count,
            textClass: `text-${color}`,
            bgClass: `bg-${color}`
        };
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
            returnUrl: this.buildReturnUrl('/csi', dailyStats[dailyStats.length - 1]?.date, dailyStats[0]?.date)
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
            returnUrl: this.buildReturnUrl('/backflow/tests', dailyStats[dailyStats.length - 1]?.date, dailyStats[0]?.date)
        };
    }

    private buildBackflowComplianceVm(snapshot: BackflowComplianceSnapshot | null): BackflowComplianceVm | null {
        if (!snapshot) {
            return null;
        }

        return {
            reportDate: snapshot.reportDate,
            total: snapshot.total,
            compliant: snapshot.compliant,
            nonCompliant: snapshot.nonCompliant,
            compliantPercentage: snapshot.compliantPercentage,
            doughnutData: {
                labels: ['Compliant', 'Non-Compliant'],
                datasets: [{
                    data: [snapshot.compliant, snapshot.nonCompliant],
                    backgroundColor: [this.complianceCompliantColor, this.complianceNonCompliantColor],
                    borderWidth: 0
                }]
            }
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
            returnUrl: this.buildReturnUrl('/fog/inspections', dailyStats[dailyStats.length - 1]?.date, dailyStats[0]?.date)
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
            returnUrl: this.buildReturnUrl('/fog/trip-tickets', dailyStats[dailyStats.length - 1]?.date, dailyStats[0]?.date)
        };
    }

    private buildReturnUrl(basePath: string, startDate: string, endDate: string): string {
        return `${basePath}?startDate=${encodeURIComponent(startDate)}&endDate=${encodeURIComponent(endDate)}`;
    }

    private formatDayName(date: string): string {
        return new Date(date + 'T00:00:00').toLocaleDateString('en-US', { weekday: 'short' });
    }

    private formatDate(date: string): string {
        return new Date(date + 'T00:00:00').toLocaleDateString('en-US', { month: 'numeric', day: 'numeric' });
    }
}

interface CountBadgeVm {
    count: number;
    textClass: string;
    bgClass: string;
}

interface DashboardStatsVm {
    pastDuePropertyLogs: CountBadgeVm;
    expiringPropertyLogs: CountBadgeVm;
    allPropertyLogCount: number;

    bpatCount: number;
    csiInspectorCount: number;
    fogInspectorCount: number;
    fogTransporterCount: number;

    unverifiedLicenses: CountBadgeVm;
    expiredLicenses: CountBadgeVm;
    expiringLicenses: CountBadgeVm;

    insurancePolicies: CountBadgeVm;
    testGauges: CountBadgeVm;
    transporterRegistrations: CountBadgeVm;

    outOfServiceReplaced: CountBadgeVm;
    outOfServiceRemoved: CountBadgeVm;
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
    returnUrl: string;
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
    returnUrl: string;
}

interface BackflowComplianceVm {
    reportDate: string;
    total: number;
    compliant: number;
    nonCompliant: number;
    compliantPercentage: number;
    doughnutData: ChartData<'doughnut'>;
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
    returnUrl: string;
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
    returnUrl: string;
}
