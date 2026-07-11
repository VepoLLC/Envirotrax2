import { Component, OnInit, TemplateRef, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { NgForm } from '@angular/forms';
import { FogTripTicketService } from '../../../shared/services/fog/fog-trip-ticket.service';
import { QueryProperty } from '../../../shared/models/query';
import { TableViewModel } from '../../../shared/models/table-view-model';
import { FogTripTicket } from '../../../shared/models/fog/fog-trip-ticket';
import { FogVehicleCapacityType } from '../../../shared/models/fog/fog-vehicle-enums';
import { FogTripTicketApprovalStatus, FogTripTicketDateType, FogTripTicketPaymentStatus, FogTripTicketStatus } from '../../../shared/models/fog/fog-trip-ticket-enums';
import { InterceptorType } from '../../../shared/enums/interceptor-type.enum';
import { PropertyType } from '../../../shared/enums/property-type.enum';
import { CellTemplateData, ColumnType, InputOption, TableColumn } from '@envirotrax/common-ui';
import { AppContainerHelperService } from '../../../shared/services/helpers/app-contaner-helper.service';

@Component({
    standalone: false,
    templateUrl: './fog-trip-ticket-list.component.html'
})
export class FogTripTicketListComponent implements OnInit {
    public showResults: boolean = false;
    public readonly PropertyType = PropertyType;
    public readonly FogVehicleCapacityType = FogVehicleCapacityType;

    public table: TableViewModel<FogTripTicket> = {
        columns: this.getColumns(),
        query: {
            sort: {},
            filter: []
        },
        freeTextSearch: {
            searchQuery: [
                { field: 'propertyBusinessName', operator: 'Ct' },
                { field: 'transporterCompanyName', operator: 'Ct' },
                { field: 'transporterContactName', operator: 'Ct' }
            ]
        }
    };

    @ViewChild('iconsCell', { static: true })
    public iconsCell?: TemplateRef<CellTemplateData<FogTripTicket>>;

    @ViewChild('removalDateCell', { static: true })
    public removalDateCell?: TemplateRef<CellTemplateData<FogTripTicket>>;

    @ViewChild('waterSupplierCell', { static: true })
    public waterSupplierCell?: TemplateRef<CellTemplateData<FogTripTicket>>;

    @ViewChild('generatorCell', { static: true })
    public generatorCell?: TemplateRef<CellTemplateData<FogTripTicket>>;

    @ViewChild('receiverCell', { static: true })
    public receiverCell?: TemplateRef<CellTemplateData<FogTripTicket>>;

    @ViewChild('wasteCell', { static: true })
    public wasteCell?: TemplateRef<CellTemplateData<FogTripTicket>>;

    public interceptorTypeOptions: InputOption[] = [
        { id: '', text: 'Any Type' },
        { id: InterceptorType.GreaseTrap, text: 'Grease Trap' },
        { id: InterceptorType.GritTrap, text: 'Grit Trap' },
        { id: InterceptorType.SepticTank, text: 'Septic Tank' },
        { id: InterceptorType.ChemicalToilet, text: 'Chemical Toilet' },
        { id: InterceptorType.Other, text: 'Other' }
    ];

    public ticketStatusOptions: InputOption[] = [
        { id: '', text: 'Any value' },
        { id: FogTripTicketStatus.PickupNotCompleted, text: 'Pickup not completed' },
        { id: FogTripTicketStatus.PickupCompleted, text: 'Pickup completed' },
        { id: FogTripTicketStatus.TripTicketCompleted, text: 'Trip ticket completed' }
    ];

    public paymentStatusOptions: InputOption[] = [
        { id: '', text: 'Any value' },
        { id: FogTripTicketPaymentStatus.Unpaid, text: 'Unpaid' },
        { id: FogTripTicketPaymentStatus.Paid, text: 'Paid' }
    ];

    public approvalStatusOptions: InputOption[] = [
        { id: '', text: 'Any value' },
        { id: FogTripTicketApprovalStatus.Approved, text: 'Approved' },
        { id: FogTripTicketApprovalStatus.Disapproved, text: 'Disapproved' }
    ];

    public propertyTypeOptions: InputOption[] = [
        { id: '', text: 'Any Value' },
        { id: PropertyType.Residential.toString(), text: 'Residential' },
        { id: PropertyType.Commercial.toString(), text: 'Commercial' }
    ];

    public dateTypeOptions: InputOption[] = [
        { id: '', text: 'Any date range' },
        { id: FogTripTicketDateType.RecordCreationDate, text: 'Record creation date' },
        { id: FogTripTicketDateType.WasteRemovalDate, text: 'Waste removal date' },
        { id: FogTripTicketDateType.WasteDeliveredDate, text: 'Waste delivered date' }
    ];

    public selectedDateType: string = '';

    constructor(
        private readonly _fogTripTicketService: FogTripTicketService,
        private readonly _router: Router,
        private readonly _activatedRoute: ActivatedRoute,
        private readonly _containerHelper: AppContainerHelperService
    ) { }

    public ngOnInit(): void {
        this.table.columns = this.getColumns();
    }

    private getColumns(): TableColumn<FogTripTicket>[] {
        return [
            {
                field: 'completed',
                caption: '',
                type: ColumnType.text,
                cellTemplate: this.iconsCell,
                queryColumnExcluded: true
            },
            {
                field: 'interceptorWasteRemovedDate',
                caption: 'Removal Date',
                type: ColumnType.date,
                cellTemplate: this.removalDateCell
            },
            {
                field: 'propertyBusinessName',
                caption: 'Generator Information',
                type: ColumnType.text,
                cellTemplate: this.generatorCell
            },

            {
                field: 'propertyBusinessName',
                caption: 'Transporter Information',
                type: ColumnType.text,
            },

            {
                field: 'receiverCompanyName',
                caption: 'Receiver Information',
                type: ColumnType.text,
                cellTemplate: this.receiverCell
            },
            {
                field: 'interceptorWasteRemovedAmount',
                caption: 'Waste Removed',
                type: ColumnType.text,
                cellTemplate: this.wasteCell
            }
        ];
    }

    public async getTripTickets(): Promise<void> {
        try {
            this.table.isLoading = true;
            this.table.items = await this._fogTripTicketService.getAll(
                this.table.items?.pageInfo || {},
                this.table.query
            );
        } finally {
            this.table.isLoading = false;
        }
    }

    public onFilterChange(queryProperties: QueryProperty[]): void {
        this.table.query.filter = queryProperties;
    }

    public setShowResults(visible: boolean): void {
        this.showResults = visible;
        this._containerHelper.setContainerVisibility(!visible);
    }

    public async search(searchForm: NgForm): Promise<void> {
        if (searchForm.valid) {
            await this.getTripTickets();
            this.setShowResults(true);
        }
    }

    public viewDetails(ticket: FogTripTicket): void {
        this._router.navigate([ticket.id], { relativeTo: this._activatedRoute });
    }
}
