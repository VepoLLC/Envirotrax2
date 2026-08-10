import { Component, OnInit } from '@angular/core';
import { AuthService } from './shared/services/auth/auth.service';
import { createPopper } from '@popperjs/core';
import { WindowService } from './shared/services/window.service';
import { WaterSupplierListComponent } from './water-suppliers/list/water-supplier-list.component';
import { SiteListComponent } from './sites/list/site-list.component';
import { CsiInspectionListComponent } from './csi/inspections/list/csi-inspection-list.component';
import { BackflowTestListComponent } from './backflow/tests/list/backflow-test-list.component';

@Component({
  selector: 'app-root',
  templateUrl: './app.html',
  standalone: false,
  styleUrl: './app.css'
})
export class App implements OnInit {
  public isAuthenticated: boolean = false;
  public isNavbarVisible: boolean = false;
  public menuItems: MenuItem[] = [];

  constructor(
    private readonly _authService: AuthService,
    private readonly _windowService: WindowService
  ) {

  }

  public ngOnInit(): void {
    this._authService.onLoggedIn().subscribe(async isLoggedIn => {
      this.isAuthenticated = isLoggedIn;

      if (this.isAuthenticated) {
        this.menuItems = this.createMenuItems();
      }
    });
  }

  private createMenuItems(): MenuItem[] {
    return [
      {
        title: 'Property Search',
        iconCss: 'fa-solid fa-house',
        onClick: this.showPropertySearch.bind(this)
      },
      {
        title: 'Water Suppliers',
        iconCss: 'fa-solid fa-droplet',
        onClick: this.showWaterSuppliers.bind(this)
      },
      {
        title: 'CSI Management',
        iconCss: 'fa-solid fa-clipboard-check',
        children: [
          {
            title: 'Inspection Search',
            iconCss: 'fa-solid fa-magnifying-glass',
            onClick: this.showCsiInspectionSearch.bind(this)
          }
        ]
      },
      {
        title: 'Backflow Management',
        iconCss: 'fa-solid fa-gauge',
        children: [
          {
            title: 'Backflow Test Search',
            iconCss: 'fa-solid fa-magnifying-glass',
            onClick: this.showBackflowTestSearch.bind(this)
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

    if (menuItem.onClick) {
      menuItem.onClick();
    }
  }

  public showWaterSuppliers(): void {
    this._windowService.addWindow(WaterSupplierListComponent, {
      title: 'Water Suppliers',
      model: {
        name: 'Test'
      }
    });
  }

  public showPropertySearch(): void {
    this._windowService.addWindow(SiteListComponent, {
      title: 'Property Search'
    });
  }

  public showCsiInspectionSearch(): void {
    this._windowService.addWindow(CsiInspectionListComponent, {
      title: 'CSI Search'
    });
  }

  public showBackflowTestSearch(): void {
    this._windowService.addWindow(BackflowTestListComponent, {
      title: 'Backflow Test Search'
    });
  }
}

interface MenuItem {
  title?: string;
  iconCss?: string;
  isExpanded?: boolean;
  children?: MenuItem[];
  type?: 'separator';
  onClick?: () => void;
}

