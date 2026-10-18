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
import { BackflowTesterWaterSupplierService } from '../../../../../shared/services/backflow/backflow-tester-water-supplier.service';
import {
    BackflowTesterWaterSupplierModalData,
    EditBackflowTesterWaterSupplierComponent
} from '../edit/edit-backflow-tester-water-supplier.component';

@Component({
    selector: 'vp-backflow-tester-water-suppliers',
    templateUrl: './backflow-tester-water-supplier-list.component.html',
    imports: [
        CommonModule,
        SharedComponentsModule
    ],
})
export class BackflowTesterWaterSupplierListComponent implements OnInit {
    @Input()
    public professionalId!: number;

    public table: TableViewModel<ProfessionalWaterSupplier> = {
        query: {
            sort: { 'waterSupplier.name': 'Asc' },
            filter: []
        }
    };

    constructor(
        private readonly _supplierService: BackflowTesterWaterSupplierService,
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
        this._modalHelper.show<BackflowTesterWaterSupplierModalData, ProfessionalWaterSupplier>(EditBackflowTesterWaterSupplierComponent, {
            title: 'Edit Water Supplier Registration',
            model: { professionalId: this.professionalId, registration },
            size: ModalSize.medium,
            mode: 'disableFullScreen'
        }).result().subscribe(() => this.getRegistrations());
    }

    // Same columns and shared cell components as the water-supplier BPAT page's registration list.
    private getColumns(): TableColumn<ProfessionalWaterSupplier>[] {
        return [
            { field: 'waterSupplier.name', caption: 'Water Supplier', type: ColumnType.text },
            { field: 'backflowCommercialTestFee', caption: 'Comm Fee', cellComponent: CurrencyCellComponent, type: ColumnType.number },
            { field: 'backflowResidentialTestFee', caption: 'Res Fee', cellComponent: CurrencyCellComponent, type: ColumnType.number },
            { field: 'hasBackflowTesting', caption: 'Active', cellComponent: CheckboxCellComponent, type: ColumnType.text },
            { field: 'isBackflowTestingSuspended', caption: 'Suspended', cellComponent: CheckboxCellComponent, type: ColumnType.text }
        ];
    }
}
