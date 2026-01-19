import { Component, OnInit } from "@angular/core";


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

    public async ngOnInit(): Promise<void> {
        this.menuItems = await this.createMenuItems();
    }

    private async createMenuItems(): Promise<MenuItem[]> {
        return [
            {
                title: 'Water Suppliers',
                iconCss: 'fa-solid fa-building',
                routerLink: ['water-suppliers'],
                hasPermission: true,
                description: 'Manage all water suppliers, their details, and configurations.'
            },
            {
                title: 'Users',
                iconCss: 'fa-solid fa-users',
                routerLink: ['users'],
                hasPermission: true,
                description: 'Manage users of your system.'
            },
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