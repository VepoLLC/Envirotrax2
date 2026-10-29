import { CommonModule } from '@angular/common';
import {
    Component,
    EventEmitter,
    Input,
    OnChanges,
    OnInit,
    Output,
    SimpleChanges,
    TemplateRef,
    ViewChild
} from '@angular/core';
import {
    CellTemplateData,
    ColumnType,
    Query,
    QueryProperty,
    TableColumn,
    TableCustomAction,
    TableViewModel,
    ToastService
} from '@envirotrax/common-ui';
import { SharedComponentsModule } from '../../../shared/components/shared.components.module';
import { PropertyType } from '../../../shared/models/sites/site';
import {
    SubmissionValidationFlag,
    SubmissionValidationRow,
    SubmissionValidationSource
} from '../../../shared/models/validations/submission-validation';
import { WindowService } from '../../../shared/services/window.service';
import { SubmissionValidationBadgeComponent } from '../badge/submission-validation-badge.component';
import { DEFAULT_SORT, getReasonFlags, NEEDS_VALIDATION_FLAG, ON_HOLD_FLAG } from '../submission-validation-flags';

@Component({
    selector: 'vp-submission-validation-tab',
    templateUrl: './submission-validation-tab.component.html',
    imports: [
        CommonModule,
        SharedComponentsModule,
        SubmissionValidationBadgeComponent
    ]
})
export class SubmissionValidationTabComponent implements OnInit, OnChanges {
    @ViewChild('holdCell', { static: true })
    public holdCell?: TemplateRef<CellTemplateData<SubmissionValidationRow>>;

    @ViewChild('statusCell', { static: true })
    public statusCell?: TemplateRef<CellTemplateData<SubmissionValidationRow>>;

    @ViewChild('reasonCell', { static: true })
    public reasonCell?: TemplateRef<CellTemplateData<SubmissionValidationRow>>;

    @ViewChild('propertyCell', { static: true })
    public propertyCell?: TemplateRef<CellTemplateData<SubmissionValidationRow>>;

    @ViewChild('infoCell', { static: true })
    public infoCell?: TemplateRef<CellTemplateData<SubmissionValidationRow>>;

    @Input()
    public source!: SubmissionValidationSource;

    // Null when none of the tab's reasons is selected, so nothing can match. Every search and refresh passes a new
    // array, which reloads the grid.
    @Input()
    public filter: QueryProperty[] | null = null;

    @Output()
    public countChange: EventEmitter<number> = new EventEmitter();

    public readonly propertyType = PropertyType;

    public legend: SubmissionValidationFlag[] = [];

    public canViewDetails: boolean = false;

    public table: TableViewModel<SubmissionValidationRow> = {
        query: {
            sort: { ...DEFAULT_SORT },
            filter: []
        }
    };

    public readonly customActions: TableCustomAction<SubmissionValidationRow>[] = [
        {
            text: 'Clear',
            iconClass: 'fa-solid fa-eraser',
            action: row => this.clear(row)
        }
    ];

    constructor(
        private readonly _windowService: WindowService,
        private readonly _toastService: ToastService
    ) {

    }

    public async ngOnChanges(changes: SimpleChanges): Promise<void> {
        if (changes['source']) {
            this.legend = [NEEDS_VALIDATION_FLAG, ON_HOLD_FLAG, ...getReasonFlags(this.source.reasons)];
            this.canViewDetails = !!this.source.detailsComponent;
        }

        if (changes['filter']) {
            if (this.table.items?.pageInfo) {
                this.table.items.pageInfo.pageNumber = 1;
            }

            await this.load();
        }
    }

    public ngOnInit(): void {
        this.table.columns = this.getColumns();
    }

    public async load(): Promise<void> {
        if (this.filter == null) {
            this.table.items = { data: [], pageInfo: { pageNumber: 1, totalItems: 0, totalPages: 0 } };
            this.countChange.emit(0);

            return;
        }

        // The grid's own query holds only its sort and any column filters, so a "Reset" in the grid can never drop
        // the validation criteria. A third click on a header clears the sort, which the CSI search can't take.
        const sort = this.table.query.sort ?? {};

        const query: Query = {
            sort: Object.keys(sort).length > 0 ? sort : { ...DEFAULT_SORT },
            filter: [...this.filter, ...(this.table.query.filter ?? [])]
        };

        try {
            this.table.isLoading = true;

            this.table.items = await this.source.load(this.table.items?.pageInfo || {}, query);
        } finally {
            this.table.isLoading = false;
        }

        this.countChange.emit(this.table.items?.pageInfo?.totalItems ?? 0);
    }

    public openDetails(row: SubmissionValidationRow): void {
        if (!this.source.detailsComponent) {
            return;
        }

        this._windowService.addWindow(this.source.detailsComponent, {
            title: row.propertyAddress ? `${row.id} - ${row.propertyAddress}` : String(row.id),
            model: { id: row.id }
        });
    }

    public async toggleHold(row: SubmissionValidationRow, event: Event): Promise<void> {
        const checkbox = event.target as HTMLInputElement;

        try {
            this.table.isLoading = true;

            await this.source.actions.updateValidationHold(row.id!, row.waterSupplier!.id!, checkbox.checked);

            this._toastService.successfullySaved();
        } finally {
            this.table.isLoading = false;

            await this.load();
        }
    }

    private async clear(row: SubmissionValidationRow): Promise<void> {
        try {
            this.table.isLoading = true;

            await this.source.actions.updateNeedsValidation(row.id!, row.waterSupplier!.id!, false);

            this._toastService.successfullySaved();
        } finally {
            this.table.isLoading = false;

            await this.load();
        }
    }

    private getColumns(): TableColumn<SubmissionValidationRow>[] {
        return [
            { field: 'id', caption: 'ID', type: ColumnType.number },
            {
                field: 'validationOnHold',
                caption: 'Hold',
                type: ColumnType.other,
                cellTemplate: this.holdCell,
                sortColumnExcluded: true,
                queryColumnExcluded: true
            },
            {
                field: 'statusFlag',
                caption: '',
                type: ColumnType.other,
                cellTemplate: this.statusCell,
                sortColumnExcluded: true,
                queryColumnExcluded: true
            },
            { field: 'waterSupplier.name', caption: 'Water Supplier', type: ColumnType.text },
            {
                field: 'reasonFlags',
                caption: '',
                type: ColumnType.other,
                cellTemplate: this.reasonCell,
                sortColumnExcluded: true,
                queryColumnExcluded: true
            },
            {
                field: 'propertyAddress',
                caption: 'Property Information',
                type: ColumnType.other,
                cellTemplate: this.propertyCell,
                sortColumnExcluded: true,
                queryColumnExcluded: true
            },
            {
                field: 'infoLines',
                caption: this.source.infoCaption,
                type: ColumnType.other,
                cellTemplate: this.infoCell,
                sortColumnExcluded: true,
                queryColumnExcluded: true
            }
        ];
    }
}
