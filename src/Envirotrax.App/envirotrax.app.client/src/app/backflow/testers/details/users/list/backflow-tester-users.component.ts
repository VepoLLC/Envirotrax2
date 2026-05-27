import { Component, Input, OnInit } from "@angular/core";
import { ProfessionalUser } from "../../../../../shared/models/professionals/professional-user";
import { TableViewModel } from "../../../../../shared/models/table-view-model";
import { TableColumn } from "../../../../../shared/components/data-components/table/table.component";
import { ColumnType } from "../../../../../shared/components/data-components/sorting-filtering/query-view-model";
import { BackflowTesterUserService } from "../../../../../shared/services/backflow/backflow-tester-user.service";

@Component({
    selector: 'vp-backflow-tester-users',
    standalone: false,
    templateUrl: './backflow-tester-users.component.html'
})
export class BackflowTesterUsersComponent implements OnInit {
    @Input() public testerId!: number;

    public table: TableViewModel<ProfessionalUser> = {
        columns: [],
        query: { sort: {}, filter: [] }
    };

    constructor(private readonly _service: BackflowTesterUserService) { }

    public async ngOnInit(): Promise<void> {
        this.table.columns = this.getColumns();
        await this.loadSubAccounts();
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
                this.testerId,
                this.table.items?.pageInfo || {},
                this.table.query
            );
        } finally {
            this.table.isLoading = false;
        }
    }
}
