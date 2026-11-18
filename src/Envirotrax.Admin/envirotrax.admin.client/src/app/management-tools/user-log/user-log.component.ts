import { CommonModule } from '@angular/common';
import { Component, OnInit, TemplateRef, ViewChild } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import {
    CellTemplateData,
    ColumnType,
    FilterPanelComponent,
    InputOption,
    PagedData,
    RecordLogType,
    recordLogTypeLabels,
    TableColumn,
    TableViewModel
} from '@envirotrax/common-ui';
import { SharedComponentsModule } from '../../shared/components/shared.components.module';
import { RecordLogSearchResult, RecordLogService } from '../../shared/services/logs/record-log.service';
import { WaterSupplierService } from '../../shared/services/water-suppliers/water-supplier.service';

interface UserLogRow extends RecordLogSearchResult {
    tableNameLabel: string;
    logTypeLabel: string;
}

const TABLE_NAME_OPTIONS: InputOption[] = [
    { id: 'BackflowGauges', text: 'Backflow Gauges' },
    { id: 'BackflowTests', text: 'Backflow Tests' },
    { id: 'CsiInspections', text: 'CSI Inspections' },
    { id: 'FogDisposalSites', text: 'FOG Disposal Sites' },
    { id: 'FogInspections', text: 'FOG Inspections' },
    { id: 'FogTripTickets', text: 'FOG Trip Tickets' },
    { id: 'FogVehiclePermits', text: 'FOG Vehicle Permits' },
    { id: 'FogVehicles', text: 'FOG Vehicles' },
    { id: 'NotificationSettings', text: 'Notification Settings' },
    { id: 'ProfessionalInsurances', text: 'Professional Insurances' },
    { id: 'ProfessionalLicenses', text: 'Professional Company Licenses' },
    { id: 'ProfessionalUserLicenses', text: 'Professional User Licenses' },
    { id: 'ProfessionalUsers', text: 'Professional Users' },
    { id: 'ProfessionalWaterSupplierRegistrations', text: 'Professional Water Supplier Registrations' },
    { id: 'Sites', text: 'Sites' },
    { id: 'WaterSupplierUserAccounts', text: 'Water Supplier User Accounts' },
    { id: 'WaterSuppliers', text: 'Water Suppliers' }
];

@Component({
    templateUrl: './user-log.component.html',
    imports: [
        CommonModule,
        FormsModule,
        SharedComponentsModule
    ],
})
export class UserLogComponent implements OnInit {
    @ViewChild(FilterPanelComponent, { static: true })
    private _filterPanel!: FilterPanelComponent;

    @ViewChild('descriptionCell', { static: true })
    public descriptionCell?: TemplateRef<CellTemplateData<UserLogRow>>;

    public showResults: boolean = false;

    public readonly defaultLogType: string = String(RecordLogType.Edit);

    public table: TableViewModel<UserLogRow> = {
        query: {
            sort: {},
            filter: []
        }
    };

    public waterSupplierOptions: InputOption[] = [{ id: '', text: 'Any Value' }];

    public readonly tableNameOptions: InputOption[] = [{ id: '', text: 'Select a table' }, ...TABLE_NAME_OPTIONS];

    public readonly logTypeOptions: InputOption[] = [RecordLogType.Edit, RecordLogType.Add, RecordLogType.Delete]
        .map(type => ({ id: String(type), text: recordLogTypeLabels[type] }));

    constructor(
        private readonly _recordLogService: RecordLogService,
        private readonly _waterSupplierService: WaterSupplierService
    ) {

    }

    public async ngOnInit(): Promise<void> {
        this.table.columns = this.getColumns();

        await this.loadWaterSuppliers();
    }

    public async search(searchForm: NgForm): Promise<void> {
        if (!searchForm.valid) {
            return;
        }

        this.table.query.filter = this._filterPanel.getFilter();
        this.table.items = undefined;

        await this.getLogs();

        this.showResults = true;
    }

    public async getLogs(): Promise<void> {
        try {
            this.table.isLoading = true;

            const logs = await this._recordLogService.getAll(this.table.items?.pageInfo || {}, this.table.query);

            this.table.items = this.toRows(logs);
        } finally {
            this.table.isLoading = false;
        }
    }

    private toRows(logs: PagedData<RecordLogSearchResult>): PagedData<UserLogRow> {
        return {
            ...logs,
            data: logs.data.map(log => ({
                ...log,
                tableNameLabel: TABLE_NAME_OPTIONS.find(option => option.id === log.tableName)?.text ?? log.tableName ?? '',
                logTypeLabel: log.logType == null ? '' : recordLogTypeLabels[log.logType]
            }))
        };
    }

    private async loadWaterSuppliers(): Promise<void> {
        const options = await this._waterSupplierService.getAllAsOptions();

        this.waterSupplierOptions = [{ id: '', text: 'Any Value' }, ...options];
    }

    private getColumns(): TableColumn<UserLogRow>[] {
        return [
            { field: 'logDate', caption: 'Log Date', type: ColumnType.dateTime },
            { field: 'waterSupplierId', caption: 'WS ID', type: ColumnType.number },
            { field: 'tableNameLabel', caption: 'Table Name', type: ColumnType.text, queryColumnExcluded: true },
            { field: 'recordId', caption: 'Record ID', type: ColumnType.number },
            { field: 'user.email', caption: 'User ID', type: ColumnType.text },
            { field: 'logTypeLabel', caption: 'Type', type: ColumnType.text, queryColumnExcluded: true },
            { field: 'description', caption: 'Description', type: ColumnType.other, cellTemplate: this.descriptionCell, queryColumnExcluded: true }
        ];
    }
}
