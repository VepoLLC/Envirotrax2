import { Component, Input, OnInit, TemplateRef, ViewChild } from "@angular/core";
import { FogVehicle } from "../../../../../shared/models/fog/fog-vehicle";
import { FogVehicleCapacityType, FOG_VEHICLE_CAPACITY_TYPE_LABELS } from "../../../../../shared/models/fog/fog-vehicle-enums";
import { FogTransporterVehiclesService } from "../../../../../shared/services/fog/fog-transporter-vehicles.service";
import { TableViewModel } from "../../../../../shared/models/table-view-model";
import { AuthService } from "../../../../../shared/services/auth/auth.service";
import { PermissionAction, PermissionType } from "../../../../../shared/models/permission-type";
import { ModalSize } from "@developer-partners/ngx-modal-dialog";
import { EditFogTransporterVehicleComponent, FogVehicleModalData } from "../edit/edit-fog-transporter-vehicle.component";
import { CellTemplateData, ColumnType, ModalHelperService, TableColumn, ToastService } from "@envirotrax/common-ui";

@Component({
    selector: 'vp-fog-transporter-vehicles',
    standalone: false,
    templateUrl: './fog-transporter-vehicles.component.html'
})
export class FogTransporterVehiclesComponent implements OnInit {
    @Input() public transporterId!: number;

    public canManage: boolean = false;

    public table: TableViewModel<FogVehicle> = {
        columns: [],
        query: { sort: {}, filter: [] }
    };

    @ViewChild('capacityTypeCell', { static: true })
    private capacityTypeCellTemplate!: TemplateRef<CellTemplateData<FogVehicle>>;

    constructor(
        private readonly _vehicleService: FogTransporterVehiclesService,
        private readonly _authService: AuthService,
        private readonly _modalHelper: ModalHelperService,
        private readonly _toastService: ToastService
    ) { }

    public async ngOnInit(): Promise<void> {
        this.canManage = await this._authService.hasAnyPermisison(PermissionAction.CanModify, PermissionType.FogTransporters);
        this.table.columns = this.getColumns();
        await this.loadVehicles();
    }

    public getCapacityTypeLabel(capacityType?: FogVehicleCapacityType): string {
        if (capacityType === undefined || capacityType === null) {
            return '';
        }
        return FOG_VEHICLE_CAPACITY_TYPE_LABELS[capacityType] ?? '';
    }

    private getColumns(): TableColumn<FogVehicle>[] {
        return [
            {
                field: 'licensePlateNumber',
                caption: 'License Plate #',
                type: ColumnType.text
            },
            {
                field: 'manufacturer',
                caption: 'Manufacturer',
                type: ColumnType.text
            },
            {
                field: 'manufacturedYear',
                caption: 'Year',
                type: ColumnType.number
            },
            {
                field: 'capacity',
                caption: 'Holding Capacity',
                type: ColumnType.number
            },
            {
                field: 'capacityType',
                caption: 'Capacity Type',
                type: ColumnType.text,
                cellTemplate: this.capacityTypeCellTemplate
            },
            {
                field: 'stickerNumber',
                caption: 'Sticker #',
                type: ColumnType.text
            }
        ];
    }

    public addVehicle(): void {
        this._modalHelper.show<FogVehicleModalData, FogVehicle>(EditFogTransporterVehicleComponent, {
            title: 'Add Vehicle',
            model: { transporterId: this.transporterId, vehicle: {} },
            size: ModalSize.large
        }).result().subscribe(() => this.loadVehicles());
    }

    public editVehicle(vehicle: FogVehicle): void {
        this._modalHelper.show<FogVehicleModalData, FogVehicle>(EditFogTransporterVehicleComponent, {
            title: 'Edit Vehicle',
            model: { transporterId: this.transporterId, vehicle },
            size: ModalSize.large
        }).result().subscribe(() => this.loadVehicles());
    }

    public deleteVehicle(vehicle: FogVehicle): void {
        this._modalHelper.showDeleteConfirmation().result().subscribe(async () => {
            try {
                this.table.isLoading = true;
                await this._vehicleService.delete(this.transporterId, vehicle.id!);
                this._toastService.successFullyDeleted('Vehicle');
            } finally {
                this.table.isLoading = false;
            }
            await this.loadVehicles();
        });
    }

    public async loadVehicles(): Promise<void> {
        try {
            this.table.isLoading = true;
            this.table.items = await this._vehicleService.getVehicles(
                this.transporterId,
                this.table.items?.pageInfo || {},
                this.table.query
            );
        } finally {
            this.table.isLoading = false;
        }
    }
}
