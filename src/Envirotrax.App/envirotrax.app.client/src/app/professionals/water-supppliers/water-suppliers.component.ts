import { Component, OnInit } from "@angular/core";
import { WaterSupplier } from "../../shared/models/water-suppliers/water-supplier";
import { TableViewModel } from "../../shared/models/table-view-model";
import { ProfessionalSupplierService } from "../../shared/services/professionals/professional-supplier.service";
import { TableColumn } from "../../shared/components/data-components/table/table.component";
import { ColumnType } from "../../shared/components/data-components/sorting-filtering/query-view-model";
import { ProfessionalWaterSupplier } from "../../shared/models/professionals/professional-water-supplier";

@Component({
    standalone: false,
    templateUrl: './water-suppliers.component.html',
    styles: `
        .vp-requirement-icon {
            max-width: 25px;
        }
    `
})
export class WaterSuppliersComponent implements OnInit {
    public suppliers: TableViewModel<ProfessionalSupplierVm> = {
        query: {
            sort: {},
            filter: []
        },
        freeTextSearch: {
            searchQuery: [
                { field: 'name', operator: 'Ct', placeholder: 'Water Supplier Name' }
            ]
        }
    };

    constructor(
        private readonly _professionalSupplierService: ProfessionalSupplierService
    ) {

    }

    public async ngOnInit(): Promise<void> {
        this.suppliers.columns = this.getColumns();
        await this.getSuppliers();
    }

    public async getSuppliers(): Promise<void> {
        try {
            this.suppliers.isLoading = true;

            const allSuppliers = await this._professionalSupplierService.getAll(this.suppliers.items?.pageInfo || {}, this.suppliers.query);
            const mySuppliers = await this._professionalSupplierService.getAllMy();

            this.suppliers.items = {
                pageInfo: allSuppliers.pageInfo,
                data: allSuppliers
                    .data
                    .map(supplier => ({
                        supplier: supplier,
                        selected: mySuppliers.find(s => s.waterSupplier?.id == supplier.id)
                    }))
            };
        } finally {
            this.suppliers.isLoading = false;
        }
    }

    private getColumns(): TableColumn<ProfessionalSupplierVm>[] {
        return [
            {
                field: 'name',
                caption: 'Water Supplier',
                type: ColumnType.text
            },
            {
                field: 'requirements',
                caption: 'Requirements',
                type: ColumnType.other
            }
        ]
    }
}

interface ProfessionalSupplierVm {
    supplier: WaterSupplier;
    selected?: ProfessionalWaterSupplier
}