import { Component, OnInit } from "@angular/core";
import { TableViewModel } from "../../../shared/models/table-view-model";
import { WaterSupplier } from "../../../shared/models/water-suppliers/water-supplier";
import { WaterSupplierService } from "../../../shared/services/water-suppliers/water-supplier.service";
import { TableColumn } from "../../../shared/components/data-components/table/table.component";
import { ActivatedRoute, Router } from "@angular/router";
import { ModalHelperService } from "../../../shared/services/helpers/modal-helper.service";
import { ColumnType } from "../../../shared/components/data-components/sorting-filtering/query-view-model";
import { CreateSupplierComponent } from "../create/create-supplier.component";
import { AuthService } from "../../../shared/services/auth/auth.service";
import { PermissionAction, PermissionType } from "../../../shared/models/permission-type";

@Component({
    templateUrl: './supplier-list.component.html',
    standalone: false
})
export class SupplierListComponent implements OnInit {
    public table: TableViewModel<WaterSupplier> = {
        columns: this.getColumns(),
        query: {
            sort: {},
            filter: []
        },
        freeTextSearch: {
            searchQuery: [
                { field: 'name', operator: 'Ct' },
                { field: 'parent.name', operator: 'Ct', multiWordSearch: true }
            ]
        }
    };

    public canAdd: boolean = false;
    public canEdit: boolean = false;
    public canDelete: boolean = false;

    constructor(
        private readonly _waterSupplierService: WaterSupplierService,
        private readonly _router: Router,
        private readonly _activatedRoute: ActivatedRoute,
        private readonly _modalHelper: ModalHelperService,
        private readonly _authService: AuthService
    ) {

    }

    private getColumns(): TableColumn<WaterSupplier>[] {
        return [
            {
                field: 'name',
                caption: 'Name',
                type: ColumnType.text
            },
            {
                field: 'parent.name',
                caption: 'Parent',
                type: ColumnType.text
            }
        ];
    }

    public async ngOnInit(): Promise<void> {
        this.canAdd = await this._authService.hasAnyPermisison(PermissionAction.CanCreate, PermissionType.WaterSuppliers);
        this.canEdit = await this._authService.hasAnyPermisison(PermissionAction.CanEdit, PermissionType.WaterSuppliers);
        this.canDelete = await this._authService.hasAnyPermisison(PermissionAction.CanDelete, PermissionType.WaterSuppliers);

        await this.getSuppliers();
    }

    public async getSuppliers(): Promise<void> {
        try {
            this.table.isLoading = true;
            this.table.items = await this._waterSupplierService.getAll(this.table.items?.pageInfo || {}, this.table.query);
        } finally {
            this.table.isLoading = false;
        }
    }

    public add(): void {
        this._modalHelper.show<WaterSupplier>(CreateSupplierComponent, {
            title: 'Create Water Supplier',
        }).result()
            .subscribe(supplier => this.edit(supplier));
    }

    public edit(supplier: WaterSupplier): void {
        this._router.navigate([supplier.id, 'edit'], {
            relativeTo: this._activatedRoute
        });
    }
}