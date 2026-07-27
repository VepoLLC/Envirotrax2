import { Component, OnInit } from "@angular/core";
import { AuthService } from "../../shared/services/auth/auth.service";
import { AppContainerHelperService } from "../../shared/services/helpers/app-contaner-helper.service";
import { FeatureType } from "../../shared/models/feature-type";
import { ROLE_DEFINITIONS } from "../../shared/models/role-definitions";

type TabType = 'backflow' | 'csi' | 'fogInspection' | 'fogTransport';

const TAB_LABELS: Record<TabType, string> = {
    backflow: 'Backflow Tester',
    csi: 'CSI Inspector',
    fogInspection: 'FOG Inspector',
    fogTransport: 'FOG Transporter'
};

const TAB_ICONS: Record<TabType, string> = {
    backflow: 'fa-regular fa-gauge',
    csi: 'fa-solid fa-building-magnifying-glass',
    fogInspection: 'fa-regular fa-tank-water',
    fogTransport: 'fa-regular fa-tank-water'
};

@Component({
    standalone: false,
    templateUrl: './checkout.component.html',
    styles: `
        .vp-checkout-tabs-mobile {
            display: none;
        }

        @media (max-width: 840px) {
            .vp-checkout-tabs-desktop {
                display: none !important;
            }

            .vp-checkout-tabs-mobile {
                display: inline-block;
            }
        }
    `
})
export class CheckoutComponent implements OnInit {
    public activeTab: TabType = 'backflow';
    public hasBackflowTesting = false;
    public hasCsiInspection = false;
    public hasFogInspection = false;
    public hasFogTransportation = false;
    public isAdmin = false;

    constructor(
        private readonly _authService: AuthService,
        private readonly _containerHelper: AppContainerHelperService
    ) {

    }

    public async ngOnInit(): Promise<void> {
        this._containerHelper.setContainerVisibility(false);

        [this.hasBackflowTesting, this.hasCsiInspection, this.hasFogInspection, this.hasFogTransportation, this.isAdmin] = await Promise.all([
            this._authService.hasAnyFeatures(FeatureType.BackflowTesting),
            this._authService.hasAnyFeatures(FeatureType.CsiInspection),
            this._authService.hasAnyFeatures(FeatureType.FogInspection),
            this._authService.hasAnyFeatures(FeatureType.FogTransportation),
            this._authService.hasAnyRoles(ROLE_DEFINITIONS.PROFESSIONALS.ADMIN)
        ]);

        if (this.hasBackflowTesting) {
            this.activeTab = 'backflow';
        } else if (this.hasCsiInspection) {
            this.activeTab = 'csi';
        } else if (this.hasFogInspection) {
            this.activeTab = 'fogInspection';
        } else if (this.hasFogTransportation) {
            this.activeTab = 'fogTransport';
        }
    }

    public setActiveTab(tab: TabType): void {
        this.activeTab = tab;
    }

    public getTabLabel(tab: TabType): string {
        return TAB_LABELS[tab];
    }

    public getTabIcon(tab: TabType): string {
        return TAB_ICONS[tab];
    }
}
