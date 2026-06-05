import { Component, Input, OnInit, TemplateRef, ViewChild } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../../shared/services/auth/auth.service';
import { BackflowTestService } from '../../../shared/services/backflow/backflow-test.service';
import { BackflowTest } from '../../../shared/models/backflow/backflow-test';
import { FeatureType } from '../../../shared/models/feature-type';
import { ROLE_DEFINITIONS } from '../../../shared/models/role-definitions';
import { Query, QueryProperty } from '../../../shared/models/query';
import { TableViewModel } from '../../../shared/models/table-view-model';
import { CellTemplateData, TableColumn } from '../../../shared/components/data-components/table/table.component';
import { ColumnType } from '../../../shared/components/data-components/sorting-filtering/query-view-model';
import { InputOption } from '../../../shared/components/input/input.component';
import { BackflowTestResult } from '../../../shared/models/backflow/backflow-test-enums';

@Component({
    selector: 'vp-site-backflow-tests',
    standalone: false,
    templateUrl: './site-backflow-tests.component.html'
})
export class SiteBackflowTestsComponent implements OnInit {
    @Input() public siteId!: number;

    @ViewChild('statusTemplate', { static: true })
    private statusTemplate!: TemplateRef<CellTemplateData<BackflowTest>>;

    @ViewChild('testDateTemplate', { static: true })
    private testDateTemplate!: TemplateRef<CellTemplateData<BackflowTest>>;

    @ViewChild('serialTemplate', { static: true })
    private serialTemplate!: TemplateRef<CellTemplateData<BackflowTest>>;

    @ViewChild('assemblyTemplate', { static: true })
    private assemblyTemplate!: TemplateRef<CellTemplateData<BackflowTest>>;

    @ViewChild('bpatTemplate', { static: true })
    private bpatTemplate!: TemplateRef<CellTemplateData<BackflowTest>>;

    public readonly BackflowTestResult = BackflowTestResult;
    public readonly todayIso = new Date().toISOString().slice(0, 10);
    public isVisible = false;
    public currentOnly = true;

    public viewTypeOptions: InputOption[] = [
        { id: 'true', text: 'View current tests only' },
        { id: 'false', text: 'View all tests' }
    ];

    public serviceStatusOptions: InputOption[] = [
        { id: 'all', text: 'All Status Types' },
        { id: 'false', text: 'In Service' },
        { id: 'true', text: 'Out of Service' }
    ];

    public selectedServiceStatus: string = 'all';

    public table: TableViewModel<BackflowTest> = {
        columns: [],
        query: { sort: {}, filter: [] }
    };

    constructor(
        private readonly _authService: AuthService,
        private readonly _backflowTestService: BackflowTestService,
        private readonly _router: Router
    ) {}

    public async ngOnInit(): Promise<void> {
        const hasFeature = await this._authService.hasAnyFeatures(FeatureType.BackflowTesting);
        const hasRole = await this._authService.hasAnyRoles(ROLE_DEFINITIONS.PROFESSIONALS.BACKFLOW_TESTER);
        this.isVisible = hasFeature && hasRole;

        if (this.isVisible) {
            this.table.columns = this.buildColumns();
            await this.loadTests();
        }
    }

    public async onViewTypeChange(value: string): Promise<void> {
        this.currentOnly = value === 'true';
        await this.loadTests();
    }

    public async onServiceStatusChange(value: string): Promise<void> {
        this.selectedServiceStatus = value;
        await this.loadTests();
    }

    public async loadTests(): Promise<void> {
        try {
            this.table.isLoading = true;
            this.table.query = this.buildQuery();
            this.table.items = await this._backflowTestService.getAllForProfessional(
                this.table.items?.pageInfo || {},
                this.table.query
            );
        } finally {
            this.table.isLoading = false;
        }
    }

    public viewTest(test: BackflowTest): void {
        const url = this._router.serializeUrl(
            this._router.createUrlTree(['/professionals/backflow/submit', test.id])
        );
        window.open(url, '_blank');
    }

    private buildQuery(): Query {
        const filter: QueryProperty[] = [
            { columnName: 'site.id', comparisonOperator: 'Eq', value: String(this.siteId) }
        ];

        if (this.currentOnly) {
            filter.push({ columnName: 'isCurrent', comparisonOperator: 'Eq', value: 'true' });
        }

        if (this.selectedServiceStatus !== 'all') {
            filter.push({ columnName: 'outOfService', comparisonOperator: 'Eq', value: this.selectedServiceStatus });
        }

        return { filter, sort: {} };
    }

    private buildColumns(): TableColumn<BackflowTest>[] {
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
                caption: 'Test/Exp Date',
                type: ColumnType.other,
                queryColumnExcluded: true,
                cellTemplate: this.testDateTemplate
            },
            {
                field: '',
                caption: 'Serial Number',
                type: ColumnType.other,
                queryColumnExcluded: true,
                cellTemplate: this.serialTemplate
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
