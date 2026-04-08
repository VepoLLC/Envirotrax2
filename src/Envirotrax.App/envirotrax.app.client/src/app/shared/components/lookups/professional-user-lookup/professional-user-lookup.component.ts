import { Component, OnInit } from "@angular/core";
import { ModalReference } from "@developer-partners/ngx-modal-dialog";
import { ProfessionalUser } from "../../../models/professionals/professional-user";
import { ProfesionalUserService } from "../../../services/professionals/professional-user.service";
import { TableViewModel } from "../../../models/table-view-model";
import { TableColumn } from "../../data-components/table/table.component";
import { ColumnType } from "../../data-components/sorting-filtering/query-view-model";

@Component({
    standalone: false,
    templateUrl: './professional-user-lookup.component.html'
})
export class ProfessionalUserLookupComponent implements OnInit {
    public table: TableViewModel<ProfessionalUser> = {
        columns: [],
        query: {
            sort: {},
            filter: []
        },
        freeTextSearch: {
            searchQuery: [
                { field: 'contactName' },
                { field: 'emailAddress' }
            ]
        }
    };

    constructor(
        private readonly _userService: ProfesionalUserService,
        private readonly _modalReference: ModalReference<ProfessionalUser>
    ) { }

    public ngOnInit(): void {
        this.table.columns = this.getColumns();
        this.getUsers();
    }

    private getColumns(): TableColumn<ProfessionalUser>[] {
        return [
            {
                field: 'contactName',
                caption: 'Name',
                type: ColumnType.text
            },
            {
                field: 'emailAddress',
                caption: 'Email',
                type: ColumnType.text
            },
            {
                field: 'jobTitle',
                caption: 'Job Title',
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

    public select(user: ProfessionalUser): void {
        this._modalReference.closeSuccess(user);
    }

    public cancel(): void {
        this._modalReference.cancel();
    }
}
