import { CommonModule } from '@angular/common';
import { Component, OnInit, TemplateRef, ViewChild } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { CellTemplateData, ColumnType, QueryProperty, TableColumn, TableViewModel } from '@envirotrax/common-ui';
import { SharedComponentsModule } from '../../../shared/components/shared.components.module';
import { CsiInspectorAccount } from '../../../shared/models/csi/csi-inspector-account';
import { CsiInspectorService } from '../../../shared/services/csi/csi-inspector.service';
import { WindowService } from '../../../shared/services/window.service';
import { CsiInspectorDetailsComponent } from '../details/csi-inspector-details.component';

@Component({
    templateUrl: './csi-inspector-list.component.html',
    imports: [
        CommonModule,
        FormsModule,
        SharedComponentsModule
    ],
})
export class CsiInspectorListComponent implements OnInit {
    @ViewChild('companyCell', { static: true })
    public companyCell?: TemplateRef<CellTemplateData<CsiInspectorAccount>>;

    @ViewChild('addressCell', { static: true })
    public addressCell?: TemplateRef<CellTemplateData<CsiInspectorAccount>>;

    @ViewChild('contactCell', { static: true })
    public contactCell?: TemplateRef<CellTemplateData<CsiInspectorAccount>>;

    public showResults: boolean = false;

    private licenseNumber: string | null = null;

    private insuranceNumber: string | null = null;

    public table: TableViewModel<CsiInspectorAccount> = {
        query: {
            sort: {},
            filter: []
        }
    };

    constructor(
        private readonly _csiInspectorService: CsiInspectorService,
        private readonly _windowService: WindowService
    ) {

    }

    public ngOnInit(): void {
        this.table.columns = this.getColumns();
    }

    public openDetails(account: CsiInspectorAccount): void {
        this._windowService.addWindow(CsiInspectorDetailsComponent, {
            title: this.buildTitle(account),
            model: account
        });
    }

    private buildTitle(account: CsiInspectorAccount): string {
        return [account.companyName, account.contactName, account.emailAddress]
            .filter(part => part)
            .join(' - ');
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

        await this.getInspectors();

        this.showResults = true;
    }

    public async getInspectors(): Promise<void> {
        try {
            this.table.isLoading = true;
            this.table.items = await this._csiInspectorService.getAll(
                this.table.items?.pageInfo || {},
                this.table.query,
                this.licenseNumber,
                this.insuranceNumber
            );
        } finally {
            this.table.isLoading = false;
        }
    }

    private getColumns(): TableColumn<CsiInspectorAccount>[] {
        return [
            { field: 'companyName', caption: 'Company/Contact', type: ColumnType.text, cellTemplate: this.companyCell },
            { field: 'address', caption: 'Address Information', type: ColumnType.text, cellTemplate: this.addressCell },
            { field: 'emailAddress', caption: 'Contact Information', type: ColumnType.text, cellTemplate: this.contactCell }
        ];
    }
}
