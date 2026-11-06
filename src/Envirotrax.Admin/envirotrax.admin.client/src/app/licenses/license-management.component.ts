import { CommonModule } from '@angular/common';
import { Component, OnInit, ViewChild } from '@angular/core';
import { SharedComponentsModule } from '../shared/components/shared.components.module';
import { LicenseAdminCounts } from '../shared/models/professionals/licenses/water-supplier-license';
import { LicenseService } from '../shared/services/licenses/license.service';
import { GaugeListComponent } from './gauges/list/gauge-list.component';
import { InsuranceListComponent } from './insurances/list/insurance-list.component';
import { LicenseListComponent } from './list/license-list.component';

type LicenseManagementTab = 'unverified' | 'expired' | 'expiring' | 'insurance' | 'gauges';

const TAB_HEADERS: Record<LicenseManagementTab, string> = {
    unverified: 'Unverified Licenses',
    expired: 'Expired Licenses',
    expiring: 'Expiring Licenses',
    insurance: 'Insurance',
    gauges: 'Gauges'
};

@Component({
    templateUrl: './license-management.component.html',
    imports: [
        CommonModule,
        SharedComponentsModule,
        LicenseListComponent,
        InsuranceListComponent,
        GaugeListComponent
    ],
})
export class LicenseManagementComponent implements OnInit {
    @ViewChild(LicenseListComponent)
    public licenseList?: LicenseListComponent;

    @ViewChild(InsuranceListComponent)
    public insuranceList?: InsuranceListComponent;

    @ViewChild(GaugeListComponent)
    public gaugeList?: GaugeListComponent;

    public activeTab: LicenseManagementTab = 'unverified';
    public header: string = TAB_HEADERS.unverified;

    public counts: LicenseAdminCounts = {
        unverifiedCount: 0,
        expiredCount: 0,
        expiringCount: 0,
        insuranceCount: 0,
        gaugeCount: 0
    };

    constructor(private readonly _licenseService: LicenseService) {

    }

    public async ngOnInit(): Promise<void> {
        await this.loadCounts();
    }

    public setActiveTab(tab: LicenseManagementTab): void {
        this.activeTab = tab;
        this.header = TAB_HEADERS[tab];
    }

    public async loadCounts(): Promise<void> {
        this.counts = await this._licenseService.getCounts();
    }

    public async refresh(): Promise<void> {
        await this.loadCounts();

        if (this.licenseList) {
            await this.licenseList.getLicenses();
        }

        if (this.insuranceList) {
            await this.insuranceList.getInsurances();
        }

        if (this.gaugeList) {
            await this.gaugeList.getGauges();
        }
    }
}
