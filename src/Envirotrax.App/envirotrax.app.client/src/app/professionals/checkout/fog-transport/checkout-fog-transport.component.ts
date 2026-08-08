import { Component, Input, OnInit, TemplateRef, ViewChild } from "@angular/core";
import { CellTemplateData, ColumnType, CurrencyCellComponent, InputOption, MAX_PAGE_SIZE, ModalHelperService, TableColumn, ToastService } from '@envirotrax/common-ui';
import { QueryProperty } from "../../../shared/models/query";
import { TableViewModel } from "../../../shared/models/table-view-model";
import { FogTripTicket } from "../../../shared/models/fog/fog-trip-ticket";
import { FogTripTicketService } from "../../../shared/services/fog/fog-trip-ticket.service";
import { ProfesionalUserService } from "../../../shared/services/professionals/professional-user.service";
import { ProfesisonalService } from "../../../shared/services/professionals/professional.service";
import { CheckoutService } from "../../../shared/services/professionals/checkout.service";
import { Router } from "@angular/router";

@Component({
    selector: 'vp-checkout-fog-transport',
    standalone: false,
    templateUrl: './checkout-fog-transport.component.html',
    styles: `
        .vp-checkout-table-scroll {
            max-height: 500px;
            overflow-y: auto;
        }
    `
})
export class CheckoutFogTransportComponent implements OnInit {
    @Input() public isAdmin = false;

    public isLoading = false;
    public items: TableViewModel<CheckoutFogTripTicketVm> = {
        query: { sort: {}, filter: [] }
    };
    public selectedFeeTotal = 0;
    public reportForOptions: InputOption[] = [];
    public reportFor = '';

    private _currentUserId?: number;
    private _professionalName?: string;

    @ViewChild('selectTemplate', { static: true })
    public selectTemplate?: TemplateRef<CellTemplateData<CheckoutFogTripTicketVm>>;

    @ViewChild('emailPdfTemplate', { static: true })
    public emailPdfTemplate?: TemplateRef<CellTemplateData<CheckoutFogTripTicketVm>>;

    @ViewChild('generatorTemplate', { static: true })
    public generatorTemplate?: TemplateRef<CellTemplateData<CheckoutFogTripTicketVm>>;

    @ViewChild('tripTicketInfoTemplate', { static: true })
    public tripTicketInfoTemplate?: TemplateRef<CellTemplateData<CheckoutFogTripTicketVm>>;

    constructor(
        private readonly _fogTripTicketService: FogTripTicketService,
        private readonly _professionalUserService: ProfesionalUserService,
        private readonly _professionalService: ProfesisonalService,
        private readonly _modalHelper: ModalHelperService,
        private readonly _toastService: ToastService,
        private readonly _checkoutService: CheckoutService,
        private readonly _router: Router
    ) {

    }

    public async ngOnInit(): Promise<void> {
        try{
            this.isLoading = true;

            this.items.columns = this.getColumns();

            const currentUser = await this._professionalUserService.getMyData();
            this._currentUserId = currentUser.id;
            this.reportFor = this.isAdmin ? '' : String(this._currentUserId);

            if (this.isAdmin) {
                const professional = await this._professionalService.getLoggedInProfessional();
                this._professionalName = professional.name;

                const users = await this._professionalUserService.getAll(
                    { pageSize: MAX_PAGE_SIZE },
                    { sort: {}, filter: [{ columnName: 'isFogTransporter', comparisonOperator: 'Eq', value: 'true' }] }
                );
                this.reportForOptions = this.buildReportForOptions(users.data);
            }

            await this.getFogTripTickets();
        }finally{
            this.isLoading = false;
        }
    }

    private buildReportForOptions(otherUsers: { id?: number; contactName?: string }[]): InputOption[] {
        const otherUserOptions: InputOption[] = otherUsers
            .filter(u => u.id !== this._currentUserId)
            .map(u => ({ id: String(u.id), text: u.contactName ?? `User ${u.id}` }));

        return [
            { id: '', text: `This account and ${this._professionalName}` },
            { id: String(this._currentUserId), text: 'This account only' },
            ...otherUserOptions
        ];
    }

    public async getFogTripTickets(): Promise<void> {
        try {
            this.isLoading = true;

            const filter: QueryProperty[] = [
                { columnName: 'transactionId', isValueNull: true },
                { columnName: 'amount', comparisonOperator: 'Gt', value: '0' }
            ];

            if (this.reportFor !== '') {
                filter.push({ columnName: 'transporter.id', comparisonOperator: 'Eq', value: this.reportFor });
            }

            this.items.query.filter = filter;

            this.items.items = await this._fogTripTicketService.searchForProfessional(
                { pageSize: MAX_PAGE_SIZE },
                this.items.query
            );

            this.items.items.data.forEach(ticket => {
                ticket.selected = true;
                ticket.emailPdf = true;
            });
            this.recalculateSelectedTotal();
        } finally {
            this.isLoading = false;
        }
    }

    public recalculateSelectedTotal(): void {
        if (this.items.items) {
            this.items.items.data = this.items.items.data.filter(ticket => ticket.selected);
        }

        this.selectedFeeTotal = (this.items.items?.data || [])
            .reduce((total, ticket) => total + (ticket.amount || 0), 0);
    }

    public viewTripTicket(ticket: CheckoutFogTripTicketVm): void {
        if (ticket?.id == null) {
            return;
        }

        this._router.navigate(['/professionals/fog/trip-tickets', ticket.id]);
    }

    public deleteTripTicket(ticket: CheckoutFogTripTicketVm): void {
        if (ticket?.id == null) {
            return;
        }

        this._modalHelper.showDeleteConfirmation()
            .result()
            .subscribe(() => this.processDelete(ticket));
    }

    private async processDelete(ticket: CheckoutFogTripTicketVm): Promise<void> {
        try {
            this.isLoading = true;
            await this._fogTripTicketService.deleteForProfessional(ticket.id);

            this._toastService.successFullyDeleted('Trip Ticket');
            this._checkoutService.refresh();
        } finally {
            this.isLoading = false;
        }

        await this.getFogTripTickets();
    }

    private getColumns(): TableColumn<CheckoutFogTripTicketVm>[] {
        return [
            {
                field: 'selected',
                caption: '',
                type: ColumnType.other,
                queryColumnExcluded: true,
                headerCssClass: 'text-center',
                cellTemplate: this.selectTemplate
            },
            {
                field: 'emailPdf',
                caption: 'Email PDF',
                type: ColumnType.other,
                queryColumnExcluded: true,
                headerCssClass: 'text-center',
                cellTemplate: this.emailPdfTemplate
            },
            {
                field: 'interceptorWasteRemovedDate',
                caption: 'Removal Date',
                type: ColumnType.date
            },
            {
                field: 'site.accountNumber',
                caption: 'Account',
                type: ColumnType.text
            },
            {
                field: 'waterSupplier.name',
                caption: 'Water Supplier',
                type: ColumnType.text
            },
            {
                field: 'propertyStreetName',
                caption: 'Generator Information',
                type: ColumnType.other,
                queryColumnExcluded: true,
                cellTemplate: this.generatorTemplate
            },
            {
                field: 'submissionId',
                caption: 'Trip Ticket Information',
                type: ColumnType.other,
                queryColumnExcluded: true,
                cellTemplate: this.tripTicketInfoTemplate
            },
            {
                field: 'amount',
                caption: 'Fee',
                type: ColumnType.number,
                cellComponent: CurrencyCellComponent
            }
        ];
    }
}

interface CheckoutFogTripTicketVm extends FogTripTicket {
    selected?: boolean;
    emailPdf?: boolean;
}
