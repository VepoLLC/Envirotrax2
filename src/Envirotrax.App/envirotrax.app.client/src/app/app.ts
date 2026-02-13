import { Component, signal, OnInit } from '@angular/core';
import { AuthService } from './shared/services/auth/auth.service';
import { WaterSupplierService } from './shared/services/water-suppliers/water-supplier.service';
import { createPopper, flip, preventOverflow } from '@popperjs/core';

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
  public waterSupplierName: string = '';
  public userEmail: string = '';

  constructor(
    private readonly _authService: AuthService,
    private readonly _waterSupplierService: WaterSupplierService
  ) {

  }

  public async ngOnInit(): Promise<void> {
    this._authService.onLoggedIn().subscribe(async isLoggedIn => {
      this.isAuthenticated = isLoggedIn;

      if (this.isAuthenticated) {
        this.menuItems = this.createMenuItems();
        this.loadUserInfo();
      }
    });
  }

  private async loadUserInfo(): Promise<void> {
    this.userEmail = await this._authService.getUserEmail() ?? '';

    this._waterSupplierService.getCurrentSupplier().subscribe(supplier => {
      this.waterSupplierName = supplier.name ?? '';
    });
  }

  public signOut(): void {
    this._authService.signOut();
  }

  private createMenuItems(): MenuItem[] {
    return [
      {
        title: 'Account Overview',
        iconCss: 'fa-regular fa-house',
        routerLink: ['/'],
        hasPermission: true
      },
      {
        title: 'My Account',
        iconCss: 'fa-solid fa-gear',
        hasPermission: true,
        children: [
          {
            title: 'Account Contact Information',
            iconCss: 'fa-regular fa-user',
            routerLink: ['/'],
            hasPermission: true
          },
          {
            title: 'User Accounts',
            iconCss: 'fa-solid fa-users',
            routerLink: ['admin/users'],
            hasPermission: true
          },
          {
            title: 'System Settings',
            iconCss: 'fa-solid fa-gear',
            routerLink: ['admin'],
            hasPermission: true
          },
          {
            title: 'Notification Management',
            iconCss: 'fa-regular fa-bell',
            routerLink: ['/'],
            hasPermission: true
          },
          {
            title: 'GIS Area Management',
            iconCss: 'fa-solid fa-globe',
            routerLink: ['/'],
            hasPermission: true
          }
        ]
      },
      {
        title: 'CSI Management',
        iconCss: 'fa-solid fa-building-magnifying-glass',
        hasPermission: true,
        children: [
          {
            title: 'Property Record Search',
            iconCss: 'fa-regular fa-building-magnifying-glass',
            routerLink: ['sites'],
            hasPermission: true
          },
          {
            title: 'Inspection Search',
            iconCss: 'fa-regular fa-file-magnifying-glass',
            routerLink: ['/'],
            hasPermission: true
          },
          {
            type: 'separator',
            hasPermission: true
          },
          {
            title: 'Inspector Management',
            iconCss: 'fa-regular fa-user',
            routerLink: ['/'],
            hasPermission: true
          },
          {
            title: 'Letter History',
            iconCss: 'fa-regular fa-envelope',
            routerLink: ['/'],
            hasPermission: true
          },
          {
            type: 'separator',
            hasPermission: true
          },
          {
            title: 'System Reports',
            iconCss: 'fa-regular fa-chart-simple-horizontal',
            routerLink: ['/'],
            hasPermission: true
          },
          {
            type: 'separator',
            hasPermission: true
          },
          {
            title: 'Compliance Management',
            iconCss: 'fa-solid fa-list-check',
            routerLink: ['/'],
            hasPermission: true
          },
          {
            title: 'Property Log Management',
            iconCss: 'fa-light fa-building-memo',
            routerLink: ['/'],
            hasPermission: true
          }

        ]
      },
      {
        title: 'Backflow Management',
        iconCss: 'fa-regular fa-gauge',
        hasPermission: true,
        children: [
          {
            title: 'Property Record Search',
            iconCss: 'fa-regular fa-building-magnifying-glass',
            routerLink: ['sites'],
            hasPermission: true
          },
          {
            title: 'Backflow Test Search',
            iconCss: 'fa-regular fa-file-magnifying-glass',
            routerLink: ['/'],
            hasPermission: true
          },
          {
            title: 'Out of Service Requests',
            iconCss: 'fa-regular fa-file-minus',
            routerLink: ['/'],
            hasPermission: true
          },
          {
            type: 'separator',
            hasPermission: true
          },
          {
            title: 'BPAT Management',
            iconCss: 'fa-regular fa-user',
            routerLink: ['/'],
            hasPermission: true
          },
          {
            title: 'Letter History',
            iconCss: 'fa-regular fa-envelope',
            routerLink: ['/'],
            hasPermission: true
          },
          {
            type: 'separator',
            hasPermission: true
          },
          {
            title: 'Backflow Report',
            iconCss: 'fa-regular fa-chart-simple-horizontal',
            routerLink: ['/'],
            hasPermission: true
          },
          {
            title: 'Current Compliance Report',
            iconCss: 'fa-regular fa-chart-pie-simple',
            routerLink: ['/'],
            hasPermission: true
          },
          {
            title: 'Compliance History Report',
            iconCss: 'fa-solid fa-chart-line-up',
            routerLink: ['/'],
            hasPermission: true
          },
          {
            title: 'New/Removed Assemblies Report',
            iconCss: 'fa-solid fa-chart-column',
            routerLink: ['/'],
            hasPermission: true
          },
          {
            type: 'separator',
            hasPermission: true
          },
          {
            title: 'Compliance Management',
            iconCss: 'fa-solid fa-list-check',
            routerLink: ['/'],
            hasPermission: true
          },
          {
            title: 'Property Log Management',
            iconCss: 'fa-light fa-building-memo',
            routerLink: ['/'],
            hasPermission: true
          }
        ]
      },
      {
        title: 'FOG Management',
        iconCss: 'fa-regular fa-tank-water',
        hasPermission: true,
        children: [
          {
            title: 'Property Record Search',
            iconCss: 'fa-regular fa-building-magnifying-glass',
            routerLink: ['sites'],
            hasPermission: true
          },
          {
            title: 'Inspection Search',
            iconCss: 'fa-regular fa-file-magnifying-glass',
            routerLink: ['/'],
            hasPermission: true
          },
          {
            title: 'Trip Ticket Search',
            iconCss: 'fa-regular fa-file-magnifying-glass',
            routerLink: ['/'],
            hasPermission: true
          },
          {
            type: 'separator',
            hasPermission: true
          },
          {
            title: 'Inspector Management',
            iconCss: 'fa-regular fa-user',
            routerLink: ['/'],
            hasPermission: true
          },
          {
            title: 'Transporter Management',
            iconCss: 'fa-regular fa-user',
            routerLink: ['/'],
            hasPermission: true
          },
          {
            title: 'Vehicle Management',
            iconCss: 'fa-solid fa-truck',
            routerLink: ['/'],
            hasPermission: true
          },
          {
            title: 'License Management',
            iconCss: 'fa-regular fa-id-card',
            routerLink: ['/'],
            hasPermission: true
          },
          {
            type: 'separator',
            hasPermission: true
          },
          {
            title: 'System Reports',
            iconCss: 'fa-regular fa-chart-simple-horizontal',
            routerLink: ['/'],
            hasPermission: true
          },
          {
            type: 'separator',
            hasPermission: true
          },
          {
            title: 'Inspection Compliance Management',
            iconCss: 'fa-solid fa-list-check',
            routerLink: ['/'],
            hasPermission: true
          },
          {
            title: 'Permit Compliance Management',
            iconCss: 'fa-solid fa-list-check',
            routerLink: ['/'],
            hasPermission: true
          },
          {
            title: 'Trip Ticket Compliance Management',
            iconCss: 'fa-solid fa-list-check',
            routerLink: ['/'],
            hasPermission: true
          },
          {
            title: 'Property Log Management',
            iconCss: 'fa-light fa-building-memo',
            routerLink: ['/'],
            hasPermission: true
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
  isExpanded?: boolean;
  children?: MenuItem[];
  type?: 'separator';
}