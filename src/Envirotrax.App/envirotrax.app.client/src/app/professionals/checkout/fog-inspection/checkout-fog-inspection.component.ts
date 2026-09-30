import { Component, Input, OnInit, TemplateRef, ViewChild } from "@angular/core";
import { CellTemplateData, ColumnType, CurrencyCellComponent, InputOption, MAX_PAGE_SIZE, ModalHelperService, TableColumn, ToastService, ToastType } from '@envirotrax/common-ui';
import { QueryProperty } from "../../../shared/models/query";
import { TableViewModel } from "../../../shared/models/table-view-model";
import { FogInspection } from "../../../shared/models/fog/fog-inspection";
import { ProfessionalCheckoutReceipt, ProfessionalCheckoutRequest } from "../../../shared/models/payments/professional-checkout";
import { CreditCardPayment } from "../../../shared/models/payments/credit-card-payment";
import { ProfessionalFogInspectionService } from "../../../shared/services/fog/professional-fog-inspection.service";
import { ProfesionalUserService } from "../../../shared/services/professionals/professional-user.service";
import { ProfesisonalService } from "../../../shared/services/professionals/professional.service";
import { CheckoutService } from "../../../shared/services/professionals/checkout.service";
import { HelperService } from "../../../shared/services/helpers/helper.service";
import { createPaymentTransactionId } from "../../../shared/utils/payment-transaction-id.util";
import { calculateCheckoutAmounts, CheckoutAmounts } from "../../../shared/utils/checkout-amounts.util";
import { CheckoutPaymentComponent } from "../shared/checkout-payment/checkout-payment.component";
import { Router } from "@angular/router";

@Component({
    selector: 'vp-checkout-fog-inspection',
    standalone: false,
    templateUrl: './checkout-fog-inspection.component.html',
    styles: `
        .vp-checkout-table-scroll {
            max-height: 500px;
            overflow-y: auto;
        }
    `
})
export class CheckoutFogInspectionComponent implements OnInit {
    @Input() public isAdmin = false;

    public isLoading = false;
    public items: TableViewModel<CheckoutFogInspectionVm> = {
        query: { sort: {}, filter: [] }
    };
    public amounts: CheckoutAmounts = { total: 0, fromBalance: 0, cardCharge: 0 };
    public accountBalance = 0;
    public reportForOptions: InputOption[] = [];
    public reportFor = '';

    public validationErrors: string[] = [];
    public receipt?: ProfessionalCheckoutReceipt<FogInspection>;

    private _currentUserId?: number;
    private _professionalName?: string;

    @ViewChild(CheckoutPaymentComponent)
    public checkoutPayment?: CheckoutPaymentComponent;

    @ViewChild('selectTemplate', { static: true })
    public selectTemplate?: TemplateRef<CellTemplateData<CheckoutFogInspectionVm>>;

    @ViewChild('generatorTemplate', { static: true })
    public generatorTemplate?: TemplateRef<CellTemplateData<CheckoutFogInspectionVm>>;

