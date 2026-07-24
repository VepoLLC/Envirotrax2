import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { AuthService } from './shared/services/auth/auth.service';
import { WaterSupplierService } from './shared/services/water-suppliers/water-supplier.service';
import { ProfesisonalService } from './shared/services/professionals/professional.service';
import { ThemeCookieService } from './shared/services/helpers/theme-cookie.service';
import { createPopper } from '@popperjs/core';
import { FeatureType } from './shared/models/feature-type';
import { PermissionAction, PermissionType } from './shared/models/permission-type';
import { ROLE_DEFINITIONS } from './shared/models/role-definitions';
import { AppContainerHelperService } from './shared/services/helpers/app-contaner-helper.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.html',
  standalone: false,
  styleUrl: './app.css'
})
export class App implements OnInit {
  public isAuthenticated: boolean = false;
  public menuItems: MenuItem[] = [];
  public isNavbarVisible: boolean = false;
  public companyName: string = '';
  public userEmail: string = '';
  public isDarkMode: boolean = false;
  public useContainer: boolean = false;

  constructor(
    private readonly _authService: AuthService,
    private readonly _waterSupplierService: WaterSupplierService,
    private readonly _professionalService: ProfesisonalService,
    private readonly _themeCookie: ThemeCookieService,
    private readonly _changeDetector: ChangeDetectorRef,
    appContainerHelper: AppContainerHelperService
  ) {
    appContainerHelper.usContainer().subscribe(value => {
      this.useContainer = value;
      this._changeDetector.detectChanges();
    });
  }

  public async ngOnInit(): Promise<void> {
    this.isDarkMode = this._themeCookie.get() === 'dark';

    if (this.isDarkMode) {
      document.body.classList.add('vp-dark-theme');
    }

    this._authService.onLoggedIn().subscribe(async isLoggedIn => {
      this.isAuthenticated = isLoggedIn;

      if (this.isAuthenticated) {
        this.menuItems = await this.createMenuItems();
        await this.loadUserInfo();
      }
    });
  }

  private async loadUserInfo(): Promise<void> {
    this.userEmail = await this._authService.getUserEmail() ?? '';

    const professionalId = await this._authService.getProfessionalId();

    if (professionalId) {
      const professional = await this._professionalService.getLoggedInProfessional();
      this.companyName = professional.name ?? '';
    } else {
      const supplier = await this._waterSupplierService.getLoggedInSupplier();
      this.companyName = supplier.name ?? '';
    }
  }

  public signOut(): void {
    this._authService.signOut();
  }

  public toggleDarkMode(): void {
    this.isDarkMode = !this.isDarkMode;

    if (this.isDarkMode) {
      document.body.classList.add('vp-dark-theme');
      this._themeCookie.set('dark');
    } else {
      document.body.classList.remove('vp-dark-theme');
      this._themeCookie.set('light');
    }
  }

  private async createMenuItems(): Promise<MenuItem[]> {
    const professionalId = await this._authService.getProfessionalId();
    return professionalId
      ? this.createProfessionalMenuItems()
      : this.createWaterSupplierMenuItems();
  }

