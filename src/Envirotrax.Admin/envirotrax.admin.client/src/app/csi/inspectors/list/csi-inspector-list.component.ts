import { CommonModule } from '@angular/common';
import { Component, OnInit, TemplateRef, ViewChild } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { CellTemplateData, ColumnType, QueryProperty, TableColumn, TableViewModel } from '@envirotrax/common-ui';
import { SharedComponentsModule } from '../../../shared/components/shared.components.module';
import { Professional } from '../../../shared/models/professionals/professional';
import { CsiInspectorSearchCriteria, CsiInspectorService } from '../../../shared/services/csi/csi-inspector.service';

const CriteriaFieldNames = ['inspectorLicenseNumber', 'insurancePolicyNumber', 'userEmail', 'contactName'];

@Component({
    templateUrl: './csi-inspector-list.component.html',
    imports: [
        CommonModule,
        FormsModule,
        SharedComponentsModule
    ],
})
export class CsiInspectorListComponent implements OnInit {
    @ViewChild('addressCell', { static: true })
    public addressCell?: TemplateRef<CellTemplateData<Professional>>;

    @ViewChild('contactCell', { static: true })
    public contactCell?: TemplateRef<CellTemplateData<Professional>>;

    public showResults: boolean = false;

    private criteria: CsiInspectorSearchCriteria = {};

    public table: TableViewModel<Professional> = {
        query: {
            sort: {},
            filter: []
        }
    };

    constructor(private readonly _csiInspectorService: CsiInspectorService) {

    }

    public ngOnInit(): void {
        this.table.columns = this.getColumns();
    }

    public onFilterChange(queryProperties: QueryProperty[]): void {
        this.criteria = {
            inspectorLicenseNumber: this.getValue(queryProperties, 'inspectorLicenseNumber'),
            insurancePolicyNumber: this.getValue(queryProperties, 'insurancePolicyNumber'),
            userEmail: this.getValue(queryProperties, 'userEmail'),
            contactName: this.getValue(queryProperties, 'contactName')
        };

        this.table.query.filter = queryProperties.filter(p => !CriteriaFieldNames.includes(p.columnName!));
    }

    private getValue(queryProperties: QueryProperty[], fieldName: string): string | null {
        const property = queryProperties.find(p => p.columnName === fieldName);

        return property?.value ? property.value : null;
    }

    public async search(searchForm: NgForm): Promise<void> {
        if (!searchForm.valid) {
            return;
        }

        await this.getInspectors();

        this.showResults = true;
    }

    public async getInspectors(): Promise<void> {
        try {
            this.table.isLoading = true;
            this.table.items = await this._csiInspectorService.getAll(
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
