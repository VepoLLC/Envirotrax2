import { AfterViewInit, ChangeDetectorRef, Component, OnInit, TemplateRef, ViewChild } from "@angular/core";
import { Router, ActivatedRoute } from "@angular/router";
import { NgForm } from "@angular/forms";
import { TableViewModel } from "../../../shared/models/table-view-model";
import { BackflowTest } from "../../../shared/models/backflow/backflow-test";
import { BackflowTestService } from "../../../shared/services/backflow/backflow-test.service";
import { CellTemplateData, TableColumn, TableCustomAction } from "../../../shared/components/data-components/table/table.component";
import { ColumnType } from "../../../shared/components/data-components/sorting-filtering/query-view-model";
import { QueryProperty } from "../../../shared/models/query";

@Component({
    standalone: false,
    templateUrl: './backflow-test-assembly-search.component.html'
})
export class BackflowTestAssemblySearchComponent implements OnInit, AfterViewInit {

    @ViewChild('assemblyInfoTemplate') private assemblyInfoTemplate!: TemplateRef<CellTemplateData<BackflowTest>>;
    @ViewChild('propertyInfoTemplate') private propertyInfoTemplate!: TemplateRef<CellTemplateData<BackflowTest>>;
    @ViewChild('testInfoTemplate') private testInfoTemplate!: TemplateRef<CellTemplateData<BackflowTest>>;

    public showResults = false;
    public accountNumber = '';
    public manufacturer = '';
    public serialNumber = '';
    public streetNumber = '';
    public errorMessage = '';

    public table: TableViewModel<BackflowTest> = {
        columns: [],
        query: { sort: {}, filter: [] }
    };

    public readonly customActions: TableCustomAction<BackflowTest>[] = [
        { text: 'Submit Test', action: (test: BackflowTest) => this.selectAssembly(test) }
    ];

    constructor(
        private readonly _backflowTestService: BackflowTestService,
        private readonly _router: Router,
        private readonly _activatedRoute: ActivatedRoute,
        private readonly _cdr: ChangeDetectorRef
    ) { }

    public ngOnInit(): void { }

    public ngAfterViewInit(): void {
        this.table.columns = this.getColumns();
        this._cdr.detectChanges();
    }

    public async search(searchForm: NgForm): Promise<void> {
        if (!searchForm.valid) return;

        if (!this.accountNumber && !(this.manufacturer && this.serialNumber) && !this.streetNumber) {
            this.errorMessage = 'Please enter an Account Number, Manufacturer and Serial Number, or a Street Number to search.';
            return;
        }

        this.errorMessage = '';
        await this.getAssemblies();
        this.showResults = true;
    }

    public async getAssemblies(): Promise<void> {
        try {
            this.table.isLoading = true;
            const filter: QueryProperty[] = [];

            if (this.accountNumber) {
                filter.push({ columnName: 'accountNumber', comparisonOperator: 'Eq', value: this.accountNumber });
            }
            if (this.manufacturer) {
                filter.push({ columnName: 'manufacturer', comparisonOperator: 'StW', value: this.manufacturer });
            }
            if (this.serialNumber) {
                filter.push({ columnName: 'serialNumber', comparisonOperator: 'Eq', value: this.serialNumber });
            }
            if (this.streetNumber) {
                filter.push({ columnName: 'propertyStreetNumber', comparisonOperator: 'StW', value: this.streetNumber });
            }
            // Only show current (latest) test per assembly
            filter.push({ columnName: 'isCurrent', comparisonOperator: 'Eq', value: 'true' });

            this.table.query.filter = filter;
            this.table.items = await this._backflowTestService.getAllForProfessional(
                this.table.items?.pageInfo || {},
                this.table.query
            );
        } finally {
            this.table.isLoading = false;
        }
    }

    public submitUnlisted(): void {
        this._router.navigate(['new'], { relativeTo: this._activatedRoute });
    }

    private selectAssembly(test: BackflowTest): void {
        this._router.navigate([test.id], { relativeTo: this._activatedRoute });
    }

    private getColumns(): TableColumn<BackflowTest>[] {
        return [
            {
                field: 'accountNumber',
                caption: 'Account #',
                type: ColumnType.text
            },
            {
                field: '',
                caption: 'Assembly Information',
                type: ColumnType.other,
                queryColumnExcluded: true,
                cellTemplate: this.assemblyInfoTemplate
            },
            {
                field: '',
                caption: 'Property Information',
                type: ColumnType.other,
                queryColumnExcluded: true,
                cellTemplate: this.propertyInfoTemplate
            },
            {
                field: '',
                caption: 'Last Test',
                type: ColumnType.other,
                queryColumnExcluded: true,
                cellTemplate: this.testInfoTemplate
            }
        ];
    }
}
