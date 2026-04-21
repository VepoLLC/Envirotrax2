import { Component, Input, OnInit } from "@angular/core";
import { ProfessionalUser } from "../../../../shared/models/professionals/professional-user";
import { TableViewModel } from "../../../../shared/models/table-view-model";
import { TableColumn } from "../../../../shared/components/data-components/table/table.component";
import { ColumnType } from "../../../../shared/components/data-components/sorting-filtering/query-view-model";
import { CsiInspectorSubAccountsService } from "../../../../shared/services/csi/csi-inspector-user.service";

@Component({
    selector: 'vp-csi-inspector-users',
    standalone: false,
    templateUrl: './csi-inspector-users.component.html'
})
export class CsiInspectorUsersComponent implements OnInit {
    @Input() public inspectorId!: number;

    public table: TableViewModel<ProfessionalUser> = {
        columns: [],
        query: { sort: {}, filter: [] }
    };

    constructor(private readonly _service: CsiInspectorSubAccountsService) { }

    public async ngOnInit(): Promise<void> {
        this.setupColumns();
        await this.loadSubAccounts();
    }

    private setupColumns(): void {
        this.table.columns = this.getColumns();
    }

    private getColumns(): TableColumn<ProfessionalUser>[] {
        return [
            {
                field: 'emailAddress',
                caption: 'Email Address',
                type: ColumnType.text
            },
            {
                field: 'contactName',
                caption: 'Contact Name',
                type: ColumnType.text
            },
            {
                field: 'jobTitle',
                caption: 'Job Title',
                type: ColumnType.text
            }
        ];
    }

    public async loadSubAccounts(): Promise<void> {
        try {
            this.table.isLoading = true;
            this.table.items = await this._service.getSubAccounts(
                this.inspectorId,
                this.table.items?.pageInfo || {},
                this.table.query
            );
        } finally {
            this.table.isLoading = false;
        }
    }
}
