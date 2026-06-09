import { Component, Input, OnInit, TemplateRef, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { BackflowTest } from '../../../shared/models/backflow/backflow-test';
import { BackflowTestService } from '../../../shared/services/backflow/backflow-test.service';
import { BackflowTestOptionsService } from '../../../shared/services/backflow/backflow-test-options.service';
import { TableViewModel } from '../../../shared/models/table-view-model';
import { ComparisonOperator, QueryProperty } from '../../../shared/models/query';
import { BackflowTestResult } from '../../../shared/models/backflow/backflow-test-enums';
import { AuthService } from '../../../shared/services/auth/auth.service';
import { PermissionAction, PermissionType } from '../../../shared/models/permission-type';
import { CellTemplateData, ColumnType, InputOption, TableColumn } from '@envirotrax/common-ui';

@Component({
    selector: 'app-site-backflow-tests',
    standalone: false,
    templateUrl: './site-backflow-tests.component.html'
})
export class SiteBackflowTestsComponent implements OnInit {
    @Input()
    public siteId?: number;

    @ViewChild('statusTemplate', { static: true })
    public statusTemplate!: TemplateRef<CellTemplateData<BackflowTest>>;

    @ViewChild('datesTemplate', { static: true })
    public datesTemplate!: TemplateRef<CellTemplateData<BackflowTest>>;

    @ViewChild('assemblyTemplate', { static: true })
    public assemblyTemplate!: TemplateRef<CellTemplateData<BackflowTest>>;

    @ViewChild('bpatTemplate', { static: true })
    public bpatTemplate!: TemplateRef<CellTemplateData<BackflowTest>>;

    public readonly BackflowTestResult = BackflowTestResult;

    public canViewTesters: boolean = false;

    private panelFilters: QueryProperty[] = [];

    public defaultIsCurrent: string = 'true';
    public defaultServiceStatus: string = 'false';

    public table: TableViewModel<BackflowTest> = {
        columns: [],
        query: {
            sort: { testDate: 'Desc' },
            filter: []
        }
    };

    public testHistoryOptions: InputOption[] = [
        { id: '', text: 'View complete test history' },
        { id: 'true', text: 'View current tests only' }
    ];

    public serviceStatusOptions: InputOption[] = [
        { id: '', text: 'All Status Types' },
        { id: 'false', text: 'In Service' },
        { id: 'true', text: 'Out of Service' }
    ];

    public rejectedStatusOptions: InputOption[] = [
        { id: '', text: 'Any Status' },
        { id: 'false', text: 'Accepted' },
        { id: 'true', text: 'Rejected' }
    ];

    public testResultOptions: InputOption[];
    public approvalStatusOptions: InputOption[];
    public hazardTypeOptions: InputOption[];

    constructor(
        private readonly _backflowTestService: BackflowTestService,
        private readonly _authService: AuthService,
        private readonly _options: BackflowTestOptionsService
    ) {
        this.testResultOptions = this._options.testResultOptions;
        this.approvalStatusOptions = this._options.approvalStatusOptions;
        this.hazardTypeOptions = this._options.hazardTypeFilterOptions;
    }

    public async ngOnInit(): Promise<void> {
        this.canViewTesters = await this._authService.hasAnyPermisison(
            PermissionAction.CanView, PermissionType.BackflowTesters);
        this.table.columns = this.getColumns();
        this.panelFilters = [
            { columnName: 'isCurrent', value: this.defaultIsCurrent, comparisonOperator: 'Eq' as ComparisonOperator },
            { columnName: 'outOfService', value: this.defaultServiceStatus, comparisonOperator: 'Eq' as ComparisonOperator }
        ];
        await this.getTests();
    }

    public onFilterChange(queryProperties: QueryProperty[]): void {
        this.panelFilters = queryProperties;
    }

    public async update(searchForm: NgForm): Promise<void> {
        if (searchForm.valid) {
            if (this.table.items?.pageInfo) {
                this.table.items.pageInfo.pageNumber = 1;
            }
            await this.getTests();
        }
    }

    public async getTests(): Promise<void> {
        if (!this.siteId) {
            return;
        }
        try {
            this.table.isLoading = true;
            this.table.query.filter = [this.siteFilter(), ...this.panelFilters];
            this.table.items = await this._backflowTestService.getAll(
                this.table.items?.pageInfo || {},
                this.table.query
            );
        } finally {
            this.table.isLoading = false;
        }
    }

    private siteFilter(): QueryProperty {
        return {
            columnName: 'site.id',
            value: this.siteId!.toString(),
            comparisonOperator: 'Eq' as ComparisonOperator
        };
    }

    private getColumns(): TableColumn<BackflowTest>[] {
        return [
            {
                field: '',
                caption: 'Status',
                type: ColumnType.other,
                queryColumnExcluded: true,
                cellTemplate: this.statusTemplate
            },
            {
                field: '',
                caption: 'Dates',
                type: ColumnType.other,
                queryColumnExcluded: true,
                cellTemplate: this.datesTemplate
            },
            {
                field: 'serialNumber',
                caption: 'Serial #',
                type: ColumnType.text
            },
            {
                field: '',
                caption: 'Assembly Description',
                type: ColumnType.other,
                queryColumnExcluded: true,
                cellTemplate: this.assemblyTemplate
            },
            {
                field: '',
                caption: 'BPAT Information',
                type: ColumnType.other,
                queryColumnExcluded: true,
                cellTemplate: this.bpatTemplate
            }
        ];
    }
}
