import { Component, Input, OnInit } from "@angular/core";
import { ProfessionalUser } from "../../../../../shared/models/professionals/professional-user";
import { TableViewModel } from "../../../../../shared/models/table-view-model";
import { BackflowTesterUserService } from "../../../../../shared/services/backflow/backflow-tester-user.service";
import { ToastService, ColumnType, ModalHelperService, TableColumn } from '@envirotrax/common-ui';
import { AuthService } from "../../../../../shared/services/auth/auth.service";
import { PermissionAction, PermissionType } from "../../../../../shared/models/permission-type";
import { FeatureType } from "../../../../../shared/models/feature-type";
import { ModalSize } from "@developer-partners/ngx-modal-dialog";
import { AddEditBackflowTesterUserComponent, BackflowUserModalData } from "../edit/add-edit-backflow-tester-user.component";

@Component({
    selector: 'vp-backflow-tester-users',
    standalone: false,
    templateUrl: './backflow-tester-users.component.html'
})
export class BackflowTesterUsersComponent implements OnInit {
    @Input() public testerId!: number;

    public canManage: boolean = false;

    public table: TableViewModel<ProfessionalUser> = {
        columns: [],
        query: { sort: {}, filter: [] }
    };

    constructor(
        private readonly _service: BackflowTesterUserService,
        private readonly _modalHelper: ModalHelperService,
        private readonly _toastService: ToastService,
        private readonly _authService: AuthService
    ) { }

    public async ngOnInit(): Promise<void> {
        await this.setPermissions();
        this.table.columns = this.getColumns();
        await this.loadSubAccounts();
    }

    private async setPermissions(): Promise<void> {
        const canEdit = await this._authService.hasAnyPermisison(PermissionAction.CanModify, PermissionType.BackflowTesters);
        this.canManage = canEdit && await this._authService.hasAnyFeatures(FeatureType.ManageProfessionalUsers);
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

    public addUser(): void {
        this._modalHelper.show<BackflowUserModalData, ProfessionalUser>(AddEditBackflowTesterUserComponent, {
            title: 'Add Sub Account',
            model: { testerId: this.testerId, user: {} },
            size: ModalSize.large
        }).result().subscribe(() => this.loadSubAccounts());
    }

    public editUser(user: ProfessionalUser): void {
        this._modalHelper.show<BackflowUserModalData, ProfessionalUser>(AddEditBackflowTesterUserComponent, {
            title: 'Edit Sub Account',
            model: { testerId: this.testerId, user },
            size: ModalSize.large
        }).result().subscribe(() => this.loadSubAccounts());
    }

    public deleteUser(user: ProfessionalUser): void {
        this._modalHelper.showDeleteConfirmation().result().subscribe(async () => {
            try {
                this.table.isLoading = true;
                await this._service.delete(this.testerId, user.id!);
                this._toastService.successFullyDeleted('Sub Account');
            } finally {
                this.table.isLoading = false;
            }
            await this.loadSubAccounts();
        });
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
