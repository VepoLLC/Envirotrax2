import { Component, OnInit, TemplateRef, ViewChild } from "@angular/core";
import { TableViewModel } from "../../../shared/models/table-view-model";
import { ProfessionalUser } from "../../../shared/models/professionals/professional-user";
import { ProfesionalUserService } from "../../../shared/services/professionals/professional-user.service";
import { ActivatedRoute, Router } from "@angular/router";
import { CreateProfessionalUserComponent } from "../create/create-professional-user.component";
import { ToastService, CellTemplateData, ColumnType, ModalHelperService, TableColumn } from '@envirotrax/common-ui';

// A grid row: a user decorated with the abbreviated job functions shown in the Job Functions column.
type ProfessionalUserRow = ProfessionalUser & {
    jobFunctions?: string;
};

@Component({
    standalone: false,
    templateUrl: './professional-user-list.component.html'
})
export class ProfessionalUserListComponent implements OnInit {
    // Kept as a template so the column's `field` stays empty: vp-table refetches when the header of a column
    // with a field is clicked, and jobFunctions is not a DTO field the API can sort on.
    @ViewChild('jobFunctionsTemplate', { static: true })
    public jobFunctionsTemplate!: TemplateRef<CellTemplateData<ProfessionalUserRow>>;

    public table: TableViewModel<ProfessionalUserRow> = {
        columns: [],
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

    private getColumns(): TableColumn<ProfessionalUserRow>[] {
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
            },
            {
                field: '',
                caption: 'Job Functions',
                type: ColumnType.other,
                queryColumnExcluded: true,
                cellTemplate: this.jobFunctionsTemplate
            }
        ];
    }

    public async ngOnInit(): Promise<void> {
        this.table.columns = this.getColumns();

        await this.getUsers();
    }

    public async getUsers(): Promise<void> {
        try {
            this.table.isLoading = true;

            const users = await this._userService.getAll(this.table.items?.pageInfo || {}, this.table.query);

            this.table.items = {
                ...users,
                data: users.data.map(user => ({ ...user, jobFunctions: this.buildJobFunctions(user) }))
            };
        } finally {
            this.table.isLoading = false;
        }
    }

    private buildJobFunctions(user: ProfessionalUser): string {
        const jobFunctions: string[] = [];

        if (user.isBackflowTester) {
            jobFunctions.push('BPAT');
        }

        if (user.isCsiInspector) {
            jobFunctions.push('CSI');
        }

        if (user.isFogInspector) {
            jobFunctions.push('FOGI');
        }

        if (user.isFogTransporter) {
            jobFunctions.push('FOGT');
        }

        return jobFunctions.join(', ');
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