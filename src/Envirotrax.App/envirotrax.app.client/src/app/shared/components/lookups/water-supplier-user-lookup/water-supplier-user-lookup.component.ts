import { Component, OnInit } from "@angular/core";
import { ModalReference } from "@developer-partners/ngx-modal-dialog";
import { ColumnType, TableColumn } from "@envirotrax/common-ui";
import { TableViewModel } from "../../../models/table-view-model";
import { WaterSupplierUser } from "../../../models/users/water-supplier-user";
import { UserService } from "../../../services/water-suppliers/user.service";

@Component({
    standalone: false,
    templateUrl: './water-supplier-user-lookup.component.html'
})
export class WaterSupplierUserLookupComponent implements OnInit {
    public table: TableViewModel<WaterSupplierUser> = {
        columns: [],
        query: {
            sort: {},
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

    constructor(
        private readonly _userService: UserService,
        private readonly _modalReference: ModalReference<WaterSupplierUser>
    ) { }

    public async ngOnInit(): Promise<void> {
        this.table.columns = this.getColumns();

        await this.getUsers();
    }

    private getColumns(): TableColumn<WaterSupplierUser>[] {
        return [
            {
                field: 'contactName',
                caption: 'Contact Name',
                type: ColumnType.text
            },
            {
                field: 'emailAddress',
                caption: 'Email',
                type: ColumnType.text
            },
            {
                field: 'cellNumber',
                caption: 'Cell Number',
                type: ColumnType.text
            }
        ];
    }

    public async getUsers(): Promise<void> {
        try {
            this.table.isLoading = true;

            this.table.items = await this._userService.getAll(
                this.table.items?.pageInfo || {},
                this.table.query
            );
        } finally {
            this.table.isLoading = false;
        }
    }

    public select(user: WaterSupplierUser): void {
        this._modalReference.closeSuccess(user);
    }
}
