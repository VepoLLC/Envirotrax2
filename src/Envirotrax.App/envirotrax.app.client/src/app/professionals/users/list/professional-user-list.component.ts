import { Component, OnInit } from "@angular/core";
import { TableViewModel } from "../../../shared/models/table-view-model";
import { ProfessionalUser } from "../../../shared/models/professionals/professional-user";
import { ProfesionalUserService } from "../../../shared/services/professionals/professional-user.service";
import { ActivatedRoute, Router } from "@angular/router";
import { ModalHelperService } from "../../../shared/services/helpers/modal-helper.service";
import { TableColumn } from "../../../shared/components/data-components/table/table.component";
import { ColumnType } from "../../../shared/components/data-components/sorting-filtering/query-view-model";
import { CreateProfessionalUserComponent } from "../create/create-professional-user.component";
import { ToastService } from "../../../shared/services/toast.service";

@Component({
    standalone: false,
    templateUrl: './professional-user-list.component.html'
})
export class ProfessionalUserListComponent implements OnInit {
    public table: TableViewModel<ProfessionalUser> = {
        columns: this.getColumns(),
        query: {
            sort: {},
            filter: []
        },
        freeTextSearch: {
            searchQuery: [
                { field: 'contactName', operator: 'Ct', multiWordSearch: true },
                { field: 'emailAddress', operator: 'Ct' }
            ]
        }
    };

    constructor(
        private readonly _userService: ProfesionalUserService,
        private readonly _router: Router,
        private readonly _activatedRoute: ActivatedRoute,
        private readonly _modalHelper: ModalHelperService,
        private readonly _toastService: ToastService
    ) {

    }

    private getColumns(): TableColumn<ProfessionalUser>[] {
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
                field: 'jobTitle',
                caption: 'Job Title',
                type: ColumnType.text
            }
        ];
    }

    public async ngOnInit(): Promise<void> {
        await this.getUsers();
    }

    public async getUsers(): Promise<void> {
        try {
            this.table.isLoading = true;
            this.table.items = await this._userService.getAll(this.table.items?.pageInfo || {}, this.table.query);
        } finally {
            this.table.isLoading = false;
        }
    }

    public add(): void {
        this._modalHelper.show<ProfessionalUser>(CreateProfessionalUserComponent, {
            title: 'Create User',
        }).result()
            .subscribe(user => this.edit(user));
    }

    public edit(user: ProfessionalUser): void {
        this._router.navigate([user.id, 'edit'], {
            relativeTo: this._activatedRoute
        });
    }

    private async processDelete(user: ProfessionalUser): Promise<void> {
        try {
            this.table.isLoading = true;
            await this._userService.delete(user.id!);
            this._toastService.successFullyDeleted('User');
        } finally {
            this.table.isLoading = false;
        }

        await this.getUsers();
    }

    public delete(user: ProfessionalUser): void {
        this._modalHelper.showDeleteConfirmation()
            .result()
            .subscribe(() => this.processDelete(user));
    }
}