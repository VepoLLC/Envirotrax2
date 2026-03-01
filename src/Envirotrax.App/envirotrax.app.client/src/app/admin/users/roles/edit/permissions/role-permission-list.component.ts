import { Component, Input, OnInit } from "@angular/core";
import { RolePermission } from "../../../../../shared/models/users/role-permission";
import { RolePermissionService } from "../../../../../shared/services/users/role-permission.service";

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
        } finally {
            this.isLoading = false;
        }
    }

    public async update(rolePermission: RolePermission) {
        try {
            this.isLoading = true;

            await this._permissionService.addOrUpdate(rolePermission);
        } finally {
            this.isLoading = false;
        }
    }
}

interface RolePermissionVm {
    category: string;
    permissions: RolePermission[];
}