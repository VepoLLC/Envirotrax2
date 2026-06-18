import { Component, OnInit, TemplateRef, ViewChild } from "@angular/core";
import { NgForm } from "@angular/forms";
import { ModalSize } from "@developer-partners/ngx-modal-dialog";
import { FogVehicle } from "../../../../../shared/models/fog/fog-vehicle";
import { FogVehicleCapacityType, FOG_VEHICLE_CAPACITY_TYPE_LABELS } from "../../../../../shared/models/fog/fog-vehicle-enums";
import { ProfessionalFogVehicleService } from "../../../../../shared/services/fog/professional-fog-vehicle.service";
import { TableViewModel } from "../../../../../shared/models/table-view-model";
import { CellTemplateData, TableColumn } from "../../../../../shared/components/data-components/table/table.component";
import { ColumnType } from "../../../../../shared/components/data-components/sorting-filtering/query-view-model";
import { InputOption } from "../../../../../shared/components/input/input.component";
import { ToastService } from "../../../../../shared/services/toast.service";
import { HelperService } from "../../../../../shared/services/helpers/helper.service";
import { ModalHelperService } from "../../../../../shared/services/helpers/modal-helper.service";
import { EditFogVehicleComponent } from "../edit/edit-fog-vehicle.component";

@Component({
    standalone: false,
    templateUrl: './professional-fog-vehicle-list.component.html'
})
export class ProfessionalFogVehicleListComponent implements OnInit {
    public table: TableViewModel<FogVehicle> = {
        columns: [],
        query: {
            sort: {},
            filter: []
        }
    };

    public editingVehicle: FogVehicle = {};
    public agreementChecked: boolean = false;
    public isFormLoading: boolean = false;
    public validationErrors: string[] = [];

    public readonly capacityTypeOptions: InputOption[] = [
        { id: FogVehicleCapacityType.Gallons, text: FOG_VEHICLE_CAPACITY_TYPE_LABELS[FogVehicleCapacityType.Gallons] },
        { id: FogVehicleCapacityType.CubicYards, text: FOG_VEHICLE_CAPACITY_TYPE_LABELS[FogVehicleCapacityType.CubicYards] }
    ];

    @ViewChild('capacityTypeCell', { static: true })
    private capacityTypeCellTemplate!: TemplateRef<CellTemplateData<FogVehicle>>;

    constructor(
        private readonly _vehicleService: ProfessionalFogVehicleService,
        private readonly _toastService: ToastService,
        private readonly _helperService: HelperService,
        private readonly _modalHelper: ModalHelperService
    ) {}

    public ngOnInit(): void {
        this.table.columns = this.getColumns();
        this.loadVehicles();
    }

    public getCapacityTypeLabel(capacityType: FogVehicleCapacityType | undefined): string {
        if (capacityType == null) { return ''; }
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

    public async loadVehicles(): Promise<void> {
        try {
            this.table.isLoading = true;
            this.table.items = await this._vehicleService.getAll(
                this.table.items?.pageInfo || {},
                this.table.query
            );
        } finally {
            this.table.isLoading = false;
        }
    }

    public edit(vehicle: FogVehicle): void {
        this._modalHelper.show<FogVehicle>(EditFogVehicleComponent, {
            title: 'Edit Vehicle',
            model: vehicle,
            size: ModalSize.large
        }).result().subscribe(() => this.loadVehicles());
    }

    public deleteVehicle(vehicle: FogVehicle): void {
        this._modalHelper.showDeleteConfirmation()
            .result()
            .subscribe(async () => {
                try {
                    this.table.isLoading = true;
                    await this._vehicleService.delete(vehicle.id!);
                    this._toastService.successFullyDeleted('Vehicle');
                    await this.loadVehicles();
                } finally {
                    this.table.isLoading = false;
                }
            });
    }

    public async saveVehicle(form: NgForm): Promise<void> {
        if (form.valid) {
            try {
                this.isFormLoading = true;
                this.validationErrors = [];
                await this._vehicleService.add(this.editingVehicle);
                this._toastService.successfullySaved('Vehicle');
                this.editingVehicle = {};
                this.agreementChecked = false;
                form.resetForm();
                await this.loadVehicles();
            } catch (error) {
                if (!this._helperService.parseValidationErrors(error, this.validationErrors)) {
                    throw error;
                }
                this._toastService.failedToSave('Vehicle');
            } finally {
                this.isFormLoading = false;
            }
        }
    }
}
