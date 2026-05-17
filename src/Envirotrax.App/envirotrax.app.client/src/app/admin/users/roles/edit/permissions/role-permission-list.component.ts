import { Component, Input, OnInit } from "@angular/core";
import { RolePermission } from "../../../../../shared/models/users/role-permission";
import { RolePermissionService } from "../../../../../shared/services/users/role-permission.service";
import { Permission } from "../../../../../shared/models/users/permission";

@Component({
    selector: 'vp-role-permission-list',
    templateUrl: './role-permission-list.component.html',
    standalone: false
})
export class RolePermissionListComponent implements OnInit {
    public isLoading: boolean = false;
    public rolePermissions: RolePermissionVm[] = [];

    @Input()
    public roleId!: number;

    public allView: boolean = false;
    public allModify: boolean = false;
    public allDelete: boolean = false;

    constructor(
        private readonly _permissionService: RolePermissionService
    ) {

    }

    public async ngOnInit(): Promise<void> {
        await this.loadPermissions();
    }

    private async loadPermissions(): Promise<void> {
        try {
            this.isLoading = true;

            const permissions = await this._permissionService.getAllPermissions();
            const rolePermissions = await this._permissionService.getAll(this.roleId);

            this.setViewModel(permissions, rolePermissions);
        } finally {
            this.isLoading = false;
        }
    }

    private setViewModel(permissions: Permission[], rolePermissions: RolePermission[]): void {
        const groups = new Map<string, RolePermissionVm>();

        for (const permission of permissions) {
            const rolePermission = rolePermissions.find(p => p.permission?.id == permission.id) ?? {
                role: { id: this.roleId },
                permission: permission
            };

            const category = permission.category ?? 'Other';

            if (!groups.has(category)) {
                groups.set(category, { category, permissions: [] });
            }

            groups.get(category)!.permissions.push(rolePermission);
        }

        this.rolePermissions = Array.from(groups.values());

        this.setAllToggles(rolePermissions);
    }

    private setAllToggles(rolePermissions: RolePermission[]): void {
        this.allView = rolePermissions.every(r => r.canView);

        const modifiable = rolePermissions.filter(r => r.permission?.canCreate || r.permission?.canEdit);
        this.allModify = modifiable.length > 0 && modifiable.every(r => r.canCreate || r.canEdit);

        this.allDelete = rolePermissions.every(r => r.canDelete);
    }

    public getModify(rolePermission: RolePermission): boolean {
        return !!(rolePermission.canCreate || rolePermission.canEdit);
    }

    public async onModifyChange(rolePermission: RolePermission, value: boolean): Promise<void> {
        if (value) {
            if (rolePermission.permission?.canCreate) {
                rolePermission.canCreate = true;
            }
            if (rolePermission.permission?.canEdit) {
                rolePermission.canEdit = true;
            }
        } else {
            rolePermission.canCreate = false;
            rolePermission.canEdit = false;
        }
        await this.update(rolePermission);
    }

    public async update(rolePermission: RolePermission) {
        try {
            this.isLoading = true;

            await this._permissionService.addOrUpdate(rolePermission);

            const allPermissions = this.rolePermissions.flatMap(g => g.permissions);
            this.setAllToggles(allPermissions);
        } finally {
            this.isLoading = false;
        }
    }

    private toggleAll(permissionName: keyof RolePermission, newValue: boolean): void {
        for (let group of this.rolePermissions) {
            for (let permission of group.permissions) {
                permission[permissionName] = newValue
            }
        }
    }

    private async bulkUpdate(): Promise<void> {
        try {
            this.isLoading = true;

            const allPermissions = this.rolePermissions.flatMap(g => g.permissions);
            await this._permissionService.bulkUpdate(this.roleId, allPermissions);
        } finally {
            this.isLoading = false;
        }
    }

    public async toggleAllView(newValue: boolean): Promise<void> {
        this.toggleAll('canView', newValue);
        await this.bulkUpdate();
    }

    public async toggleAllModify(newValue: boolean): Promise<void> {
        for (let group of this.rolePermissions) {
            for (let permission of group.permissions) {
                if (newValue) {
                    if (permission.permission?.canCreate) {
                        permission.canCreate = true;
                    }
                    if (permission.permission?.canEdit) {
                        permission.canEdit = true;
                    }
                } else {
                    permission.canCreate = false;
                    permission.canEdit = false;
                }
            }
        }
        await this.bulkUpdate();
    }

    public async toggleAllDelete(newValue: boolean): Promise<void> {
        this.toggleAll('canDelete', newValue);
        await this.bulkUpdate();
    }
}

interface RolePermissionVm {
    category: string;
    permissions: RolePermission[];
}