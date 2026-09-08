import { CommonModule } from '@angular/common';
import { Component, OnInit, TemplateRef, ViewChild } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { CellTemplateData, ColumnType, QueryProperty, TableColumn, TableViewModel } from '@envirotrax/common-ui';
import { SharedComponentsModule } from '../../../shared/components/shared.components.module';
import { BackflowTesterAccount } from '../../../shared/models/backflow/backflow-tester-account';
import { BackflowTesterService } from '../../../shared/services/backflow/backflow-tester.service';

@Component({
    templateUrl: './backflow-tester-list.component.html',
    imports: [
        CommonModule,
        FormsModule,
        SharedComponentsModule
    ],
})
export class BackflowTesterListComponent implements OnInit {
    @ViewChild('accountTypeCell', { static: true })
    public accountTypeCell?: TemplateRef<CellTemplateData<BackflowTesterAccount>>;

    @ViewChild('companyCell', { static: true })
    public companyCell?: TemplateRef<CellTemplateData<BackflowTesterAccount>>;

    @ViewChild('addressCell', { static: true })
    public addressCell?: TemplateRef<CellTemplateData<BackflowTesterAccount>>;

    @ViewChild('contactCell', { static: true })
    public contactCell?: TemplateRef<CellTemplateData<BackflowTesterAccount>>;

    public showResults: boolean = false;

    private licenseNumber: string | null = null;

    private insuranceNumber: string | null = null;

    public table: TableViewModel<BackflowTesterAccount> = {
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
        const license = queryProperties.find(p => p.columnName === 'licenseNumber');
        const insurance = queryProperties.find(p => p.columnName === 'insuranceNumber');

        this.licenseNumber = license?.value ? license.value : null;
        this.insuranceNumber = insurance?.value ? insurance.value : null;

        this.table.query.filter = queryProperties.filter(p =>
            p.columnName !== 'licenseNumber' && p.columnName !== 'insuranceNumber');
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
                this.licenseNumber,
                this.insuranceNumber
            );
        } finally {
            this.table.isLoading = false;
        }
    }

    private getColumns(): TableColumn<BackflowTesterAccount>[] {
        return [
            // Presentation-only icon column - a non-empty field would add a sortable header that refires the search.
            { field: '', caption: '', type: ColumnType.other, cellTemplate: this.accountTypeCell },
            { field: 'companyName', caption: 'Company/Contact', type: ColumnType.text, cellTemplate: this.companyCell },
            { field: 'address', caption: 'Address Information', type: ColumnType.text, cellTemplate: this.addressCell },
            { field: 'emailAddress', caption: 'Contact Information', type: ColumnType.text, cellTemplate: this.contactCell }
        ];
    }
}
