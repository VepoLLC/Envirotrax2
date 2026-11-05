import { CommonModule } from '@angular/common';
import { Component, Input, OnInit } from '@angular/core';
import { ModalSize } from '@developer-partners/ngx-modal-dialog';
import { ColumnType, ModalHelperService, TableColumn, TableViewModel, ToastService, ToastType } from '@envirotrax/common-ui';
import { SharedComponentsModule } from '../../../../../shared/components/shared.components.module';
import { FogVehicle, fogVehicleCapacityTypeLabels } from '../../../../../shared/models/fog/fog-vehicle';
import { FogTransporterVehicleService } from '../../../../../shared/services/fog/fog-transporter-vehicle.service';
import {
    AddEditFogTransporterVehicleComponent,
    FogTransporterVehicleModalData
} from '../edit/add-edit-fog-transporter-vehicle.component';

interface FogTransporterVehicleVm extends FogVehicle {
    capacityTypeName: string;
}

@Component({
    selector: 'vp-fog-transporter-vehicles',
    templateUrl: './fog-transporter-vehicle-list.component.html',
    imports: [
        CommonModule,
        SharedComponentsModule
    ],
})
export class FogTransporterVehicleListComponent implements OnInit {
    @Input()
    public professionalId!: number;

    public table: TableViewModel<FogTransporterVehicleVm> = {
        query: {
            sort: { licensePlateNumber: 'Asc' },
            filter: []
        }
    };

    constructor(
        private readonly _vehicleService: FogTransporterVehicleService,
        private readonly _modalHelper: ModalHelperService,
        private readonly _toastService: ToastService
    ) {

    }

    public async ngOnInit(): Promise<void> {
        this.table.columns = this.getColumns();

        await this.getVehicles();
    }

    public async getVehicles(): Promise<void> {
        try {
            this.table.isLoading = true;

            const vehicles = await this._vehicleService.getAll(
                this.professionalId,
                this.table.items?.pageInfo || {},
                this.table.query
            );

            this.table.items = {
                pageInfo: vehicles.pageInfo,
                data: vehicles.data.map(vehicle => this.toViewModel(vehicle))
            };
        } finally {
            this.table.isLoading = false;
        }
    }

    public addVehicle(): void {
        this.showEditor('Add Vehicle', {});
    }

    public editVehicle(vehicle: FogVehicle): void {
        this.showEditor('Edit Vehicle', vehicle);
    }

    public deleteVehicle(vehicle: FogVehicle): void {
        this._modalHelper.showDeleteConfirmation().result().subscribe(async () => {
            try {
                this.table.isLoading = true;

                await this._vehicleService.delete(this.professionalId, vehicle.id!);

                this._toastService.successFullyDeleted('Vehicle');
            } catch (error) {
                this._toastService.show({ text: 'Failed to delete Vehicle.', type: ToastType.Error });

                return;
            } finally {
                this.table.isLoading = false;
            }

            await this.getVehicles();
        });
    }

    private showEditor(title: string, vehicle: FogVehicle): void {
        this._modalHelper.show<FogTransporterVehicleModalData, FogVehicle>(AddEditFogTransporterVehicleComponent, {
            title,
            model: { professionalId: this.professionalId, vehicle },
            size: ModalSize.medium,
            mode: 'disableFullScreen'
        }).result().subscribe(() => this.getVehicles());
    }

    private toViewModel(vehicle: FogVehicle): FogTransporterVehicleVm {
        return {
            ...vehicle,
            capacityTypeName: vehicle.capacityType != null ? fogVehicleCapacityTypeLabels[vehicle.capacityType] : ''
        };
    }

    private getColumns(): TableColumn<FogTransporterVehicleVm>[] {
        return [
            { field: 'licensePlateNumber', caption: 'License Plate #', type: ColumnType.text },
            { field: 'manufacturer', caption: 'Manufacturer', type: ColumnType.text },
            { field: 'manufacturedYear', caption: 'Year', type: ColumnType.number },
            { field: 'capacity', caption: 'Holding Capacity', type: ColumnType.number },
            { field: 'capacityTypeName', caption: 'Capacity Type', type: ColumnType.text, sortColumnExcluded: true, queryColumnExcluded: true },
            { field: 'stickerNumber', caption: 'Sticker #', type: ColumnType.text }
        ];
    }
}
