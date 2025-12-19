import { Component } from "@angular/core";
import { TableViewModel } from "../../../shared/models/table-view-model";
import { WaterSupplier } from "../../../shared/models/water-suppliers/water-supplier";
import { WaterSupplierService } from "../../../shared/services/water-suppliers/water-supplier.service";
import { TableColumn } from "../../../shared/components/data-components/table/table.component";
import { ActivatedRoute, Router } from "@angular/router";
import { ModalHelperService } from "../../../shared/services/helpers/modal-helper.service";
import { ColumnType } from "../../../shared/components/data-components/sorting-filtering/query-view-model";

@Component({
    templateUrl: './supplier-list.component.html',
    standalone: false
})
export class SupplierListComponent {
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

    constructor(
        private readonly _waterSupplierService: WaterSupplierService,
        private readonly _router: Router,
        private readonly _activatedRoute: ActivatedRoute,
        private readonly _modalHelper: ModalHelperService
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

    }

    public edit(supplier: WaterSupplier): void {
        this._router.navigate([supplier.id, 'edit'], {
            relativeTo: this._activatedRoute
        });
    }
}