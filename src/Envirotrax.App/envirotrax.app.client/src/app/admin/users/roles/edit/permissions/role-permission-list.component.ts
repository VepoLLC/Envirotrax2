import { Component, Input, OnInit } from "@angular/core";
import { RolePermission } from "../../../../../shared/models/users/role-permission";
import { RolePermissionService } from "../../../../../shared/services/users/role-permission.service";

@Component({
    selector: 'dp-role-permission-list',
    templateUrl: './role-permission-list.component.html',
    standalone: false
})
export class RolePermissionListComponent implements OnInit {
    public isLoading: boolean = false;
    public rolePermissions: RolePermission[] = [];

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

            for (let permission of permissions) {
                const rolePermission = rolePermissions.find(p => p.permission?.id == permission.id) ?? {
                    role: { id: this.roleId },
                    permission: permission
                };

                this.rolePermissions.push(rolePermission)
            }
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