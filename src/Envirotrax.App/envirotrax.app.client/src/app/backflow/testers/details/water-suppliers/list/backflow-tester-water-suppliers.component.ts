import { Component, Input, OnInit, TemplateRef, ViewChild } from "@angular/core";
import { ProfessionalWaterSupplier } from "../../../../../shared/models/professionals/professional-water-supplier";
import { TableViewModel } from "../../../../../shared/models/table-view-model";
import { CellTemplateData, TableColumn } from "../../../../../shared/components/data-components/table/table.component";
import { ColumnType } from "../../../../../shared/components/data-components/sorting-filtering/query-view-model";
import { BackflowTesterWaterSuppliersService } from "../../../../../shared/services/backflow/backflow-tester-water-suppliers.service";
import { CheckboxCellComponent } from "../../../../../shared/components/data-components/table/table-cells/checkbox-cell.component";
import { CurrencyCellComponent } from "../../../../../shared/components/data-components/table/table-cells/currency-cell.component";

@Component({
    selector: 'vp-backflow-tester-water-suppliers',
    standalone: false,
    templateUrl: './backflow-tester-water-suppliers.component.html'
})
export class BackflowTesterWaterSuppliersComponent implements OnInit {
    @Input() public testerId!: number;

    public table: TableViewModel<ProfessionalWaterSupplier> = {
        columns: [],
        query: { sort: {}, filter: [] }
    };

    @ViewChild('supplierNameCell', { static: true })
    private supplierNameCellTemplate!: TemplateRef<CellTemplateData<ProfessionalWaterSupplier>>;

    constructor(private readonly _service: BackflowTesterWaterSuppliersService) { }

    public async ngOnInit(): Promise<void> {
        this.table.columns = this.getColumns();
        await this.loadWaterSuppliers();
    }

    private getColumns(): TableColumn<ProfessionalWaterSupplier>[] {
        return [
            {
                field: 'waterSupplierId',
                caption: 'Water Supplier',
                cellTemplate: this.supplierNameCellTemplate,
                type: ColumnType.text
            },
            {
                field: 'backflowCommercialTestFee',
                caption: 'Com. Fee',
                cellComponent: CurrencyCellComponent,
                type: ColumnType.number
            },
            {
                field: 'backflowResidentialTestFee',
                caption: 'Res. Fee',
                cellComponent: CurrencyCellComponent,
                type: ColumnType.number
            },
            {
                field: 'hasBackflowTesting',
                caption: 'Active',
                cellComponent: CheckboxCellComponent,
                type: ColumnType.text
            },
            {
                field: 'isBanned',
                caption: 'Suspended',
                cellComponent: CheckboxCellComponent,
                type: ColumnType.text
            }
        ];
    }

    public async loadWaterSuppliers(): Promise<void> {
        try {
            this.table.isLoading = true;
            this.table.items = await this._service.getWaterSuppliers(
                this.testerId,
                this.table.items?.pageInfo || {},
                this.table.query
            );
        } finally {
            this.table.isLoading = false;
        }
    }
}
