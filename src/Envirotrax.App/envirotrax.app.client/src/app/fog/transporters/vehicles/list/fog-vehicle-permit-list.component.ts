import { Component, OnInit, TemplateRef, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { ModalSize } from '@developer-partners/ngx-modal-dialog';
import { CellTemplateData, ColumnType, InputOption, ModalHelperService, TableColumn } from '@envirotrax/common-ui';
import { FogVehiclePermitSearch } from '../../../../shared/models/fog/fog-vehicle-permit';
import { FOG_VEHICLE_CAPACITY_TYPE_LABELS, FogVehicleInspectionDueStatus } from '../../../../shared/models/fog/fog-vehicle-enums';
import { FogVehiclePermitService } from '../../../../shared/services/fog/fog-vehicle-permit.service';
import { QueryProperty } from '../../../../shared/models/query';
import { TableViewModel } from '../../../../shared/models/table-view-model';
import { DownloadConfig } from '../../../../shared/models/download-config';
import { DownloadService } from '../../../../shared/services/download.service';
import { AuthService } from '../../../../shared/services/auth/auth.service';
import { PermissionAction, PermissionType } from '../../../../shared/models/permission-type';
import { AppContainerHelperService } from '../../../../shared/services/helpers/app-contaner-helper.service';
import { EditFogVehiclePermitComponent, FogVehiclePermitModalData } from '../edit/edit-fog-vehicle-permit.component';

interface FogVehiclePermitRowVm extends FogVehiclePermitSearch {
    transporterName: string;
    transporterAddress: string;
    transporterCityStateZip: string;
    vehicleDescription: string;
    capacityDescription: string;
    hasPermit: boolean;
    inspectionDueStatusClass: string;
}

@Component({
    standalone: false,
    templateUrl: './fog-vehicle-permit-list.component.html'
})
export class FogVehiclePermitListComponent implements OnInit {
    public showResults: boolean = false;
    public canManage: boolean = false;

    public table: TableViewModel<FogVehiclePermitRowVm> = {
        columns: [],
        query: {
            sort: {},
            filter: []
        },
        freeTextSearch: {
            searchQuery: [
                { field: 'professional.name', operator: 'Ct', multiWordSearch: true, placeholder: 'Transporter Information' },
                { field: 'licensePlateNumber', operator: 'Ct' },
                { field: 'stickerNumber', operator: 'Ct' },
                { field: 'permit.permitNumber', operator: 'Ct', placeholder: 'Permit #' }
            ]
        }
    };

    @ViewChild('transporterCell', { static: true })
    private transporterCellTemplate!: TemplateRef<CellTemplateData<FogVehiclePermitRowVm>>;

    @ViewChild('numbersCell', { static: true })
    private numbersCellTemplate!: TemplateRef<CellTemplateData<FogVehiclePermitRowVm>>;

    @ViewChild('inspectionDueDateCell', { static: true })
    private inspectionDueDateCellTemplate!: TemplateRef<CellTemplateData<FogVehiclePermitRowVm>>;

    @ViewChild('activeCell', { static: true })
    private activeCellTemplate!: TemplateRef<CellTemplateData<FogVehiclePermitRowVm>>;

    @ViewChild('permitActionCell', { static: true })
    private permitActionCellTemplate!: TemplateRef<CellTemplateData<FogVehiclePermitRowVm>>;

    public readonly activeOptions: InputOption[] = [
        { id: '', text: 'Any Value' },
        { id: 'true', text: 'Active' },
        { id: 'false', text: 'Disabled' }
    ];

    public readonly inspectionDueStatusClasses: { [key: number]: string } = {
        [FogVehicleInspectionDueStatus.None]: '',
        [FogVehicleInspectionDueStatus.Current]: 'text-bg-primary',
        [FogVehicleInspectionDueStatus.PastDue]: 'text-bg-danger'
    };

    public downloadConfig: DownloadConfig;

    constructor(
        private readonly _permitService: FogVehiclePermitService,
        private readonly _downloadService: DownloadService,
        private readonly _authService: AuthService,
        private readonly _modalHelper: ModalHelperService,
        private readonly _containerHelper: AppContainerHelperService
    ) {
        this.downloadConfig = {
            fileName: 'FOG Vehicle Permits',
            endpoint: this._permitService.getAllEndpoint(),
            suppoertedFormats: ['CSV', 'Excel'],
            columns: [
                { field: 'professional.id', caption: 'Transporter ID' },
                { field: 'professional.name', caption: 'Transporter Company Name' },
                { field: 'professional.address', caption: 'Transporter Address' },
                { field: 'professional.city', caption: 'Transporter City' },
                { field: 'professional.state.code', caption: 'Transporter State' },
                { field: 'professional.zipCode', caption: 'Transporter ZIP' },
                { field: 'professional.phoneNumber', caption: 'Transporter Phone Number' },
                { field: 'professional.faxNumber', caption: 'Transporter Fax Number' },
                { field: 'professional.companyEmail', caption: 'Transporter Email Address' },
                { field: 'licensePlateNumber', caption: 'Vehicle License Plate #' },
                { field: 'manufacturer', caption: 'Vehicle Manufacturer' },
                { field: 'manufacturedYear', caption: 'Vehicle Year' },
                { field: 'capacity', caption: 'Vehicle Capacity' },
                { field: 'capacityType', caption: 'Vehicle Capacity Type' },
                { field: 'stickerNumber', caption: 'Vehicle Sticker #' },
                { field: 'permit.permitNumber', caption: 'Permit #' },
                { field: 'permit.inspectionDueDate', caption: 'Inspection Due Date' },
                { field: 'permit.isActive', caption: 'Active' }
            ]
        };
    }

    public async ngOnInit(): Promise<void> {
        this.canManage = await this._authService.hasAnyPermisison(PermissionAction.CanModify, PermissionType.FogVehicles);
        this.table.columns = this.getColumns();
    }

    private getColumns(): TableColumn<FogVehiclePermitRowVm>[] {
        return [
            {
                field: 'professional.name',
                caption: 'Transporter Information',
                type: ColumnType.text,
                cellTemplate: this.transporterCellTemplate
            },
            // Composed on the view model, so there is no backend column to sort or filter on.
            {
                field: 'vehicleDescription',
                caption: 'Vehicle',
                type: ColumnType.text,
                queryColumnExcluded: true
            },
            {
                field: 'capacityDescription',
                caption: 'Vehicle Capacity',
                type: ColumnType.text,
                queryColumnExcluded: true
            },
            {
                field: 'licensePlateNumber',
                caption: 'License/Sticker #',
                type: ColumnType.text,
                cellTemplate: this.numbersCellTemplate
            },
            {
                field: 'permit.permitNumber',
                caption: 'Permit #',
                type: ColumnType.text
            },
            {
                field: 'permit.inspectionDueDate',
                caption: 'Inspection Due Date',
                type: ColumnType.date,
                cellTemplate: this.inspectionDueDateCellTemplate
            },
            {
                field: 'permit.isActive',
                caption: 'Active',
                type: ColumnType.other,
                cellTemplate: this.activeCellTemplate
            },
            {
                field: '',
                caption: '',
                type: ColumnType.other,
                cellTemplate: this.permitActionCellTemplate,
                queryColumnExcluded: true,
                isDownloadExcluded: true
            }
        ];
    }

    private toRowVm(result: FogVehiclePermitSearch): FogVehiclePermitRowVm {
        const capacityTypeLabel = result.capacityType != null
            ? FOG_VEHICLE_CAPACITY_TYPE_LABELS[result.capacityType]
            : '';

        const cityState = [result.professional?.city, result.professional?.state?.code]
            .filter(part => !!part)
            .join(', ');

        return {
            ...result,
            transporterName: result.professional?.name ?? '',
            transporterAddress: result.professional?.address ?? '',
            transporterCityStateZip: [cityState, result.professional?.zipCode].filter(part => !!part).join(' '),
            vehicleDescription: [result.manufacturedYear, result.manufacturer].filter(part => !!part).join(' '),
            capacityDescription: [result.capacity, capacityTypeLabel].filter(part => !!part).join(' '),
            hasPermit: !!result.permit,
            inspectionDueStatusClass: this.inspectionDueStatusClasses[
                result.inspectionDueStatus ?? FogVehicleInspectionDueStatus.None
            ]
        };
    }

    public async getVehiclePermits(): Promise<void> {
        try {
            this.table.isLoading = true;

            const results = await this._permitService.getAll(
                this.table.items?.pageInfo || {},
                this.table.query
            );

            this.table.items = {
                ...results,
                data: results.data.map(result => this.toRowVm(result))
            };
        } finally {
            this.table.isLoading = false;
        }
    }

    public onFilterChange(queryProperties: QueryProperty[]): void {
        this.table.query.filter = queryProperties.map(qp => {
            // V1 matched the permit number exactly while every other input on the page used LIKE.
            if (qp.columnName === 'permit.permitNumber') {
                return { ...qp, comparisonOperator: 'Eq' as const };
            }
            return qp;
        });
    }

    public setShowResults(visible: boolean): void {
        this.showResults = visible;
        this._containerHelper.setContainerVisibility(!visible);
    }

    public async search(searchForm: NgForm): Promise<void> {
        if (searchForm.valid) {
            await this.getVehiclePermits();
            this.setShowResults(true);
        }
    }

    public editPermit(vehicle: FogVehiclePermitRowVm): void {
        this._modalHelper.show<FogVehiclePermitModalData, FogVehiclePermitSearch>(EditFogVehiclePermitComponent, {
            title: vehicle.hasPermit ? 'Edit Permit' : 'Add Permit',
            model: { vehicle },
            size: ModalSize.large
        }).result().subscribe(() => this.getVehiclePermits());
    }

    public showDownloadManager(): void {
        this._downloadService.showDownloadManager(this.downloadConfig, this.table.query);
    }
}
