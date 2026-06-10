import { Component, Input, OnInit, TemplateRef, ViewChild } from "@angular/core";
import { Router } from "@angular/router";
import { BackflowTest } from "../../../../shared/models/backflow/backflow-test";
import { BackflowTestService } from "../../../../shared/services/backflow/backflow-test.service";
import { BackflowTestResult } from "../../../../shared/models/backflow/backflow-test-enums";
import { TableViewModel } from "../../../../shared/models/table-view-model";
import { CellTemplateData, TableColumn } from "../../../../shared/components/data-components/table/table.component";
import { ColumnType } from "../../../../shared/components/data-components/sorting-filtering/query-view-model";
import { ComparisonOperator } from "../../../../shared/models/query";

@Component({
    selector: 'vp-backflow-test-history',
    standalone: false,
    templateUrl: './backflow-test-history.component.html'
})
export class BackflowTestHistoryComponent implements OnInit {
    @Input() public test!: BackflowTest;
    @Input() public canViewTesters: boolean = false;

    public readonly BackflowTestResult = BackflowTestResult;

    @ViewChild('historyStatusTemplate', { static: true })
    public historyStatusTemplate!: TemplateRef<CellTemplateData<BackflowTest>>;

    @ViewChild('historyDatesTemplate', { static: true })
    public historyDatesTemplate!: TemplateRef<CellTemplateData<BackflowTest>>;

    @ViewChild('historyAssemblyTemplate', { static: true })
    public historyAssemblyTemplate!: TemplateRef<CellTemplateData<BackflowTest>>;

    @ViewChild('historyBpatTemplate', { static: true })
    public historyBpatTemplate!: TemplateRef<CellTemplateData<BackflowTest>>;

    public historyTable: TableViewModel<BackflowTest> = {
        columns: [],
        query: {
            sort: { testDate: 'Desc' },
            filter: []
        }
    };

    constructor(
        private readonly _router: Router,
        private readonly _testService: BackflowTestService
    ) { }

    public async ngOnInit(): Promise<void> {
        this.historyTable.columns = this.getHistoryColumns();
        await this.loadHistory();
    }

    public viewHistoryTest(test: BackflowTest): void {
        if (test?.id == null) {
            return;
        }
        const url = this._router.serializeUrl(
            this._router.createUrlTree(['/backflow', 'tests', test.id, 'view'])
        );
        window.open(url, '_blank');
    }

    public async loadHistoryPage(): Promise<void> {
        await this.loadHistory();
    }

    private async loadHistory(): Promise<void> {
        const siteId = this.test?.site?.id;
        if (siteId == null) {
            return;
        }

        try {
            this.historyTable.isLoading = true;
            this.historyTable.query.filter = [{
                columnName: 'site.id',
                value: siteId.toString(),
                comparisonOperator: 'Eq' as ComparisonOperator
            }];
            this.historyTable.items = await this._testService.getAll(
                this.historyTable.items?.pageInfo || {},
                this.historyTable.query
            );
        } finally {
            this.historyTable.isLoading = false;
        }
    }

    private getHistoryColumns(): TableColumn<BackflowTest>[] {
        return [
            {
                field: '',
                caption: 'Status',
                type: ColumnType.other,
                queryColumnExcluded: true,
                cellTemplate: this.historyStatusTemplate
            },
            {
                field: '',
                caption: 'Dates',
                type: ColumnType.other,
                queryColumnExcluded: true,
                cellTemplate: this.historyDatesTemplate
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
                cellTemplate: this.historyAssemblyTemplate
            },
            {
                field: '',
                caption: 'BPAT Information',
                type: ColumnType.other,
                queryColumnExcluded: true,
                cellTemplate: this.historyBpatTemplate
            }
        ];
    }
}
