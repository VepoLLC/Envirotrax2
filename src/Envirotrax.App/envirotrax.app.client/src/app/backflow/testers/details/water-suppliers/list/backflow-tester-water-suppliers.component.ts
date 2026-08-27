import { Component, Input, OnInit } from "@angular/core";
import { ProfessionalWaterSupplier } from "../../../../../shared/models/professionals/professional-water-supplier";
import { TableViewModel } from "../../../../../shared/models/table-view-model";
import { BackflowTesterWaterSuppliersService } from "../../../../../shared/services/backflow/backflow-tester-water-suppliers.service";
import { AuthService } from "../../../../../shared/services/auth/auth.service";
import { PermissionAction, PermissionType } from "../../../../../shared/models/permission-type";
import { ModalSize } from "@developer-partners/ngx-modal-dialog";
import { EditBackflowTesterWaterSupplierComponent, EditBackflowWaterSupplierModalData } from "../edit/edit-backflow-tester-water-supplier.component";
import { CheckboxCellComponent, ColumnType, CurrencyCellComponent, ModalHelperService, TableColumn } from "@envirotrax/common-ui";

@Component({
    selector: 'vp-backflow-tester-water-suppliers',
    standalone: false,
    templateUrl: './backflow-tester-water-suppliers.component.html'
})
export class BackflowTesterWaterSuppliersComponent implements OnInit {
    @Input() public testerId!: number;

    public canEdit: boolean = false;

    public table: TableViewModel<ProfessionalWaterSupplier> = {
        columns: [],
        query: { sort: {}, filter: [] }
    };

    constructor(
        private readonly _service: BackflowTesterWaterSuppliersService,
        private readonly _authService: AuthService,
        private readonly _modalHelper: ModalHelperService
    ) { }

    public async ngOnInit(): Promise<void> {
        this.canEdit = await this._authService.hasAnyPermisison(PermissionAction.CanModify, PermissionType.BackflowTesters);
        this.table.columns = this.getColumns();
        await this.loadWaterSuppliers();
    }

    private getColumns(): TableColumn<ProfessionalWaterSupplier>[] {
        return [
            {
                field: 'waterSupplier.name',
                caption: 'Water Supplier',
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

    public editWaterSupplier(supplier: ProfessionalWaterSupplier): void {
        this._modalHelper.show<EditBackflowWaterSupplierModalData, ProfessionalWaterSupplier>(EditBackflowTesterWaterSupplierComponent, {
            title: 'Edit Water Supplier Registration',
            model: { testerId: this.testerId, supplier },
            size: ModalSize.medium
        }).result().subscribe(() => this.loadWaterSuppliers());
    }
}
