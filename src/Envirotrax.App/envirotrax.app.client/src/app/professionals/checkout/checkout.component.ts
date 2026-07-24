import { Component, OnInit, TemplateRef, ViewChild } from "@angular/core";
import { CellTemplateData, ColumnType, CurrencyCellComponent, InputOption, MAX_PAGE_SIZE, ModalHelperService, TableColumn } from "@envirotrax/common-ui";
import { QueryProperty } from "../../shared/models/query";
import { TableViewModel } from "../../shared/models/table-view-model";
import { AuthService } from "../../shared/services/auth/auth.service";
import { AppContainerHelperService } from "../../shared/services/helpers/app-contaner-helper.service";
import { FeatureType } from "../../shared/models/feature-type";
import { ROLE_DEFINITIONS } from "../../shared/models/role-definitions";
import { BackflowTest } from "../../shared/models/backflow/backflow-test";
import { BackflowTestService } from "../../shared/services/backflow/backflow-test.service";
import { CsiInspection } from "../../shared/models/csi/csi-inspection";
import { CsiInspectionService } from "../../shared/services/csi/csi-inspection.service";
import { FogInspection } from "../../shared/models/fog/fog-inspection";
import { ProfessionalFogInspectionService } from "../../shared/services/fog/professional-fog-inspection.service";
import { FogTripTicket } from "../../shared/models/fog/fog-trip-ticket";
import { FogTripTicketService } from "../../shared/services/fog/fog-trip-ticket.service";
import { ProfesionalUserService } from "../../shared/services/professionals/professional-user.service";
import { ProfesisonalService } from "../../shared/services/professionals/professional.service";
import { ToastService } from "../../shared/services/toast.service";
import { Router } from "@angular/router";

type TabType = 'backflow' | 'csi' | 'fogInspection' | 'fogTransport';

@Component({
    standalone: false,
    templateUrl: './checkout.component.html',
    styles: `
        .vp-checkout-table-scroll {
            max-height: 500px;
            overflow-y: auto;
        }
    `
})
export class CheckoutComponent implements OnInit {
    public activeTab: TabType = 'backflow';
    public hasBackflowTesting = false;
    public hasCsiInspection = false;
    public hasFogInspection = false;
    public hasFogTransportation = false;
    public isAdmin = false;
    public isLoading = false;

    private _currentUserId?: number;
    private _professionalId?: number;
    private _professionalName?: string;

    // Backflow
    public backflowItems: TableViewModel<CheckoutBackflowTestVm> = {
        query: { sort: {}, filter: [] }
    };
    public backflowSelectedFeeTotal = 0;
    public backflowReportForOptions: InputOption[] = [];
    public backflowReportFor = '';

    @ViewChild('backflowSelectTemplate', { static: true })
    public backflowSelectTemplate?: TemplateRef<CellTemplateData<CheckoutBackflowTestVm>>;

    @ViewChild('backflowEmailPdfTemplate', { static: true })
    public backflowEmailPdfTemplate?: TemplateRef<CellTemplateData<CheckoutBackflowTestVm>>;

    @ViewChild('assemblyTemplate', { static: true })
    public assemblyTemplate?: TemplateRef<CellTemplateData<CheckoutBackflowTestVm>>;

    @ViewChild('backflowPropertyTemplate', { static: true })
    public backflowPropertyTemplate?: TemplateRef<CellTemplateData<CheckoutBackflowTestVm>>;

    // CSI
    public csiItems: TableViewModel<CheckoutCsiInspectionVm> = {
        query: { sort: {}, filter: [] }
    };
    public csiSelectedFeeTotal = 0;
    public csiReportForOptions: InputOption[] = [];
    public csiReportFor = '';

    @ViewChild('csiSelectTemplate', { static: true })
    public csiSelectTemplate?: TemplateRef<CellTemplateData<CheckoutCsiInspectionVm>>;

    @ViewChild('csiEmailPdfTemplate', { static: true })
    public csiEmailPdfTemplate?: TemplateRef<CellTemplateData<CheckoutCsiInspectionVm>>;

    @ViewChild('csiPropertyAddressTemplate', { static: true })
    public csiPropertyAddressTemplate?: TemplateRef<CellTemplateData<CheckoutCsiInspectionVm>>;

