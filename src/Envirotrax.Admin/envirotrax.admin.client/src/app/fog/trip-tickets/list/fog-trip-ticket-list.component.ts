import { CommonModule } from '@angular/common';
import { Component, OnInit, TemplateRef, ViewChild } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import {
    CellTemplateData,
    ColumnType,
    DateRange,
    InputOption,
    QueryProperty,
    SortOperator,
    TableColumn,
    TableViewModel
} from '@envirotrax/common-ui';
import {
    InterceptorCapacityType,
    interceptorCapacityTypeLabels,
    InterceptorType
} from '../../../shared/models/fog/fog-inspection';
import {
    FogTripTicket,
    FogTripTicketDateField,
    FogTripTicketRow,
    FogTripTicketStatus
} from '../../../shared/models/fog/fog-trip-ticket';
import { PropertyType } from '../../../shared/models/sites/site';
import {
    FogTripTicketSearchCriteria,
    FogTripTicketService
} from '../../../shared/services/fog/fog-trip-ticket.service';
import { SharedComponentsModule } from '../../../shared/components/shared.components.module';
import { WaterSupplierService } from '../../../shared/services/water-suppliers/water-supplier.service';

const CRITERIA_FIELDS: (keyof FogTripTicketSearchCriteria)[] = [
    'userEmail'
];

@Component({
    templateUrl: './fog-trip-ticket-list.component.html',
    imports: [
        CommonModule,
        FormsModule,
        SharedComponentsModule
    ],
})
export class FogTripTicketListComponent implements OnInit {
    @ViewChild('statusCell', { static: true })
    public statusCell?: TemplateRef<CellTemplateData<FogTripTicketRow>>;

    @ViewChild('generatorCell', { static: true })
    public generatorCell?: TemplateRef<CellTemplateData<FogTripTicketRow>>;

    @ViewChild('transporterCell', { static: true })
    public transporterCell?: TemplateRef<CellTemplateData<FogTripTicketRow>>;

    @ViewChild('receiverCell', { static: true })
    public receiverCell?: TemplateRef<CellTemplateData<FogTripTicketRow>>;

    @ViewChild('wasteCell', { static: true })
    public wasteCell?: TemplateRef<CellTemplateData<FogTripTicketRow>>;

    public readonly propertyType = PropertyType;

    public showResults: boolean = false;

    public dateSearchField: string = '';

    public dateRange?: DateRange;

    private panelFilter: QueryProperty[] = [];

    private criteria: FogTripTicketSearchCriteria = {};

    public table: TableViewModel<FogTripTicketRow> = {
        query: {
            sort: {},
            filter: []
        }
    };

    public waterSupplierOptions: InputOption[] = [{ id: '', text: 'Any Value' }];

    public readonly interceptorTypeOptions: InputOption[] = [
        { id: '', text: 'Any Type' },
        { id: InterceptorType.GreaseTrap, text: 'Grease Trap' },
        { id: InterceptorType.GritTrap, text: 'Grit Trap' },
        { id: InterceptorType.SepticTank, text: 'Septic Tank' },
        { id: InterceptorType.ChemicalToilet, text: 'Chemical Toilet' },
        { id: InterceptorType.Other, text: 'Other' }
    ];

    public readonly propertyTypeOptions: InputOption[] = [
        { id: '', text: 'Any Value' },
        { id: String(PropertyType.Residential), text: 'Residential' },
        { id: String(PropertyType.Commercial), text: 'Commercial' }
    ];

    public readonly ticketStatusOptions: InputOption[] = [
        { id: '', text: 'Any Value' },
        { id: FogTripTicketStatus.PickupNotCompleted, text: 'Pickup Not Completed' },
        { id: FogTripTicketStatus.PickupCompleted, text: 'Pickup Completed' },
        { id: FogTripTicketStatus.TicketCompleted, text: 'Ticket Completed' }
    ];

    public readonly dateSearchOptions: InputOption[] = [
        { id: '', text: 'None' },
        { id: FogTripTicketDateField.RecordCreationDate, text: 'Record Creation Date' },
        { id: FogTripTicketDateField.WasteRemovalDate, text: 'Waste Removal Date' },
        { id: FogTripTicketDateField.WasteDeliveredDate, text: 'Waste Delivered Date' }
    ];

    constructor(
        private readonly _fogTripTicketService: FogTripTicketService,
        private readonly _waterSupplierService: WaterSupplierService
    ) {

    }

    public async ngOnInit(): Promise<void> {
        this.table.columns = this.getColumns();

        await this.loadWaterSuppliers();
    }

    public onFilterChange(queryProperties: QueryProperty[]): void {
        this.criteria = {};

        for (const field of CRITERIA_FIELDS) {
            const property = queryProperties.find(p => p.columnName === field);

            this.criteria[field] = property?.value ? property.value : null;
        }

        this.panelFilter = queryProperties
            .filter(p => !CRITERIA_FIELDS.includes(p.columnName as keyof FogTripTicketSearchCriteria))
            .map(property => this.rewriteTicketStatus(property));

        this.table.query.filter = this.buildFilter();
    }

