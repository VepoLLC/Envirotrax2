import { Component, OnInit, TemplateRef, ViewChild } from "@angular/core";
import { NgForm } from "@angular/forms";
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

    // Add form state
    public editingVehicle: FogVehicle = {};
    public agreementChecked: boolean = false;
    public isFormLoading: boolean = false;
    public validationErrors: string[] = [];

    // Inline row edit state
    public editingRowId: number | null = null;
    public editingRowVehicle: FogVehicle = {};

    public readonly capacityTypeLabels = FOG_VEHICLE_CAPACITY_TYPE_LABELS;

    public getCapacityTypeLabel(capacityType: FogVehicleCapacityType | undefined): string {
        if (capacityType == null) { return ''; }
        return FOG_VEHICLE_CAPACITY_TYPE_LABELS[capacityType] ?? '';
    }
    public readonly capacityTypeOptions: InputOption[] = [
        { id: FogVehicleCapacityType.Gallons, text: FOG_VEHICLE_CAPACITY_TYPE_LABELS[FogVehicleCapacityType.Gallons] },
        { id: FogVehicleCapacityType.CubicYards, text: FOG_VEHICLE_CAPACITY_TYPE_LABELS[FogVehicleCapacityType.CubicYards] }
    ];

    @ViewChild('licensePlateCell', { static: true })
    private licensePlateCellTemplate!: TemplateRef<CellTemplateData<FogVehicle>>;

    @ViewChild('manufacturerCell', { static: true })
    private manufacturerCellTemplate!: TemplateRef<CellTemplateData<FogVehicle>>;

    @ViewChild('yearCell', { static: true })
    private yearCellTemplate!: TemplateRef<CellTemplateData<FogVehicle>>;

    @ViewChild('capacityCell', { static: true })
    private capacityCellTemplate!: TemplateRef<CellTemplateData<FogVehicle>>;

    @ViewChild('capacityTypeCell', { static: true })
    private capacityTypeCellTemplate!: TemplateRef<CellTemplateData<FogVehicle>>;

    @ViewChild('stickerNumberCell', { static: true })
    private stickerNumberCellTemplate!: TemplateRef<CellTemplateData<FogVehicle>>;

    @ViewChild('actionsCell', { static: true })
    private actionsCellTemplate!: TemplateRef<CellTemplateData<FogVehicle>>;

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

    private getColumns(): TableColumn<FogVehicle>[] {
        return [
            {
                field: 'licensePlateNumber',
                caption: 'License Plate #',
                type: ColumnType.text,
                cellTemplate: this.licensePlateCellTemplate
            },
            {
                field: 'manufacturer',
                caption: 'Manufacturer',
                type: ColumnType.text,
                cellTemplate: this.manufacturerCellTemplate
            },
            {
                field: 'manufacturedYear',
                caption: 'Year',
                type: ColumnType.number,
                cellTemplate: this.yearCellTemplate
            },
            {
                field: 'capacity',
                caption: 'Holding Capacity',
                type: ColumnType.number,
                cellTemplate: this.capacityCellTemplate
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
                type: ColumnType.text,
                cellTemplate: this.stickerNumberCellTemplate
            },
            {
                field: '',
                caption: '',
                type: ColumnType.other,
                queryColumnExcluded: true,
                isDownloadExcluded: true,
                cellTemplate: this.actionsCellTemplate
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

    // Inline row editing
    public startRowEdit(vehicle: FogVehicle): void {
        this.editingRowId = vehicle.id!;
        this.editingRowVehicle = this._helperService.copy(vehicle);
    }

    public cancelRowEdit(): void {
        this.editingRowId = null;
        this.editingRowVehicle = {};
    }

    public async saveRowEdit(): Promise<void> {
        try {
            this.table.isLoading = true;
            await this._vehicleService.update(this.editingRowVehicle.id!, this.editingRowVehicle);
            this._toastService.successfullySaved('Vehicle');
            this.editingRowId = null;
            this.editingRowVehicle = {};
            await this.loadVehicles();
        } catch (error) {
            if (!this._helperService.parseValidationErrors(error, this.validationErrors)) {
                throw error;
            }
            this._toastService.failedToSave('Vehicle');
        } finally {
            this.table.isLoading = false;
        }
    }

    // Add form
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
}
