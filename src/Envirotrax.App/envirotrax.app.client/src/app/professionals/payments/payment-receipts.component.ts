import { Component, OnInit, TemplateRef, ViewChild } from "@angular/core";
import { CellTemplateData, ColumnType, CurrencyCellComponent, InputOption, TableColumn } from "@envirotrax/common-ui";
import { QueryProperty } from "../../shared/models/query";
import { TableViewModel } from "../../shared/models/table-view-model";
import { PagedData } from "../../shared/models/paged-data";
import { ProfessionalTransaction, ProfessionalTransactionType, professionalTransactionTypeLabels } from "../../shared/models/payments/professional-transaction";
import { ProfessionalTransactionService } from "../../shared/services/professionals/professional-transaction.service";
import { AuthService } from "../../shared/services/auth/auth.service";
import { DownloadService } from "../../shared/services/download.service";
import { FeatureType } from "../../shared/models/feature-type";
import { ROLE_DEFINITIONS } from "../../shared/models/role-definitions";

interface PaymentReceiptRowVm extends ProfessionalTransaction {
    account?: string;
    typeLabel: string;
}

@Component({
    selector: 'vp-payment-receipts',
    standalone: false,
    templateUrl: './payment-receipts.component.html'
})
export class PaymentReceiptsComponent implements OnInit {
    @ViewChild('dateCell', { static: true })
    private _dateCellTemplate!: TemplateRef<CellTemplateData<PaymentReceiptRowVm>>;

    @ViewChild('receiptCell', { static: true })
    private _receiptCellTemplate!: TemplateRef<CellTemplateData<PaymentReceiptRowVm>>;

    public isLoading = false;

    public transactionTypeOptions: InputOption[] = [];
    public transactionType = '';

    public fromDate = '';
    public toDate = '';

    private _searchFilter: QueryProperty[] = [];
    private _hiddenTypes: ProfessionalTransactionType[] = [];

    public table: TableViewModel<PaymentReceiptRowVm> = {
        columns: [],
        query: { sort: { transactionDate: 'Desc' }, filter: [] }
    };

    constructor(
        private readonly _transactionService: ProfessionalTransactionService,
        private readonly _authService: AuthService,
        private readonly _downloadService: DownloadService
    ) {

    }

    public async ngOnInit(): Promise<void> {
        const today = new Date();
        const monthAgo = new Date(today.getFullYear(), today.getMonth() - 1, today.getDate());

        this.fromDate = this.toDateInputValue(monthAgo);
        this.toDate = this.toDateInputValue(today);
        this.table.columns = this.getColumns();

        try {
            this.isLoading = true;
            this.transactionTypeOptions = await this.getTransactionTypeOptions();
        } finally {
            this.isLoading = false;
        }

        await this.search();
    }

    private async getTransactionTypeOptions(): Promise<InputOption[]> {
        const [
            hasBackflowTesting,
            hasCsiInspection,
            hasFogInspection,
            hasFogTransportation,
            isAdmin,
            isBackflowTester,
            isCsiInspector,
            isFogInspector,
            isFogTransporter
        ] = await Promise.all([
            this._authService.hasAnyFeatures(FeatureType.BackflowTesting),
            this._authService.hasAnyFeatures(FeatureType.CsiInspection),
            this._authService.hasAnyFeatures(FeatureType.FogInspection),
            this._authService.hasAnyFeatures(FeatureType.FogTransportation),
            this._authService.hasAnyRoles(ROLE_DEFINITIONS.PROFESSIONALS.ADMIN),
            this._authService.hasAnyRoles(ROLE_DEFINITIONS.PROFESSIONALS.BACKFLOW_TESTER),
            this._authService.hasAnyRoles(ROLE_DEFINITIONS.PROFESSIONALS.CSI_INSPECTOR),
            this._authService.hasAnyRoles(ROLE_DEFINITIONS.PROFESSIONALS.FOG_INSPECTOR),
            this._authService.hasAnyRoles(ROLE_DEFINITIONS.PROFESSIONALS.FOG_TRANSPORTER)
        ]);

        const typeAccess: { type: ProfessionalTransactionType, text: string, canView: boolean }[] = [
            { type: ProfessionalTransactionType.BackflowTestPayment, text: 'View Backflow Test Payments', canView: hasBackflowTesting && (isBackflowTester || isAdmin) },
            { type: ProfessionalTransactionType.CsiInspection, text: 'View CSI Inspection Payments', canView: hasCsiInspection && (isCsiInspector || isAdmin) },
            { type: ProfessionalTransactionType.FogInspectionPayment, text: 'View FOG Inspection Payments', canView: hasFogInspection && (isFogInspector || isAdmin) },
            { type: ProfessionalTransactionType.FogTripTicketPayment, text: 'View Trip Ticket Payments', canView: hasFogTransportation && (isFogTransporter || isAdmin) }
        ];

        this._hiddenTypes = typeAccess.filter(access => !access.canView).map(access => access.type);

        return [
            { id: '', text: 'View all payments' },
            ...typeAccess
                .filter(access => access.canView)
                .map(access => ({ id: String(access.type), text: access.text })),
            { id: String(ProfessionalTransactionType.BalanceAdjustment), text: 'View Account Balance Payments' }
        ];
    }

