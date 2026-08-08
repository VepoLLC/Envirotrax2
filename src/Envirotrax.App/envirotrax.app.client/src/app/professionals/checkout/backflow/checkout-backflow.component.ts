import { Component, Input, OnInit, TemplateRef, ViewChild } from "@angular/core";
import { CellTemplateData, ColumnType, CurrencyCellComponent, InputOption, MAX_PAGE_SIZE, ModalHelperService, TableColumn, ToastService } from '@envirotrax/common-ui';
import { QueryProperty } from "../../../shared/models/query";
import { TableViewModel } from "../../../shared/models/table-view-model";
import { BackflowTest } from "../../../shared/models/backflow/backflow-test";
import { BackflowTestService } from "../../../shared/services/backflow/backflow-test.service";
import { ProfesionalUserService } from "../../../shared/services/professionals/professional-user.service";
import { ProfesisonalService } from "../../../shared/services/professionals/professional.service";
import { CheckoutService } from "../../../shared/services/professionals/checkout.service";
import { Router } from "@angular/router";

@Component({
    selector: 'vp-checkout-backflow',
    standalone: false,
    templateUrl: './checkout-backflow.component.html',
    styles: `
        .vp-checkout-table-scroll {
            max-height: 500px;
            overflow-y: auto;
        }
    `
})
export class CheckoutBackflowComponent implements OnInit {
    @Input() public isAdmin = false;

    public isLoading = false;
    public items: TableViewModel<CheckoutBackflowTestVm> = {
        query: { sort: {}, filter: [] }
    };
    public selectedFeeTotal = 0;
    public reportForOptions: InputOption[] = [];
    public reportFor = '';

    private _currentUserId?: number;
    private _professionalName?: string;

    @ViewChild('selectTemplate', { static: true })
    public selectTemplate?: TemplateRef<CellTemplateData<CheckoutBackflowTestVm>>;

    @ViewChild('emailPdfTemplate', { static: true })
    public emailPdfTemplate?: TemplateRef<CellTemplateData<CheckoutBackflowTestVm>>;

    @ViewChild('assemblyTemplate', { static: true })
    public assemblyTemplate?: TemplateRef<CellTemplateData<CheckoutBackflowTestVm>>;

    @ViewChild('propertyTemplate', { static: true })
    public propertyTemplate?: TemplateRef<CellTemplateData<CheckoutBackflowTestVm>>;

    constructor(
        private readonly _backflowTestService: BackflowTestService,
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
                    { sort: {}, filter: [{ columnName: 'isBackflowTester', comparisonOperator: 'Eq', value: 'true' }] }
                );
                this.reportForOptions = this.buildReportForOptions(users.data);
            }

            await this.getBackflowTests();
        }finally {
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

    public async getBackflowTests(): Promise<void> {
        try {
            this.isLoading = true;

            const filter: QueryProperty[] = [
                { columnName: 'transactionId', isValueNull: true },
                { columnName: 'amount', comparisonOperator: 'Gt', value: '0' }
            ];

            if (this.reportFor !== '') {
                filter.push({ columnName: 'bpat.id', comparisonOperator: 'Eq', value: this.reportFor });
            }

            this.items.query.filter = filter;

            this.items.items = await this._backflowTestService.getAllForProfessional(
                { pageSize: MAX_PAGE_SIZE },
                this.items.query
            );

            this.items.items.data.forEach(test => {
                test.selected = true;
                test.emailPdf = true;
            });
            this.recalculateSelectedTotal();
        } finally {
            this.isLoading = false;
        }
    }

    public recalculateSelectedTotal(): void {
        if (this.items.items) {
            this.items.items.data = this.items.items.data.filter(test => test.selected);
        }

        this.selectedFeeTotal = (this.items.items?.data || [])
            .reduce((total, test) => total + (test.amount || 0), 0);
    }

    public viewTest(test: CheckoutBackflowTestVm): void {
        if (test?.id == null) {
            return;
        }

        this._router.navigate(['/professionals/backflow/tests', test.id, 'view']);
    }

    public editTest(test: CheckoutBackflowTestVm): void {
        if (test?.id == null) {
            return;
        }

        this._router.navigate(['/professionals/backflow/submit', test.id]);
    }

    public deleteTest(test: CheckoutBackflowTestVm): void {
        if (test?.id == null) {
            return;
        }

        this._modalHelper.showDeleteConfirmation()
            .result()
            .subscribe(() => this.processDelete(test));
    }

    private async processDelete(test: CheckoutBackflowTestVm): Promise<void> {
        try {
            this.isLoading = true;
            await this._backflowTestService.deleteForProfessional(test.id);

            this._toastService.successFullyDeleted('Test Report');
            this._checkoutService.refresh();
        } finally {
            this.isLoading = false;
        }

        await this.getBackflowTests();
    }

    private getColumns(): TableColumn<CheckoutBackflowTestVm>[] {
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
                cellTemplate: this.propertyTemplate
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
