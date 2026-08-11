import { CommonModule } from "@angular/common";
import { Component, OnInit } from "@angular/core";
import { ModalReference } from "@developer-partners/ngx-modal-dialog";
import { ColumnType, PagedData, TableColumn, TableViewModel } from "@envirotrax/common-ui";
import { WaterSupplier } from "../../models/water-suppliers/water-supplier";
import { WaterSupplierService } from "../../services/water-suppliers/water-supplier.service";
import { SharedComponentsModule } from "../shared.components.module";

@Component({
    templateUrl: './water-supplier-lookup.component.html',
    standalone: true,
    imports: [CommonModule, SharedComponentsModule]
})
export class WaterSupplierLookupComponent implements OnInit {
    public table: TableViewModel<WaterSupplier> = {
        query: {
            sort: { name: 'Asc' },
            filter: []
        },
        freeTextSearch: {
            searchQuery: [
                { field: 'name' },
                { field: 'pwsId' },
                { field: 'city' }
            ]
        }
    };

    constructor(
        private readonly _waterSupplierService: WaterSupplierService,
        private readonly _modalReference: ModalReference<PagedData<WaterSupplier>, WaterSupplier>
    ) {

    }

    public ngOnInit(): void {
        this.table.columns = this.getColumns();
        this.table.items = this._modalReference.config.model;
    }

    public async getSuppliers(): Promise<void> {
        try {
            this.table.isLoading = true;
            this.table.items = await this._waterSupplierService.getAll(this.table.items?.pageInfo || {}, this.table.query);
        } finally {
            this.table.isLoading = false;
        }
    }

    public select(supplier: WaterSupplier): void {
        this._modalReference.closeSuccess(supplier);
    }

    private getColumns(): TableColumn<WaterSupplier>[] {
        return [
            { field: 'id', caption: 'ID', type: ColumnType.number },
            { field: 'name', caption: 'Name', type: ColumnType.text },
            { field: 'pwsId', caption: 'PWS ID', type: ColumnType.text },
            { field: 'city', caption: 'City', type: ColumnType.text },
            { field: 'state.code', caption: 'ST', type: ColumnType.text }
        ];
    }
}
