import { Component, Input, OnInit, TemplateRef, ViewChild } from "@angular/core";
import { ProfessionalWaterSupplier } from "../../../../../shared/models/professionals/professional-water-supplier";
import { TableViewModel } from "../../../../../shared/models/table-view-model";
import { CellTemplateData, TableColumn } from "../../../../../shared/components/data-components/table/table.component";
import { ColumnType } from "../../../../../shared/components/data-components/sorting-filtering/query-view-model";
import { CsiInspectorWaterSuppliersService } from "../../../../../shared/services/csi/csi-inspector-water-suppliers.service";
import { CheckboxCellComponent } from "../../../../../shared/components/data-components/table/table-cells/checkbox-cell.component";
import { CurrencyCellComponent } from "../../../../../shared/components/data-components/table/table-cells/currency-cell.component";
import { AuthService } from "../../../../../shared/services/auth/auth.service";
import { PermissionAction, PermissionType } from "../../../../../shared/models/permission-type";
import { ModalHelperService } from "../../../../../shared/services/helpers/modal-helper.service";
import { ModalSize } from "@developer-partners/ngx-modal-dialog";
import { EditCsiInspectorWaterSupplierComponent, EditWaterSupplierModalData } from "../edit/edit-csi-inspector-water-supplier.component";

@Component({
    selector: 'vp-csi-inspector-water-suppliers',
    standalone: false,
    templateUrl: './csi-inspector-water-suppliers.component.html'
})
export class CsiInspectorWaterSuppliersComponent implements OnInit {
    @Input() public inspectorId!: number;

    public canEdit: boolean = false;

    public table: TableViewModel<ProfessionalWaterSupplier> = {
        columns: [],
        query: { sort: {}, filter: [] }
    };

    @ViewChild('supplierNameCell', { static: true })
    private supplierNameCellTemplate!: TemplateRef<CellTemplateData<ProfessionalWaterSupplier>>;

    constructor(
        private readonly _service: CsiInspectorWaterSuppliersService,
        private readonly _authService: AuthService,
        private readonly _modalHelper: ModalHelperService
    ) { }

    public async ngOnInit(): Promise<void> {
        this.canEdit = await this._authService.hasAnyPermisison(PermissionAction.CanEdit, PermissionType.CsiInspectors);
        this.setupColumns();
        await this.loadWaterSuppliers();
    }

    private setupColumns(): void {
        this.table.columns = this.getColumns();
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
                field: 'csiCommercialInspectionFee',
                caption: 'Com. Fee',
                cellComponent: CurrencyCellComponent,
                type: ColumnType.number
            },
            {
                field: 'csiResidentialInspectionFee',
                caption: 'Res. Fee',
                cellComponent: CurrencyCellComponent,
                type: ColumnType.number
            },
            {
                field: 'hasCsiInspection',
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

    public editWaterSupplier(supplier: ProfessionalWaterSupplier): void {
        this._modalHelper.show<EditWaterSupplierModalData, ProfessionalWaterSupplier>(EditCsiInspectorWaterSupplierComponent, {
            title: 'Edit Water Supplier Registration',
            model: { inspectorId: this.inspectorId, supplier },
            size: ModalSize.medium
        }).result().subscribe(() => this.loadWaterSuppliers());
    }

}
