import { Component, Input, OnInit } from "@angular/core";
import { ProfessionalWaterSupplier } from "../../../../../shared/models/professionals/professional-water-supplier";
import { TableViewModel } from "../../../../../shared/models/table-view-model";
import { FogTransporterWaterSuppliersService } from "../../../../../shared/services/fog/fog-transporter-water-suppliers.service";
import { AuthService } from "../../../../../shared/services/auth/auth.service";
import { PermissionAction, PermissionType } from "../../../../../shared/models/permission-type";
import { ModalSize } from "@developer-partners/ngx-modal-dialog";
import { EditFogTransporterWaterSupplierComponent, EditWaterSupplierModalData } from "../edit/edit-fog-transporter-water-supplier.component";
import { CheckboxCellComponent, ColumnType, CurrencyCellComponent, ModalHelperService, TableColumn } from "@envirotrax/common-ui";

@Component({
    selector: 'vp-fog-transporter-water-suppliers',
    standalone: false,
    templateUrl: './fog-transporter-water-suppliers.component.html'
})
export class FogTransporterWaterSuppliersComponent implements OnInit {
    @Input() public transporterId!: number;

    public canEdit: boolean = false;

    public table: TableViewModel<ProfessionalWaterSupplier> = {
        columns: [],
        query: { sort: {}, filter: [] }
    };

    constructor(
        private readonly _service: FogTransporterWaterSuppliersService,
        private readonly _authService: AuthService,
        private readonly _modalHelper: ModalHelperService
    ) { }

    public async ngOnInit(): Promise<void> {
        this.canEdit = await this._authService.hasAnyPermisison(PermissionAction.CanModify, PermissionType.FogTransporters);
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
                field: 'fogTransportFee',
                caption: 'Com. Fee',
                cellComponent: CurrencyCellComponent,
                type: ColumnType.number
            },
            {
                field: 'hasFogTransportation',
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
                this.transporterId,
                this.table.items?.pageInfo || {},
                this.table.query
            );
        } finally {
            this.table.isLoading = false;
        }
    }

    public editWaterSupplier(supplier: ProfessionalWaterSupplier): void {
        this._modalHelper.show<EditWaterSupplierModalData, ProfessionalWaterSupplier>(EditFogTransporterWaterSupplierComponent, {
            title: 'Edit Water Supplier Registration',
            model: { transporterId: this.transporterId, supplier },
            size: ModalSize.medium
        }).result().subscribe(() => this.loadWaterSuppliers());
    }
}
