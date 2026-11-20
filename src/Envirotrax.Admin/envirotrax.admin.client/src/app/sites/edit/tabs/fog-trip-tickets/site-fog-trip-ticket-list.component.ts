import { CommonModule } from '@angular/common';
import { Component, Input, OnInit, TemplateRef, ViewChild } from '@angular/core';
import { CellTemplateData, ColumnType, TableColumn, TableViewModel } from '@envirotrax/common-ui';
import { FogTripTicketDetailsComponent } from '../../../../fog/trip-tickets/details/fog-trip-ticket-details.component';
import { SharedComponentsModule } from '../../../../shared/components/shared.components.module';
import { InterceptorCapacityType, interceptorCapacityTypeLabels, InterceptorType } from '../../../../shared/models/fog/fog-inspection';
import { FogPaymentStatus } from '../../../../shared/models/fog/fog-payment-status';
import { FogTripTicket } from '../../../../shared/models/fog/fog-trip-ticket';
import { MODULE_COLORS } from '../../../../shared/models/module-colors';
import { FogTripTicketService } from '../../../../shared/services/fog/fog-trip-ticket.service';
import { WindowService } from '../../../../shared/services/window.service';

interface SiteFogTripTicketRow extends FogTripTicket {
    transporterCityStateZip: string;
    receiverCityStateZip: string;
    interceptorTypeDescription: string;
    interceptorCapacityDescription: string;
    wasteRemovedDescription: string;
}

@Component({
    selector: 'app-site-fog-trip-ticket-list',
    templateUrl: './site-fog-trip-ticket-list.component.html',
    imports: [CommonModule, SharedComponentsModule],
})
export class SiteFogTripTicketListComponent implements OnInit {
    @Input()
    public siteId: number = 0;

    @ViewChild('transporterCell', { static: true })
    public transporterCell?: TemplateRef<CellTemplateData<SiteFogTripTicketRow>>;

    @ViewChild('receiverCell', { static: true })
    public receiverCell?: TemplateRef<CellTemplateData<SiteFogTripTicketRow>>;

    @ViewChild('wasteCell', { static: true })
    public wasteCell?: TemplateRef<CellTemplateData<SiteFogTripTicketRow>>;

    public table: TableViewModel<SiteFogTripTicketRow> = {
        query: {
            sort: { interceptorWasteRemovedDate: 'Desc' },
            filter: []
        }
    };

    constructor(
        private readonly _fogTripTicketService: FogTripTicketService,
        private readonly _windowService: WindowService
    ) {

    }

    public async ngOnInit(): Promise<void> {
        this.table.columns = this.getColumns();

        await this.load();
    }

    public async load(): Promise<void> {
        if (this.table.items) {
            this.table.items.pageInfo.pageNumber = 1;
        }

        await this.getTripTickets();
    }

    public async getTripTickets(): Promise<void> {
        try {
            this.table.isLoading = true;
            this.table.query.filter = [
                { columnName: 'site.id', value: String(this.siteId), comparisonOperator: 'Eq' }
            ];

            const tickets = await this._fogTripTicketService.getAll(this.table.items?.pageInfo || {}, this.table.query, {}, FogPaymentStatus.Paid);

            const rows: SiteFogTripTicketRow[] = [];

            for (const ticket of tickets.data) {
                rows.push(this.toRow(ticket));
            }

            this.table.items = {
                pageInfo: tickets.pageInfo,
                data: rows
            };
        } finally {
            this.table.isLoading = false;
        }
    }

    public viewDetails(ticket: SiteFogTripTicketRow): void {
        this._windowService.addWindow(FogTripTicketDetailsComponent, {
            title: this.buildWindowTitle(ticket),
            color: MODULE_COLORS.FOG,
            model: { id: ticket.id }
        });
    }

    private buildWindowTitle(ticket: FogTripTicket): string {
        let address = `${ticket.propertyStreetNumber ?? ''} ${ticket.propertyStreetName ?? ''}`.trim();

        if (ticket.propertyNumber) {
            address = `${address} #${ticket.propertyNumber}`.trim();
        }

        return address ? `${ticket.id} - ${address}` : String(ticket.id);
    }

    private toRow(ticket: FogTripTicket): SiteFogTripTicketRow {
        return {
            ...ticket,
            transporterCityStateZip: this.buildCityStateZip(ticket.transporterCity, ticket.transporterState, ticket.transporterZip),
            receiverCityStateZip: this.buildCityStateZip(ticket.receiverCity, ticket.receiverState, ticket.receiverZip),
            interceptorTypeDescription: this.buildInterceptorTypeDescription(ticket),
            interceptorCapacityDescription: this.buildVolumeDescription(ticket.interceptorCapacity, ticket.interceptorCapacityType),
            wasteRemovedDescription: this.buildVolumeDescription(ticket.interceptorWasteRemovedAmount, ticket.interceptorWasteRemovedType)
        };
    }

    private buildCityStateZip(city?: string, state?: string, zip?: string): string {
        return `${city ?? ''}, ${state ?? ''}  ${zip ?? ''}`;
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

    private getColumns(): TableColumn<SiteFogTripTicketRow>[] {
        return [
            { field: 'id', caption: 'ID', type: ColumnType.number },
            { field: 'transporterCompanyName', caption: 'Transporter Information', type: ColumnType.other, cellTemplate: this.transporterCell, queryColumnExcluded: true },
            { field: 'receiverCompanyName', caption: 'Receiver Information', type: ColumnType.other, cellTemplate: this.receiverCell, queryColumnExcluded: true },
            { field: 'interceptorWasteRemovedDate', caption: 'Waste Removed', type: ColumnType.other, cellTemplate: this.wasteCell, queryColumnExcluded: true }
        ];
    }
}
