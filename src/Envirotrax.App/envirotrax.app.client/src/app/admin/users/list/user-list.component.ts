import { Component, OnInit } from "@angular/core";
import { TableViewModel } from "../../../shared/models/table-view-model";
import { WaterSupplierUser } from "../../../shared/models/users/water-supplier-user";
import { UserService } from "../../../shared/services/water-suppliers/user.service";
import { ActivatedRoute, Router } from "@angular/router";
import { ModalHelperService } from "../../../shared/services/helpers/modal-helper.service";
import { TableColumn } from "../../../shared/components/data-components/table/table.component";
import { ColumnType } from "../../../shared/components/data-components/sorting-filtering/query-view-model";
import { CreateUserComponent } from "../create/create-user.component";
import { AuthService } from "../../../shared/services/auth/auth.service";
import { PermissionAction, PermissionType } from "../../../shared/models/permission-type";

@Component({
    standalone: false,
    templateUrl: './user-list.component.html'
})
export class UserListComponent implements OnInit {
    public table: TableViewModel<WaterSupplierUser> = {
        columns: this.getColumns(),
        query: {
            sort: {},
            filter: []
        },
        freeTextSearch: {
            searchQuery: [
                { field: 'contactName', operator: 'Ct', multiWordSearch: true },
                { field: 'emailAddress', operator: 'Ct' }
            ]
        }
    };

    public canAdd: boolean = false;
    public canEdit: boolean = false;
    public canDelete: boolean = false;

    constructor(
        private readonly _userService: UserService,
        private readonly _router: Router,
        private readonly _activatedRoute: ActivatedRoute,
        private readonly _modalHelper: ModalHelperService,
        private readonly _authService: AuthService
    ) {

    }

    public async ngOnInit(): Promise<void> {
        this.canAdd = await this._authService.hasAnyPermisison(PermissionAction.CanCreate, PermissionType.Users);
        this.canEdit = await this._authService.hasAnyPermisison(PermissionAction.CanEdit, PermissionType.Users);
        this.canDelete = await this._authService.hasAnyPermisison(PermissionAction.CanDelete, PermissionType.Users);

        await this.getUsers();
    }

    private getColumns(): TableColumn<WaterSupplierUser>[] {
        return [
            {
                field: 'contactName',
                caption: 'Contact Name',
                type: ColumnType.text
            },
            {
                field: 'emailAddress',
                caption: 'Email',
                type: ColumnType.text
            }
        ];
    }

    public async getUsers(): Promise<void> {
        try {
            this.table.isLoading = true;
            this.table.items = await this._userService.getAll(this.table.items?.pageInfo || {}, this.table.query);
        } finally {
            this.table.isLoading = false;
        }
    }

    public add(): void {
        this._modalHelper.show<WaterSupplierUser>(CreateUserComponent, {
            title: 'Create User',
        }).result()
            .subscribe(user => this.edit(user));
    }

    public edit(user: WaterSupplierUser): void {
        this._router.navigate([user.id, 'edit'], {
            relativeTo: this._activatedRoute
        });
    }

    private async processDelete(user: WaterSupplierUser): Promise<void> {
        try {
            this.table.isLoading = true;
            await this._userService.delete(user.id!);
        } finally {
            this.table.isLoading = false;
        }

        this.getUsers();
    }

    public delete(user: WaterSupplierUser): void {
        this._modalHelper.showDeleteConfirmation()
            .result()
            .subscribe(() => this.processDelete(user));
    }
}