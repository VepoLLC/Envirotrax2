import { Component, Input, OnInit, TemplateRef, ViewChild } from "@angular/core";
import { CellTemplateData, ColumnType, CurrencyCellComponent, InputOption, MAX_PAGE_SIZE, ModalHelperService, TableColumn } from "@envirotrax/common-ui";
import { QueryProperty } from "../../../shared/models/query";
import { TableViewModel } from "../../../shared/models/table-view-model";
import { FogInspection } from "../../../shared/models/fog/fog-inspection";
import { ProfessionalFogInspectionService } from "../../../shared/services/fog/professional-fog-inspection.service";
import { ProfesionalUserService } from "../../../shared/services/professionals/professional-user.service";
import { ProfesisonalService } from "../../../shared/services/professionals/professional.service";
import { ToastService } from "../../../shared/services/toast.service";
import { CheckoutService } from "../../../shared/services/professionals/checkout.service";
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
    public selectedFeeTotal = 0;
    public reportForOptions: InputOption[] = [];
    public reportFor = '';

    private _currentUserId?: number;
    private _professionalName?: string;

    @ViewChild('selectTemplate', { static: true })
    public selectTemplate?: TemplateRef<CellTemplateData<CheckoutFogInspectionVm>>;

    @ViewChild('generatorTemplate', { static: true })
    public generatorTemplate?: TemplateRef<CellTemplateData<CheckoutFogInspectionVm>>;

    constructor(
        private readonly _fogInspectionService: ProfessionalFogInspectionService,
        private readonly _professionalUserService: ProfesionalUserService,
        private readonly _professionalService: ProfesisonalService,
        private readonly _modalHelper: ModalHelperService,
        private readonly _toastService: ToastService,
        private readonly _checkoutService: CheckoutService,
        private readonly _router: Router
    ) {

    }

    public async ngOnInit(): Promise<void> {
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
                { columnName: 'transactionId', isValueNull: true },
                { columnName: 'amount', comparisonOperator: 'Gt', value: '0' }
            ];

            if (this.reportFor !== '') {
                filter.push({ columnName: 'inspector.id', comparisonOperator: 'Eq', value: this.reportFor });
            }

            this.items.query.filter = filter;

            this.items.items = await this._fogInspectionService.getAll(
                { pageSize: MAX_PAGE_SIZE },
                this.items.query,
                false
            );

            this.items.items.data.forEach(inspection => inspection.selected = true);
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

    public viewInspection(inspection: CheckoutFogInspectionVm): void {
        if (inspection?.id == null) {
            return;
        }

        this._router.navigate(['/professionals/fog/inspections', inspection.id]);
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
