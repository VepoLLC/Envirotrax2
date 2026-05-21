import { Component, Input, OnInit } from "@angular/core";
import { ProfessionalUser } from "../../../../../shared/models/professionals/professional-user";
import { TableViewModel } from "../../../../../shared/models/table-view-model";
import { TableColumn } from "../../../../../shared/components/data-components/table/table.component";
import { ColumnType } from "../../../../../shared/components/data-components/sorting-filtering/query-view-model";
import { CsiInspectorSubAccountsService } from "../../../../../shared/services/csi/csi-inspector-user.service";
import { ModalHelperService } from "../../../../../shared/services/helpers/modal-helper.service";
import { ToastService } from "../../../../../shared/services/toast.service";
import { AuthService } from "../../../../../shared/services/auth/auth.service";
import { PermissionAction, PermissionType } from "../../../../../shared/models/permission-type";
import { FeatureType } from "../../../../../shared/models/feature-type";
import { ModalSize } from "@developer-partners/ngx-modal-dialog";
import { AddEditCsiInspectorUserComponent, CsiUserModalData } from "../edit/add-edit-csi-inspector-user.component";

@Component({
    selector: 'vp-csi-inspector-users',
    standalone: false,
    templateUrl: './csi-inspector-users.component.html'
})
export class CsiInspectorUsersComponent implements OnInit {
    @Input() public inspectorId!: number;

    public canManage: boolean = false;

    public table: TableViewModel<ProfessionalUser> = {
        columns: [],
        query: { sort: {}, filter: [] }
    };

    constructor(
        private readonly _service: CsiInspectorSubAccountsService,
        private readonly _modalHelper: ModalHelperService,
        private readonly _toastService: ToastService,
        private readonly _authService: AuthService
    ) { }

    public async ngOnInit(): Promise<void> {
        await this.setPermissions();
        this.setupColumns();
        await this.loadSubAccounts();
    }

    private async setPermissions(): Promise<void> {
        const canEdit = await this._authService.hasAnyPermisison(PermissionAction.CanEdit, PermissionType.CsiInspectors);
        this.canManage = canEdit && await this._authService.hasAnyFeatures(FeatureType.ManageProfessionalUsers);
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

    public addUser(): void {
        this._modalHelper.show<CsiUserModalData, ProfessionalUser>(AddEditCsiInspectorUserComponent, {
            title: 'Add Sub Account',
            model: { inspectorId: this.inspectorId, user: {} },
            size: ModalSize.large
        }).result().subscribe(() => this.loadSubAccounts());
    }

    public editUser(user: ProfessionalUser): void {
        this._modalHelper.show<CsiUserModalData, ProfessionalUser>(AddEditCsiInspectorUserComponent, {
            title: 'Edit Sub Account',
            model: { inspectorId: this.inspectorId, user },
            size: ModalSize.large
        }).result().subscribe(() => this.loadSubAccounts());
    }

    public deleteUser(user: ProfessionalUser): void {
        this._modalHelper.showDeleteConfirmation().result().subscribe(async () => {
            try {
                this.table.isLoading = true;
                await this._service.delete(this.inspectorId, user.id!);
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
                this.inspectorId,
                this.table.items?.pageInfo || {},
                this.table.query
            );
        } finally {
            this.table.isLoading = false;
        }
    }
}