    private rewriteTicketStatus(property: QueryProperty): QueryProperty {
        if (property.columnName !== 'ticketStatus') {
            return property;
        }

        const pickupCompleted = property.value === FogTripTicketStatus.PickupCompleted;

        if (property.value === FogTripTicketStatus.TicketCompleted) {
            return { columnName: 'completed', value: 'true', comparisonOperator: 'Eq' };
        }

        return {
            columnName: 'pickupCompleted',
            children: [
                {
                    columnName: 'pickupCompleted',
                    value: String(pickupCompleted),
                    comparisonOperator: 'Eq',
                    logicalOperator: 'And'
                },
                {
                    columnName: 'completed',
                    value: 'false',
                    comparisonOperator: 'Eq',
                    logicalOperator: 'And'
                }
            ]
        };
    }

    public async search(searchForm: NgForm): Promise<void> {
        if (!searchForm.valid) {
            return;
        }

        this.table.query.filter = this.buildFilter();
        this.table.query.sort = this.buildSort();

        await this.getTripTickets();

        this.showResults = true;
    }

    public async getTripTickets(): Promise<void> {
        try {
            this.table.isLoading = true;

            const tickets = await this._fogTripTicketService.getAll(
                this.table.items?.pageInfo || {},
                this.table.query,
                this.criteria
            );

            this.table.items = {
                ...tickets,
                data: tickets.data.map(ticket => this.toRow(ticket))
            };
        } finally {
            this.table.isLoading = false;
        }
    }

    private toRow(ticket: FogTripTicket): FogTripTicketRow {
        return {
            ...ticket,
            propertyAddress: this.buildPropertyAddress(ticket),
            propertyCityStateZip: this.buildCityStateZip(ticket.propertyCity, ticket.propertyState?.code, ticket.propertyZip),
            transporterCityStateZip: this.buildCityStateZip(ticket.transporterCity, ticket.transporterState, ticket.transporterZip),
            receiverCityStateZip: this.buildCityStateZip(ticket.receiverCity, ticket.receiverState, ticket.receiverZip),
            interceptorTypeDescription: this.buildInterceptorTypeDescription(ticket),
            interceptorCapacityDescription: this.buildVolumeDescription(ticket.interceptorCapacity, ticket.interceptorCapacityType),
            wasteRemovedDescription: this.buildVolumeDescription(ticket.interceptorWasteRemovedAmount, ticket.interceptorWasteRemovedType)
        };
    }

    private buildPropertyAddress(ticket: FogTripTicket): string {
        const address = `${ticket.propertyStreetNumber ?? ''} ${ticket.propertyStreetName ?? ''}`.trim();

        return ticket.propertyNumber ? `${address} #${ticket.propertyNumber}`.trim() : address;
    }

    private buildCityStateZip(city?: string, state?: string, zip?: string): string {
        const cityPart = city ? `${city},` : '';

        return `${cityPart} ${state ?? ''} ${zip ?? ''}`.trim();
    }

    private buildInterceptorTypeDescription(ticket: FogTripTicket): string {
        if (ticket.interceptorType === InterceptorType.Other) {
            return ticket.interceptorOtherDescription ?? '';
        }

        return ticket.interceptorType ?? '';
    }

    private buildVolumeDescription(amount?: number, type?: InterceptorCapacityType): string {
        if (amount == null) {
            return '';
        }

        return `${amount} ${interceptorCapacityTypeLabels[type ?? InterceptorCapacityType.Gallons]}`;
    }

    private buildFilter(): QueryProperty[] {
        const filter = [...this.panelFilter];

        if (!this.dateSearchField) {
            return filter;
        }

        const children: QueryProperty[] = [];

        if (this.dateRange?.startDate) {
            children.push({
                columnName: this.dateSearchField,
                value: this.dateRange.startDate,
                comparisonOperator: 'Gte',
                logicalOperator: 'And'
            });
        }

        if (this.dateRange?.endDate) {
            children.push({
                columnName: this.dateSearchField,
                value: this.dateRange.endDate,
                comparisonOperator: 'Lte',
                logicalOperator: 'And'
            });
        }

        if (children.length > 0) {
            filter.push({ columnName: this.dateSearchField, children });
        }

        return filter;
    }

    private buildSort(): { [key: string]: SortOperator } {
        return { [this.dateSearchField || FogTripTicketDateField.RecordCreationDate]: 'Desc' };
    }

    private async loadWaterSuppliers(): Promise<void> {
        const options = await this._waterSupplierService.getAllAsOptions();

        this.waterSupplierOptions = [{ id: '', text: 'Any Value' }, ...options];
    }

    private getColumns(): TableColumn<FogTripTicketRow>[] {
        return [
            { field: 'id', caption: 'ID', type: ColumnType.number },
            { field: '', caption: '', type: ColumnType.other, cellTemplate: this.statusCell },
            { field: '', caption: 'Generator Information', type: ColumnType.other, cellTemplate: this.generatorCell },
            { field: '', caption: 'Transporter Information', type: ColumnType.other, cellTemplate: this.transporterCell },
            { field: '', caption: 'Receiver Information', type: ColumnType.other, cellTemplate: this.receiverCell },
            { field: '', caption: 'Waste Removed', type: ColumnType.other, cellTemplate: this.wasteCell }
        ];
    }
}