    @ViewChild('csiMailingAddressTemplate', { static: true })
    public csiMailingAddressTemplate?: TemplateRef<CellTemplateData<CheckoutCsiInspectionVm>>;

    // FOG Transport
    public fogTransportItems: TableViewModel<CheckoutFogTripTicketVm> = {
        query: { sort: {}, filter: [] }
    };
    public fogTransportSelectedFeeTotal = 0;
    public fogTransportReportForOptions: InputOption[] = [];
    public fogTransportReportFor = '';

    @ViewChild('fogSelectTemplate', { static: true })
    public fogSelectTemplate?: TemplateRef<CellTemplateData<CheckoutFogTripTicketVm>>;

    @ViewChild('fogEmailPdfTemplate', { static: true })
    public fogEmailPdfTemplate?: TemplateRef<CellTemplateData<CheckoutFogTripTicketVm>>;

    @ViewChild('fogGeneratorTemplate', { static: true })
    public fogGeneratorTemplate?: TemplateRef<CellTemplateData<CheckoutFogTripTicketVm>>;

    @ViewChild('fogTripTicketInfoTemplate', { static: true })
    public fogTripTicketInfoTemplate?: TemplateRef<CellTemplateData<CheckoutFogTripTicketVm>>;

    // FOG Inspection
    public fogInspectionItems: TableViewModel<CheckoutFogInspectionVm> = {
        query: { sort: {}, filter: [] }
    };
    public fogInspectionSelectedFeeTotal = 0;
    public fogInspectionReportForOptions: InputOption[] = [];
    public fogInspectionReportFor = '';

    @ViewChild('fogInspectionSelectTemplate', { static: true })
    public fogInspectionSelectTemplate?: TemplateRef<CellTemplateData<CheckoutFogInspectionVm>>;

    @ViewChild('fogInspectionGeneratorTemplate', { static: true })
    public fogInspectionGeneratorTemplate?: TemplateRef<CellTemplateData<CheckoutFogInspectionVm>>;

    constructor(
        private readonly _authService: AuthService,
        private readonly _containerHelper: AppContainerHelperService,
        private readonly _backflowTestService: BackflowTestService,
        private readonly _csiInspectionService: CsiInspectionService,
        private readonly _fogTripTicketService: FogTripTicketService,
        private readonly _fogInspectionService: ProfessionalFogInspectionService,
        private readonly _professionalUserService: ProfesionalUserService,
        private readonly _professionalService: ProfesisonalService,
        private readonly _modalHelper: ModalHelperService,
        private readonly _toastService: ToastService,
        private readonly _router: Router
    ) {

    }

    public async ngOnInit(): Promise<void> {
        this._containerHelper.setContainerVisibility(false);

        [this.hasBackflowTesting, this.hasCsiInspection, this.hasFogInspection, this.hasFogTransportation, this.isAdmin] = await Promise.all([
            this._authService.hasAnyFeatures(FeatureType.BackflowTesting),
            this._authService.hasAnyFeatures(FeatureType.CsiInspection),
            this._authService.hasAnyFeatures(FeatureType.FogInspection),
            this._authService.hasAnyFeatures(FeatureType.FogTransportation),
            this._authService.hasAnyRoles(ROLE_DEFINITIONS.PROFESSIONALS.ADMIN)
        ]);

        if (this.hasBackflowTesting) {
            this.activeTab = 'backflow';
        } else if (this.hasCsiInspection) {
            this.activeTab = 'csi';
        } else if (this.hasFogInspection) {
            this.activeTab = 'fogInspection';
        } else if (this.hasFogTransportation) {
            this.activeTab = 'fogTransport';
        }

        const currentUser = await this._professionalUserService.getMyData();
        this._currentUserId = currentUser.id;

        if (this.isAdmin) {
            const professional = await this._professionalService.getLoggedInProfessional();
            this._professionalId = professional.id;
            this._professionalName = professional.name;
        }

        if (this.activeTab === 'backflow') {
            await this.initBackflowTab();
        } else if (this.activeTab === 'csi') {
            await this.initCsiTab();
        } else if (this.activeTab === 'fogInspection') {
            await this.initFogInspectionTab();
        } else if (this.activeTab === 'fogTransport') {
            await this.initFogTransportTab();
        }
    }

