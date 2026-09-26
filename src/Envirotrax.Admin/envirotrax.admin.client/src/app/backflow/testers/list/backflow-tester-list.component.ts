import { CommonModule } from '@angular/common';
import { Component, OnInit, TemplateRef, ViewChild } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { CellTemplateData, ColumnType, QueryProperty, TableColumn, TableViewModel } from '@envirotrax/common-ui';
import { SharedComponentsModule } from '../../../shared/components/shared.components.module';
import { Professional } from '../../../shared/models/professionals/professional';
import { BackflowTesterSearchCriteria, BackflowTesterService } from '../../../shared/services/backflow/backflow-tester.service';

/**
 * Criteria that match a child collection (licences, insurances) or a person inside the company rather than a
 * column on the company row. They are stripped out of the emitted filter and sent as their own query
 * parameters - the same split the CSI window uses.
 */
const CRITERIA_FIELDS: (keyof BackflowTesterSearchCriteria)[] = [
    'bpatLicenseNumber',
    'fireLicenseNumber',
    'insurancePolicyNumber',
    'userEmail',
    'contactName',
    'cellNumber'
];

@Component({
    templateUrl: './backflow-tester-list.component.html',
    imports: [
        CommonModule,
        FormsModule,
        SharedComponentsModule
    ],
})
export class BackflowTesterListComponent implements OnInit {
    @ViewChild('addressCell', { static: true })
    public addressCell?: TemplateRef<CellTemplateData<Professional>>;

    @ViewChild('contactCell', { static: true })
    public contactCell?: TemplateRef<CellTemplateData<Professional>>;

    public showResults: boolean = false;

    private criteria: BackflowTesterSearchCriteria = {};

    public table: TableViewModel<Professional> = {
        query: {
            sort: {},
            filter: []
        }
    };

    constructor(
        private readonly _backflowTesterService: BackflowTesterService
    ) {

    }

    public ngOnInit(): void {
        this.table.columns = this.getColumns();
    }

    public onFilterChange(queryProperties: QueryProperty[]): void {
        this.criteria = {};

        for (const field of CRITERIA_FIELDS) {
            const property = queryProperties.find(p => p.columnName === field);

            this.criteria[field] = property?.value ? property.value : null;
        }

        this.table.query.filter = queryProperties.filter(p => !CRITERIA_FIELDS.includes(p.columnName as keyof BackflowTesterSearchCriteria));
    }

    public async search(searchForm: NgForm): Promise<void> {
        if (!searchForm.valid) {
            return;
        }

        await this.getTesters();

        this.showResults = true;
    }

    public async getTesters(): Promise<void> {
        try {
            this.table.isLoading = true;
            this.table.items = await this._backflowTesterService.getAll(
                this.table.items?.pageInfo || {},
                this.table.query,
                this.criteria
            );
        } finally {
            this.table.isLoading = false;
        }
    }

    private getColumns(): TableColumn<Professional>[] {
        return [
            { field: 'name', caption: 'Company Name', type: ColumnType.text },
            { field: 'address', caption: 'Address Information', type: ColumnType.text, cellTemplate: this.addressCell },
            { field: 'companyEmail', caption: 'Contact Information', type: ColumnType.text, cellTemplate: this.contactCell }
        ];
    }
}