  private async createWaterSupplierMenuItems(): Promise<MenuItem[]> {
    const hasLicenseAccess = await this._authService.hasAnyPermisison(PermissionAction.CanView, PermissionType.Licenses)
      || await this._authService.hasAnyFeatures(FeatureType.ManageProfessionalLicenses);

    return [
      {
        title: 'Account Overview',
        iconCss: 'fa-regular fa-house',
        routerLink: ['/account-overview'],
        hasFeature: true,
        hasPermission: true
      },
      {
        title: 'My Account',
        iconCss: 'fa-solid fa-gear',
        hasPermission: true,
        hasFeature: true,
        children: [
          {
            title: 'Account Contact Information',
            iconCss: 'fa-regular fa-user',
            routerLink: ['/'],
            hasPermission: true,
            hasFeature: true
          },
          {
            title: 'User Accounts',
            iconCss: 'fa-solid fa-users',
            routerLink: ['admin/users'],
            hasPermission: await this._authService.hasAnyPermisison(PermissionAction.CanView, PermissionType.Users),
            hasFeature: true
          },
          {
            title: 'System Settings',
            iconCss: 'fa-solid fa-gear',
            routerLink: ['admin'],
            hasPermission: await this._authService.hasAnyPermisison(PermissionAction.CanView, PermissionType.Settings),
            hasFeature: true
          },
          {
            title: 'Notification Management',
            iconCss: 'fa-regular fa-bell',
            routerLink: ['/'],
            hasPermission: await this._authService.hasAnyPermisison(PermissionAction.CanView, PermissionType.Notifications),
            hasFeature: true
          },
          {
            title: 'GIS Area Management',
            iconCss: 'fa-solid fa-globe',
            routerLink: ['admin/gis-areas'],
            hasPermission: await this._authService.hasAnyPermisison(PermissionAction.CanView, PermissionType.Settings),
            hasFeature: true
          }
        ]
      },
      {
        title: 'CSI Management',
        iconCss: 'fa-solid fa-building-magnifying-glass',
        hasPermission: true,
        hasFeature: await this._authService.hasAnyFeatures(FeatureType.CsiInspection),
        children: [
          {
            title: 'Property Record Search',
            iconCss: 'fa-regular fa-building-magnifying-glass',
            routerLink: ['sites'],
            hasPermission: await this._authService.hasAnyPermisison(PermissionAction.CanView, PermissionType.Sites),
            hasFeature: true
          },
          {
            title: 'Inspection Search',
            iconCss: 'fa-regular fa-file-magnifying-glass',
            routerLink: ['csi'],
            hasPermission: await this._authService.hasAnyPermisison(PermissionAction.CanView, PermissionType.CsiInspections),
            hasFeature: true
          },
          {
            type: 'separator',
            hasPermission: await this._authService.hasAnyPermisison(PermissionAction.CanView, PermissionType.Sites, PermissionType.CsiInspections),
            hasFeature: true
          },
          {
            title: 'Inspector Management',
            iconCss: 'fa-regular fa-user',
            routerLink: ['csi/inspectors'],
            hasPermission: await this._authService.hasAnyPermisison(PermissionAction.CanView, PermissionType.CsiInspectors),
            hasFeature: true
          },
          {
            title: 'License Management',
            iconCss: 'fa-regular fa-id-card',
            routerLink: ['licenses'],
            hasPermission: hasLicenseAccess,
            hasFeature: true
          },
          {
            title: 'Letter History',
            iconCss: 'fa-regular fa-envelope',
            routerLink: ['/'],
            hasPermission: true,
            hasFeature: true
          },
          {
            type: 'separator',
            hasPermission: true,
            hasFeature: true
          },
          {
            title: 'System Reports',
            iconCss: 'fa-regular fa-chart-simple-horizontal',
            routerLink: ['csi/reports/system'],
            hasPermission: await this._authService.hasAnyPermisison(PermissionAction.CanView, PermissionType.CsiReports),
            hasFeature: true
          },
          {
            type: 'separator',
            hasPermission: await this._authService.hasAnyPermisison(PermissionAction.CanView, PermissionType.CsiReports),
            hasFeature: false
          },
          {
            title: 'Compliance Management',
            iconCss: 'fa-solid fa-list-check',
            routerLink: ['csi/compliance'],
            hasPermission: await this._authService.hasAnyPermisison(PermissionAction.CanView, PermissionType.CsiReports),
            hasFeature: true
          },
          {
            title: 'Property Log Management',
            iconCss: 'fa-light fa-building-memo',
            routerLink: ['sites/reports/property-log-management'],
            hasPermission: await this._authService.hasAnyPermisison(PermissionAction.CanView, PermissionType.CsiReports, PermissionType.BackflowReports, PermissionType.FogReports),
            hasFeature: true
          }
        ]
      },
      {
        title: 'Backflow Management',
        iconCss: 'fa-regular fa-gauge',
        hasPermission: true,
        hasFeature: await this._authService.hasAnyFeatures(FeatureType.BackflowTesting),
        children: [
          {
            title: 'Property Record Search',
            iconCss: 'fa-regular fa-building-magnifying-glass',
            routerLink: ['sites'],
            hasPermission: await this._authService.hasAnyPermisison(PermissionAction.CanView, PermissionType.Sites),
            hasFeature: true
          },
          {
            title: 'Backflow Test Search',
            iconCss: 'fa-regular fa-file-magnifying-glass',
            routerLink: ['backflow/tests'],
            hasPermission: await this._authService.hasAnyPermisison(PermissionAction.CanView, PermissionType.BackflowTests),
            hasFeature: true
          },
          {
            title: 'Out of Service Requests',
            iconCss: 'fa-regular fa-file-minus',
            routerLink: ['backflow', 'out-of-service'],
            hasPermission: await this._authService.hasAnyPermisison(PermissionAction.CanView, PermissionType.BackflowOutOfService),
            hasFeature: true
          },
          {
            type: 'separator',
            hasPermission: await this._authService.hasAnyPermisison(PermissionAction.CanView, PermissionType.Sites, PermissionType.BackflowTests, PermissionType.BackflowOutOfService),
            hasFeature: true
          },
          {
            title: 'BPAT Management',
            iconCss: 'fa-regular fa-user',
            routerLink: ['backflow/testers'],
            hasPermission: await this._authService.hasAnyPermisison(PermissionAction.CanView, PermissionType.BackflowTesters),
            hasFeature: true
          },
          {
            title: 'License Management',
            iconCss: 'fa-regular fa-id-card',
            routerLink: ['licenses'],
            hasPermission: hasLicenseAccess,
            hasFeature: true
          },
          {
            title: 'Letter History',
            iconCss: 'fa-regular fa-envelope',
            routerLink: ['/'],
            hasPermission: true,
            hasFeature: true
          },
          {
            type: 'separator',
            hasPermission: await this._authService.hasAnyPermisison(PermissionAction.CanView, PermissionType.BackflowTesters),
            hasFeature: true
          },
          {
            title: 'Backflow Report',
            iconCss: 'fa-regular fa-chart-simple-horizontal',
            routerLink: ['backflow/reports', 'test-reports'],
            hasPermission: await this._authService.hasAnyPermisison(PermissionAction.CanView, PermissionType.BackflowReports),
            hasFeature: true
          },
          {
            title: 'Current Compliance Report',
            iconCss: 'fa-regular fa-chart-pie-simple',
            routerLink: ['backflow/reports', 'current-compliance'],
            hasPermission: await this._authService.hasAnyPermisison(PermissionAction.CanView, PermissionType.BackflowReports),
            hasFeature: true
          },
          {
            title: 'Compliance History Report',
            iconCss: 'fa-solid fa-chart-line-up',
            routerLink: ['backflow/reports', 'compliance-history'],
            hasPermission: await this._authService.hasAnyPermisison(PermissionAction.CanView, PermissionType.BackflowReports),
            hasFeature: true
          },
          {
            title: 'New/Removed Assemblies Report',
            iconCss: 'fa-solid fa-chart-column',
            routerLink: ['backflow/reports', 'new-removed'],
            hasPermission: await this._authService.hasAnyPermisison(PermissionAction.CanView, PermissionType.BackflowReports),
            hasFeature: true
          },
          {
            type: 'separator',
            hasPermission: true,
            hasFeature: true
          },
          {
            title: 'Compliance Management',
            iconCss: 'fa-solid fa-list-check',
            routerLink: ['backflow/compliance'],
            hasPermission: await this._authService.hasAnyPermisison(PermissionAction.CanView, PermissionType.BackflowReports),
            hasFeature: true
          },
          {
            title: 'Property Log Management',
            iconCss: 'fa-light fa-building-memo',
            routerLink: ['sites/reports/property-log-management'],
            hasPermission: await this._authService.hasAnyPermisison(PermissionAction.CanView, PermissionType.CsiReports, PermissionType.BackflowReports, PermissionType.FogReports),
            hasFeature: true
          }
        ]
      },
      {
        title: 'FOG Management',
        iconCss: 'fa-regular fa-tank-water',
        hasPermission: true,
        hasFeature: await this._authService.hasAnyFeatures(FeatureType.FogInspection, FeatureType.FogTransportation),
        children: [
          {
            title: 'Property Record Search',
            iconCss: 'fa-regular fa-building-magnifying-glass',
            routerLink: ['sites'],
            hasPermission: await this._authService.hasAnyPermisison(PermissionAction.CanView, PermissionType.Sites),
            hasFeature: true
          },
          {
            title: 'Inspection Search',
            iconCss: 'fa-regular fa-file-magnifying-glass',
            routerLink: ['/fog/inspections'],
            hasPermission: await this._authService.hasAnyPermisison(PermissionAction.CanView, PermissionType.FogInspections),
            hasFeature: await this._authService.hasAnyFeatures(FeatureType.FogInspection)
          },
          {
            title: 'Trip Ticket Search',
            iconCss: 'fa-regular fa-file-magnifying-glass',
            routerLink: ['/fog/trip-tickets'],
            hasPermission: await this._authService.hasAnyPermisison(PermissionAction.CanView, PermissionType.FogTripTickets),
            hasFeature: await this._authService.hasAnyFeatures(FeatureType.FogTransportation)
          },
          {
            type: 'separator',
            hasPermission: await this._authService.hasAnyPermisison(PermissionAction.CanView, PermissionType.Sites, PermissionType.FogInspections, PermissionType.FogTripTickets),
            hasFeature: true
          },
          {
            title: 'Inspector Management',
            iconCss: 'fa-regular fa-user',
            routerLink: ['/fog/inspectors'],
            hasPermission: await this._authService.hasAnyPermisison(PermissionAction.CanView, PermissionType.FogInspectors),
            hasFeature: await this._authService.hasAnyFeatures(FeatureType.FogInspection)
          },
          {
            title: 'Transporter Management',
            iconCss: 'fa-regular fa-user',
            routerLink: ['/fog/transporters'],
            hasPermission: await this._authService.hasAnyPermisison(PermissionAction.CanView, PermissionType.FogTransporters),
            hasFeature: await this._authService.hasAnyFeatures(FeatureType.FogTransportation)
          },
          {
            title: 'Vehicle Management',
            iconCss: 'fa-solid fa-truck',
            routerLink: ['/'],
            hasPermission: await this._authService.hasAnyPermisison(PermissionAction.CanView, PermissionType.FogVehicles),
            hasFeature: await this._authService.hasAnyFeatures(FeatureType.FogTransportation)
          },
          {
            title: 'License Management',
            iconCss: 'fa-regular fa-id-card',
            routerLink: ['licenses'],
            hasPermission: hasLicenseAccess,
            hasFeature: true
          },
          {
            type: 'separator',
            hasPermission: true,
            hasFeature: true
          },
          {
            title: 'System Reports',
            iconCss: 'fa-regular fa-chart-simple-horizontal',
            routerLink: ['/'],
            hasPermission: await this._authService.hasAnyPermisison(PermissionAction.CanView, PermissionType.FogReports),
            hasFeature: true
          },
          {
            type: 'separator',
            hasPermission: await this._authService.hasAnyPermisison(PermissionAction.CanView, PermissionType.FogReports),
            hasFeature: true
          },
          {
            title: 'Inspection Compliance Management',
            iconCss: 'fa-solid fa-list-check',
            routerLink: ['/'],
            hasPermission: true,
            hasFeature: await this._authService.hasAnyFeatures(FeatureType.FogInspection)
          },
          {
            title: 'Permit Compliance Management',
            iconCss: 'fa-solid fa-list-check',
            routerLink: ['/'],
            hasPermission: true,
            hasFeature: true
          },
          {
            title: 'Trip Ticket Compliance Management',
            iconCss: 'fa-solid fa-list-check',
            routerLink: ['/'],
            hasPermission: true,
            hasFeature: await this._authService.hasAnyFeatures(FeatureType.FogTransportation)
          },
          {
            title: 'Property Log Management',
            iconCss: 'fa-light fa-building-memo',
            routerLink: ['sites/reports/property-log-management'],
            hasPermission: await this._authService.hasAnyPermisison(PermissionAction.CanView, PermissionType.CsiReports, PermissionType.BackflowReports, PermissionType.FogReports),
            hasFeature: true
          }
        ]
      }
    ];
  }

