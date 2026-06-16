import { Component, Input, OnInit } from "@angular/core";
import { ProfessionalWaterSupplier } from "../../../../../shared/models/professionals/professional-water-supplier";
import { TableViewModel } from "../../../../../shared/models/table-view-model";
import { FogInspectorWaterSuppliersService } from "../../../../../shared/services/fog/fog-inspector-water-suppliers.service";
import { CheckboxCellComponent, ColumnType, CurrencyCellComponent, TableColumn } from "@envirotrax/common-ui";

@Component({
    selector: 'vp-fog-inspector-water-suppliers',
    standalone: false,
    templateUrl: './fog-inspector-water-suppliers.component.html'
})
export class FogInspectorWaterSuppliersComponent implements OnInit {
    @Input() public inspectorId!: number;

    public table: TableViewModel<ProfessionalWaterSupplier> = {
        columns: [],
        query: { sort: {}, filter: [] }
    };

    constructor(
        private readonly _service: FogInspectorWaterSuppliersService
    ) { }

    public async ngOnInit(): Promise<void> {
        this.setupColumns();
        await this.loadWaterSuppliers();
    }

    private setupColumns(): void {
        this.table.columns = this.getColumns();
    }

    private getColumns(): TableColumn<ProfessionalWaterSupplier>[] {
        return [
            {
                field: 'waterSupplier.name',
                caption: 'Water Supplier',
                type: ColumnType.text
            },
            {
                field: 'fogInspectorFee',
                caption: 'Com. Fee',
                cellComponent: CurrencyCellComponent,
                type: ColumnType.number
            },
            {
                field: 'hasFogInspection',
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
                this.inspectorId,
                this.table.items?.pageInfo || {},
                this.table.query
            );
        } finally {
            this.table.isLoading = false;
        }
    }
}
