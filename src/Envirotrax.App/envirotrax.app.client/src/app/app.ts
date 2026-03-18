import { Component, signal, OnInit } from '@angular/core';
import { AuthService } from './shared/services/auth/auth.service';
import { WaterSupplierService } from './shared/services/water-suppliers/water-supplier.service';
import { ProfesisonalService } from './shared/services/professionals/professional.service';
import { createPopper, flip, preventOverflow } from '@popperjs/core';
import { FeatureType } from './shared/models/feature-tyype';
import { PermissionAction, PermissionType } from './shared/models/permission-type';

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

  constructor(
    private readonly _authService: AuthService,
    private readonly _waterSupplierService: WaterSupplierService,
    private readonly _professionalService: ProfesisonalService
  ) {

  }

  public async ngOnInit(): Promise<void> {
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

  private async createMenuItems(): Promise<MenuItem[]> {
    const professionalId = await this._authService.getProfessionalId();
    const supplierId = await this._authService.getWaterSupplierId();

    return [
      {
        title: 'Account Overview',
        iconCss: 'fa-regular fa-house',
        routerLink: ['/'],
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
            routerLink: professionalId ? ['/profile'] : ['/'],
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
            hasFeature: !!supplierId
          },
          {
            title: 'Water Supplier Management',
            iconCss: 'fa-solid fa-droplet',
            routerLink: ['professionals/water-suppliers'],
            hasPermission: true,
            hasFeature: !!professionalId
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
            routerLink: ['/'],
            hasPermission: true,
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
            routerLink: ['/'],
            hasPermission: await this._authService.hasAnyPermisison(PermissionAction.CanView, PermissionType.CsiInspectors),
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
            routerLink: ['/'],
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
            routerLink: ['/'],
            hasPermission: true,
            hasFeature: true
          },
          {
            title: 'Property Log Management',
            iconCss: 'fa-light fa-building-memo',
            routerLink: ['/'],
            hasPermission: true,
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
            routerLink: ['/'],
            hasPermission: await this._authService.hasAnyPermisison(PermissionAction.CanView, PermissionType.BackflowTests),
            hasFeature: true
          },
          {
            title: 'Out of Service Requests',
            iconCss: 'fa-regular fa-file-minus',
            routerLink: ['/'],
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
            routerLink: ['/'],
            hasPermission: await this._authService.hasAnyPermisison(PermissionAction.CanView, PermissionType.BackflowTesters),
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
            routerLink: ['/'],
            hasPermission: await this._authService.hasAnyPermisison(PermissionAction.CanView, PermissionType.BackflowReports),
            hasFeature: true
          },
          {
            title: 'Current Compliance Report',
            iconCss: 'fa-regular fa-chart-pie-simple',
            routerLink: ['/'],
            hasPermission: await this._authService.hasAnyPermisison(PermissionAction.CanView, PermissionType.BackflowReports),
            hasFeature: true
          },
          {
            title: 'Compliance History Report',
            iconCss: 'fa-solid fa-chart-line-up',
            routerLink: ['/'],
            hasPermission: await this._authService.hasAnyPermisison(PermissionAction.CanView, PermissionType.BackflowReports),
            hasFeature: true
          },
          {
            title: 'New/Removed Assemblies Report',
            iconCss: 'fa-solid fa-chart-column',
            routerLink: ['/'],
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
            routerLink: ['/'],
            hasPermission: true,
            hasFeature: true
          },
          {
            title: 'Property Log Management',
            iconCss: 'fa-light fa-building-memo',
            routerLink: ['/'],
            hasPermission: true,
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
            routerLink: ['/'],
            hasPermission: await this._authService.hasAnyPermisison(PermissionAction.CanView, PermissionType.FogInspections),
            hasFeature: await this._authService.hasAnyFeatures(FeatureType.FogInspection)
          },
          {
            title: 'Trip Ticket Search',
            iconCss: 'fa-regular fa-file-magnifying-glass',
            routerLink: ['/'],
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
            routerLink: ['/'],
            hasPermission: await this._authService.hasAnyPermisison(PermissionAction.CanView, PermissionType.FogInspectors),
            hasFeature: await this._authService.hasAnyFeatures(FeatureType.FogInspection)
          },
          {
            title: 'Transporter Management',
            iconCss: 'fa-regular fa-user',
            routerLink: ['/'],
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
            routerLink: ['/'],
            hasPermission: true,
            hasFeature: true
          }
        ]
      }
    ]
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