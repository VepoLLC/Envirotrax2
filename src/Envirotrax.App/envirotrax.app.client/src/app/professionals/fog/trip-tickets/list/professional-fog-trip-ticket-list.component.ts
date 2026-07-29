import { Component, ElementRef, OnInit, TemplateRef, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { NgForm } from '@angular/forms';
import { FogTripTicketService } from '../../../../shared/services/fog/fog-trip-ticket.service';
import { ProfessionalFogVehicleService } from '../../../../shared/services/fog/professional-fog-vehicle.service';
import { ProfessionalFogDisposalSiteService } from '../../../../shared/services/fog/professional-fog-disposal-site.service';
import { ProfessionalSupplierService } from '../../../../shared/services/professionals/professional-supplier.service';
import { ProfesionalUserService } from '../../../../shared/services/professionals/professional-user.service';
import { QueryProperty } from '../../../../shared/models/query';
import { TableViewModel } from '../../../../shared/models/table-view-model';
import { FogTripTicket } from '../../../../shared/models/fog/fog-trip-ticket';
import { FogVehicleCapacityType } from '../../../../shared/models/fog/fog-vehicle-enums';
import { FogTripTicketDateType, FogTripTicketStatus } from '../../../../shared/models/fog/fog-trip-ticket-enums';
import { InterceptorType } from '../../../../shared/enums/interceptor-type.enum';
import { PropertyType } from '../../../../shared/enums/property-type.enum';
import { CellTemplateData, ColumnType, InputOption, TableColumn } from '@envirotrax/common-ui';
import { AppContainerHelperService } from '../../../../shared/services/helpers/app-contaner-helper.service';
import { MAX_PAGE_SIZE } from '../../../../shared/models/page-info';
import { PrintableTableService } from '../../../../shared/services/printable-table.service';

@Component({
    standalone: false,
    templateUrl: './professional-fog-trip-ticket-list.component.html'
})
export class ProfessionalFogTripTicketListComponent implements OnInit {
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

    @ViewChild('printableSection')
    private _printableSection!: ElementRef;

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

    public propertyTypeOptions: InputOption[] = [
        { id: '', text: 'Any Value' },
        { id: PropertyType.Residential.toString(), text: 'Residential' },
        { id: PropertyType.Commercial.toString(), text: 'Commercial' }
    ];

    public dateTypeOptions: InputOption[] = [
        { id: '', text: 'Any date range' },
        { id: FogTripTicketDateType.WasteRemovalDate, text: 'Waste removal date' },
        { id: FogTripTicketDateType.WasteDeliveredDate, text: 'Waste delivered date' }
    ];

    public transporterOptions: InputOption[] = [];
    public vehicleOptions: InputOption[] = [];
    public disposalSiteOptions: InputOption[] = [];
    public waterSupplierOptions: InputOption[] = [];

    public get selectedDateType(): string {
        return this.table.query.filter?.find(qp => qp.columnName === 'dateType')?.value ?? '';
    }

    private get selectedWaterSupplierId(): number | undefined {
        const val = this.table.query.filter?.find(qp => qp.columnName === 'waterSupplier.id')?.value;
        return val ? Number(val) : undefined;
    }

    constructor(
        private readonly _fogTripTicketService: FogTripTicketService,
        private readonly _vehicleService: ProfessionalFogVehicleService,
        private readonly _disposalSiteService: ProfessionalFogDisposalSiteService,
        private readonly _professionalSupplierService: ProfessionalSupplierService,
        private readonly _userService: ProfesionalUserService,
        private readonly _router: Router,
        private readonly _activatedRoute: ActivatedRoute,
        private readonly _containerHelper: AppContainerHelperService,
        private readonly _printService: PrintableTableService,
    ) { 
        this.table.query.filter = [this.paidFilter()];
    }

    public async ngOnInit(): Promise<void> {
        this.table.columns = this.getColumns();
        this.loadLookups();
    }

    private async loadLookups(): Promise<void> {
        const [transporterResult, vehiclesResult, disposalSitesResult, waterSuppliers] = await Promise.all([
            this._userService.getAllAsOptions(true, 'Any transporter', { filter: [{ columnName: 'isFogTransporter', value: 'true' }] }),
            this._vehicleService.getAllAsOptions(true, 'Any vehicle'),
            this._disposalSiteService.getAllRegisteredAsOptions(true, 'Any disposal site'),
            this._professionalSupplierService.getMyAsOptions({ hasFogTransportation: true })
        ]);

        this.transporterOptions = transporterResult;
        this.vehicleOptions = vehiclesResult;
        this.disposalSiteOptions = disposalSitesResult;
        this.waterSupplierOptions = waterSuppliers;
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
                field: 'waterSupplier.name',
                caption: 'Water Supplier',
                type: ColumnType.text,
                cellTemplate: this.waterSupplierCell
            },
            {
                field: 'propertyBusinessName',
                caption: 'Generator Information',
                type: ColumnType.text,
                cellTemplate: this.generatorCell
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
            this.table.items = await this._fogTripTicketService.searchForProfessional(
                this.table.items?.pageInfo || {}, this.table.query, this.selectedWaterSupplierId);
        } finally {
            this.table.isLoading = false;
        }
    }

    public onFilterChange(queryProperties: QueryProperty[]): void {
        this.table.query.filter = [...queryProperties, this.paidFilter()];
    }

     private paidFilter(): QueryProperty {
        return { columnName: 'transactionId', isValueNull: true, comparisonOperator: 'NotEq', logicalOperator: 'And' };
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
        const url = this._router.serializeUrl(
            this._router.createUrlTree([ticket.id], { relativeTo: this._activatedRoute })
        );
        window.open(url, '_blank');
    }

    public viewPrintableTable(): void {
        this._printService.open(this._printableSection.nativeElement);
    }
}
