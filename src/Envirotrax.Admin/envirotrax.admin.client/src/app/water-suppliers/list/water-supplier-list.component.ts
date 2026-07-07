import { Component } from '@angular/core';
import { ColumnType, TableColumn, TableViewModel } from '@envirotrax/common-ui';
import { WaterSupplier } from '../../shared/models/water-suppliers/water-supplier';
import { WaterSupplierService } from '../../shared/services/water-suppliers/water-supplier.service';

@Component({
    templateUrl: './water-supplier-list.component.html',
    standalone: false,
})
export class WaterSupplierListComponent {
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

    constructor(private readonly _waterSupplierService: WaterSupplierService) {

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
}