    public async search(): Promise<void> {
        this._searchFilter = this.buildFilter();

        if (this.table.items) {
            this.table.items.pageInfo.pageNumber = 1;
        }

        await this.getTransactions();
    }

    public async getTransactions(): Promise<void> {
        const query = { ...this.table.query, filter: [...this._searchFilter, ...(this.table.query.filter ?? [])] };

        try {
            this.isLoading = true;

            const transactions = await this._transactionService.getAll(this.table.items?.pageInfo || {}, query);

            this.table.items = this.toRowVms(transactions);
        } finally {
            this.isLoading = false;
        }
    }

    public async viewReceipt(transaction: PaymentReceiptRowVm): Promise<void> {
        await this._downloadService.openBlobInNewWindow(() => this._transactionService.getReceiptPdf(transaction.transactionId));
    }

    private buildFilter(): QueryProperty[] {
        const from = new Date(`${this.fromDate}T00:00:00`).toISOString();
        const to = new Date(`${this.toDate}T23:59:59.999`).toISOString();

        const filter: QueryProperty[] = [{
            columnName: 'transactionDate',
            children: [
                { columnName: 'transactionDate', value: from, comparisonOperator: 'Gte', logicalOperator: 'And' },
                { columnName: 'transactionDate', value: to, comparisonOperator: 'Lte', logicalOperator: 'And' }
            ]
        }];

        if (this.transactionType) {
            filter.push({ columnName: 'transactionType', value: this.transactionType, comparisonOperator: 'Eq' });
        } else {
            for (const hiddenType of this._hiddenTypes) {
                filter.push({ columnName: 'transactionType', value: String(hiddenType), comparisonOperator: 'NotEq' });
            }
        }

        return filter;
    }

    private toRowVms(transactions: PagedData<ProfessionalTransaction>): PagedData<PaymentReceiptRowVm> {
        return {
            ...transactions,
            data: transactions.data.map(transaction => ({
                ...transaction,
                account: transaction.user?.emailAddress,
                typeLabel: professionalTransactionTypeLabels[transaction.transactionType]
            }))
        };
    }

    private toDateInputValue(date: Date): string {
        const month = String(date.getMonth() + 1).padStart(2, '0');
        const day = String(date.getDate()).padStart(2, '0');

        return `${date.getFullYear()}-${month}-${day}`;
    }

    private getColumns(): TableColumn<PaymentReceiptRowVm>[] {
        return [
            {
                field: 'transactionDate',
                caption: 'Date',
                type: ColumnType.date,
                cellTemplate: this._dateCellTemplate
            },
            {
                field: 'transactionId',
                caption: 'Transaction ID',
                type: ColumnType.text
            },
            {
                field: 'account',
                caption: 'Account',
                type: ColumnType.text,
                queryColumnExcluded: true
            },
            {
                field: 'typeLabel',
                caption: 'Type',
                type: ColumnType.text,
                queryColumnExcluded: true
            },
            {
                field: 'referenceDescription',
                caption: 'Description',
                type: ColumnType.text
            },
            {
                field: 'balanceAdjustment',
                caption: 'Balance Adjustment',
                cellComponent: CurrencyCellComponent,
                type: ColumnType.number
            },
            {
                field: 'cardCharge',
                caption: 'CC Charge',
                cellComponent: CurrencyCellComponent,
                type: ColumnType.number
            },
            {
                field: 'amount',
                caption: 'Amount',
                cellComponent: CurrencyCellComponent,
                type: ColumnType.number
            },
            {
                field: '',
                caption: '',
                type: ColumnType.other,
                queryColumnExcluded: true,
                cellTemplate: this._receiptCellTemplate
            }
        ];
    }
}
