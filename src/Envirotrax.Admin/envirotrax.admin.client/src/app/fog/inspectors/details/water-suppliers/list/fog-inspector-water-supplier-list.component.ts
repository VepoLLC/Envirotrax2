import { CommonModule } from '@angular/common';
import { Component, Input, OnInit } from '@angular/core';
import { ModalSize } from '@developer-partners/ngx-modal-dialog';
import {
    CheckboxCellComponent,
    ColumnType,
    CurrencyCellComponent,
    ModalHelperService,
    TableColumn,
    TableViewModel
} from '@envirotrax/common-ui';
import { SharedComponentsModule } from '../../../../../shared/components/shared.components.module';
import { ProfessionalWaterSupplier } from '../../../../../shared/models/professionals/professional-water-supplier';
import { FogInspectorWaterSupplierService } from '../../../../../shared/services/fog/fog-inspector-water-supplier.service';
import {
    EditFogInspectorWaterSupplierComponent,
    FogInspectorWaterSupplierModalData
} from '../edit/edit-fog-inspector-water-supplier.component';

@Component({
    selector: 'vp-fog-inspector-water-suppliers',
    templateUrl: './fog-inspector-water-supplier-list.component.html',
    imports: [
        CommonModule,
        SharedComponentsModule
    ],
})
export class FogInspectorWaterSupplierListComponent implements OnInit {
    @Input()
    public professionalId!: number;

    public table: TableViewModel<ProfessionalWaterSupplier> = {
        query: {
            sort: { 'waterSupplier.name': 'Asc' },
            filter: []
        }
    };

    constructor(
        private readonly _supplierService: FogInspectorWaterSupplierService,
        private readonly _modalHelper: ModalHelperService
    ) {

    }

    public async ngOnInit(): Promise<void> {
        this.table.columns = this.getColumns();

        await this.getRegistrations();
    }

    public async getRegistrations(): Promise<void> {
        try {
            this.table.isLoading = true;
            this.table.items = await this._supplierService.getAll(
                this.professionalId,
                this.table.items?.pageInfo || {},
                this.table.query
            );
        } finally {
            this.table.isLoading = false;
        }
    }

    public editRegistration(registration: ProfessionalWaterSupplier): void {
        this._modalHelper.show<FogInspectorWaterSupplierModalData, ProfessionalWaterSupplier>(EditFogInspectorWaterSupplierComponent, {
            title: 'Edit Water Supplier Registration',
            model: { professionalId: this.professionalId, registration },
            size: ModalSize.medium,
            mode: 'disableFullScreen'
        }).result().subscribe(() => this.getRegistrations());
    }

    private getColumns(): TableColumn<ProfessionalWaterSupplier>[] {
        return [
            { field: 'waterSupplier.name', caption: 'Water Supplier', type: ColumnType.text },
            { field: 'fogInspectorFee', caption: 'Fee', cellComponent: CurrencyCellComponent, type: ColumnType.number },
            { field: 'hasFogInspection', caption: 'Active', cellComponent: CheckboxCellComponent, type: ColumnType.text },
            { field: 'isFogInspectionSuspended', caption: 'Suspended', cellComponent: CheckboxCellComponent, type: ColumnType.text }
        ];
    }
}