    constructor(
        private readonly _fogInspectionService: ProfessionalFogInspectionService,
        private readonly _professionalUserService: ProfesionalUserService,
        private readonly _professionalService: ProfesisonalService,
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

            const currentUser = await this._professionalUserService.getMyData();
            this._currentUserId = currentUser.id;
            this.reportFor = this.isAdmin ? '' : String(this._currentUserId);

            if (this.isAdmin) {
                const professional = await this._professionalService.getLoggedInProfessional();
                this._professionalName = professional.name;

                const users = await this._professionalUserService.getAll(
                    { pageSize: MAX_PAGE_SIZE },
                    { sort: {}, filter: [{ columnName: 'isFogInspector', comparisonOperator: 'Eq', value: 'true' }] }
                );
                this.reportForOptions = this.buildReportForOptions(users.data);
            }

            await this.getFogInspections();
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

    public async getFogInspections(): Promise<void> {
        try {
            this.isLoading = true;

            const filter: QueryProperty[] = [
                { columnName: 'transactionId', isValueNull: true }
            ];

            if (this.reportFor !== '') {
                filter.push({ columnName: 'inspector.id', comparisonOperator: 'Eq', value: this.reportFor });
            }

            this.items.query.filter = filter;

            const [inspections, professional] = await Promise.all([
                this._fogInspectionService.getAll({ pageSize: MAX_PAGE_SIZE }, this.items.query, false),
                this._professionalService.reloadLoggedInProfessional()
            ]);

            this.items.items = inspections;
            this.accountBalance = professional.accountBalance ?? 0;

            this.items.items.data.forEach(inspection => inspection.selected = true);
            this.recalculateAmounts();
        } finally {
            this.isLoading = false;
        }
    }

    public removeUnselectedInspections(): void {
        if (this.items.items) {
            this.items.items.data = this.items.items.data.filter(inspection => inspection.selected);
        }

        this.recalculateAmounts();
    }

    public toggleSelected(inspection: CheckoutFogInspectionVm): void {
        inspection.selected = !inspection.selected;
        this.recalculateAmounts();
    }

    private recalculateAmounts(): void {
        this.amounts = calculateCheckoutAmounts(this.getSelectedInspections().map(inspection => inspection.amount || 0), this.accountBalance);
    }

    private getSelectedInspections(): CheckoutFogInspectionVm[] {
        return (this.items.items?.data || []).filter(inspection => inspection.selected);
    }

    public viewInspection(inspection: CheckoutFogInspectionVm): void {
        if (inspection?.id == null) {
            return;
        }

        this._router.navigate(['/professionals/fog/inspections', inspection.id]);
    }

    public editInspection(inspection: CheckoutFogInspectionVm): void {
        if (inspection?.id == null) {
            return;
        }

        this._router.navigate(['/professionals/fog/inspections/edit', inspection.id]);
    }

    public deleteInspection(inspection: CheckoutFogInspectionVm): void {
        if (inspection?.id == null) {
            return;
        }

        this._modalHelper.showDeleteConfirmation()
            .result()
            .subscribe(() => this.processDelete(inspection));
    }

    private async processDelete(inspection: CheckoutFogInspectionVm): Promise<void> {
        try {
            this.isLoading = true;
            await this._fogInspectionService.deleteForProfessional(inspection.id!);

            this._toastService.successFullyDeleted('Inspection');
            this._checkoutService.refresh();
        } finally {
            this.isLoading = false;
        }

        await this.getFogInspections();
    }

    public async completePayment(card?: CreditCardPayment): Promise<void> {
        if (this.isLoading) {
            return;
        }

        this.recalculateAmounts();

        const selectedInspections = this.getSelectedInspections();

        if (selectedInspections.length === 0) {
            return;
        }

        const request: ProfessionalCheckoutRequest = {
            transactionId: createPaymentTransactionId(),
            items: selectedInspections.map(inspection => ({ id: inspection.id, emailPdf: false })),
            expectedTotal: this.amounts.total,
            expectedCardCharge: this.amounts.cardCharge,
            card
        };

        this.validationErrors = [];

        try {
            this.isLoading = true;
            this.receipt = await this._fogInspectionService.checkout(request);

            this._toastService.show({ text: 'Payment completed.', type: ToastType.Success });
            this._checkoutService.refresh();
        } catch (error) {
            if (!this._helper.parseValidationErrors(error, this.validationErrors)) {
                throw error;
            }

            this.checkoutPayment?.reset();
            await this.getFogInspections();
        } finally {
            this.isLoading = false;
        }
    }

    private getColumns(): TableColumn<CheckoutFogInspectionVm>[] {
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
                cellTemplate: this.generatorTemplate
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

interface CheckoutFogInspectionVm extends FogInspection {
    selected?: boolean;
}
