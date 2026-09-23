import { Component, Input, OnInit, TemplateRef, ViewChild } from "@angular/core";
import { NgForm } from "@angular/forms";
import { CellTemplateData, ColumnType, CurrencyCellComponent, InputOption, MAX_PAGE_SIZE, ModalHelperService, TableColumn, ToastService, ToastType } from '@envirotrax/common-ui';
import { QueryProperty } from "../../../shared/models/query";
import { TableViewModel } from "../../../shared/models/table-view-model";
import { BackflowTest } from "../../../shared/models/backflow/backflow-test";
import { BackflowCheckoutReceipt, BackflowCheckoutRequest } from "../../../shared/models/backflow/backflow-checkout";
import { State } from "../../../shared/models/lookup/state";
import { ProfessionalUser } from "../../../shared/models/professionals/professional-user";
import { BackflowTestService } from "../../../shared/services/backflow/backflow-test.service";
import { ProfesionalUserService } from "../../../shared/services/professionals/professional-user.service";
import { ProfesisonalService } from "../../../shared/services/professionals/professional.service";
import { CheckoutService } from "../../../shared/services/professionals/checkout.service";
import { LookupService } from "../../../shared/services/lookup/lookup.service";
import { HelperService } from "../../../shared/services/helpers/helper.service";
import { CreditCardPaymentComponent, CreditCardToken } from "../../../shared/components/credit-card-payment/credit-card-payment.component";
import { createPaymentTransactionId } from "../../../shared/utils/payment-transaction-id.util";
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
    public amounts: CheckoutAmounts = { total: 0, fromBalance: 0, cardCharge: 0 };
    public accountBalance = 0;
    public reportForOptions: InputOption[] = [];
    public reportFor = '';

    public professionalUser: ProfessionalUser = {};
    public states: InputOption<State>[] = [];
    public validationErrors: string[] = [];
    public receipt?: BackflowCheckoutReceipt;

    private _currentUserId?: number;
    private _professionalName?: string;

    @ViewChild(CreditCardPaymentComponent)
    public creditCardPayment?: CreditCardPaymentComponent;

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
        private readonly _lookupService: LookupService,
        private readonly _helper: HelperService,
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

            const [currentUser, states] = await Promise.all([
                this._professionalUserService.getMyData(),
                this._lookupService.getAllStatesAsOptions(true)
            ]);

            this._currentUserId = currentUser.id;
            this.professionalUser = currentUser;
            this.states = states;
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
                { columnName: 'transactionId', isValueNull: true }
            ];

            if (this.reportFor !== '') {
                filter.push({ columnName: 'bpat.id', comparisonOperator: 'Eq', value: this.reportFor });
            }

            this.items.query.filter = filter;

            const [tests, professional] = await Promise.all([
                this._backflowTestService.getAllForProfessional({ pageSize: MAX_PAGE_SIZE }, this.items.query),
                this._professionalService.reloadLoggedInProfessional()
            ]);

            this.items.items = tests;
            this.accountBalance = professional.accountBalance ?? 0;

            this.items.items.data.forEach(test => {
                test.selected = true;
                test.emailPdf = true;
            });
            this.recalculateAmounts();
        } finally {
            this.isLoading = false;
        }
    }

    public removeUnselectedTests(): void {
        if (this.items.items) {
            this.items.items.data = this.items.items.data.filter(test => test.selected);
        }

        this.recalculateAmounts();
    }

    public toggleSelected(test: CheckoutBackflowTestVm): void {
        test.selected = !test.selected;
        this.recalculateAmounts();
    }

    private recalculateAmounts(): void {
        const total = roundToCents(this.getSelectedTests().reduce((sum, test) => sum + (test.amount || 0), 0));
        const availableBalance = Math.floor(this.accountBalance * 100) / 100;
        const fromBalance = Math.min(availableBalance, total);

        this.amounts = { total, fromBalance, cardCharge: roundToCents(total - fromBalance) };
    }

    private getSelectedTests(): CheckoutBackflowTestVm[] {
        return (this.items.items?.data || []).filter(test => test.selected);
    }

    public stateChanged(stateId: number): void {
        this.professionalUser.billingState = stateId ? { id: stateId } : undefined;
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

        this._router.navigate(['/professionals/backflow/tests', test.id, 'edit']);
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

    public onCardTokenCaptured(token: CreditCardToken, form: NgForm): void {
        if (form.invalid) {
            return;
        }

        this.completePayment({
            dataDescriptor: token.dataDescriptor,
            dataValue: token.dataValue,
            billingFirstName: this.professionalUser.billingFirstName!,
            billingLastName: this.professionalUser.billingLastName!,
            billingAddress: this.professionalUser.billingAddress!,
            billingCity: this.professionalUser.billingCity!,
            billingState: this.professionalUser.billingState!,
            billingZipCode: this.professionalUser.billingZipCode!
        });
    }

    public async completePayment(card?: BackflowCheckoutRequest['card']): Promise<void> {
        if (this.isLoading) {
            return;
        }

        this.recalculateAmounts();

        const selectedTests = this.getSelectedTests();

        if (selectedTests.length === 0) {
            return;
        }

        const request: BackflowCheckoutRequest = {
            transactionId: createPaymentTransactionId(),
            tests: selectedTests.map(test => ({ id: test.id, emailPdf: !!test.emailPdf })),
            expectedTotal: this.amounts.total,
            expectedCcCharge: this.amounts.cardCharge,
            card
        };

        this.validationErrors = [];

        try {
            this.isLoading = true;
            this.receipt = await this._backflowTestService.checkout(request);

            this._toastService.show({ text: 'Payment completed.', type: ToastType.Success });
            this._checkoutService.refresh();
        } catch (error) {
            if (!this._helper.parseValidationErrors(error, this.validationErrors)) {
                throw error;
            }

            this.creditCardPayment?.reset();
            await this.getBackflowTests();
        } finally {
            this.isLoading = false;
        }
    }

    public printReceipt(): void {
        window.print();
    }

    public returnToAccountOverview(): void {
        this._router.navigate(['/']);
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

interface CheckoutAmounts {
    total: number;
    fromBalance: number;
    cardCharge: number;
}

function roundToCents(amount: number): number {
    return Math.round(amount * 100) / 100;
}
