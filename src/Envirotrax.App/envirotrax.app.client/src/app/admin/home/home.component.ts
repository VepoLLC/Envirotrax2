import { Component, OnInit } from "@angular/core";
import { AuthService } from "../../shared/services/auth/auth.service";
import { PermissionAction, PermissionType } from "../../shared/models/permission-type";


@Component({
    templateUrl: './home.component.html',
    standalone: false,
    styles: `
        .vp-box-icon {
            font-size: 2rem;
        }
    `
})
export class HomeComponent implements OnInit {
    public menuItems: MenuItem[] = [];
    public searchText: string = '';

    constructor(private readonly _authService: AuthService) { }

    public async ngOnInit(): Promise<void> {
        this.menuItems = await this.createMenuItems();
    }

    private async createMenuItems(): Promise<MenuItem[]> {
        return [
            {
                title: 'Water Suppliers',
                iconCss: 'fa-solid fa-building',
                routerLink: ['water-suppliers'],
                hasPermission: await this._authService.hasAnyPermisison(PermissionAction.CanView, PermissionType.WaterSuppliers),
                description: 'Manage all water suppliers, their details, and configurations.'
            },
            {
                title: 'General Settings & Fees',
                iconCss: 'fa-solid fa-gear',
                routerLink: ['settings'],
                hasPermission: await this._authService.hasAnyPermisison(PermissionAction.CanView, PermissionType.Settings),
                description: 'Manage program settings and submissions fees.'
            },
            {
                title: 'Users',
                iconCss: 'fa-solid fa-users',
                routerLink: ['users'],
                hasPermission: await this._authService.hasAnyPermisison(PermissionAction.CanView, PermissionType.Users),
                description: 'Manage users of your system.'
            },
            {
                title: 'Roles',
                iconCss: 'fa-solid fa-sitemap',
                routerLink: ['users/roles'],
                hasPermission: await this._authService.hasAnyPermisison(PermissionAction.CanView, PermissionType.Roles),
                description: 'Create and assign roles to control permissions and access.'
            }
        ];
    }

    public onSearch(text: any): void {
        if (!text) {
            for (const menuItem of this.menuItems) {
                menuItem.isHidden = false;
            }
        }

        for (const menuItem of this.menuItems) {
            if (!menuItem.title.toLowerCase().includes(text.toLowerCase())) {
                menuItem.isHidden = true;
            }
        }
    }
}

interface MenuItem {
    title: string;
    routerLink: string[];
    iconCss: string;
    hasPermission: boolean;
    description?: string;
    isHidden?: boolean
}