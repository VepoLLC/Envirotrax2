import { Component, Input, OnInit, TemplateRef, ViewChild } from "@angular/core";
import { CellTemplateData, ColumnType, CurrencyCellComponent, InputOption, MAX_PAGE_SIZE, ModalHelperService, TableColumn, ToastService } from '@envirotrax/common-ui';
import { QueryProperty } from "../../../shared/models/query";
import { TableViewModel } from "../../../shared/models/table-view-model";
import { CsiInspection } from "../../../shared/models/csi/csi-inspection";
import { CsiInspectionService } from "../../../shared/services/csi/csi-inspection.service";
import { ProfesionalUserService } from "../../../shared/services/professionals/professional-user.service";
import { ProfesisonalService } from "../../../shared/services/professionals/professional.service";
import { CheckoutService } from "../../../shared/services/professionals/checkout.service";
import { Router } from "@angular/router";

@Component({
    selector: 'vp-checkout-csi',
    standalone: false,
    templateUrl: './checkout-csi.component.html',
    styles: `
        .vp-checkout-table-scroll {
            max-height: 500px;
            overflow-y: auto;
        }
    `
})
export class CheckoutCsiComponent implements OnInit {
    @Input() public isAdmin = false;

    public isLoading = false;
    public items: TableViewModel<CheckoutCsiInspectionVm> = {
        query: { sort: {}, filter: [] }
    };
    public selectedFeeTotal = 0;
    public reportForOptions: InputOption[] = [];
    public reportFor = '';

    private _currentUserId?: number;
    private _professionalName?: string;

    @ViewChild('selectTemplate', { static: true })
    public selectTemplate?: TemplateRef<CellTemplateData<CheckoutCsiInspectionVm>>;

    @ViewChild('emailPdfTemplate', { static: true })
    public emailPdfTemplate?: TemplateRef<CellTemplateData<CheckoutCsiInspectionVm>>;

    @ViewChild('propertyAddressTemplate', { static: true })
    public propertyAddressTemplate?: TemplateRef<CellTemplateData<CheckoutCsiInspectionVm>>;

    @ViewChild('mailingAddressTemplate', { static: true })
    public mailingAddressTemplate?: TemplateRef<CellTemplateData<CheckoutCsiInspectionVm>>;

    constructor(
        private readonly _csiInspectionService: CsiInspectionService,
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
                    { sort: {}, filter: [{ columnName: 'isCsiInspector', comparisonOperator: 'Eq', value: 'true' }] }
                );
                this.reportForOptions = this.buildReportForOptions(users.data);
            }

            await this.getCsiInspections();
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

    public async getCsiInspections(): Promise<void> {
        try {
            this.isLoading = true;

            const filter: QueryProperty[] = [
                { columnName: 'transactionId', isValueNull: true },
                { columnName: 'amount', comparisonOperator: 'Gt', value: '0' }
            ];

            if (this.reportFor !== '') {
                filter.push({ columnName: 'inspectorUser.id', comparisonOperator: 'Eq', value: this.reportFor });
            }

            this.items.query.filter = filter;

            this.items.items = await this._csiInspectionService.getProfessionalInspections(
                { pageSize: MAX_PAGE_SIZE },
                this.items.query,
                false
            );

            this.items.items.data.forEach(inspection => {
                inspection.selected = true;
                inspection.emailPdf = true;
            });
            this.recalculateSelectedTotal();
        } finally {
            this.isLoading = false;
        }
    }

    public recalculateSelectedTotal(): void {
        if (this.items.items) {
            this.items.items.data = this.items.items.data.filter(inspection => inspection.selected);
        }

        this.selectedFeeTotal = (this.items.items?.data || [])
            .reduce((total, inspection) => total + (inspection.amount || 0), 0);
    }

    public viewInspection(inspection: CheckoutCsiInspectionVm): void {
        if (inspection?.id == null) {
            return;
        }

        this._router.navigate(['/professionals/csi/inspections', inspection.id]);
    }

    public editInspection(inspection: CheckoutCsiInspectionVm): void {
        if (inspection?.site?.id == null) {
            return;
        }

        this._router.navigate(['/professionals/csi/inspections/create', inspection.site.id]);
    }

    public deleteInspection(inspection: CheckoutCsiInspectionVm): void {
        if (inspection?.id == null) {
            return;
        }

        this._modalHelper.showDeleteConfirmation()
            .result()
            .subscribe(() => this.processDelete(inspection));
    }

    private async processDelete(inspection: CheckoutCsiInspectionVm): Promise<void> {
        try {
            this.isLoading = true;
            await this._csiInspectionService.deleteForProfessional(inspection.id!);

            this._toastService.successFullyDeleted('Inspection');
            this._checkoutService.refresh();
        } finally {
            this.isLoading = false;
        }

        await this.getCsiInspections();
    }

    private getColumns(): TableColumn<CheckoutCsiInspectionVm>[] {
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
                cellTemplate: this.propertyAddressTemplate
            },
            {
                field: 'mailingStreetName',
                caption: 'Mailing Address',
                type: ColumnType.other,
                queryColumnExcluded: true,
                cellTemplate: this.mailingAddressTemplate
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

interface CheckoutCsiInspectionVm extends CsiInspection {
    selected?: boolean;
    emailPdf?: boolean;
}
