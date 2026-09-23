import { CommonModule } from '@angular/common';
import { Component, Input, OnInit, TemplateRef, ViewChild } from '@angular/core';
import { CellTemplateData, ColumnType, TableColumn, TableViewModel } from '@envirotrax/common-ui';
import { SharedComponentsModule } from '../../../shared/components/shared.components.module';
import { WaterSupplierUser } from '../../../shared/models/water-suppliers/water-supplier-user';
import { WaterSupplierUserService } from '../../../shared/services/water-suppliers/water-supplier-user.service';

@Component({
    selector: 'vp-water-supplier-users',
    imports: [
        CommonModule,
        SharedComponentsModule
    ],
    templateUrl: './water-supplier-user-list.component.html'
})
export class WaterSupplierUserListComponent implements OnInit {
    @Input()
    public waterSupplierId!: number;

    public table: TableViewModel<WaterSupplierUser> = {
        query: {
            sort: { contactName: 'Asc' },
            filter: []
        },
        freeTextSearch: {
            searchQuery: [
                { field: 'contactName', operator: 'Ct', multiWordSearch: true },
                { field: 'emailAddress', operator: 'Ct' },
                { field: 'cellNumber', operator: 'Ct' }
            ]
        }
    };

    @ViewChild('rolesCell', { static: true })
    private rolesCellTemplate!: TemplateRef<CellTemplateData<WaterSupplierUser>>;

    constructor(
        private readonly _waterSupplierUserService: WaterSupplierUserService
    ) {

    }

    public async ngOnInit(): Promise<void> {
        this.table.columns = this.getColumns();

        await this.getUsers();
    }

    public async getUsers(): Promise<void> {
        try {
            this.table.isLoading = true;

            this.table.items = await this._waterSupplierUserService.getAll(
                this.waterSupplierId,
                this.table.items?.pageInfo || {},
                this.table.query);
        } finally {
            this.table.isLoading = false;
        }
    }

    private getColumns(): TableColumn<WaterSupplierUser>[] {
        return [
            {
                field: 'contactName',
                caption: 'Full Name',
                type: ColumnType.text
            },
            {
                field: 'emailAddress',
                caption: 'User ID',
                type: ColumnType.text
            },
            {
                field: 'cellNumber',
                caption: 'Cell Number',
                type: ColumnType.text
            },
            {
                field: 'roles',
                caption: 'Roles',
                type: ColumnType.other,
                queryColumnExcluded: true,
                cellTemplate: this.rolesCellTemplate
            }
        ];
    }
}
