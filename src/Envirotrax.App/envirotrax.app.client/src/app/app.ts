import { Component, signal, OnInit } from '@angular/core';
import { AuthService } from './shared/services/auth/auth.service';
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

  constructor(private readonly _authService: AuthService) {

  }

  public async ngOnInit(): Promise<void> {
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
          }
        ]
      },
      {
        title: 'CSI Management',
        iconCss: 'fa-regular fa-building',
        routerLink: [''],
        hasPermission: true
      }
    ]
  }

  public toggleExpanded(e: Event, buttonElement: HTMLElement, dropdownElement: HTMLElement, menuItem: MenuItem) {
    if (menuItem.children && menuItem.children.length > 0) {
      if (menuItem.isExpanded) {
        menuItem.isExpanded = false;
      } else {
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
  title: string;
  routerLink?: string[];
  iconCss: string;
  hasPermission: boolean;
  isExpanded?: boolean;
  children?: MenuItem[]
}