  private async createProfessionalMenuItems(): Promise<MenuItem[]> {
    const isAdmin = await this._authService.hasAnyRoles(ROLE_DEFINITIONS.PROFESSIONALS.ADMIN);
    const isCsiInspector = await this._authService.hasAnyRoles(ROLE_DEFINITIONS.PROFESSIONALS.CSI_INSPECTOR);
    const isBackflowTester = await this._authService.hasAnyRoles(ROLE_DEFINITIONS.PROFESSIONALS.BACKFLOW_TESTER);
    const isFogInspector = await this._authService.hasAnyRoles(ROLE_DEFINITIONS.PROFESSIONALS.FOG_INSPECTOR);
    const isFogTransporter = await this._authService.hasAnyRoles(ROLE_DEFINITIONS.PROFESSIONALS.FOG_TRANSPORTER);

    return [
      {
        title: 'Account Overview',
        iconCss: 'fa-regular fa-house',
        routerLink: ['/professionals/account-overview'],
        hasFeature: true,
        hasPermission: true
      },
      {
        title: 'My Account',
        iconCss: 'fa-solid fa-gear',
        hasPermission: true,
        hasFeature: true,
        children: [
          {
            title: 'Account Contact Information',
            iconCss: 'fa-regular fa-user',
            routerLink: ['/profile'],
            hasPermission: true,
            hasFeature: true
          },
          {
            title: 'User Accounts',
            iconCss: 'fa-solid fa-users',
            routerLink: ['professionals/users'],
            hasPermission: isAdmin,
            hasFeature: true
          },
          {
            title: 'Water Supplier Management',
            iconCss: 'fa-solid fa-droplet',
            routerLink: ['professionals/water-suppliers'],
            hasPermission: isAdmin,
            hasFeature: true
          },
          {
            title: 'Licenses & Insurance Policies',
            iconCss: 'fa-solid fa-shield-halved',
            routerLink: ['professionals/insurances'],
            hasPermission: isAdmin,
            hasFeature: true
          },
          {
            title: 'Gauge Management',
            iconCss: 'fa-solid fa-gauge-simple',
            routerLink: ['professionals/backflow/gauges'],
            hasPermission: isBackflowTester,
            hasFeature: await this._authService.hasAnyFeatures(FeatureType.BackflowTesting)
          },
          {
            title: 'Vehicle Management',
            iconCss: 'fa-solid fa-truck',
            routerLink: ['professionals/fog/transportation/vehicles'],
            hasPermission: isFogTransporter,
            hasFeature: await this._authService.hasAnyFeatures(FeatureType.FogTransportation)
          },
          {
            title: 'Disposal Site Management',
            iconCss: 'fa-solid fa-location-dot',
            routerLink: ['professionals/fog/transportation/disposal-sites'],
            hasPermission: isFogTransporter,
            hasFeature: await this._authService.hasAnyFeatures(FeatureType.FogTransportation)
          }
        ]
      },
      {
        title: 'CSI Management',
        iconCss: 'fa-solid fa-building-magnifying-glass',
        hasPermission: isCsiInspector,
        hasFeature: await this._authService.hasAnyFeatures(FeatureType.CsiInspection),
        children: [
          {
            title: 'Property Record Search',
            iconCss: 'fa-regular fa-building-magnifying-glass',
            routerLink: ['professionals/sites'],
            hasPermission: isCsiInspector,
            hasFeature: true
          },
          {
            title: 'Inspection Search',
            iconCss: 'fa-regular fa-file-magnifying-glass',
            routerLink: ['professionals/csi/inspections'],
            hasPermission: isCsiInspector,
            hasFeature: true
          },
          {
            title: 'Submit CSI',
            iconCss: 'fa-regular fa-file-plus',
            routerLink: ['professionals/csi/inspections/create'],
            hasPermission: isCsiInspector,
            hasFeature: true
          }
        ]
      },
      {
        title: 'Backflow Management',
        iconCss: 'fa-regular fa-gauge',
        hasPermission: isBackflowTester,
        hasFeature: await this._authService.hasAnyFeatures(FeatureType.BackflowTesting),
        children: [
          {
            title: 'Property Record Search',
            iconCss: 'fa-regular fa-building-magnifying-glass',
            routerLink: ['professionals/sites'],
            hasPermission: isBackflowTester,
            hasFeature: true
          },
          {
            title: 'Backflow Test Search',
            iconCss: 'fa-regular fa-file-magnifying-glass',
            routerLink: ['professionals/backflow/tests'],
            hasPermission: isBackflowTester,
            hasFeature: true
          },
          {
            title: 'Submit Backflow Test',
            iconCss: 'fa-regular fa-file-plus',
            routerLink: ['professionals/backflow/submit'],
            hasPermission: isBackflowTester,
            hasFeature: true
          }
        ]
      },
      {
        title: 'FOG Management',
        iconCss: 'fa-regular fa-tank-water',
        hasPermission: isFogInspector,
        hasFeature: await this._authService.hasAnyFeatures(FeatureType.FogInspection),
        children: [
          {
            title: 'Property Record Search',
            iconCss: 'fa-regular fa-building-magnifying-glass',
            routerLink: ['professionals/sites'],
            hasPermission: isFogInspector,
            hasFeature: true
          },
          {
            title: 'Inspection Search',
            iconCss: 'fa-regular fa-file-magnifying-glass',
            routerLink: ['professionals/fog/inspections'],
            hasPermission: isFogInspector,
            hasFeature: true
          },
          {
            title: 'Trip Ticket Search',
            iconCss: 'fa-regular fa-file-magnifying-glass',
            routerLink: ['professionals/fog/trip-tickets'],
            hasPermission: isFogTransporter,
            hasFeature: true
          },
          {
            title: 'Submit Inspection',
            iconCss: 'fa-regular fa-file-plus',
            routerLink: ['professionals/fog/inspections/create'],
            hasPermission: isFogInspector,
            hasFeature: true
          }
        ]
      }
    ];
  }

  public toggleExpanded(e: Event, buttonElement: HTMLElement, dropdownElement: HTMLElement, menuItem: MenuItem) {
    if (menuItem.children && menuItem.children.length > 0) {
      if (menuItem.isExpanded) {
        menuItem.isExpanded = false;
      } else {
        // Close all other menu items
        this.menuItems.forEach(item => {
          if (item !== menuItem) {
            item.isExpanded = false;
          }
        });

        menuItem.isExpanded = true;

        e.stopPropagation();

        setTimeout((() => {
          const popper = createPopper(buttonElement, dropdownElement, {
            strategy: "fixed",
            placement: 'bottom'
          });
        }), 0);
      }
    }
  }
}

interface MenuItem {
  title?: string;
  routerLink?: string[];
  iconCss?: string;
  hasPermission: boolean;
  hasFeature: boolean;
  isExpanded?: boolean;
  children?: MenuItem[];
  type?: 'separator';
}
