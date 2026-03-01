import { Component, OnInit, ViewContainerRef } from "@angular/core";
import { TableViewModel } from "../../../../shared/models/table-view-model";
import { TableColumn } from "../../../../shared/components/data-components/table/table.component";
import { ColumnType } from "../../../../shared/components/data-components/sorting-filtering/query-view-model";
import { ModalHelperService } from "../../../../shared/services/helpers/modal-helper.service";
import { CreateRoleComponent } from "../create/create-role.component";
import { ActivatedRoute, Router } from "@angular/router";
import { ModalSize } from "@developer-partners/ngx-modal-dialog";
import { Role } from "../../../../shared/models/users/role";
import { RoleService } from "../../../../shared/services/users/role.service";

@Component({
    templateUrl: './role-list.component.html',
    standalone: false,
})
export class RoleListComponent implements OnInit {
    public table: TableViewModel<Role> = {
        columns: this.getColumns(),
        query: {
            sort: {},
            filter: []
        },
        freeTextSearch: {
            searchQuery: [
                { field: 'name' },
                { field: 'description' }
            ]
        }
    };

    public canAdd: boolean = true;
    public canEdit: boolean = true;
    public canDelete: boolean = true;

    constructor(
        private readonly _roleService: RoleService,
        private readonly _modalHelper: ModalHelperService,
        private readonly _router: Router,
        private readonly _activeRoute: ActivatedRoute,
        private readonly _viewContainerRef: ViewContainerRef
    ) {

    }

    private getColumns(): TableColumn<Role>[] {
        return [
            {
                field: 'name',
                caption: 'Name',
                type: ColumnType.text
            },
            {
                field: 'description',
                caption: 'Description',
                type: ColumnType.text
            }
        ];
    }

    public async ngOnInit(): Promise<void> {
        this.getRoles();
    }

    public async getRoles(): Promise<void> {
        try {
            this.table.isLoading = true;
            this.table.items = await this._roleService.getAll(this.table.items?.pageInfo || {}, this.table.query);
        } finally {
            this.table.isLoading = false;
        }
    }

    public add(): void {
        this._modalHelper.show<Role>(CreateRoleComponent, {
            title: 'Add Role',
            viewContainerRef: this._viewContainerRef,
            size: ModalSize.large
        }).result()
            .subscribe(role => this.edit(role));
    }

    public edit(role: Role): void {
        this._router.navigate([role.id, 'edit'], {
            relativeTo: this._activeRoute
        });
    }

    private async processDelete(role: Role): Promise<void> {
        try {
            this.table.isLoading = true;
            this._roleService.delete(role.id!);
        } finally {
            this.table.isLoading = false;
        }

        this.getRoles();
    }

    public delete(role: Role): void {
        this._modalHelper.showDeleteConfirmation()
            .result()
            .subscribe(() => this.processDelete(role));
    }
}