    public setActiveTab(tab: TabType): void {
        this.activeTab = tab;

        if (tab === 'backflow') {
            this.initBackflowTab();
        } else if (tab === 'csi') {
            this.initCsiTab();
        } else if (tab === 'fogInspection') {
            this.initFogInspectionTab();
        } else if (tab === 'fogTransport') {
            this.initFogTransportTab();
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

    // Backflow

    private async initBackflowTab(): Promise<void> {
        this.backflowItems.columns = this.getBackflowColumns();
        this.backflowReportFor = this.isAdmin ? '' : String(this._currentUserId);

        if (this.isAdmin) {
            const users = await this._professionalUserService.getAll(
                { pageSize: MAX_PAGE_SIZE },
                { sort: {}, filter: [{ columnName: 'isBackflowTester', comparisonOperator: 'Eq', value: 'true' }] }
            );
            this.backflowReportForOptions = this.buildReportForOptions(users.data);
        }

        await this.getBackflowTests();
    }

    public async getBackflowTests(): Promise<void> {
        try {
            this.isLoading = true;

            const filter: QueryProperty[] = [
                { columnName: 'transactionId', isValueNull: true },
                { columnName: 'amount', comparisonOperator: 'Gt', value: '0' }
            ];

            if (this.backflowReportFor === '' && this._professionalId != null) {
                filter.push({ columnName: 'professional.id', comparisonOperator: 'Eq', value: String(this._professionalId) });
            } else {
                filter.push({ columnName: 'bpat.id', comparisonOperator: 'Eq', value: this.backflowReportFor });
            }

            this.backflowItems.query.filter = filter;

            this.backflowItems.items = await this._backflowTestService.getAllForProfessional(
                { pageSize: MAX_PAGE_SIZE },
                this.backflowItems.query
            );

            this.backflowItems.items.data.forEach(test => {
                test.selected = true;
                test.emailPdf = true;
            });
            this.recalculateBackflowSelectedTotal();
        } finally {
            this.isLoading = false;
        }
    }

    public recalculateBackflowSelectedTotal(): void {
        if (this.backflowItems.items) {
            this.backflowItems.items.data = this.backflowItems.items.data.filter(test => test.selected);
        }

        this.backflowSelectedFeeTotal = (this.backflowItems.items?.data || [])
            .reduce((total, test) => total + (test.amount || 0), 0);
    }

    public viewBackflowTest(test: CheckoutBackflowTestVm): void {
        if (test?.id == null) {
            return;
        }

        this._router.navigate(['/professionals/backflow/tests', test.id, 'view']);
    }

    public editBackflowTest(test: CheckoutBackflowTestVm): void {
        if (test?.id == null) {
            return;
        }

        this._router.navigate(['/professionals/backflow/submit', test.id]);
    }

    public deleteBackflowTest(test: CheckoutBackflowTestVm): void {
        if (test?.id == null) {
            return;
        }

        this._modalHelper.showDeleteConfirmation()
            .result()
            .subscribe(() => this.processDeleteBackflowTest(test));
    }

    private async processDeleteBackflowTest(test: CheckoutBackflowTestVm): Promise<void> {
        try {
            this.isLoading = true;
            await this._backflowTestService.deleteForProfessional(test.id);

            this._toastService.successFullyDeleted('Test Report');
        } finally {
            this.isLoading = false;
        }

        await this.getBackflowTests();
    }

    private getBackflowColumns(): TableColumn<CheckoutBackflowTestVm>[] {
        return [
            {
                field: 'selected',
                caption: '',
                type: ColumnType.other,
                queryColumnExcluded: true,
                headerCssClass: 'text-center',
                cellTemplate: this.backflowSelectTemplate
            },
            {
                field: 'emailPdf',
                caption: 'Email PDF',
                type: ColumnType.other,
                queryColumnExcluded: true,
                headerCssClass: 'text-center',
                cellTemplate: this.backflowEmailPdfTemplate
            },
            {
                field: 'accountNumber',
                caption: 'Account',
                type: ColumnType.text
            },
            {
                field: 'testDate',
                caption: 'Test Date',
                type: ColumnType.date
            },
            {
                field: 'waterSupplier.name',
                caption: 'Water Supplier',
                type: ColumnType.text
            },
            {
                field: 'serialNumber',
                caption: 'Serial Number',
                type: ColumnType.text
            },
            {
                field: 'manufacturer',
                caption: 'Assembly Description',
                type: ColumnType.other,
                queryColumnExcluded: true,
                cellTemplate: this.assemblyTemplate
            },
            {
                field: 'propertyStreetName',
                caption: 'Property Information',
                type: ColumnType.other,
                queryColumnExcluded: true,
                cellTemplate: this.backflowPropertyTemplate
            },
            {
                field: 'amount',
                caption: 'Fee',
                type: ColumnType.number,
                cellComponent: CurrencyCellComponent
            }
        ];
    }

    // CSI

    private async initCsiTab(): Promise<void> {
        this.csiItems.columns = this.getCsiColumns();
        this.csiReportFor = this.isAdmin ? '' : String(this._currentUserId);

        if (this.isAdmin) {
            const users = await this._professionalUserService.getAll(
                { pageSize: MAX_PAGE_SIZE },
                { sort: {}, filter: [{ columnName: 'isCsiInspector', comparisonOperator: 'Eq', value: 'true' }] }
            );
            this.csiReportForOptions = this.buildReportForOptions(users.data);
        }

        await this.getCsiInspections();
    }

    public async getCsiInspections(): Promise<void> {
        try {
            this.isLoading = true;

            const filter: QueryProperty[] = [
                { columnName: 'transactionId', isValueNull: true },
                { columnName: 'amount', comparisonOperator: 'Gt', value: '0' }
            ];

            if (this.csiReportFor === '' && this._professionalId != null) {
                filter.push({ columnName: 'professional.id', comparisonOperator: 'Eq', value: String(this._professionalId) });
            } else {
                filter.push({ columnName: 'inspectorUser.id', comparisonOperator: 'Eq', value: this.csiReportFor });
            }

            this.csiItems.query.filter = filter;

            this.csiItems.items = await this._csiInspectionService.getProfessionalInspections(
                { pageSize: MAX_PAGE_SIZE },
                this.csiItems.query,
                false
            );

            this.csiItems.items.data.forEach(inspection => {
                inspection.selected = true;
                inspection.emailPdf = true;
            });
            this.recalculateCsiSelectedTotal();
        } finally {
            this.isLoading = false;
        }
    }

    public recalculateCsiSelectedTotal(): void {
        if (this.csiItems.items) {
            this.csiItems.items.data = this.csiItems.items.data.filter(inspection => inspection.selected);
        }

        this.csiSelectedFeeTotal = (this.csiItems.items?.data || [])
            .reduce((total, inspection) => total + (inspection.amount || 0), 0);
    }

    public viewCsiInspection(inspection: CheckoutCsiInspectionVm): void {
        if (inspection?.id == null) {
            return;
        }

        this._router.navigate(['/professionals/csi/inspections', inspection.id]);
    }

    public editCsiInspection(inspection: CheckoutCsiInspectionVm): void {
        if (inspection?.site?.id == null) {
            return;
        }

        this._router.navigate(['/professionals/csi/inspections/create', inspection.site.id]);
    }

    public deleteCsiInspection(inspection: CheckoutCsiInspectionVm): void {
        if (inspection?.id == null) {
            return;
        }

        this._modalHelper.showDeleteConfirmation()
            .result()
            .subscribe(() => this.processDeleteCsiInspection(inspection));
    }

    private async processDeleteCsiInspection(inspection: CheckoutCsiInspectionVm): Promise<void> {
        try {
            this.isLoading = true;
            await this._csiInspectionService.deleteForProfessional(inspection.id!);

            this._toastService.successFullyDeleted('Inspection');
        } finally {
            this.isLoading = false;
        }

        await this.getCsiInspections();
    }

    private getCsiColumns(): TableColumn<CheckoutCsiInspectionVm>[] {
        return [
            {
                field: 'selected',
                caption: '',
                type: ColumnType.other,
                queryColumnExcluded: true,
                headerCssClass: 'text-center',
                cellTemplate: this.csiSelectTemplate
            },
            {
                field: 'emailPdf',
                caption: 'Email PDF',
                type: ColumnType.other,
                queryColumnExcluded: true,
                headerCssClass: 'text-center',
                cellTemplate: this.csiEmailPdfTemplate
            },
            {
                field: 'site.accountNumber',
                caption: 'Account',
                type: ColumnType.text
            },
            {
                field: 'inspectionDate',
                caption: 'Inspection Date',
                type: ColumnType.date
            },
            {
                field: 'waterSupplier.name',
                caption: 'Water Supplier',
                type: ColumnType.text
            },
            {
                field: 'propertyStreetName',
                caption: 'Property Address',
                type: ColumnType.other,
                queryColumnExcluded: true,
                cellTemplate: this.csiPropertyAddressTemplate
            },
            {
                field: 'mailingStreetName',
                caption: 'Mailing Address',
                type: ColumnType.other,
                queryColumnExcluded: true,
                cellTemplate: this.csiMailingAddressTemplate
            },
            {
                field: 'amount',
                caption: 'Fee',
                type: ColumnType.number,
                cellComponent: CurrencyCellComponent
            }
        ];
    }

    // FOG Transport

    private async initFogTransportTab(): Promise<void> {
        this.fogTransportItems.columns = this.getFogTransportColumns();
        this.fogTransportReportFor = this.isAdmin ? '' : String(this._currentUserId);

        if (this.isAdmin) {
            const users = await this._professionalUserService.getAll(
                { pageSize: MAX_PAGE_SIZE },
                { sort: {}, filter: [{ columnName: 'isFogTransporter', comparisonOperator: 'Eq', value: 'true' }] }
            );
            this.fogTransportReportForOptions = this.buildReportForOptions(users.data);
        }

        await this.getFogTripTickets();
    }

    public async getFogTripTickets(): Promise<void> {
        try {
            this.isLoading = true;

            const filter: QueryProperty[] = [
                { columnName: 'transactionId', isValueNull: true },
                { columnName: 'amount', comparisonOperator: 'Gt', value: '0' }
            ];

            if (this.fogTransportReportFor === '' && this._professionalId != null) {
                filter.push({ columnName: 'professional.id', comparisonOperator: 'Eq', value: String(this._professionalId) });
            } else {
                filter.push({ columnName: 'transporter.id', comparisonOperator: 'Eq', value: this.fogTransportReportFor });
            }

            this.fogTransportItems.query.filter = filter;

            this.fogTransportItems.items = await this._fogTripTicketService.searchForProfessional(
                { pageSize: MAX_PAGE_SIZE },
                this.fogTransportItems.query
            );

            this.fogTransportItems.items.data.forEach(ticket => {
                ticket.selected = true;
                ticket.emailPdf = true;
            });
            this.recalculateFogTransportSelectedTotal();
        } finally {
            this.isLoading = false;
        }
    }

    public recalculateFogTransportSelectedTotal(): void {
        if (this.fogTransportItems.items) {
            this.fogTransportItems.items.data = this.fogTransportItems.items.data.filter(ticket => ticket.selected);
        }

        this.fogTransportSelectedFeeTotal = (this.fogTransportItems.items?.data || [])
            .reduce((total, ticket) => total + (ticket.amount || 0), 0);
    }

    public viewFogTripTicket(ticket: CheckoutFogTripTicketVm): void {
        if (ticket?.id == null) {
            return;
        }

        this._router.navigate(['/professionals/fog/trip-tickets', ticket.id]);
    }

    public deleteFogTripTicket(ticket: CheckoutFogTripTicketVm): void {
        if (ticket?.id == null) {
            return;
        }

        this._modalHelper.showDeleteConfirmation()
            .result()
            .subscribe(() => this.processDeleteFogTripTicket(ticket));
    }

    private async processDeleteFogTripTicket(ticket: CheckoutFogTripTicketVm): Promise<void> {
        try {
            this.isLoading = true;
            await this._fogTripTicketService.deleteForProfessional(ticket.id);

            this._toastService.successFullyDeleted('Trip Ticket');
        } finally {
            this.isLoading = false;
        }

        await this.getFogTripTickets();
    }

    private getFogTransportColumns(): TableColumn<CheckoutFogTripTicketVm>[] {
        return [
            {
                field: 'selected',
                caption: '',
                type: ColumnType.other,
                queryColumnExcluded: true,
                headerCssClass: 'text-center',
                cellTemplate: this.fogSelectTemplate
            },
            {
                field: 'emailPdf',
                caption: 'Email PDF',
                type: ColumnType.other,
                queryColumnExcluded: true,
                headerCssClass: 'text-center',
                cellTemplate: this.fogEmailPdfTemplate
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
                cellTemplate: this.fogGeneratorTemplate
            },
            {
                field: 'submissionId',
                caption: 'Trip Ticket Information',
                type: ColumnType.other,
                queryColumnExcluded: true,
                cellTemplate: this.fogTripTicketInfoTemplate
            },
            {
                field: 'amount',
                caption: 'Fee',
                type: ColumnType.number,
                cellComponent: CurrencyCellComponent
            }
        ];
    }

    // FOG Inspection

    private async initFogInspectionTab(): Promise<void> {
        this.fogInspectionItems.columns = this.getFogInspectionColumns();
        this.fogInspectionReportFor = this.isAdmin ? '' : String(this._currentUserId);

        if (this.isAdmin) {
            const users = await this._professionalUserService.getAll(
                { pageSize: MAX_PAGE_SIZE },
                { sort: {}, filter: [{ columnName: 'isFogInspector', comparisonOperator: 'Eq', value: 'true' }] }
            );
            this.fogInspectionReportForOptions = this.buildReportForOptions(users.data);
        }

        await this.getFogInspections();
    }

    public async getFogInspections(): Promise<void> {
        try {
            this.isLoading = true;

            const filter: QueryProperty[] = [
                { columnName: 'transactionId', isValueNull: true },
                { columnName: 'amount', comparisonOperator: 'Gt', value: '0' }
            ];

            if (this.fogInspectionReportFor === '' && this._professionalId != null) {
                filter.push({ columnName: 'professional.id', comparisonOperator: 'Eq', value: String(this._professionalId) });
            } else {
                filter.push({ columnName: 'inspector.id', comparisonOperator: 'Eq', value: this.fogInspectionReportFor });
            }

            this.fogInspectionItems.query.filter = filter;

            this.fogInspectionItems.items = await this._fogInspectionService.getAll(
                { pageSize: MAX_PAGE_SIZE },
                this.fogInspectionItems.query,
                false
            );

            this.fogInspectionItems.items.data.forEach(inspection => inspection.selected = true);
            this.recalculateFogInspectionSelectedTotal();
        } finally {
            this.isLoading = false;
        }
    }

    public recalculateFogInspectionSelectedTotal(): void {
        if (this.fogInspectionItems.items) {
            this.fogInspectionItems.items.data = this.fogInspectionItems.items.data.filter(inspection => inspection.selected);
        }

        this.fogInspectionSelectedFeeTotal = (this.fogInspectionItems.items?.data || [])
            .reduce((total, inspection) => total + (inspection.amount || 0), 0);
    }

    public viewFogInspection(inspection: CheckoutFogInspectionVm): void {
        if (inspection?.id == null) {
            return;
        }

        this._router.navigate(['/professionals/fog/inspections', inspection.id]);
    }

    public deleteFogInspection(inspection: CheckoutFogInspectionVm): void {
        if (inspection?.id == null) {
            return;
        }

        this._modalHelper.showDeleteConfirmation()
            .result()
            .subscribe(() => this.processDeleteFogInspection(inspection));
    }

    private async processDeleteFogInspection(inspection: CheckoutFogInspectionVm): Promise<void> {
        try {
            this.isLoading = true;
            await this._fogInspectionService.deleteForProfessional(inspection.id!);

            this._toastService.successFullyDeleted('Inspection');
        } finally {
            this.isLoading = false;
        }

        await this.getFogInspections();
    }

    private getFogInspectionColumns(): TableColumn<CheckoutFogInspectionVm>[] {
        return [
            {
                field: 'selected',
                caption: '',
                type: ColumnType.other,
                queryColumnExcluded: true,
                headerCssClass: 'text-center',
                cellTemplate: this.fogInspectionSelectTemplate
            },
            {
                field: 'inspectionDate',
                caption: 'Inspection Date',
                type: ColumnType.date
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
                cellTemplate: this.fogInspectionGeneratorTemplate
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

interface CheckoutBackflowTestVm extends BackflowTest {
    selected?: boolean;
    emailPdf?: boolean;
}

interface CheckoutCsiInspectionVm extends CsiInspection {
    selected?: boolean;
    emailPdf?: boolean;
}

interface CheckoutFogTripTicketVm extends FogTripTicket {
    selected?: boolean;
    emailPdf?: boolean;
}

interface CheckoutFogInspectionVm extends FogInspection {
    selected?: boolean;
}
