import { Component, Input, OnInit, TemplateRef, ViewChild } from '@angular/core';
import { Router } from '@angular/router';
import { FogTripTicket } from '../../../../shared/models/fog/fog-trip-ticket';
import { FogTripTicketService } from '../../../../shared/services/fog/fog-trip-ticket.service';
import { FOG_VEHICLE_CAPACITY_TYPE_LABELS, FogVehicleCapacityType } from '../../../../shared/models/fog/fog-vehicle-enums';
import { TableViewModel } from '../../../../shared/models/table-view-model';
import { ComparisonOperator, QueryProperty } from '../../../../shared/models/query';
import { CellTemplateData, ColumnType, TableColumn } from '@envirotrax/common-ui';

@Component({
    selector: 'app-site-fog-trip-tickets',
    standalone: false,
    templateUrl: './site-fog-trip-tickets.component.html'
})
export class SiteFogTripTicketsComponent implements OnInit {
    @Input()
    public siteId?: number;

    @ViewChild('statusTemplate', { static: true })
    public statusTemplate!: TemplateRef<CellTemplateData<FogTripTicket>>;

    @ViewChild('transporterTemplate', { static: true })
    public transporterTemplate!: TemplateRef<CellTemplateData<FogTripTicket>>;

    @ViewChild('receiverTemplate', { static: true })
    public receiverTemplate!: TemplateRef<CellTemplateData<FogTripTicket>>;

    @ViewChild('wasteTemplate', { static: true })
    public wasteTemplate!: TemplateRef<CellTemplateData<FogTripTicket>>;

    public table: TableViewModel<FogTripTicket> = {
        columns: [],
        query: {
            // Newest pickups first; the repository otherwise falls back to Id Asc.
            sort: { interceptorWasteRemovedDate: 'Desc' },
            filter: []
        }
    };

    constructor(
        private readonly _fogTripTicketService: FogTripTicketService,
        private readonly _router: Router
    ) { }

    public async ngOnInit(): Promise<void> {
        this.table.columns = this.getColumns();

        await this.getTripTickets();
    }

    public viewTripTicket(ticket: FogTripTicket): void {
        if (!ticket.id) {
            return;
        }

        const url = this._router.serializeUrl(
            this._router.createUrlTree(['/fog', 'trip-tickets', ticket.id])
        );

        window.open(url, '_blank');
    }

    public async getTripTickets(): Promise<void> {
        if (!this.siteId) {
            return;
        }

        try {
            this.table.isLoading = true;
            this.table.query.filter = [this.siteFilter()];
            this.table.items = await this._fogTripTicketService.getAll(
                this.table.items?.pageInfo || {},
                this.table.query
            );
        } finally {
            this.table.isLoading = false;
        }
    }

    public capacityUnit(capacityType?: FogVehicleCapacityType): string {
        if (capacityType === undefined || capacityType === null) {
            return '';
        }

        return FOG_VEHICLE_CAPACITY_TYPE_LABELS[capacityType] ?? '';
    }

    private siteFilter(): QueryProperty {
        return {
            columnName: 'site.id',
            value: this.siteId!.toString(),
            comparisonOperator: 'Eq' as ComparisonOperator
        };
    }

    private getColumns(): TableColumn<FogTripTicket>[] {
        return [
            {
                field: '',
                caption: 'Status',
                type: ColumnType.other,
                queryColumnExcluded: true,
                cellTemplate: this.statusTemplate
            },
            {
                field: 'interceptorWasteRemovedDate',
                caption: 'Waste Removal Date',
                type: ColumnType.dateTime
            },
            {
                field: '',
                caption: 'Transporter Information',
                type: ColumnType.other,
                queryColumnExcluded: true,
                cellTemplate: this.transporterTemplate
            },
            {
                field: 'receiverWasteDeliveredDate',
                caption: 'Waste Delivery Date',
                type: ColumnType.dateTime
            },
            {
                field: '',
                caption: 'Receiver Information',
                type: ColumnType.other,
                queryColumnExcluded: true,
                cellTemplate: this.receiverTemplate
            },
            {
                field: '',
                caption: 'Waste Removed',
                type: ColumnType.other,
                queryColumnExcluded: true,
                cellTemplate: this.wasteTemplate
            }
        ];
    }
}
