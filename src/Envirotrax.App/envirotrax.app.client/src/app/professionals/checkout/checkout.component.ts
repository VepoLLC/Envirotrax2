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
    public canViewBackflowTesting = false;
    public canViewCsiInspection = false;
    public canViewFogInspection = false;
    public canViewFogTransportation = false;
    public isAdmin = false;

    constructor(
        private readonly _authService: AuthService,
        private readonly _containerHelper: AppContainerHelperService
    ) {

    }

    public async ngOnInit(): Promise<void> {
        this._containerHelper.setContainerVisibility(false);

        const [
            hasBackflowTesting,
            hasCsiInspection,
            hasFogInspection,
            hasFogTransportation,
            isAdmin,
            isBackflowTester,
            isCsiInspector,
            isFogInspector,
            isFogTransporter
        ] = await Promise.all([
            this._authService.hasAnyFeatures(FeatureType.BackflowTesting),
            this._authService.hasAnyFeatures(FeatureType.CsiInspection),
            this._authService.hasAnyFeatures(FeatureType.FogInspection),
            this._authService.hasAnyFeatures(FeatureType.FogTransportation),
            this._authService.hasAnyRoles(ROLE_DEFINITIONS.PROFESSIONALS.ADMIN),
            this._authService.hasAnyRoles(ROLE_DEFINITIONS.PROFESSIONALS.BACKFLOW_TESTER),
            this._authService.hasAnyRoles(ROLE_DEFINITIONS.PROFESSIONALS.CSI_INSPECTOR),
            this._authService.hasAnyRoles(ROLE_DEFINITIONS.PROFESSIONALS.FOG_INSPECTOR),
            this._authService.hasAnyRoles(ROLE_DEFINITIONS.PROFESSIONALS.FOG_TRANSPORTER)
        ]);

        // Having the feature enabled isn't enough - the professional also needs the matching role,
        // otherwise the backend correctly 403s the tab's checkout calls.
        this.isAdmin = isAdmin;
        this.canViewBackflowTesting = hasBackflowTesting && isBackflowTester;
        this.canViewCsiInspection = hasCsiInspection && isCsiInspector;
        this.canViewFogInspection = hasFogInspection && isFogInspector;
        this.canViewFogTransportation = hasFogTransportation && isFogTransporter;

        if (this.canViewBackflowTesting) {
            this.activeTab = 'backflow';
        } else if (this.canViewCsiInspection) {
            this.activeTab = 'csi';
        } else if (this.canViewFogInspection) {
            this.activeTab = 'fogInspection';
        } else if (this.canViewFogTransportation) {
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
